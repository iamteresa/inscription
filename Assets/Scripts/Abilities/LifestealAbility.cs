// Assets/Scripts/Abilities/LifestealAbility.cs
using UnityEngine;

/// <summary>
/// Lifesteal: 공격 시 적 또는 플레이어에게 가한 대미지만큼 자신의 체력을 회복합니다.
/// </summary>
public class LifestealAbility : IAbility
{
    public string Id => "Lifesteal";
    private FieldCard _owner;
    private int _healAmount;

    public void Initialize(FieldCard owner, CardData data)
    {
        _owner = owner;
        // CardData.AbilityValue 에 회복량(예: 3)을 넣어두세요.
        _healAmount = data.AbilityValue > 0 ? data.AbilityValue : data.Attack;
        CardEventBus.OnAttack += OnAttack;
    }

    private void OnAttack(FieldCard attacker, FieldCard defender)
    {
        if (attacker != _owner) return;

        // 대미지를 가한 뒤에 체력 회복
        _owner.Heal(_healAmount);
        Debug.Log($"[Lifesteal] {_owner.name} 이(가) 공격 후 {_healAmount}만큼 체력 회복했습니다.");
    }

    public void Cleanup()
    {
        CardEventBus.OnAttack -= OnAttack;
    }
}
