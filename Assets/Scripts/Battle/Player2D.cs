using UnityEngine;

public class Player2D : MonoBehaviour
{
    [Header("Movimentação")]
    public float moveSpeed = 500f;
    public float maxX = 260f;
    public float maxY = 260f;

    [Header("Joystick")]
    public Joystick joystick;

    private Vector2 input;
    private RectTransform rectTransform;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    void Update()
    {
        input = new Vector2(joystick.Horizontal, joystick.Vertical);
        if (input.magnitude > 1f) input = input.normalized;

        Move();
    }

    void Move()
    {
        Vector2 movement = input * moveSpeed * Time.deltaTime;

        Vector3 newPosition = rectTransform.anchoredPosition + movement;

        newPosition.x = Mathf.Clamp(newPosition.x, -maxX, maxX);
        newPosition.y = Mathf.Clamp(newPosition.y, -maxY, maxY);

        rectTransform.anchoredPosition = newPosition;
    }

    public void TakeDamage()
    {
        Debug.Log("TOMOU!");
    }
}