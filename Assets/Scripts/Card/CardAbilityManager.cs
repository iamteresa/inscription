using System.Collections.Generic;
using UnityEngine;

public class CardAbilityManager : MonoBehaviour
{
    public static CardAbilityManager Instance { get; private set; }

    void Awake() => Instance = this;

    public void RegisterAll(FieldCard card, CardData data)
    {
        // 종족 능력
        IAbility speciesAb = CreateSpeciesAbility(data.Species);
        if (speciesAb != null)
        {
            speciesAb.Initialize(card, data);
            card.RegisterAbility(speciesAb);
        }

        // 스킬 능력
        IAbility skillAb = CreateAbility(data.AbilityType);
        if (skillAb != null)
        {
            skillAb.Initialize(card, data);
            card.RegisterAbility(skillAb);
        }
    }

    IAbility CreateSpeciesAbility(CardData.CardSpecies s)
    {
        switch (s)
        {
            case CardData.CardSpecies.Beast: return new BeastSpeciesAbility();
            case CardData.CardSpecies.Undead: return new UndeadSpeciesAbility();
            case CardData.CardSpecies.Machine: return new MachineSpeciesAbility();
            case CardData.CardSpecies.Zombie: return new ZombieSpeciesAbility();
            case CardData.CardSpecies.Vampire: return new VampireSpeciesAbility();
            case CardData.CardSpecies.Dragon: return new DragonSpeciesAbility();
            case CardData.CardSpecies.Savage: return new SavageSpeciesAbility();
            //나머지 종족은 필요 시 추가...
            default: return null;
        }
    }

    IAbility CreateAbility(CardData.CardAbilityType t)
    {
        switch (t)
        {
            //case CardData.CardAbilityType.Lifesteal: return new LifestealAbility();
            //case CardData.CardAbilityType.Deathrattle: return new DeathrattleAbility();
            //case CardData.CardAbilityType.Revenger: return new RevengerAbility();
            //// 나머지 추가 능력도 동일 패턴으로…
            default: return null;
        }
    }
}