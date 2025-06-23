using UnityEngine;

/// <summary>
/// 전장에 배치된 카드의 데이터를 설정하고,
/// 데미지나 회복 시 자동으로 CardDisplay를 업데이트하며,
/// 데미지를 받을 때 Hited 애니메이션을 재생합니다.
/// </summary>
[RequireComponent(typeof(CardDisplay))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(PlayerCostManager))]
public class FieldCard : MonoBehaviour
{
    public enum CardFaction { Player, Enemy }

    [Header("FieldCard Settings")]
    [Tooltip("카드의 진영을 설정하세요.")]
    public CardFaction faction;

    private CardDisplay _cardDisplay;
    private Animator _animator;
    private CardData _cardData;
    private PlayerCostManager _playerCostManager;
    private int _maxHealth;
    private int _currentHealth;
    private int _attackPower;

    void Awake()
    {
        _cardDisplay = GetComponent<CardDisplay>();
        if (_cardDisplay == null)
            Debug.LogError("FieldCard requires a CardDisplay component.", this);

        _animator = GetComponent<Animator>();
        if (_animator == null)
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
       

        _cardData = data;
        faction = cardFaction;
        _maxHealth = data.Health;
        _currentHealth = _maxHealth;
        _attackPower = data.Attack;

        RefreshUI();
    }

    /// <summary>
    /// 현재 스탯을 UI에 반영합니다.
    /// </summary>
    private void RefreshUI()
    {
        if (_cardData != null)
            _cardDisplay.SetCardDisplay(_cardData);

        _cardDisplay.UpdateStatsDisplay(_attackPower, _currentHealth);
    }

    /// <summary>
    /// 카드에 데미지를 입힙니다. Hited 애니메이션을 재생하고,
    /// 체력이 0 이하면 필드에서 제거합니다.
    /// </summary>
    public void TakeDamage(int amount)
    {
        if (!Application.isPlaying) return;

        _currentHealth = Mathf.Max(0, _currentHealth - amount);
        RefreshUI();

        if (_animator != null)
        {
            _animator.ResetTrigger("Hited");
            _animator.SetTrigger("Hited");
        }

        if (_currentHealth <= 0)
        {
            RemoveFromField();
            
        }
            


    }

    /// <summary>
    /// 카드 체력을 회복합니다.
    /// </summary>
    public void Heal(int amount)
    {
        if (!Application.isPlaying) return;

        _currentHealth = Mathf.Min(_maxHealth, _currentHealth + amount);
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
    public int GetCurrentHealth() => _currentHealth;
    /// <summary>공격력을 반환합니다.</summary>
    public int GetAttackPower() => _attackPower;
}
