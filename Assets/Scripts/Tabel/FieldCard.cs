using UnityEngine;

/// <summary>
/// 전장에 배치된 카드의 데이터를 설정하고,
/// 데미지나 회복 시 자동으로 CardDisplay를 업데이트하며,
/// 데미지를 받을 때 Hited 애니메이션을 재생합니다.
/// </summary>
[RequireComponent(typeof(CardDisplay))]
[RequireComponent(typeof(Animator))]
public class FieldCard : MonoBehaviour
{
    public enum CardFaction { Player, Enemy }

    [Header("FieldCard Settings")]
    [Tooltip("카드의 진영을 설정하세요.")]
    public CardFaction faction;

    private CardDisplay cardDisplay;
    private Animator animator;
    private CardData cardData;
    private int maxHealth;
    private int currentHealth;
    private int attackPower;

    void Awake()
    {
        cardDisplay = GetComponent<CardDisplay>();
        if (cardDisplay == null)
            Debug.LogError("FieldCard requires a CardDisplay component.", this);

        animator = GetComponent<Animator>();
        if (animator == null)
            Debug.LogWarning("FieldCard: Animator 컴포넌트가 없습니다. Hited 애니메이션이 동작하지 않습니다.", this);
    }

    /// <summary>
    /// 카드 ScriptableObject 데이터를 설정하고 런타임 스탯을 초기화합니다.
    /// </summary>
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
            cardDisplay.SetCardDisplay(cardData);

        cardDisplay.UpdateStatsDisplay(attackPower, currentHealth);
    }

    /// <summary>
    /// 카드에 데미지를 입힙니다. Hited 애니메이션을 재생하고,
    /// 체력이 0 이하면 필드에서 제거합니다.
    /// </summary>
    public void TakeDamage(int amount)
    {
        if (!Application.isPlaying) return;

        currentHealth = Mathf.Max(0, currentHealth - amount);
        RefreshUI();

        if (animator != null)
        {
            animator.ResetTrigger("Hited");
            animator.SetTrigger("Hited");
        }

        if (currentHealth <= 0)
            RemoveFromField();
    }

    /// <summary>
    /// 카드 체력을 회복합니다.
    /// </summary>
    public void Heal(int amount)
    {
        if (!Application.isPlaying) return;

        currentHealth = Mathf.Min(maxHealth, currentHealth + amount);
        RefreshUI();
    }

    /// <summary>
    /// 전장에서 카드를 제거합니다. Play 모드에서만 파괴됩니다.
    /// </summary>
    public void RemoveFromField()
    {
        if (!Application.isPlaying) return;
        Destroy(gameObject);
    }

    /// <summary>현재 체력을 반환합니다.</summary>
    public int GetCurrentHealth() => currentHealth;
    /// <summary>공격력을 반환합니다.</summary>
    public int GetAttackPower() => attackPower;
}
