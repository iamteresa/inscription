using UnityEngine;
public class FlyerAbility : IAbility
{
    public string Id => "Flyer";
    FieldCard _owner;
    PlayerHpManger _enemyHp;

    public void Initialize(FieldCard owner, CardData data)
    {
        _owner = owner;
        _enemyHp = Object.FindObjectOfType<PlayerHpManger>();
        CardEventBus.OnAttack += OnAttack;
    }

    void OnAttack(FieldCard attacker, FieldCard defender)
    {
        if (attacker != _owner && _enemyHp != null) return;
        // 무조건 플레이어에게
        _enemyHp.TakeDamage(attacker.GetAttackPower());
    }

    public void Cleanup()
    {
        CardEventBus.OnAttack -= OnAttack;
    }
}
