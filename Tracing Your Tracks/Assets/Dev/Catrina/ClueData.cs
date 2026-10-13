using UnityEngine;

[CreateAssetMenu(fileName = "NewClue", menuName = "Clues/Clue")]
public class ClueData : ScriptableObject
{
    public string clueTitle;
    [TextArea(2, 5)] public string description;
    public Sprite icon;
    public AudioClip pickupSound;
}