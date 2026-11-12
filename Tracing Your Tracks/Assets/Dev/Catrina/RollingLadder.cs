using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class RollingLadder : MonoBehaviour
{
    [System.Serializable]
    public class LadderStop
    {
        public Transform point;
        public DialogueLine[] pushLines;
    }

    [Header("Ladder")]
    public Transform ladder;
    public LadderStop[] stops;
    public float speed = 1.5f;

    [Header("Blockers (first entry whose requires pass plays, no push)")]
    public FlagLines[] blockers;

    [Header("Flags")]
    public string positionCounter = "LADDER_POS";
    public string[] setsWhenDone;
    public GameObject hideWhenDone;
    public UnityEvent onFinalReached;

    bool moving;

    int Reached()
    {
        int n = 0;
        while (n < stops.Length && GameState.CheckAll(new[] { positionCounter + " >= " + (n + 1) })) n++;
        return n;
    }

    void Start()
    {
        int n = Reached();
        if (n > 0) ladder.position = Flat(stops[n - 1].point.position);
        if (n >= stops.Length) Finish();
    }

    public void Interact()
    {
        if (moving || DialogueManager.Instance == null) return;
        int n = Reached();
        if (n >= stops.Length) return;

        foreach (var b in blockers)
            if (GameState.CheckAll(b.requires)) { Play(b.lines, new UnityEvent()); return; }

        var done = new UnityEvent();
        done.AddListener(() => StartCoroutine(Slide(n)));
        Play(stops[n].pushLines, done);
    }

    void Play(DialogueLine[] lines, UnityEvent done)
    {
        if (lines == null || lines.Length == 0) { done.Invoke(); return; }
        DialogueManager.Instance.StartDialogue(lines, done, DialoguePriority.Clue);
    }

    IEnumerator Slide(int n)
    {
        moving = true;
        DialogueManager.Instance.SetLocked(true);
        Vector3 target = Flat(stops[n].point.position);
        while ((ladder.position - target).sqrMagnitude > 0.0001f)
        {
            ladder.position = Vector3.MoveTowards(ladder.position, target, speed * Time.deltaTime);
            yield return null;
        }
        ladder.position = target;
        GameState.ApplyAll(new[] { positionCounter + "+1" });
        DialogueManager.Instance.SetLocked(false);
        moving = false;
        if (n + 1 >= stops.Length)
        {
            GameState.ApplyAll(setsWhenDone);
            Finish();
        }
    }

    Vector3 Flat(Vector3 p) => new Vector3(p.x, ladder.position.y, p.z);

    void Finish()
    {
        onFinalReached?.Invoke();
        if (hideWhenDone != null) hideWhenDone.SetActive(false);
    }
}