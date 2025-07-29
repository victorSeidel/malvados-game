using UnityEngine;

[RequireComponent(typeof(Collider))]
public class MusicZone : MonoBehaviour
{
    [SerializeField] AudioClip areaMusic;
    [SerializeField] string areaName;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            MusicManager.Instance.PlayMusic(areaMusic, areaName);
        }
    }
}
