using UnityEngine;

/// <summary>
/// 전장에 배치된 카드의 데이터를 설정하고,
/// 데미지나 회복 시 자동으로 CardDisplay를 업데이트합니다.
/// </summary>
[RequireComponent(typeof(CardDisplay))]
public class FieldCard : MonoBehaviour
{
    public enum CardFaction { Player, Enemy }

    [Header("FieldCard Settings")]
    [Tooltip("카드의 진영을 설정하세요.")]
    public CardFaction faction;

    private CardDisplay cardDisplay;
    private CardData cardData;
    private int maxHealth;
    private int currentHealth;
    private int attackPower;

    void Awake()
    {
        cardDisplay = GetComponent<CardDisplay>();
        if (cardDisplay == null)
            Debug.LogError("FieldCard requires a CardDisplay component.", this);
    }

    /// <summary>
    /// 카드 ScriptableObject 데이터를 설정하고,
    /// 런타임 스탯을 초기화합니다.
    /// </summary>
    /// <param name="data">할당할 CardData</param>
    /// <param name="cardFaction">카드 진영 (Player/Enemy)</param>
    public void Initialize(CardData data, CardFaction cardFaction)
    {
        if (data == null)
        {
            Debug.LogError("FieldCard.Initialize: CardData is null.", this);
            return;
        }

        cardData = data;
        faction = cardFaction;
        maxHealth = data.Health;
        currentHealth = maxHealth;
        attackPower = data.Attack;

        RefreshUI();
    }

    /// <summary>
    /// 현재 스탯을 UI에 반영합니다.
    /// </summary>
    private void RefreshUI()
    {
        if (cardData != null)
        {
            cardDisplay.SetCardDisplay(cardData);
        }

        cardDisplay.UpdateStatsDisplay(attackPower, currentHealth);
    }

    /// <summary>
    /// 카드에 데미지를 입힙니다.
    /// </summary>
    /// <param name="amount">데미지 양</param>
    public void TakeDamage(int amount)
    {
        if (!Application.isPlaying) return;

        currentHealth = Mathf.Max(0, currentHealth - amount);
        RefreshUI();

        if (currentHealth <= 0)
        {
            RemoveFromField();
        }
    }

    /// <summary>
    /// 카드 체력을 회복합니다.
    /// </summary>
    /// <param name="amount">회복 양</param>
    public void Heal(int amount)
    {
        if (!Application.isPlaying) return;

        currentHealth = Mathf.Min(maxHealth, currentHealth + amount);
        RefreshUI();
    }

    /// <summary>
    /// 전장에서 카드를 제거합니다.
    /// Play 모드에서만 씬 인스턴스를 파괴합니다.
    /// </summary>
    public void RemoveFromField()
    {
        if (!Application.isPlaying) return;
        Destroy(gameObject);
    }

    /// <summary>
    /// 현재 체력을 반환합니다.
    /// </summary>
    public int GetCurrentHealth()
    {
        return currentHealth;
    }

    /// <summary>
    /// 공격력을 반환합니다.
    /// </summary>
    public int GetAttackPower()
    {
        return attackPower;
    }
}
