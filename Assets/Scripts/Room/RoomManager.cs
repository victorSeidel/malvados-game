using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.SceneManagement;

public class RoomManager : MonoBehaviour
{
    public Camera mainCamera;

    public bool useSkyColor;
    public Color skyColor;

    private Transform _player;

    private void Start()
    {
        if (useSkyColor)
        {
            mainCamera.clearFlags = CameraClearFlags.SolidColor;
            mainCamera.backgroundColor = skyColor;
        }
        else
        {
            RenderSettings.skybox = RenderSettings.skybox;
        }

        DynamicGI.UpdateEnvironment();

        _player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    public void EnterRoom()
    {
        mainCamera.clearFlags = CameraClearFlags.SolidColor;
        mainCamera.backgroundColor = Color.black;

        SceneManager.LoadSceneAsync(0);
    }

    public void ExitToMainMap()
    {
        if (useSkyColor)
        {
            mainCamera.clearFlags = CameraClearFlags.SolidColor;
            mainCamera.backgroundColor = skyColor;
        }
        else
        {
            RenderSettings.skybox = RenderSettings.skybox;
        }

        DynamicGI.UpdateEnvironment();
    }
    
    public bool CheckQuests()
    {
        List<Quest> quests = QuestSystem.instance.allQuests;
        return quests.All(q => q.state == QuestState.Completed);
    }
}