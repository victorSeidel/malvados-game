using System.Collections.Generic;
using System.IO;
using UnityEngine;

[System.Serializable]
public class PendriveSaveData
{
    public string[] collectedPendriveIDs;
}

public static class SaveSystem
{
    private static readonly string inventoryPath = Application.persistentDataPath + "/inventory.json";
    private static readonly string questsPath = Application.persistentDataPath + "/quests.json";

    private static readonly string booksPath = Application.persistentDataPath + "/books.json";


    [System.Serializable]
    private class InventoryData
    {
        public InventoryItem[] items;
        public DeliveredItem[] deliveredItems;
        public PendriveItem[] collectedPendrives;
    }

    [System.Serializable]
    private class QuestsData
    {
        public Quest[] quests;
    }

    [System.Serializable]
    public class BookSaveData
    {
        public bool[] unlockedPages;
    }

    public static void SaveInventory()
    {
        InventoryData data = new InventoryData
        {
            items = InventorySystem.instance.items.ToArray(),
            deliveredItems = InventorySystem.instance.deliveredItems.ToArray(),
            collectedPendrives = InventorySystem.instance.collectedPendrives.ToArray()
        };
        string json = JsonUtility.ToJson(data);
        File.WriteAllText(inventoryPath, json);
    }

    public static void LoadInventory()
    {
        if (File.Exists(inventoryPath))
        {
            string json = File.ReadAllText(inventoryPath);
            InventoryData data = JsonUtility.FromJson<InventoryData>(json);

            if (InventorySystem.instance != null)
            {
                InventorySystem.instance.items.Clear();
                InventorySystem.instance.items.AddRange(data.items);
                InventorySystem.instance.deliveredItems.AddRange(data.deliveredItems);
                InventorySystem.instance.collectedPendrives.AddRange(data.collectedPendrives);
            }
        }
    }

    public static void SaveQuests()
    {
        QuestsData data = new QuestsData
        {
            quests = new List<Quest>(QuestSystem.instance.activeQuests.Values).ToArray()
        };

        string json = JsonUtility.ToJson(data);
        File.WriteAllText(questsPath, json);
    }

    public static void LoadQuests()
    {
        if (File.Exists(questsPath))
        {
            string json = File.ReadAllText(questsPath);
            QuestsData data = JsonUtility.FromJson<QuestsData>(json);

            foreach (Quest quest in data.quests)
            {
                if (QuestSystem.instance != null) QuestSystem.instance.activeQuests.Add(quest.questID, quest);
            }
        }
    }

    public static void SaveBookProgress(BookManager book)
    {
        BookSaveData data = new BookSaveData();
        data.unlockedPages = new bool[book.pages.Length];

        for (int i = 0; i < book.pages.Length; i++)
        {
            data.unlockedPages[i] = book.pages[i].unlocked;
        }

        string json = JsonUtility.ToJson(data);
        File.WriteAllText(booksPath, json);
    }

    public static void LoadBookProgress(BookManager book)
    {
        if (File.Exists(booksPath))
        {
            string json = File.ReadAllText(booksPath);
            BookSaveData data = JsonUtility.FromJson<BookSaveData>(json);

            for (int i = 0; i < Mathf.Min(data.unlockedPages.Length, book.pages.Length); i++)
            {
                book.pages[i].unlocked = data.unlockedPages[i];
            }
        }
    }
}