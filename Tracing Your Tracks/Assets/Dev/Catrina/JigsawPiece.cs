using UnityEngine;
using UnityEngine.EventSystems;

public class JigsawPiece : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public RectTransform slot;
    public float snapDistance = 80f;
    [HideInInspector] public bool placed;

    RectTransform rect;
    Vector2 offset;

    void Awake() { rect = GetComponent<RectTransform>(); }

    public void OnBeginDrag(PointerEventData e)
    {
        if (placed) return;
        offset = (Vector2)rect.position - e.position;
        rect.SetAsLastSibling();
    }

    public void OnDrag(PointerEventData e)
    {
        if (placed) return;
        rect.position = e.position + offset;
    }

    public void OnEndDrag(PointerEventData e)
    {
        if (placed) return;
        if (Vector2.Distance(rect.position, slot.position) <= snapDistance * rect.lossyScale.x)
        {
            rect.position = slot.position;
            placed = true;
            GetComponentInParent<JigsawPuzzle>().PiecePlaced();
        }
    }
}