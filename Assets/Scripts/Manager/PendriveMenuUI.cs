using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;
using UnityEngine.Video;

public class PendriveMenuUI : MonoBehaviour
{
    public GameObject pendriveMenu;
    public Transform contentParent;
    public GameObject pendriveButtonPrefab;
    public AudioSource audioPlayer;
    public VideoPlayer videoPlayer;

    public void ToggleMenu()
    {
        pendriveMenu.SetActive(!pendriveMenu.activeSelf);
        if (pendriveMenu.activeSelf) UpdateMenu();
    }

    private void UpdateMenu()
    {
        foreach (Transform child in contentParent) Destroy(child.gameObject);

        foreach (var pendrive in InventorySystem.instance.collectedPendrives.OrderBy(p => p.itemName))
        {
            var buttonInstance = Instantiate(pendriveButtonPrefab);

            buttonInstance.GetComponentInChildren<TMP_Text>().text = pendrive.itemName;

            buttonInstance.GetComponent<Button>().onClick.AddListener(() => PlayClip(pendrive.audioClip, pendrive.videoClip));

            buttonInstance.transform.SetParent(contentParent, false);
        }
    }

    private void PlayClip(AudioClip audioClip, VideoClip videoClip = null)
    {
        contentParent.gameObject.SetActive(false);

        if (audioClip)
        {
            audioPlayer.clip = audioClip;
            audioPlayer.Play();
        }

        if (videoClip)
        {
            videoPlayer.clip = videoClip;
            videoPlayer.Play();
        }
    }

    public void StopClip()
    {
        audioPlayer.Stop();
        videoPlayer.Stop();

        contentParent.gameObject.SetActive(true);
    }
}