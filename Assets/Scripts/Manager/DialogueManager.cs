using UnityEngine;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;
using System.Collections;

[System.Serializable]
public class Dialogue
{
    [TextArea(3, 10)]
    public string[] sentences;
}

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager instance;
    
    private Dialogue currentDialogue;
    public float typewriterSpeed = 0.05f;
    private int currentSentenceIndex;
    private Coroutine typingCoroutine;

    [Header("Componentes UI")]
    public GameObject dialoguePanel;
    public TextMeshProUGUI speakerNameText;
    public TextMeshProUGUI dialogueText;
    public Button continueButton;

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
        continueButton.onClick.AddListener(DisplayNextSentence);

        dialoguePanel.SetActive(false);
    }

    public void StartDialogue(Dialogue dialogue, string npcName)
    {
        currentDialogue = dialogue;
        currentSentenceIndex = 0;
    
        dialoguePanel.SetActive(true);
        speakerNameText.text = npcName;

        DisplaySentence(dialogue.sentences[0]);
    }
    
    private void DisplaySentence(string sentence)
    {
        if(typingCoroutine != null) StopCoroutine(typingCoroutine);
        
        typingCoroutine = StartCoroutine(TypeSentence(sentence));
    }

    private IEnumerator TypeSentence(string sentence)
    {
        dialogueText.text = "";
        continueButton.gameObject.SetActive(false);
        
        foreach (char letter in sentence.ToCharArray())
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(typewriterSpeed);
        }
        
        continueButton.gameObject.SetActive(true);
        typingCoroutine = null;
    }

    public void DisplayNextSentence()
    {
        currentSentenceIndex++;

        if (currentSentenceIndex < currentDialogue.sentences.Length)
        {
            DisplaySentence(currentDialogue.sentences[currentSentenceIndex]);
        }
        else
        {
            EndDialogue();
        }
    }

    void EndDialogue()
    {
        dialoguePanel.SetActive(false);
    }
}