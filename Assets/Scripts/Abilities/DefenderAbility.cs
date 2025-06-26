// Assets/Scripts/Abilities/DefenderAbility.cs
using UnityEngine;

/// <summary>
/// Defender 능력: 전투에서 자신이 처음 받는 데미지를 1회 무시합니다.
/// </summary>
public class DefenderAbility : IAbility
{
    public string Id => "Defender";

    private FieldCard _owner;
    private bool _used = false;

    /// <summary>
    /// 소환 직후 호출됩니다. 자신에 대한 OnDamaged 이벤트를 구독합니다.
    /// </summary>
    public void Initialize(FieldCard owner, CardData data)
    {
        _owner = owner;
        CardEventBus.OnDamaged += HandleDamaged;
    }

    /// <summary>
    /// OnDamaged 이벤트 핸들러.
    /// 자신이 처음 맞는 데미지를 무시(Heal)하고, 이후엔 더 이상 동작하지 않도록 구독 해제합니다.
    /// </summary>
    private void HandleDamaged(FieldCard target, int dmg)
    {
        if (_used) return;              // 이미 1회 무시했으면 아무것도 하지 않음
        if (target != _owner) return;   // 내가 아닌 다른 카드면 패스
        if (dmg <= 0) return;     // 데미지 양이 없으면 패스

        // 받은 데미지만큼 즉시 회복시켜서 무시 효과를 냅니다.
        _owner.Heal(dmg);
        Debug.Log($"[Defender] {_owner.name}이(가) 첫 번째 공격 {dmg}를 무시했습니다.");

        // 1회 사용 플래그 세우고, 더 이상 구독 해제
        _used = true;
        CardEventBus.OnDamaged -= HandleDamaged;
    }

    /// <summary>
    /// 카드가 전장에서 제거되기 직전에 호출됩니다.
    /// 혹시 남아 있는 이벤트 구독이 있으면 해제합니다.
    /// </summary>
    public void Cleanup()
    {
        CardEventBus.OnDamaged -= HandleDamaged;
    }
}
