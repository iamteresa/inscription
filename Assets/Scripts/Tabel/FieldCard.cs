using UnityEngine;

/// <summary>
/// ���忡 ��ġ�� ī���� �����͸� �����ϰ�,
/// �������� ȸ�� �� �ڵ����� CardDisplay�� ������Ʈ�ϸ�,
/// �������� ���� �� Hited �ִϸ��̼��� ����մϴ�.
/// </summary>
[RequireComponent(typeof(CardDisplay))]
[RequireComponent(typeof(Animator))]
public class FieldCard : MonoBehaviour
{
    public enum CardFaction { Player, Enemy }

    [Header("FieldCard Settings")]
    [Tooltip("ī���� ������ �����ϼ���.")]
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
            Debug.LogWarning("FieldCard: Animator ������Ʈ�� �����ϴ�. Hited �ִϸ��̼��� �������� �ʽ��ϴ�.", this);
    }

    /// <summary>
    /// ī�� ScriptableObject �����͸� �����ϰ� ��Ÿ�� ������ �ʱ�ȭ�մϴ�.
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
    /// ���� ������ UI�� �ݿ��մϴ�.
    /// </summary>
    private void RefreshUI()
    {
        if (cardData != null)
            cardDisplay.SetCardDisplay(cardData);

        cardDisplay.UpdateStatsDisplay(attackPower, currentHealth);
    }

    /// <summary>
    /// ī�忡 �������� �����ϴ�. Hited �ִϸ��̼��� ����ϰ�,
    /// ü���� 0 ���ϸ� �ʵ忡�� �����մϴ�.
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
    /// ī�� ü���� ȸ���մϴ�.
    /// </summary>
    public void Heal(int amount)
    {
        if (!Application.isPlaying) return;

        currentHealth = Mathf.Min(maxHealth, currentHealth + amount);
        RefreshUI();
    }

    /// <summary>
    /// ���忡�� ī�带 �����մϴ�. Play ��忡���� �ı��˴ϴ�.
    /// </summary>
    public void RemoveFromField()
    {
        if (!Application.isPlaying) return;
        Destroy(gameObject);
    }

    /// <summary>���� ü���� ��ȯ�մϴ�.</summary>
    public int GetCurrentHealth() => currentHealth;
    /// <summary>���ݷ��� ��ȯ�մϴ�.</summary>
    public int GetAttackPower() => attackPower;
}
