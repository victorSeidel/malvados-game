using UnityEngine;
using UnityEngine.UI;

public class NPCInteractionButton : MonoBehaviour
{
    [Header("Configurações")]
    public float interactionRadius = 3f;
    public LayerMask npcLayer;
    
    [Header("UI")]
    public GameObject buttonPrefab;
    public Canvas canvas;
    
    private GameObject _currentButton;
    private InteractiveNPC _currentNPC;
    private Transform _player;
    private bool isInteracting;

    private void Start()
    {
        _player = GameObject.FindGameObjectWithTag("Player").transform;

        _currentButton = Instantiate(buttonPrefab, canvas.transform);
        _currentButton.SetActive(false);

        Button btn = _currentButton.GetComponent<Button>();
        btn.onClick.AddListener(InteractWithNPC);
    }

    private void Update()
    {
        Collider[] npcs = Physics.OverlapSphere(_player.position, interactionRadius, npcLayer);

        if (npcs.Length > 0)
        {
            Transform nearestNPC = GetNearestNPC(npcs);
            _currentNPC = nearestNPC.GetComponent<InteractiveNPC>();

            if (_currentNPC != null)
            {
                Vector3 screenPos = Camera.main.WorldToScreenPoint(nearestNPC.position + Vector3.up * 2f);
                _currentButton.transform.position = screenPos;

                float pulseScale = 1f + Mathf.PingPong(Time.time * 0.5f, 0.2f);
                _currentButton.transform.localScale = new Vector3(0.7f, 0.7f, 0.7f) * pulseScale;

                if (!_currentButton.activeSelf && !isInteracting) _currentButton.SetActive(true);
            }
        }
        else
        {
            if (_currentButton.activeSelf) _currentButton.SetActive(false);
            isInteracting = false;
        }
    }

    private Transform GetNearestNPC(Collider[] npcs)
    {
        Transform nearest = null;
        float minDistance = Mathf.Infinity;
        
        foreach (Collider npc in npcs)
        {
            float distance = Vector3.Distance(_player.position, npc.transform.position);
            if (distance < minDistance)
            {
                minDistance = distance;
                nearest = npc.transform;
            }
        }
        
        return nearest;
    }

    private void InteractWithNPC()
    {
        if (_currentNPC != null)
        {
            _currentNPC.Interact();
            isInteracting = true;
            _currentButton.SetActive(false);
        }
    }
}