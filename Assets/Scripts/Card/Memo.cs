using static FieldCard;
using System;
using UnityEngine;

몇 초 동안 생각함
아래와 같이 “Abilities” 폴더를 만들고, 그 안에 능력 시스템 관련 스크립트를 정리해보세요.

1. IAbility 인터페이스 (Assets/Scripts/Abilities/IAbility.cs)
csharp
복사
편집
public interface IAbility
{
    /// <summary>이 능력의 고유 ID. 중복 방지용.</summary>
    string Id { get; }

    /// <summary>
    /// 카드가 소환될 때(Initialize 호출) 능력에 필요한 초기 설정을 합니다.
    /// owner: 이 능력이 붙는 FieldCard
    /// data: CardData 스크립터블 오브젝트
    /// </summary>
    void Initialize(FieldCard owner, CardData data);

    /// <summary>
    /// 카드가 전장에서 제거되기 직전에 호출됩니다.
    /// 이벤트 구독 해제 등 정리 작업을 여기에.</summary>
    void Cleanup();
}
2.CardAbilityManager(싱글턴으로 등록 및 팩토리 역할)
Assets / Scripts / CardAbilityManager.cs

csharp
복사
편집
using UnityEngine;

public class CardAbilityManager : MonoBehaviour
{
    public static CardAbilityManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    /// <summary>
    /// 소환 직후 FieldCard.Initialize 안에서 호출.
    /// 종족 능력 + 카드 능력 타입 두 가지를 모두 등록합니다.</summary>
    public void RegisterAll(FieldCard card, CardData data)
    {
        // 1) 종족 능력
        var speciesAb = CreateSpeciesAbility(data.Species);
        if (speciesAb != null)
        {
            speciesAb.Initialize(card, data);
            card.RegisterAbility(speciesAb);
        }

        // 2) 스킬 능력
        var skillAb = CreateAbility(data.AbilityType);
        if (skillAb != null)
        {
            skillAb.Initialize(card, data);
            card.RegisterAbility(skillAb);
        }
    }

    private IAbility CreateSpeciesAbility(CardData.CardSpecies s)
    {
        switch (s)
        {
            case CardData.CardSpecies.Beast: return new BeastSpeciesAbility();
            case CardData.CardSpecies.Undead: return new UndeadSpeciesAbility();
            case CardData.CardSpecies.Machine: return new MachineSpeciesAbility();
            case CardData.CardSpecies.Savage: return new SavageSpeciesAbility();
            case CardData.CardSpecies.Dragon: return new DragonSpeciesAbility();
            case CardData.CardSpecies.Zombie: return new ZombieSpeciesAbility();
            case CardData.CardSpecies.Vampire: return new VampireSpeciesAbility();
            default: return null;
        }
    }

    private IAbility CreateAbility(CardData.CardAbilityType t)
    {
        switch (t)
        {
            case CardData.CardAbilityType.Lifesteal: return new LifestealAbility();
            case CardData.CardAbilityType.Deathrattle: return new DeathrattleAbility();
            case CardData.CardAbilityType.Revenger: return new RevengerAbility();
            // … 기타 스킬 능력 추가
            default: return null;
        }
    }
}
3.종족 능력 예시
Assets/Scripts/Abilities/BeastSpeciesAbility.cs

csharp
복사
편집
using UnityEngine;

public class BeastSpeciesAbility : IAbility
{
    public string Id => "Species_Beast";
    FieldCard _owner;

    public void Initialize(FieldCard owner, CardData data)
    {
        _owner = owner;
        CardEventBus.OnAttack += OnAttack;  // 공격했을 때 이벤트
    }

    private void OnAttack(FieldCard attacker, FieldCard target)
    {
        if (attacker == _owner)
        {
            // Beast: 공격 시 자신의 체력 1 회복
            _owner.Heal(1);
        }
    }

    public void Cleanup()
    {
        CardEventBus.OnAttack -= OnAttack;
    }
}
Scripts / Abilities / SavageSpeciesAbility.cs

csharp
복사
편집
using UnityEngine;

public class SavageSpeciesAbility : IAbility
{
    public string Id => "Species_Savage";
    FieldCard _owner;

    public void Initialize(FieldCard owner, CardData data)
    {
        _owner = owner;
        CardEventBus.OnDeath += OnDeath;  // 카드 사망 시 이벤트
    }

    private void OnDeath(FieldCard dead)
    {
        if (dead == _owner)
        {
            // Savage: 사망 시 코스트 +1
            var pcm = Object.FindObjectOfType<PlayerCostManager>();
            if (pcm != null) pcm.GainCost(1);
        }
    }

    public void Cleanup()
    {
        CardEventBus.OnDeath -= OnDeath;
    }
}
나머지 종족 능력도 같은 패턴으로 구현하시면 됩니다.

4. 스킬 능력 예시
Assets/Scripts/Abilities/LifestealAbility.cs

csharp
복사
편집
using UnityEngine;

public class LifestealAbility : IAbility
{
    public string Id => "Skill_Lifesteal";
    FieldCard _owner;

    public void Initialize(FieldCard owner, CardData data)
    {
        _owner = owner;
        CardEventBus.OnAttack += OnAttack;
    }

    private void OnAttack(FieldCard attacker, FieldCard target)
    {
        if (attacker == _owner)
        {
            // 공격한 만큼 체력 회복 (data.AbilityValue 만큼 추가도 가능)
            _owner.Heal(attacker.GetAttackPower());
        }
    }

    public void Cleanup()
    {
        CardEventBus.OnAttack -= OnAttack;
    }
}
Assets / Scripts / Abilities / DeathrattleAbility.cs

csharp
복사
편집
using UnityEngine;

public class DeathrattleAbility : IAbility
{
    public string Id => "Skill_Deathrattle";
    FieldCard _owner;
    int _value;

    public void Initialize(FieldCard owner, CardData data)
    {
        _owner = owner;
        _value = data.AbilityValue; // 예: 10 데미지
        CardEventBus.OnDeath += OnDeath;
    }

    private void OnDeath(FieldCard dead)
    {
        if (dead == _owner)
        {
            // 사망 시 상대 전장 전체에 _value만큼 데미지
            var all = Object.FindObjectsOfType<FieldCard>();
            foreach (var fc in all)
            {
                if (fc.faction != _owner.faction)
                    fc.TakeDamage(_value);
            }
        }
    }

    public void Cleanup()
    {
        CardEventBus.OnDeath -= OnDeath;
    }
}
5.FieldCard 에 능력 등록 호출
FieldCard.Initialize 안에서 한 번만 호출하도록 해주세요:

csharp
복사
편집
public void Initialize(CardData data, CardFaction cardFaction)
{
    …
    RefreshUI();
    // 능력(Ability) 등록
    CardAbilityManager.Instance.RegisterAll(this, data);
}
이제:

카드가 소환되면 FieldCard.Initialize → CardAbilityManager.RegisterAll → 각 능력 Initialize (이벤트 구독 등)

공격/사망 시 CardEventBus 호출 → 구독된 IAbility 로직이 실행

카드가 필드에서 제거 직전 Cleanup 으로 이벤트 모두 해제

이 흐름만 잘 연결되면 모든 종족·스킬 능력이 정상 작동할 겁니다.
필요하신 능력을 위 예시 패턴대로 추가하시면 됩니다!