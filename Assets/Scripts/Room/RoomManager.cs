using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.SceneManagement;

public class RoomManager : MonoBehaviour
{
    public void EnterRoom()
    {
        SceneManager.LoadSceneAsync(0);
    }
    
    public bool CheckQuests()
    {
        List<Quest> quests = QuestSystem.instance.allQuests;
        return quests.All(q => q.state == QuestState.Completed);
    }
}