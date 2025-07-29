using UnityEngine;

public class Projectile : MonoBehaviour
{
    [HideInInspector] public float speed;
    [HideInInspector] public Vector2 direction = Vector2.right;
    public float hitRadius = 20f;

    private Transform player;

    void Start()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player2D");
        if (playerObj != null) player = playerObj.transform;
    }

    void Update()
    {
        transform.Translate(direction.normalized * speed * Time.deltaTime);

        if (player != null && IsHittingPlayer())
        {
            Unit playerUnity = player.GetComponent<Unit>();
            if (playerUnity != null)
            {
                playerUnity.TakeDamage(1);
                Destroy(gameObject);
            }
        }
    }

    bool IsHittingPlayer()
    {
        float distance = Vector2.Distance(transform.position, player.position);
        return distance <= hitRadius;
    }
}
