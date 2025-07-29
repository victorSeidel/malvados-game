using UnityEngine;

[CreateAssetMenu(fileName = "Novo Inimigo", menuName = "Inimigos/Novo Inimigo")]
public class Enemy : ScriptableObject
{
    public string enemyName;
    public int maxHP;
    public int currentHP;
    public int damage;
    public float criticalZone;
    public float attackDuration;
}