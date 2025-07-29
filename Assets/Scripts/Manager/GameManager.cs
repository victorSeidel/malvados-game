using System.Collections;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private void Awake()
    {
        StartCoroutine(DelayedLoad());
    }

    IEnumerator DelayedLoad()
    {
        yield return new WaitUntil(() => InventorySystem.instance != null && QuestSystem.instance != null);
        
        SaveSystem.LoadInventory();
        SaveSystem.LoadQuests();
    }

    private void OnApplicationQuit()
    {
        SaveSystem.SaveInventory();
        SaveSystem.SaveQuests();
    }

    private void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus)
        {
            SaveSystem.SaveInventory();
            SaveSystem.SaveQuests();
        }
    }
}