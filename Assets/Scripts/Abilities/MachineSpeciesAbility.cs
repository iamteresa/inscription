// MachineSpeciesAbility.cs
using UnityEngine;

public class MachineSpeciesAbility : IAbility
{
    public string Id => "Machine";
    FieldCard _owner;

    public void Initialize(FieldCard owner, CardData data)
    {
        _owner = owner;
        CardEventBus.OnDamaged += OnDamaged;
    }

    void OnDamaged(FieldCard target, int dmg)
    {
        if (target == _owner)
            _owner.Heal(1); // 받는 데미지 1 감소
                            // 힐 1로 상쇄
    }

    public void Cleanup()
    {
        CardEventBus.OnDamaged -= OnDamaged;
    }
}
