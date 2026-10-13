using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Range(0f, 1f)] public float sfxVolume = 1f;

    // Sounds we haven't found yet. Fill these in later.
    public AudioClip dialogueAdvance;
    public AudioClip stealthOpen;
    public AudioClip spotted;

    AudioSource source;

    void Awake()
    {
        Instance = this;
        source = GetComponent<AudioSource>();
        source.playOnAwake = false;
        source.spatialBlend = 0f;   // 2D sound: same volume everywhere
    }

    // Anyone can call AudioManager.Play(clip). Missing clips or managers are ignored.
    public static void Play(AudioClip clip, float volume = 1f)
    {
        if (clip == null || Instance == null) return;
        Instance.source.PlayOneShot(clip, volume * Instance.sfxVolume);
    }

    public static void PlayDialogueAdvance() { if (Instance != null) Play(Instance.dialogueAdvance); }
    public static void PlayStealthOpen()     { if (Instance != null) Play(Instance.stealthOpen); }
    public static void PlaySpotted()         { if (Instance != null) Play(Instance.spotted); }
}