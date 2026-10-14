using System.Collections;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class MusicManager : MonoBehaviour
{
    public static MusicManager Instance;

    public AudioClip startingTrack;
    public bool playOnStart = true;
    [Range(0f, 1f)] public float musicVolume = 0.6f;
    public float fadeSeconds = 1.5f;

    AudioSource source;
    Coroutine routine;

    void Awake()
    {
        Instance = this;
        source = GetComponent<AudioSource>();
        source.playOnAwake = false;
        source.loop = true;
        source.spatialBlend = 0f;   // 2D: same volume everywhere
        source.volume = 0f;
    }

    void Start()
    {
        if (playOnStart && startingTrack != null) Play(startingTrack);
    }

    void Update()
    {
        // Lets you tweak Music Volume live while testing
        if (routine == null) source.volume = musicVolume;
    }

    // Switch to a new track with a crossfade. Same track already playing = ignored.
    public static void PlayTrack(AudioClip clip)
    {
        if (Instance != null) Instance.Play(clip);
    }

    public static void StopMusic()
    {
        if (Instance != null) Instance.Stop();
    }

    public void Play(AudioClip clip)
    {
        if (clip == null) return;
        if (source.clip == clip && source.isPlaying) return;
        if (routine != null) StopCoroutine(routine);
        routine = StartCoroutine(SwitchTo(clip));
    }

    public void Stop()
    {
        if (routine != null) StopCoroutine(routine);
        routine = StartCoroutine(FadeOutAndStop());
    }

    IEnumerator SwitchTo(AudioClip clip)
    {
        if (source.isPlaying) yield return Fade(source.volume, 0f);
        source.clip = clip;
        source.loop = true;
        source.Play();
        yield return Fade(0f, musicVolume);
        routine = null;
    }

    IEnumerator FadeOutAndStop()
    {
        yield return Fade(source.volume, 0f);
        source.Stop();
        routine = null;
    }

    // Unscaled time, so fades still work while the game is paused
    IEnumerator Fade(float from, float to)
    {
        float t = 0f;
        while (t < fadeSeconds)
        {
            t += Time.unscaledDeltaTime;
            source.volume = Mathf.Lerp(from, to, t / fadeSeconds);
            yield return null;
        }
        source.volume = to;
    }
}
