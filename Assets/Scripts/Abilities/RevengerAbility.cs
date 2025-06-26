// Assets/Scripts/Abilities/RevengerAbility.cs
using UnityEngine;

public class RevengerAbility : IAbility
{
    public string Id => "Revenger";
    private FieldCard _owner;

    public void Initialize(FieldCard owner, CardData data)
    {
        _owner = owner;
        // 공격 이벤트 구독: 내가 방금 공격을 받은 적이 있을 때
        CardEventBus.OnAttack += OnAttack;
    }

    private void OnAttack(FieldCard attacker, FieldCard defender)
    {
        // 만약 공격받은 대상이 내 주인(_owner)이면, 공격자에게 1 피해를 준다
        if (defender == _owner && attacker != null)
        {
            attacker.TakeDamage(1);
            Debug.Log($"[RevengerAbility] {_owner.name} 이/가 공격당해서 {attacker.name} 에게 1 데미지 반격!");
        }
    }

    public void Cleanup()
    {
        // 구독 해제
        CardEventBus.OnAttack -= OnAttack;
    }
}
