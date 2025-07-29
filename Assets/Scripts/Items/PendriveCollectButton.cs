using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PendriveCollectButton : MonoBehaviour
{
    [Header("Configurações")]
    public float interactionRadius = 3f;
    public LayerMask pendriveLayer;
    
    [Header("UI")]
    public GameObject buttonPrefab;
    public Canvas canvas;
    
    private GameObject _currentButton;
    private PendriveCollectItem _nearbyPendrive;
    private Transform _player;

    private void Start()
    {
        _player = GameObject.FindGameObjectWithTag("Player").transform;
        
        _currentButton = Instantiate(buttonPrefab, canvas.transform);
        _currentButton.SetActive(false);
        
        Button btn = _currentButton.GetComponent<Button>();
        btn.onClick.AddListener(CollectItem);
    }

    private void Update()
    {
        Collider[] nearbyItems  = Physics.OverlapSphere(_player.position, interactionRadius, pendriveLayer);
        
        if (nearbyItems.Length > 0)
        {
            Transform nearestItem = GetNearestItem(nearbyItems);
            _nearbyPendrive = nearestItem.GetComponent<PendriveCollectItem>();
            
            if (_nearbyPendrive != null)
            {
                Vector3 screenPos = Camera.main.WorldToScreenPoint(nearestItem.position + Vector3.up * 0.5f);
                _currentButton.transform.position = screenPos;
                
                float pulseScale = 1f + Mathf.PingPong(Time.time * 0.5f, 0.2f);
                _currentButton.transform.localScale = Vector3.one * pulseScale;
                
                if (!_currentButton.activeSelf)  _currentButton.SetActive(true);
            }
        }
        else
        {
            if (_currentButton.activeSelf) _currentButton.SetActive(false);
                
            _nearbyPendrive = null;
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

    private void CollectItem()
    {
        if (_nearbyPendrive != null)
        {
            InventorySystem.instance.AddPendrive(
                _nearbyPendrive.itemID,
                _nearbyPendrive.itemName,
                _nearbyPendrive.description,
                _nearbyPendrive.icon,
                _nearbyPendrive.audioClip,
                _nearbyPendrive.videoClip
            );
            
            ShowCollectFeedback();
            
            Destroy(_nearbyPendrive.gameObject);
            _currentButton.SetActive(false);
        }
    }

    private void ShowCollectFeedback()
    {
        GameObject feedback = new GameObject("CollectFeedback");
        TextMeshProUGUI text = feedback.AddComponent<TextMeshProUGUI>();
        text.text = "+ " + _nearbyPendrive.itemName;
        text.color = Color.green;
        text.fontSize = 32;
        text.alignment = TextAlignmentOptions.Center;
        
        RectTransform rt = feedback.GetComponent<RectTransform>();
        rt.SetParent(canvas.transform);
        rt.anchoredPosition = WorldToCanvasPosition(_nearbyPendrive.transform.position + Vector3.up * 0.5f);
        rt.sizeDelta = new Vector2(200, 50);

        StartCoroutine(AnimateFeedback(feedback));
    }

    private Vector2 WorldToCanvasPosition(Vector3 worldPos)
    {
        Vector2 viewportPos = Camera.main.WorldToViewportPoint(worldPos);
        RectTransform canvasRect = canvas.GetComponent<RectTransform>();
        
        return new Vector2(
            (viewportPos.x * canvasRect.sizeDelta.x) - (canvasRect.sizeDelta.x * 0.5f),
            (viewportPos.y * canvasRect.sizeDelta.y) - (canvasRect.sizeDelta.y * 0.5f)
        );
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