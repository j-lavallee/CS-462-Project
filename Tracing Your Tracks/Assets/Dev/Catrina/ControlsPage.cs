using System.Text;
using TMPro;
using UnityEngine;

public class ControlsPage : MonoBehaviour
{
    public ControlsData data;
    public TMP_Text listText;

    void OnEnable()
    {
        Refresh();
    }

    public void Refresh()
    {
        if (data == null || listText == null || data.entries == null) return;

        var sb = new StringBuilder();
        foreach (var e in data.entries)
            sb.AppendLine("<b>[" + e.key + "]</b><pos=25%>" + e.action);
        listText.text = sb.ToString();
    }
}