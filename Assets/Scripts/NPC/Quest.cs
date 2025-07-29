using UnityEngine;

public enum QuestState
{
    NotStarted,
    InProgress,
    Completed
}

[CreateAssetMenu(fileName = "NovaMissão", menuName = "Missões/Nova Missão")]
public class Quest : ScriptableObject
{
    public string questID;
    public string questName;
    public string description;
    public QuestState state = QuestState.NotStarted;
    public string requiredItemID;
    public int requiredItemQuantity = 1;
    public string npcName;
    public Dialogue startDialogue;
    public Dialogue inProgressDialogue;
    public Dialogue completedDialogue;
    public Dialogue rewardedDialogue;
    public string newQuestText;
}