using System;
using UnityEngine;

[Serializable]
public class BulletPattern
{
    public string patternName;
    public float spawnRate;
    public float projectileSpeed;
    public int bulletCount;
    public float spreadAngle;

    [HideInInspector] public float timer;
}