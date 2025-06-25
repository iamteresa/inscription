using UnityEngine;
using System.Collections.Generic;  // List<T> 사용
// (Animator, CardDisplay 등 RequireComponent는 그대로 두셔도 됩니다)

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

    // ● 능력(Ability) 인스턴스들을 보관할 리스트
    private List<IAbility> _abilities = new List<IAbility>();
 

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

        // ● 능력(Ability) 등록
        CardAbilityManager.Instance.RegisterAll(this, _cardData);

        Debug.Log($"[RegisterAll] 종족: {data.Species}, 스킬: {data.AbilityType}");
    }

    /// <summary>
    /// 외부에서 AbilityFactory가 호출하여, 실제 인스턴스를 추가할 때 사용합니다.
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
    /// 카드에 데미지를 입힙니다. Hited 애니메이션을 재생하고,
    /// 체력이 0 이하면 필드에서 제거하기 전 Ability 정리.
    /// </summary>
    public void TakeDamage(int amount)
    {
        if (!Application.isPlaying) return;

        _currentHealth = Mathf.Max(0, _currentHealth - amount);
        RefreshUI();

        // Hited 애니메이션
        if (_animator != null)
        {
            _animator.ResetTrigger("Hited");
            _animator.SetTrigger("Hited");
        }

        // 수정된 FieldCard.TakeDamage 내 죽음 처리 부분:
        if (_currentHealth <= 0)
        {
            // ▶ 올바르게 이벤트 발행하기
            //CardEventBus.Death(this);
            CardEventBus.Death(this, _cardData);
            // 등록된 Ability 해제
            foreach (var ab in _abilities)
                ab.Cleanup();
            Debug.Log("Death event fired for " + this.name);
            
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
