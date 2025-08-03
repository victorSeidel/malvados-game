using UnityEngine;

public class InteractiveNPC : MonoBehaviour
{
    public string npcID;

    [Header("Quest")]
    public Quest associatedQuest;
    public bool requireItem = true;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip[] clips;
    
    public void Interact()
    {
        if (associatedQuest == null)
        {
            Debug.Log("NPC não tem missões associadas.");
            return;
        }

        Quest quest = QuestSystem.instance.GetQuest(associatedQuest.questID);

        if (!requireItem)
        {
            if (audioSource)
            {
                audioSource.clip = clips[0];
                audioSource.Play();
            }

            DialogueManager.instance.StartDialogue(quest.startDialogue, quest.npcName);

            QuestSystem.instance.StartQuest(quest.questID);
            QuestSystem.instance.CompleteQuest(quest.questID);

            return;
        }

        switch (quest.state)
        {
            case QuestState.NotStarted:
                if (InventorySystem.instance.HasItem(quest.requiredItemID, quest.requiredItemQuantity))
                {
                    if (audioSource)
                    {
                        audioSource.clip = clips[2];
                        audioSource.Play();
                    }

                    DialogueManager.instance.StartDialogue(quest.completedDialogue, quest.npcName);

                    QuestSystem.instance.CompleteQuest(quest.questID);
                    QuestSystem.instance.SetQuestText(quest.newQuestText);

                    InventorySystem.instance.MarkItemAsDelivered(quest.requiredItemID);
                    InventorySystem.instance.RemoveItem(quest.requiredItemID, quest.requiredItemQuantity);
                }
                else
                {
                    if (audioSource)
                    {
                        audioSource.clip = clips[0];
                        audioSource.Play();
                    }

                    DialogueManager.instance.StartDialogue(quest.startDialogue, quest.npcName);
                    QuestSystem.instance.StartQuest(quest.questID);
                }
                break;

            case QuestState.InProgress:
                if (InventorySystem.instance.HasItem(quest.requiredItemID, quest.requiredItemQuantity))
                {
                    if (audioSource)
                    {
                        audioSource.clip = clips[2];
                        audioSource.Play();
                    }

                    DialogueManager.instance.StartDialogue(quest.completedDialogue, quest.npcName);

                    QuestSystem.instance.CompleteQuest(quest.questID);
                    QuestSystem.instance.SetQuestText(quest.newQuestText);

                    InventorySystem.instance.MarkItemAsDelivered(quest.requiredItemID);
                    InventorySystem.instance.RemoveItem(quest.requiredItemID, quest.requiredItemQuantity);
                }
                else
                {
                    if (audioSource)
                    {
                        audioSource.clip = clips[1];
                        audioSource.Play();
                    }

                    DialogueManager.instance.StartDialogue(quest.inProgressDialogue, quest.npcName);
                }

                break;

            case QuestState.Completed:
                if (audioSource)
                {
                    audioSource.clip = clips[3];
                    audioSource.Play();
                }

                DialogueManager.instance.StartDialogue(quest.rewardedDialogue, quest.npcName);
                QuestSystem.instance.SetQuestText(quest.newQuestText);
                break;
        }
    }
}