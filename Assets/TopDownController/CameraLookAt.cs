using UnityEngine;
using UnityEngine.EventSystems;

public class CameraLookAt : MonoBehaviour
{
    [SerializeField] Transform target;
    [SerializeField] float sensitivity = 5f;
    [SerializeField] float distance;
    [SerializeField] float height;
    [SerializeField] float area;

    float currentAngle = 0f;
    Vector2 lastTouchPos;
    bool isDragging;

    int cameraTouchId = -1;
    void Update()
    {
        UpdateCameraPosition();

        foreach (Touch touch in Input.touches)
        {
            bool isRightSide = touch.position.x > Screen.width * area;

            if (EventSystem.current.IsPointerOverGameObject(touch.fingerId) && isRightSide)
            {
                return;
            }

            switch (touch.phase)
            {
                case TouchPhase.Began:
                    if (isRightSide && cameraTouchId == -1)
                    {
                        cameraTouchId = touch.fingerId;
                        lastTouchPos = touch.position;
                        isDragging = true;
                    }
                    break;

                case TouchPhase.Moved:
                    if (isDragging && touch.fingerId == cameraTouchId)
                    {
                        Vector2 delta = touch.position - lastTouchPos;
                        currentAngle += delta.x * sensitivity * Time.deltaTime;
                        lastTouchPos = touch.position;
                    }
                    break;

                case TouchPhase.Ended:
                case TouchPhase.Canceled:
                    if (touch.fingerId == cameraTouchId)
                    {
                        isDragging = false;
                        cameraTouchId = -1;
                    }
                    break;
            }
        }

    #if UNITY_EDITOR
        if (Input.GetMouseButtonDown(0))
        {
            if (Input.mousePosition.x > Screen.width * 0.4f)
            {
                isDragging = true;
                lastTouchPos = Input.mousePosition;
            }
        }
        else if (Input.GetMouseButtonUp(0))
        {
            isDragging = false;
        }

        if (isDragging)
        {
            Vector2 delta = (Vector2)Input.mousePosition - lastTouchPos;
            currentAngle += delta.x * sensitivity * Time.deltaTime;
            lastTouchPos = Input.mousePosition;
        }
    #endif
    }

    void UpdateCameraPosition()
    {
        if (!target) return;

        Quaternion rotation = Quaternion.Euler(0f, currentAngle, 0f);
        Vector3 position = target.position - (rotation * Vector3.forward * distance);
        position.y = target.position.y + height;

        transform.position = position;
        transform.LookAt(target.position + Vector3.up * 1.5f);
    }
}
