using UnityEngine;

// Sizes this object's texture in real-world meters and lines it up with
// world position, so every path piece shows same sized stones and the
// pattern continues across the joins between pieces.

[ExecuteAlways]
[RequireComponent(typeof(MeshRenderer), typeof(MeshFilter))]
public class WorldAlignedTiling : MonoBehaviour
{
    // How many meters the texture covers 
    public float textureSize = 3.4f;

    // Tiling (x, y) and offset (z, w)
    [SerializeField, HideInInspector] Vector4 tilingAndOffset = new Vector4(1, 1, 0, 0);

    static readonly int BaseMapST = Shader.PropertyToID("_BaseMap_ST");
    MaterialPropertyBlock block;
    bool needsUpdate = true;

    void OnEnable()
    {
        needsUpdate = true;
        Apply();
    }

    void OnValidate()
    {
        needsUpdate = true;   
    }

    void Update()
    {
        if (Application.isPlaying) return;   
        if (needsUpdate || transform.hasChanged)
        {
            needsUpdate = false;
            transform.hasChanged = false;
            Apply();
        }
    }

    void Apply()
    {
        if (!Application.isPlaying) Recalculate();

        var rend = GetComponent<MeshRenderer>();
        if (block == null) block = new MaterialPropertyBlock();
        rend.GetPropertyBlock(block);
        block.SetVector(BaseMapST, tilingAndOffset);
        rend.SetPropertyBlock(block);
    }

    // Reads the mesh's top face to learn which way its texture coordinates run,
    // then works out a tiling and offset that make texture = world position / textureSize.
    void Recalculate()
    {
        if (textureSize <= 0f) return;
        Mesh mesh = GetComponent<MeshFilter>().sharedMesh;
        if (mesh == null || !mesh.isReadable) return;

        Vector3[] verts = mesh.vertices;
        Vector3[] normals = mesh.normals;
        Vector2[] uvs = mesh.uv;

        // Find three corners of the upward-facing face
        int a = -1, alongX = -1, alongZ = -1;
        for (int i = 0; i < verts.Length; i++)
        {
            if (normals[i].y < 0.9f) continue;   
            if (a < 0) { a = i; continue; }
            bool sameX = Mathf.Abs(verts[i].x - verts[a].x) < 0.001f;
            bool sameZ = Mathf.Abs(verts[i].z - verts[a].z) < 0.001f;
            if (sameZ && !sameX && alongX < 0) alongX = i;
            if (sameX && !sameZ && alongZ < 0) alongZ = i;
        }
        if (a < 0 || alongX < 0 || alongZ < 0) return;

        Vector2 u = Channel(0, a, alongX, alongZ, verts, uvs);
        Vector2 v = Channel(1, a, alongX, alongZ, verts, uvs);
        Vector4 result = new Vector4(u.x, v.x, u.y, v.y);

        if (result != tilingAndOffset)
        {
            tilingAndOffset = result;
#if UNITY_EDITOR
            UnityEditor.EditorUtility.SetDirty(this);   // Make sure Unity saves the new values
#endif
        }
    }

    // For one texture coordinate (0 = U, 1 = V), returns tiling, offset.
    Vector2 Channel(int ch, int a, int alongX, int alongZ, Vector3[] verts, Vector2[] uvs)
    {
        // Which edge does this coordinate change along?
        float changeX = uvs[alongX][ch] - uvs[a][ch];
        float changeZ = uvs[alongZ][ch] - uvs[a][ch];
        int b = Mathf.Abs(changeX) > Mathf.Abs(changeZ) ? alongX : alongZ;
        float uvChange = uvs[b][ch] - uvs[a][ch];

        // How far that edge runs in the world, and along which world axis
        Vector3 worldA = transform.TransformPoint(verts[a]);
        Vector3 worldB = transform.TransformPoint(verts[b]);
        Vector3 delta = worldB - worldA;
        bool worldX = Mathf.Abs(delta.x) > Mathf.Abs(delta.z);
        float worldChange = worldX ? delta.x : delta.z;
        float worldStart = worldX ? worldA.x : worldA.z;

        // Meters of world per one unit of this texture coordinate
        float metersPerUV = worldChange / uvChange;
        float tiling = metersPerUV / textureSize;
        float offset = (worldStart - uvs[a][ch] * metersPerUV) / textureSize;
        return new Vector2(tiling, offset);
    }
}
