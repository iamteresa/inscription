using UnityEngine;

/// <summary>
/// 카드 소환 직후 IAbility 구현체를 생성·초기화하여 FieldCard에 등록합니다.
/// </summary>
public class CardAbilityManager : MonoBehaviour
{


    public static CardAbilityManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this) Destroy(this);
        else Instance = this;
    }

    /// <summary>
    /// 새로 생성된 FieldCard에 능력을 등록할 때 호출됩니다.
    /// </summary>
    public void RegisterAll(FieldCard card, CardData data)
    {
        // 1) 종족(species) 능력
        var speciesAb = CreateSpeciesAbility(data.Species);
        if (speciesAb != null)
        {
            speciesAb.Initialize(card, data);
            card.RegisterAbility(speciesAb);

        }
        // 2) 스킬(AbilityType) 능력
        var skillAb = CreateSkillAbility(data.AbilityType);
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
            case CardData.CardAbilityType.Lifesteal: return new LifestealAbility();
            case CardData.CardAbilityType.Deathrattle: return new DeathrattleAbility();
            case CardData.CardAbilityType.Revenger: return new RevengerAbility();
            case CardData.CardAbilityType.Defender: return new DefenderAbility();
            case CardData.CardAbilityType.Flyer: return new FlyerAbility();
            case CardData.CardAbilityType.Killer: return new KillerAbility();
            case CardData.CardAbilityType.Weaker: return new WeakerAbility();
            case CardData.CardAbilityType.GoblinRoad: return new GoblinRoadAbility();   //_goblinRoadAbility
            case CardData.CardAbilityType.Poisoner: return new PoisonerAbility();
            case CardData.CardAbilityType.Mover: return new MoverAbility();
            case CardData.CardAbilityType.Diver: return new DiverAbility();
            

            // … 추가 스킬 능력 …
            default: return null;
        }
    }
}
