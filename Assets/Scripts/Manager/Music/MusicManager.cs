using System.Collections;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public static MusicManager Instance;

    [SerializeField] AudioSource musicSource;
    [SerializeField] float fadeDuration = 1.5f;

    AudioClip currentClip;
    Coroutine fadeCoroutine;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void PlayMusic(AudioClip newClip, string locationName = "")
    {
        if (newClip == currentClip) return;

        if (!string.IsNullOrEmpty(locationName)) LocationTransitionUI.Instance.ShowLocation(locationName);

        if (fadeCoroutine != null) StopCoroutine(fadeCoroutine);

        fadeCoroutine = StartCoroutine(FadeToNewClip(newClip));
    }

    IEnumerator FadeToNewClip(AudioClip newClip)
    {
        float t = 0f;

        while (t < fadeDuration)
        {
            musicSource.volume = Mathf.Lerp(1f, 0f, t / fadeDuration);
            t += Time.deltaTime;
            yield return null;
        }

        musicSource.clip = newClip;
        musicSource.Play();

        t = 0f;
        while (t < fadeDuration)
        {
            musicSource.volume = Mathf.Lerp(0f, 1f, t / fadeDuration);
            t += Time.deltaTime;
            yield return null;
        }

        musicSource.volume = 1f;
        currentClip = newClip;
    }

    public void PauseMusic()
    {
        musicSource.Pause();
    }

    public void ContinueMusic()
    {
        PlayMusic(currentClip);
    }
}
