using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class LocationTransitionUI : MonoBehaviour
{
    [SerializeField] CanvasGroup canvasGroup;
    [SerializeField] TextMeshProUGUI locationText;
    [SerializeField] RawImage backgroundImage;
    [SerializeField] float showDuration = 3f;
    [SerializeField] float fadeDuration = 1f;

    public static LocationTransitionUI Instance;

    void Awake()
    {
        Instance = this;
        canvasGroup.alpha = 0f;
    }

    public void ShowLocation(string locationName)
    {
        locationText.text = locationName;
        StartCoroutine(PlayTransition());
    }

    IEnumerator PlayTransition()
    {
        float t = 0;
        while (t < fadeDuration)
        {
            canvasGroup.alpha = Mathf.Lerp(0f, 1f, t / fadeDuration);
            t += Time.deltaTime;
            yield return null;
        }

        canvasGroup.alpha = 1f;
        yield return new WaitForSeconds(showDuration);

        t = 0;
        while (t < fadeDuration)
        {
            canvasGroup.alpha = Mathf.Lerp(1f, 0f, t / fadeDuration);
            t += Time.deltaTime;
            yield return null;
        }

        canvasGroup.alpha = 0f;
    }
}
