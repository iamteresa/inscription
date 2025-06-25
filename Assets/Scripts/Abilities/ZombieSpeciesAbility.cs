// Assets/Scripts/Abilities/ZombieSpeciesAbility.cs
using UnityEngine;

public class ZombieSpeciesAbility : IAbility
{
    public string Id => "Zombie";
    FieldCard _owner;
    bool _revived = false;

    public void Initialize(FieldCard owner, CardData data)
    {
        _owner = owner;
        CardEventBus.OnDamaged += OnDamaged;
    }

    void OnDamaged(FieldCard target, int dmg)
    {
        // TakeDamage 후 HP가 0 이하라면 부활
        if (target == _owner && !_revived && _owner.GetCurrentHealth() - dmg <= 0)
        {
            _revived = true;
            _owner.Heal(1);
            Debug.Log($"{_owner.name}: Zombie 부활!");
        }
    }

    public void Cleanup()
    {
        CardEventBus.OnDamaged -= OnDamaged;
    }
}
