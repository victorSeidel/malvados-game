using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CollectableItem : MonoBehaviour
{
    [Header("Item")]
    public string itemID;
    public string itemName;
    public int quantity = 1;
    public Sprite icon;

    public Enemy enemyToFight;
    private bool battleStarted = false;

    [Header("Interação")]
    public float interactionRadius = 20f;

    public GameObject interactPrefab;
    private Canvas _canvas;

    private GameObject _currentButton;
    private CollectableItem _nearbyItem;
    private Transform _player;

    private void Start()
    {
        if (InventorySystem.instance.HasItem(itemID) || InventorySystem.instance.HasDeliveredItem(itemID)) Destroy(gameObject);

        _canvas = GameObject.FindGameObjectWithTag("Canvas").GetComponent<Canvas>();
        _player = GameObject.FindGameObjectWithTag("Player").transform;

        _currentButton = Instantiate(interactPrefab, _canvas.transform);
        _currentButton.SetActive(false);
    }

    private void Update()
    {
        if (InventorySystem.instance && (InventorySystem.instance.HasItem(itemID) || InventorySystem.instance.HasDeliveredItem(itemID))) Destroy(gameObject);

        Collider[] nearbyItems = Physics.OverlapSphere(_player.position, interactionRadius, LayerMask.GetMask("Item"));

        if (nearbyItems.Length > 0)
        {
            Transform nearestItem = GetNearestItem(nearbyItems);
            _nearbyItem = nearestItem.GetComponent<CollectableItem>();

            if (_nearbyItem != null)
            {
                Vector3 screenPos = Camera.main.WorldToScreenPoint(nearestItem.position + Vector3.up * 0.5f);
                _currentButton.transform.position = screenPos;

                float pulseScale = 1f + Mathf.PingPong(Time.time * 0.5f, 0.2f);
                _currentButton.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f) * pulseScale;

                if (!_currentButton.activeSelf && !battleStarted) _currentButton.SetActive(true);
            }
        }
        else
        {
            if (_currentButton.activeSelf) _currentButton.SetActive(false);

            _nearbyItem = null;
        }
    }

    private Transform GetNearestItem(Collider[] items)
    {
        Transform nearest = null;
        float minDistance = Mathf.Infinity;

        foreach (Collider item in items)
        {
            float distance = Vector3.Distance(_player.position, item.transform.position);
            if (distance < minDistance)
            {
                minDistance = distance;
                nearest = item.transform;
            }
        }

        return nearest;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !battleStarted)
        {
            battleStarted = true;
            _currentButton.SetActive(false);
            BattleSystem.instance.StartBattle(enemyToFight, this);
        }
    }

    public void EndBattle(bool won)
    {
        if (won)
        {
            InventorySystem.instance.AddItem(itemID, itemName, quantity, icon);
            ShowCollectFeedback();
            Destroy(_currentButton);
            Destroy(gameObject);
        }

        battleStarted = false;
    }
    
    private void ShowCollectFeedback()
    {
        GameObject feedback = new GameObject("CollectFeedback");
        TextMeshProUGUI text = feedback.AddComponent<TextMeshProUGUI>();
        text.text = "+ " + _nearbyItem.itemName;
        text.color = Color.green;
        text.fontSize = 50;
        text.alignment = TextAlignmentOptions.Center;
        
        RectTransform rt = feedback.GetComponent<RectTransform>();
        rt.SetParent(_canvas.transform);
        rt.anchoredPosition = WorldToCanvasPosition(_nearbyItem.transform.position + Vector3.up * 0.5f);
        rt.sizeDelta = new Vector2(400, 100);

        StartCoroutine(AnimateFeedback(feedback));
    }

    private Vector2 WorldToCanvasPosition(Vector3 worldPos)
    {
        Vector2 viewportPos = Camera.main.WorldToViewportPoint(worldPos);
        RectTransform canvasRect = _canvas.GetComponent<RectTransform>();
        return new Vector2((viewportPos.x * canvasRect.sizeDelta.x) - (canvasRect.sizeDelta.x * 0.5f), (viewportPos.y * canvasRect.sizeDelta.y) - (canvasRect.sizeDelta.y * 0.5f));
    }

    private IEnumerator AnimateFeedback(GameObject feedback)
    {
        float duration = 1f;
        float elapsed = 0f;
        TextMeshProUGUI text = feedback.GetComponent<TextMeshProUGUI>();
        Vector2 startPos = feedback.GetComponent<RectTransform>().anchoredPosition;
        Vector2 endPos = startPos + new Vector2(0, 100f);
        Color startColor = text.color;
        Color endColor = new Color(startColor.r, startColor.g, startColor.b, 0f);

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            
            feedback.GetComponent<RectTransform>().anchoredPosition = Vector2.Lerp(startPos, endPos, t);
            
            text.color = Color.Lerp(startColor, endColor, t);
            
            yield return null;
        }

        Destroy(feedback);
    }
}