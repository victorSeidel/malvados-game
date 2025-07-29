using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BookManager : MonoBehaviour
{
    [System.Serializable]
    public class BookPage
    {
        [HideInInspector] public bool unlocked = false;
        public string requiredQuestID;
        [TextArea(5, 10)] public string textoRasurado;
        [TextArea(5, 10)] public string textoCompleto;
    }

    [Header("Configurações")]
    public GameObject bookUI;
    public TextMeshProUGUI pageText;
    public Button nextPageButton;
    public Button prevPageButton;

    [Header("Páginas")]
    public BookPage[] pages;
    private int currentPageIndex = 0;

    private void Start()
    {
        SaveSystem.LoadBookProgress(this);
    }

    private void UpdateBook()
    {
        foreach (var page in pages)
        {
            Quest quest = QuestSystem.instance.GetQuest(page.requiredQuestID);
            if (quest != null && quest.state == QuestState.Completed)
            {
                if (!page.unlocked)
                {
                    page.unlocked = true;
                    SaveSystem.SaveBookProgress(this);
                }
            }
            else
            {
                page.unlocked = false;
                SaveSystem.SaveBookProgress(this);
            }
        }
    }

    public void ToggleBook()
    {
        UpdateBook();
        bookUI.SetActive(!bookUI.activeSelf);
        if (bookUI.activeSelf) UpdatePage();
    }

    private void UpdatePage()
    {
        BookPage page = pages[currentPageIndex];
        bool pageUnlocked = page.unlocked; 

        pageText.text = pageUnlocked ? page.textoCompleto : page.textoRasurado;

        prevPageButton.gameObject.SetActive(currentPageIndex > 0);
        nextPageButton.gameObject.SetActive(currentPageIndex < pages.Length - 1);
    }

    public void NextPage()
    {
        if (currentPageIndex < pages.Length - 1)
        {
            currentPageIndex++;
            UpdatePage();
        }
    }

    public void PrevPage()
    {
        if (currentPageIndex > 0)
        {
            currentPageIndex--;
            UpdatePage();
        }
    }
}