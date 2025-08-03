using System.Collections;
using TMPro;
using UnityEngine;

public class EnemyCollider : MonoBehaviour
{
    [Header("Battle")]
    public Enemy[] enemies;
    private Enemy _enemyToFight;
    private bool _battleStarted = false;

    [Header("Patrol")]
    public float patrolRadius = 5f;
    public float patrolSpeed = 2f;
    public float waitTimeAtPoint = 1f;
    private Vector3 _nextPatrolPoint;
    private bool _waiting = false;

    void Start()
    {
        int index = Random.Range(0, enemies.Length);
        _enemyToFight = enemies[index];

        GenerateNextPatrolPoint();
    }

    void Update()
    {
        if (!_battleStarted) Patrol();
    }

    void Patrol()
    {
        if (_waiting) return;

        transform.position = Vector3.MoveTowards(transform.position, _nextPatrolPoint, patrolSpeed * Time.deltaTime);

        if (Vector3.Distance(transform.position, _nextPatrolPoint) < 0.1f) StartCoroutine(WaitAndMove());
    }

    IEnumerator WaitAndMove()
    {
        _waiting = true;
        yield return new WaitForSeconds(waitTimeAtPoint);
        GenerateNextPatrolPoint();
        _waiting = false;
    }

    void GenerateNextPatrolPoint()
    {
        Vector2 randomCircle = Random.insideUnitCircle * patrolRadius;
        _nextPatrolPoint = new Vector3(transform.position.x + randomCircle.x, transform.position.y, transform.position.z + randomCircle.y);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !_battleStarted)
        {
            _battleStarted = true;
            BattleSystem.instance.StartBattle(_enemyToFight, this);
        }
    }

    public void EndBattle(bool won)
    {
        if (won)
        {
            Destroy(gameObject);
        }

        _battleStarted = false;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.black;
        Gizmos.DrawSphere(transform.position, patrolRadius);
    }
}