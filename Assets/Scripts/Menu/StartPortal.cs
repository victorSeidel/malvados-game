using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StartPortal : MonoBehaviour
{
    [Header("UI")]
    public GameObject buttonPrefab;
    public Canvas canvas;
    public GameObject loadingPanel;

    private GameObject _currentButton;


    private void Start()
    {
        _currentButton = Instantiate(buttonPrefab, canvas.transform);
        _currentButton.SetActive(false);

        Button btn = _currentButton.GetComponent<Button>();
        btn.onClick.AddListener(Enter);
    }

    void OnTriggerEnter(Collider other)
    {
        Vector3 screenPos = Camera.main.WorldToScreenPoint(transform.position);
        _currentButton.transform.position = screenPos;

        float pulseScale = 1f + Mathf.PingPong(Time.time * 0.5f, 0.2f);
        _currentButton.transform.localScale = new Vector3(0.75f, 0.75f, 0.75f) * pulseScale;

        if (!_currentButton.activeSelf) _currentButton.SetActive(true);
    }

    void OnTriggerExit(Collider other)
    {
        if (_currentButton.activeSelf) _currentButton.SetActive(false);
    }

    private void Enter()
    {
        _currentButton.SetActive(false);
        loadingPanel.SetActive(true);
        SceneManager.LoadSceneAsync(1);
    }
}