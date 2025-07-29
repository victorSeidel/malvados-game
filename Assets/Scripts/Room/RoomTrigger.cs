using UnityEngine;

public class RoomTrigger : MonoBehaviour
{
    public RoomManager roomManager;

    private void Update()
    {
        if (roomManager.CheckQuests()) QuestSystem.instance.SetQuestText("Volte para a cabine");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && roomManager.CheckQuests())
        {
            roomManager.EnterRoom();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        roomManager.ExitToMainMap();
    }
}