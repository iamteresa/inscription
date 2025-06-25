using UnityEngine;

public class BeastSpeciesAbility : IAbility
{
    public string Id => "Beast";
    FieldCard _owner;

    public void Initialize(FieldCard owner, CardData data)
    {
        _owner = owner;
        CardEventBus.OnAttack += OnAttack;
    }

    void OnAttack(FieldCard atk, FieldCard def)
    {
        if (atk == _owner)
            _owner.Heal(1); // Áü½Â: °ø°Ý½Ã Ã¼·Â 1 È¸º¹
    }

    public void Cleanup()
    {
        CardEventBus.OnAttack -= OnAttack;
    }
}