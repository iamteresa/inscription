// Assets/Scripts/Abilities/WeakerAbility.cs
using UnityEngine;

public class WeakerAbility : IAbility
{
    public string Id => "Weaker";
    private FieldCard _owner;

    public void Initialize(FieldCard owner, CardData data)
    {
        _owner = owner;
        // CardEventBus.OnTurnStart(Action<CardFaction>) 시그니처에 맞게 구독
        CardEventBus.OnTurnStart += OnTurnStart;
    }

    // 반드시 CardFaction 파라미터를 하나 받아야 합니다!
    private void OnTurnStart(FieldCard.CardFaction faction)
    {
        // 내 진영의 턴일 때만 발동
        if (faction != _owner.faction) return;

        int atk = _owner.GetAttackPower();
        // 턴마다 공격력을 2씩 줄이되 최소 1 유지
        int newAtk = Mathf.Max(1, atk - 2);
        _owner.SetAttackPower(newAtk);
        Debug.Log($"[WeakerAbility] {_owner.name} 공격력 감소: {atk} → {newAtk}");
    }

    public void Cleanup()
    {
        CardEventBus.OnTurnStart -= OnTurnStart;
    }
}
