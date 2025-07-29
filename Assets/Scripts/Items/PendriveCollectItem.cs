using UnityEngine;
using UnityEngine.Video;

public class PendriveCollectItem : MonoBehaviour
{
    public string itemID;
    public string itemName;
    [TextArea] public string description;
    public Sprite icon;
    public AudioClip audioClip = null;
    public VideoClip videoClip = null;

    private void Update()
    {
        if (InventorySystem.instance.HasPendrive(itemID))
        {
            Destroy(gameObject);
        }
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            InventorySystem.instance.AddPendrive(itemID, itemName, description, icon, audioClip, videoClip);

            Destroy(gameObject);
        }
    }
}