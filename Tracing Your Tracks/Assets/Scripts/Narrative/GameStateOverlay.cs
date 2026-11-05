using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameStateOverlay : MonoBehaviour
{
    bool show;
    GUIStyle style;

    void Update()
    {
        var kb = Keyboard.current;
        if (kb != null && kb.f1Key.wasPressedThisFrame) show = !show;
    }

    void OnGUI()
    {
        if (!show) return;
        if (style == null)
        {
            style = new GUIStyle(GUI.skin.box);
            style.alignment = TextAnchor.UpperLeft;
            style.fontSize = 16;
            style.normal.textColor = Color.white;
        }

        var sb = new StringBuilder("GAME STATE (F1 to hide)\n\nCOUNTERS\n");
        foreach (var c in GameState.AllCounters().OrderBy(c => c.Key))
            sb.AppendLine(c.Key + " = " + c.Value);
        sb.AppendLine("\nFLAGS");
        foreach (var f in GameState.AllFlags().OrderBy(f => f))
            sb.AppendLine(f);

        GUI.Box(new Rect(10, 10, 380, Screen.height - 20), sb.ToString(), style);
    }
}
