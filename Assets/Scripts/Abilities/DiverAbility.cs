// Assets/Scripts/Abilities/DiverAbility.cs
using UnityEngine;

/// <summary>
/// Diver: 카드의 공격을 무시(회복)하고, 대신 상대 플레이어에게 동일한 대미지를 되돌려줍니다.
/// </summary>
public class DiverAbility : IAbility
{
    public string Id => "Diver";
    private FieldCard _owner;

    public void Initialize(FieldCard owner, CardData data)
    {
        _owner = owner;
        CardEventBus.OnDamaged += OnDamaged;
    }

    private void OnDamaged(FieldCard target, int dmg)
    {
        if (target != _owner || dmg <= 0) return;

        // 받은 대미지를 즉시 회복 → 무시 효과
        _owner.Heal(dmg);

        // 적 플레이어에게 반사 대미지
        var enemyHp = Object.FindObjectOfType<PlayerHpManger>();
        if (enemyHp != null)
        {
            enemyHp.TakeDamage(dmg);
            Debug.Log($"[Diver] {_owner.name} 이(가) 카드 공격 {dmg}를 무시하고 적 플레이어에게 반사했습니다.");
        }
    }

    public void Cleanup()
    {
        CardEventBus.OnDamaged -= OnDamaged;
    }
}
