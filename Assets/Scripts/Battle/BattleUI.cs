using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BattleUI : MonoBehaviour
{
    public GameObject actionPanel;
    public TMP_Text dialogText;

    [Header("Player UI")]
    public GameObject[] playerHPBar;

    [Header("Enemy UI")]
    public TMP_Text enemyNameText;
    public Slider enemyHPBar;

    [Header("Attack UI")]
    public GameObject attackMiniGame;
    public GameObject joystick;
    public GameObject attackButton;
    public Slider attackTimingBar;
    public RectTransform criticalZoneRect;

    [Header("Audio")]
    public AudioSource fightSound;

    private void Start()
    {
        SetDialogText("");
    }

    public void SetupHUD(Unit enemy)
    {
        for (int i = 0; i < playerHPBar.Length; i++) playerHPBar[i].SetActive(true);

        enemyNameText.text = enemy.unitName;
        enemyHPBar.gameObject.SetActive(true);    
        enemyHPBar.maxValue = enemy.maxHP;
        enemyHPBar.value = enemy.currentHP;
    }

    public void SetDialogText(string text)
    {
        dialogText.text = text;
    }

    public void UpdateHPBars(Unit unit, bool isPlayer)
    {
        if (isPlayer)
        {
            if (unit.currentHP >= 0 && unit.currentHP <= 4) playerHPBar[unit.currentHP].SetActive(false);
        }
        else
        {
            enemyHPBar.value = unit.currentHP;
            if (unit.currentHP <= 0)
            {
                enemyNameText.text = "";
                enemyHPBar.gameObject.SetActive(false); 
            }
        }
    }

    public void SetPlayerActions(bool isActive)
    {
        joystick.SetActive(!isActive);
        attackButton.SetActive(isActive);
        attackTimingBar.gameObject.SetActive(isActive);
    }

    public void SetCriticalZoneVisual(float zoneSize)
    {
        float barWidth = ((RectTransform)attackTimingBar.fillRect.parent).rect.width;

        float zoneWidth = barWidth * zoneSize;

        criticalZoneRect.sizeDelta = new Vector2(zoneWidth, criticalZoneRect.sizeDelta.y);
    }

    public void DestroyAllProjectiles()
    {
        GameObject[] projectiles = GameObject.FindGameObjectsWithTag("Projectile");
        for (int i = 0; i < projectiles.Length; i++) Destroy(projectiles[i]);
    }
}