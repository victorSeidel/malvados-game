using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Video;

[System.Serializable]
public class InventoryItem
{
    public string itemID;
    public string itemName;
    public int quantity;
    public Sprite icon;
}

[System.Serializable]
public class DeliveredItem
{
    public string itemID;
    public int timesDelivered;
}

[System.Serializable]
public class PendriveItem
{
    public string itemID;
    public string itemName;
    [TextArea] public string description;
    public Sprite icon;
    public AudioClip audioClip = null;
    public VideoClip videoClip = null;
}

public class InventorySystem : MonoBehaviour
{
    public static InventorySystem instance;

    [Header("Items")]
    public List<InventoryItem> items = new List<InventoryItem>();
    public List<DeliveredItem> deliveredItems = new List<DeliveredItem>();
    public List<PendriveItem> collectedPendrives = new List<PendriveItem>();

    [Header("UI")]
    public GameObject inventoryPanel;
    public GameObject bagPrefab;
    public GameObject bagContainer;
    private Dictionary<string, GameObject> itemUIMap = new Dictionary<string, GameObject>();

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void AddItem(string itemID, string itemName, int quantity = 1, Sprite icon = null)
    {
        InventoryItem existingItem = items.Find(item => item.itemID == itemID);

        if (existingItem != null)
        {
            existingItem.quantity += quantity;
        }
        else
        {
            items.Add(new InventoryItem { itemID = itemID, itemName = itemName, quantity = quantity, icon = icon });

            if (!itemUIMap.ContainsKey(itemID))
            {
                GameObject bagUI = Instantiate(bagPrefab, bagContainer.transform);
                TextMeshProUGUI textComponent = bagUI.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
                textComponent.text = itemName;
                itemUIMap[itemID] = bagUI;
            }
        }

        SaveSystem.SaveInventory();
    }

    public bool HasItem(string itemID, int quantity = 1)
    {
        InventoryItem item = items.Find(i => i.itemID == itemID);
        return item != null && item.quantity >= quantity;
    }

    public void RemoveItem(string itemID, int quantity = 1)
    {
        InventoryItem item = items.Find(i => i.itemID == itemID);

        if (item != null)
        {
            item.quantity -= quantity;

            if (item.quantity <= 0)
            {
                items.Remove(item);

                if (itemUIMap.TryGetValue(itemID, out GameObject uiObject))
                {
                    Destroy(uiObject);
                    itemUIMap.Remove(itemID);
                }
            }

            SaveSystem.SaveInventory();
        }
    }

    public bool HasDeliveredItem(string itemID)
    {
        return deliveredItems.Exists(i => i.itemID == itemID && i.timesDelivered > 0);
    }

    public void MarkItemAsDelivered(string itemID)
    {
        var item = deliveredItems.Find(i => i.itemID == itemID);
        if (item != null)
        {
            item.timesDelivered++;
        }
        else
        {
            deliveredItems.Add(new DeliveredItem { itemID = itemID, timesDelivered = 1 });
        }
        SaveSystem.SaveInventory();
    }

    public void AddPendrive(string itemID, string itemName, string description, Sprite icon = null, AudioClip audioClip = null, VideoClip videoClip = null)
    {
        PendriveItem existingItem = collectedPendrives.Find(item => item.itemID == itemID);

        collectedPendrives.Add(new PendriveItem
        {
            itemID = itemID,
            itemName = itemName,
            description = description,
            icon = icon,
            audioClip = audioClip,
            videoClip = videoClip
        });

        SaveSystem.SaveInventory();
    }

    public bool HasPendrive(string pendriveID)
    {
        return collectedPendrives.Exists(p => p.itemID == pendriveID);
    }

    public void ToggleInventory()
    {
        inventoryPanel.SetActive(!inventoryPanel.activeSelf);
    }
}