// Assets/Scripts/Abilities/DragonSpeciesAbility.cs
using UnityEngine;

public class DragonSpeciesAbility : IAbility
{
    public string Id => "Dragon";
    FieldCard _owner;

    public void Initialize(FieldCard owner, CardData data)
    {
        _owner = owner;
        CardEventBus.OnAttack += OnAttack;
    }

    void OnAttack(FieldCard attacker, FieldCard defender)
    {
        if (attacker == _owner && defender != null)
        {
            Debug.LogWarning($"{_owner.name}: Dragon 종족은 다른 카드를 공격할 수 없습니다!");
            // 데미지를 되돌리기 위해
            defender.Heal(attacker.GetAttackPower());
        }
    }

    public void Cleanup()
    {
        CardEventBus.OnAttack -= OnAttack;
    }
}
