// UndeadSpeciesAbility.cs
using UnityEngine;

public class UndeadSpeciesAbility : IAbility
{
    public string Id => "Undead";
    FieldCard _owner;

    public void Initialize(FieldCard owner, CardData data)
    {
        _owner = owner;
        CardEventBus.OnAttack += OnAttack;
    }

    void OnAttack(FieldCard atk, FieldCard def)
    {
        if (atk == _owner)
            _owner.TakeDamage(1); // 언데드: 공격시 자신 체력 1 감소
    }

    public void Cleanup()
    {
        CardEventBus.OnAttack -= OnAttack;
    }
}
