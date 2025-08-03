using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class MediaButton : MonoBehaviour
{
    [Header("Configurações")]
    public float interactionRadius = 3f;
    public LayerMask pilarLayer;

    [Header("Media")]
    public GameObject mediaPanel;
    public RawImage imageContainer;
    public VideoPlayer videoContainer;
    public RawImage videoImage;

    [Header("UI")]
    public GameObject buttonPrefab;
    public Canvas canvas;

    private GameObject _currentButton;
    private MediaShow _nearbyPilar;
    private Transform _player;

    private bool _isViewing = false;

    private void Start()
    {
        _player = GameObject.FindGameObjectWithTag("Player").transform;

        _currentButton = Instantiate(buttonPrefab, canvas.transform);
        _currentButton.SetActive(false);

        Button btn = _currentButton.GetComponent<Button>();
        btn.onClick.AddListener(ShowMedia);
    }

    private void Update()
    {
        Collider[] nearbyItems = Physics.OverlapSphere(_player.position, interactionRadius, pilarLayer);

        if (nearbyItems.Length > 0)
        {
            Transform nearestItem = GetNearestItem(nearbyItems);
            _nearbyPilar = nearestItem.GetComponent<MediaShow>();

            if (_nearbyPilar != null)
            {
                Vector3 screenPos = Camera.main.WorldToScreenPoint(nearestItem.position + Vector3.up * 0.5f);
                _currentButton.transform.position = screenPos;

                float pulseScale = 1f + Mathf.PingPong(Time.time * 0.5f, 0.2f);
                _currentButton.transform.localScale = new Vector3(0.75f, 0.75f, 0.75f) * pulseScale;

                if (!_currentButton.activeSelf && !_isViewing) _currentButton.SetActive(true);
            }
        }
        else
        {
            if (_currentButton.activeSelf) _currentButton.SetActive(false);

            _nearbyPilar = null;
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

    private void ShowMedia()
    {
        if (_nearbyPilar != null)
        {
            _isViewing = true;

            if (_nearbyPilar.image)
            {
                imageContainer.texture = _nearbyPilar.image;
                imageContainer.gameObject.SetActive(true);
            }
            else
            {
                imageContainer.gameObject.SetActive(false);
            }

            if (_nearbyPilar.video)
            {
                videoContainer.clip = _nearbyPilar.video;
                videoImage.gameObject.SetActive(true);
                videoContainer.Play();
            }
            else
            {
                videoImage.gameObject.SetActive(false);
            }

            mediaPanel.SetActive(true);

            _currentButton.SetActive(false);
        }
    }

    public void CloseMedia()
    {
        imageContainer.texture = null;
        videoContainer.clip = null;

        videoContainer.Stop();

        mediaPanel.SetActive(false);

        _isViewing = false;
    }
}