using UnityEngine;

public class Unit : MonoBehaviour
{
    public string unitName;
    public int maxHP;
    public int currentHP;
    public int damage;
    public float criticalZone;
    public float attackDuration;
    [HideInInspector] public bool isDead;
    public AudioSource damageSound;

    public void Setup(Enemy enemy)
    {
        unitName = enemy.enemyName;
        maxHP = enemy.maxHP;
        currentHP = enemy.currentHP;
        damage = enemy.damage;
        criticalZone = enemy.criticalZone;
        attackDuration = enemy.attackDuration;
        isDead = false;
    }

    public void RestartPlayer()
    {
        maxHP = 5;
        currentHP = 5;
        damage = 10;
        isDead = false;
    }
    
    public bool TakeDamage(int dmg)
    {
        currentHP -= dmg;
        if (dmg == 1) damageSound.Play();

        if (currentHP <= 0)
        {
            currentHP = 0;
            isDead = true;
            return isDead;
        }

        return isDead;
    }
}