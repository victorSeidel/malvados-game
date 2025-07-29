using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class ProjetilSpawner : MonoBehaviour
{
    public GameObject projectilePrefab;
    public RectTransform canvasTransform;
    public RectTransform player;
    public float maxDir;
    private Vector2 spawnerPosition;
    public List<BulletPattern> patterns = new List<BulletPattern>();
    private BulletPattern currentPattern;

    private bool canStart = false;

    public void SetStartPatterns(bool set)
    {
        canStart = set;

        SelectRandomPattern();
        if (currentPattern.timer >= 1)
        {
            currentPattern.timer = 0f; 
            ExecutePattern(currentPattern);
        }
    }

    private void Update()
    {
        if (currentPattern == null || !canStart) return;

        currentPattern.timer += Time.deltaTime;
        if (currentPattern.timer >= currentPattern.spawnRate)
        {
            currentPattern.timer = 0f;
            ExecutePattern(currentPattern);
            SelectRandomPattern();
        }
    }

    private void SelectRandomPattern()
    {
        currentPattern = patterns[Random.Range(0, patterns.Count)];
    }

    private void ExecutePattern(BulletPattern pattern)
    {
        spawnerPosition = new Vector2(Random.Range(-maxDir, maxDir), Random.Range(-maxDir, maxDir));

        switch (pattern.patternName)
        {
            case "Circle":
                FireCircle(pattern);
                break;
            case "Fan":
                FireFanTowardsPlayer(pattern);
                break;
            case "RandomBurst":
                FireRandomBurst(pattern);
                break;
            case "Spiral":
                StartCoroutine(FireSpiral(pattern));
                break;
            case "Cross":
                FireCross(pattern);
                break;
            case "Line":
                FireLine(pattern);
                break;
            case "Snipe":
                FireSnipe(pattern);
                break;
            default:
                break;
        }
    }

    private void FireCircle(BulletPattern pattern)
    {
        for (int i = 0; i < pattern.bulletCount; i++)
        {
            float angle = 360f / pattern.bulletCount * i;
            Vector2 dir = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad));
            SpawnBullet(dir, pattern.projectileSpeed);
        }
    }

    private void FireFanTowardsPlayer(BulletPattern pattern)
    {
        Vector2 toPlayer = (player.anchoredPosition - spawnerPosition).normalized;
        float baseAngle = Mathf.Atan2(toPlayer.y, toPlayer.x) * Mathf.Rad2Deg;
        float startAngle = baseAngle - pattern.spreadAngle / 2f;

        for (int i = 0; i < pattern.bulletCount; i++)
        {
            float angle = startAngle + pattern.spreadAngle / (pattern.bulletCount - 1) * i;
            Vector2 dir = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad));
            SpawnBullet(dir, pattern.projectileSpeed);
        }
    }

    private void FireRandomBurst(BulletPattern pattern)
    {
        for (int i = 0; i < pattern.bulletCount; i++)
        {
            float angle = Random.Range(0f, 360f);
            Vector2 dir = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad));
            SpawnBullet(dir, pattern.projectileSpeed);
        }
    }

    private IEnumerator FireSpiral(BulletPattern pattern)
    {
        float delay = 0.05f;
        for (int i = 0; i < pattern.bulletCount; i++)
        {
            float angle = i * 15f % 360f;
            Vector2 dir = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad));
            SpawnBullet(dir, pattern.projectileSpeed);
            yield return new WaitForSeconds(delay);
        }
    }

    void FireCross(BulletPattern pattern)
    {
        Vector2[] dirs =
        {
            Vector2.up, Vector2.down,
            Vector2.left, Vector2.right
        };

        foreach (var dir in dirs) SpawnBullet(dir, pattern.projectileSpeed);
    }

    void FireLine(BulletPattern pattern)
    {
        float spacing = 40f;
        for (int i = 0; i < pattern.bulletCount; i++)
        {
            Vector2 offset = new Vector2((i - pattern.bulletCount / 2) * spacing, 0f);
            GameObject proj = Instantiate(projectilePrefab, canvasTransform);
            RectTransform projRect = proj.GetComponent<RectTransform>();
            projRect.anchoredPosition = spawnerPosition + offset;

            Projectile bullet = proj.AddComponent<Projectile>();
            bullet.direction = Vector2.down;
            bullet.speed = pattern.projectileSpeed;
        }
    }

    void FireSnipe(BulletPattern pattern)
    {
        Vector2 toPlayer = (player.anchoredPosition - spawnerPosition).normalized;
        SpawnBullet(toPlayer, pattern.projectileSpeed);
    }

    private void SpawnBullet(Vector2 direction, float speed)
    {
        GameObject proj = Instantiate(projectilePrefab, canvasTransform);
        Destroy(proj, 10f);
        RectTransform projRect = proj.GetComponent<RectTransform>();
        projRect.anchoredPosition = spawnerPosition;

        Projectile bullet = proj.AddComponent<Projectile>();
        bullet.direction = direction;
        bullet.speed = speed;
    }
}