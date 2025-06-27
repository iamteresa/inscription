using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(CardDisplay))]
[RequireComponent(typeof(Animator))]
public class FieldCard : MonoBehaviour
{
    public enum CardFaction { Player, Enemy }

    [Header("FieldCard Settings")]
    [Tooltip("카드의 진영을 설정하세요.")]
    public CardFaction faction;

    private CardDisplay _cardDisplay;
    private Animator _animator;
    private CardData _cardData;
    private int _maxHealth;
    private int _currentHealth;
    private int _attackPower;

    // 붙어 있는 IAbility 인스턴스들
    private List<IAbility> _abilities = new List<IAbility>();

    /// <summary>
    /// 이 카드의 능력 타입(Flyer, Killer, 등)을 외부에서 읽을 수 있도록.
    /// </summary>
    public CardData.CardAbilityType AbilityType => _cardData.AbilityType;

    void Awake()
    {
        _cardDisplay = GetComponent<CardDisplay>();
        if (_cardDisplay == null)
            Debug.LogError("FieldCard requires a CardDisplay component.", this);

        _animator = GetComponent<Animator>();
        if (_animator == null)
            Debug.LogWarning("FieldCard: Animator 컴포넌트가 없습니다. Hited 애니메이션 동작 불가.", this);
    }

    /// <summary>
    /// 카드 ScriptableObject 데이터를 설정하고 런타임 스탯을 초기화합니다.
    /// 반드시 소환 직후 호출해야 합니다.
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

        // 1) 능력 등록
        CardAbilityManager.Instance.RegisterAll(this, _cardData);

        // 2) 소환 이벤트 발행
        CardEventBus.Summon(this);

        Debug.Log($"[RegisterAll] 종족: {_cardData.Species}, 스킬: {_cardData.AbilityType}");
    }

    /// <summary>
    /// CardAbilityManager가 생성한 IAbility 인스턴스를 여기에 추가합니다.
    /// </summary>
    public void RegisterAbility(IAbility ability)
    {
        if (ability != null)
            _abilities.Add(ability);
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
    /// 이 카드로 target을 공격하는 메서드입니다.
    /// 외부에서 호출하거나, FieldCardAttack에서 사용하세요.
    /// </summary>
    public void AttackTarget(FieldCard target)
    {
        // 공격 이벤트 발행
        CardEventBus.Attack(this, target);

        if (target != null)
            target.TakeDamage(_attackPower);
    }

    /// <summary>
    /// 카드가 amount만큼 데미지를 입습니다.
    /// </summary>
    public void TakeDamage(int amount)
    {
        if (!Application.isPlaying) return;

        // 피격 이벤트 발행
        CardEventBus.Damaged(this, amount);

        _currentHealth = Mathf.Max(0, _currentHealth - amount);
        RefreshUI();

        // Hited 애니메이션 트리거
        if (_animator != null)
        {
            _animator.ResetTrigger("Hited");
            _animator.SetTrigger("Hited");
        }

        // 사망 처리
        if (_currentHealth <= 0)
        {
            // 사망 이벤트 발행 (CardData 함께)
            CardEventBus.Death(this, _cardData);

            // 능력 정리
            foreach (var ab in _abilities)
                ab.Cleanup();

            Debug.Log($"Death event fired for {name}");
            RemoveFromField();
        }
    }

    /// <summary>
    /// 카드가 amount만큼 체력을 회복합니다.
    /// </summary>
    public void Heal(int amount)
    {
        if (!Application.isPlaying) return;

        // 회복 이벤트 발행
        CardEventBus.Heal(this, amount);

        _currentHealth = Mathf.Min(_maxHealth, _currentHealth + amount);
        RefreshUI();
    }

    /// <summary>
    /// 공격력을 외부에서 갱신할 수 있도록 허용합니다.
    /// </summary>
    public void SetAttackPower(int newAttack)
    {
        _attackPower = Mathf.Max(0, newAttack);
        _cardDisplay.UpdateStatsDisplay(_attackPower, _currentHealth);
    }

    /// <summary>
    /// 전장에서 카드를 제거합니다.
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
