using UnityEngine;

public class VampireSpeciesAbility : IAbility
{
    public string Id => "Vampire";
    FieldCard _owner;
    int _value;

    public void Initialize(FieldCard owner, CardData data)
    {
        _owner = owner;
        _value = data.AbilityValue;
        CardEventBus.OnAttack += OnAttack;
    }

    void OnAttack(FieldCard attacker, FieldCard defender)
    {
        if (attacker == _owner)
            _owner.Heal(_value);
    }

    public void Cleanup()
    {
        CardEventBus.OnAttack -= OnAttack;
    }
}