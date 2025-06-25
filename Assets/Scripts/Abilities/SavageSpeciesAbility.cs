// Assets/Scripts/Abilities/SavageSpeciesAbility.cs
using UnityEngine;

public class SavageSpeciesAbility : IAbility
{
    public string Id => "Savage";
    private FieldCard _owner;

    public void Initialize(FieldCard owner, CardData data)
    {
        _owner = owner;
        // 이벤트 구독: 두 파라미터를 받는 람다를 등록
        CardEventBus.OnDeath += HandleOnDeath;
    }

    private void HandleOnDeath(FieldCard dead, CardData data)
    {
        // 죽은 카드가 내 오너 카드이고, 종족이 Savage 라면 코스트 획득
        if (dead == _owner && data.Species == CardData.CardSpecies.Savage )
        {
            var pcm = Object.FindObjectOfType<PlayerCostManager>();
            if (pcm != null)
                pcm.GainCost(1);
        }
    }

    public void Cleanup()
    {
        // 반드시 같은 델리게이트를 해제해야 합니다.
        CardEventBus.OnDeath -= HandleOnDeath;
    }
}
