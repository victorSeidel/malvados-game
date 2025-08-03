using UnityEngine;
using System.Collections;

public enum BattleState { START, PLAYER_TURN, ENEMY_TURN, WON, LOST }

public class BattleSystem : MonoBehaviour
{
    [HideInInspector] public static BattleSystem instance;

    [HideInInspector] public BattleState state;
    public GameObject player;
    public Transform playerBattleStation;
    public GameObject enemyPrefab;

    public BattleUI battleUI;

    private Unit playerUnit;
    private GameObject enemyGO;
    private Unit enemyUnit;

    [Header("Player Attack")]
    public float attackSpeed = 2.0f;
    private bool isBarMoving = false;
    private bool attackTriggered = false;

    [Header("Enemy Attack")]
    public ProjetilSpawner projetilSpawner;
    private EnemyCollider enemyCollider;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        if (state == BattleState.ENEMY_TURN)
        {
            battleUI.UpdateHPBars(playerUnit, true);

            if (playerUnit.isDead)
            {
                state = BattleState.LOST;
                StartCoroutine(EndBattle());
            }
        }
    }

    public void StartBattle(Enemy enemy, EnemyCollider enemyColl)
    {
        playerUnit = player.GetComponent<Unit>();
        playerUnit.RestartPlayer();

        enemyGO = Instantiate(enemyPrefab);
        enemyUnit = enemyGO.GetComponent<Unit>();
        enemyUnit.Setup(enemy);
        enemyCollider = enemyColl;

        battleUI.SetPlayerActions(false);
        battleUI.SetCriticalZoneVisual(enemyUnit.criticalZone);
        battleUI.gameObject.SetActive(true);
        battleUI.SetupHUD(enemyUnit);

        state = BattleState.START;

        MusicManager.Instance.PauseMusic();
        battleUI.PlayFightSound();
        PlayerTurn();
    }

    void PlayerTurn()
    {
        state = BattleState.PLAYER_TURN;

        player.SetActive(false);

        battleUI.SetPlayerActions(true);
        battleUI.SetDialogText("Ataque no centro.");
        battleUI.DestroyAllProjectiles();

        StartCoroutine(AttackMiniGame());
    }

    IEnumerator AttackMiniGame()
    {
        isBarMoving = true;
        attackTriggered = false;
        float barValue = 0f;
        bool increasing = true;

        battleUI.attackMiniGame.SetActive(true);
        battleUI.SetPlayerActions(true);

        while (!attackTriggered)
        {
            if (increasing)
            {
                barValue += 0.01f * attackSpeed;
                if (barValue >= 1f)
                {
                    barValue = 1f;
                    increasing = false;
                }
            }
            else
            {
                barValue -= 0.01f * attackSpeed;
                if (barValue <= 0f)
                {
                    barValue = 0f;
                    increasing = true;
                }
            }

            battleUI.attackTimingBar.value = barValue;
            yield return null;
        }

        bool isPerfectHit = (battleUI.attackTimingBar.value >= 0.5f - enemyUnit.criticalZone / 2f) && (battleUI.attackTimingBar.value <= 0.5f + enemyUnit.criticalZone / 2f);

        int damage;
        if (isPerfectHit)
        {
            battleUI.SetDialogText("Dano crítico!");
            damage = playerUnit.damage * 2;
        }
        else
        {
            battleUI.SetDialogText("Você acertou!");
            damage = playerUnit.damage;
        }

        isBarMoving = false;

        bool isDead = enemyUnit.TakeDamage(damage);
        battleUI.UpdateHPBars(enemyUnit, false);

        if (isDead)
        {
            state = BattleState.WON;
            StartCoroutine(EndBattle());

            yield return new WaitForSeconds(2f);

            battleUI.gameObject.SetActive(false);

            yield return null;
        }

        battleUI.SetPlayerActions(false);
        player.transform.localPosition = Vector3.zero;
        player.SetActive(true);
        
        yield return new WaitForSeconds(2);

        state = BattleState.ENEMY_TURN;
        StartCoroutine(EnemyTurn());
    }

    IEnumerator EnemyTurn()
    {
        battleUI.SetDialogText("SOBREVIVA");

        yield return new WaitForSeconds(1f);

        projetilSpawner.SetStartPatterns(true);

        yield return new WaitForSeconds(enemyUnit.attackDuration);

        projetilSpawner.SetStartPatterns(false);

        if (playerUnit.isDead || state == BattleState.WON || state == BattleState.LOST)
        {
            yield return null;
        }

        state = BattleState.PLAYER_TURN;
        PlayerTurn();
    }

    IEnumerator EndBattle()
    {
        projetilSpawner.SetStartPatterns(false);
        player.SetActive(false);
        battleUI.SetPlayerActions(false);
        battleUI.DestroyAllProjectiles();

        if (state == BattleState.WON)
        {
            battleUI.SetDialogText("Você ganhou a batalha!");
            enemyCollider.EndBattle(true);
        }
        else if (state == BattleState.LOST)
        {
            battleUI.SetDialogText("Você foi derrotado.");
            enemyCollider.EndBattle(false);
        }
        
        yield return new WaitForSeconds(2f);

        battleUI.StopFightSound();
        MusicManager.Instance.ContinueMusic();
        battleUI.gameObject.SetActive(false);
    }
    
    public void OnAttackButtonPressed()
    {
        if (state != BattleState.PLAYER_TURN) return; 

        if (isBarMoving && !attackTriggered) attackTriggered = true;
    }
}