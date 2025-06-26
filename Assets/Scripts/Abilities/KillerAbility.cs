// Assets/Scripts/Abilities/KillerAbility.cs
using UnityEngine;

/// <summary>
/// Killer: 공격 시 상대 카드가 즉사합니다.
/// </summary>
public class KillerAbility : IAbility
{
    public string Id => "Killer";
    private FieldCard _owner;

    public void Initialize(FieldCard owner, CardData data)
    {
        _owner = owner;
        CardEventBus.OnAttack += OnAttack;
    }

    private void OnAttack(FieldCard attacker, FieldCard defender)
    {
        if (attacker != _owner) return;
        if (defender == null) return;

        // 상대 체력을 강제로 0으로 만들어 즉사시킨다
        int hp = defender.GetCurrentHealth();
        if (hp > 0)
        {
            defender.TakeDamage(hp);
            Debug.Log($"[Killer] {_owner.name} 이(가) {defender.name} 을(를) 즉사시켰습니다.");
        }
    }

    public void Cleanup()
    {
        CardEventBus.OnAttack -= OnAttack;
    }
}
