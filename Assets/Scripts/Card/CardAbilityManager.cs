using UnityEngine;

/// <summary>
/// 카드 소환 직후 IAbility 구현체를 생성·초기화하여 FieldCard에 등록합니다.
/// </summary>
public class CardAbilityManager : MonoBehaviour
{
    public static CardAbilityManager Instance { get; private set; }

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    /// <summary>FieldCard.Initialize 안에서 반드시 호출</summary>
    public void RegisterAll(FieldCard card, CardData data)
    {
        // 1) 종족 능력
        IAbility speciesAb = CreateSpeciesAbility(data.Species);
        if (speciesAb != null)
        {
            speciesAb.Initialize(card, data);
            card.RegisterAbility(speciesAb);
        }

        // 2) 스킬 능력
        IAbility skillAb = CreateSkillAbility(data.AbilityType);
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

    private IAbility CreateSkillAbility(CardData.CardAbilityType t)
    {
        switch (t)
        {
            //case CardData.CardAbilityType.Lifesteal: return new LifestealAbility();
            //case CardData.CardAbilityType.Deathrattle: return new DeathrattleAbility();
            //case CardData.CardAbilityType.Revenger: return new RevengerAbility();
            // … 추가 스킬 능력 …
            default: return null;
        }
    }
}
