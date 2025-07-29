using UnityEngine;

public class PlayFootstepSound : MonoBehaviour
    {
    [SerializeField] AudioSource footstepSource;

    public void PlaySound()
    {
        footstepSource.Play();
    }
}
