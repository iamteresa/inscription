using System.Collections.Generic;
using UnityEngine;

public class CardAbilityManager : MonoBehaviour
{
    public static CardAbilityManager Instance { get; private set; }

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    /// <summary>
    /// FieldCard.Initialize 직후에 호출하세요.
    /// </summary>
    public void RegisterAbilities(FieldCard card, CardData data)
    {
        if (data.AbilityType == CardData.CardAbilityType.None) return;

        IAbility ability = CreateAbility(data.AbilityType);
        if (ability == null) return;

        ability.Initialize(card, data);
       // card.RegisterAbility(ability);
    }

    IAbility CreateAbility(CardData.CardAbilityType t)
    {
        switch (t)
        {
            //case CardData.CardAbilityType.Lifesteal: return new LifestealAbility();
            //case CardData.CardAbilityType.Deathrattle: return new DeathrattleAbility();
            //case CardData.CardAbilityType.Revenger: return new RevengerAbility();
            //case CardData.CardAbilityType.Poisoner: return new PoisonerAbility();
            // case … 나머지 능력들 구현 …
            default: return null;
        }
    }
}
