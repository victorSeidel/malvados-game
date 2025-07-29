using UnityEngine;

public class InteractiveNPC : MonoBehaviour
{
    public string npcID;
    public Quest associatedQuest;

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

        switch (quest.state)
        {
            case QuestState.NotStarted:
                if (InventorySystem.instance.HasItem(quest.requiredItemID, quest.requiredItemQuantity))
                {
                    audioSource.clip = clips[2];
                    audioSource.Play();

                    DialogueManager.instance.StartDialogue(quest.completedDialogue, quest.npcName);

                    QuestSystem.instance.CompleteQuest(quest.questID);

                    InventorySystem.instance.MarkItemAsDelivered(quest.requiredItemID);
                    InventorySystem.instance.RemoveItem(quest.requiredItemID, quest.requiredItemQuantity);
                }
                else
                {
                    audioSource.clip = clips[0];
                    audioSource.Play();

                    DialogueManager.instance.StartDialogue(quest.startDialogue, quest.npcName);
                    QuestSystem.instance.StartQuest(quest.questID);
                }
                break;

            case QuestState.InProgress:
                if (InventorySystem.instance.HasItem(quest.requiredItemID, quest.requiredItemQuantity))
                {
                    audioSource.clip = clips[2];
                    audioSource.Play();

                    DialogueManager.instance.StartDialogue(quest.completedDialogue, quest.npcName);

                    QuestSystem.instance.CompleteQuest(quest.questID);

                    InventorySystem.instance.MarkItemAsDelivered(quest.requiredItemID);
                    InventorySystem.instance.RemoveItem(quest.requiredItemID, quest.requiredItemQuantity);
                }
                else
                {
                    audioSource.clip = clips[1];
                    audioSource.Play();

                    DialogueManager.instance.StartDialogue(quest.inProgressDialogue, quest.npcName);
                }

                break;

            case QuestState.Completed:
                audioSource.clip = clips[3];
                audioSource.Play();

                DialogueManager.instance.StartDialogue(quest.rewardedDialogue, quest.npcName);
                QuestSystem.instance.SetQuestText(quest.newQuestText);
                break;
        }
    }
}