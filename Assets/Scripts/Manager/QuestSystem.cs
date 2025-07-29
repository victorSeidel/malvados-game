using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class QuestSystem : MonoBehaviour
{
    public static QuestSystem instance;

    public List<Quest> allQuests;

    public Dictionary<string, Quest> activeQuests = new Dictionary<string, Quest>();

    public TMP_Text questText;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip startClip;
    public AudioClip completeClip;

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

    private void Start()
    {
        LoadInitialQuests();
    }

    private void LoadInitialQuests()
    {
        foreach (Quest q in allQuests)
        {
            if (q.state == QuestState.InProgress || q.state == QuestState.Completed)
            {
                activeQuests[q.questID] = q;
            }
        }
    }

    public void StartQuest(string questID)
    {
        Quest quest = allQuests.Find(q => q.questID == questID);

        if (quest != null && !activeQuests.ContainsKey(questID))
        {
            quest.state = QuestState.InProgress;
            activeQuests.Add(questID, quest);
            SaveSystem.SaveQuests();

            audioSource.clip = startClip;
            audioSource.Play();
        }
    }

    public void CompleteQuest(string questID)
    {
        if (activeQuests.TryGetValue(questID, out Quest quest))
        {
            if (InventorySystem.instance.HasItem(quest.requiredItemID, quest.requiredItemQuantity))
            {
                quest.state = QuestState.Completed;
                SaveSystem.SaveQuests();

                audioSource.clip = completeClip;
                audioSource.Play();
            }
            else
            {
                SaveSystem.SaveQuests();
            }
        }
    }

    public Quest GetQuest(string questID)
    {
        if (activeQuests.TryGetValue(questID, out Quest quest))
        {
            return quest;
        }
        return allQuests.Find(q => q.questID == questID);
    }

    public bool CheckQuestItem(string itemID)
    {
        foreach (var quest in activeQuests.Values)
        {
            if (quest.state == QuestState.InProgress &&
                quest.requiredItemID == itemID &&
                InventorySystem.instance.HasItem(itemID, quest.requiredItemQuantity))
            {
                return true;
            }
        }
        return false;
    }

    public void SetQuestText(string newText)
    {
        questText.text = newText;
    }
}