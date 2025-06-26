// Assets/Scripts/Abilities/MoverAbility.cs
using UnityEngine;

public class MoverAbility : IAbility
{
    public string Id => "Mover";
    FieldCard _owner;
    BattlefieldManager _bfm;

    public void Initialize(FieldCard owner, CardData data)
    {
        _owner = owner;
        // 필드 매니저 참조
        _bfm = Object.FindObjectOfType<BattlefieldManager>();
        CardEventBus.OnAttack += OnAttack;
    }

    void OnAttack(FieldCard attacker, FieldCard defender)
    {
        if (attacker != _owner || _bfm == null) return;

        // 내 슬롯 인덱스 조회
        int idx = _bfm.GetSlotIndex(attacker.gameObject);
        // 오른쪽 이동 시도
        if (_bfm.TryMoveTo(idx, idx + 1, attacker.gameObject)) return;
        // 실패하면 왼쪽 이동
        _bfm.TryMoveTo(idx, idx - 1, attacker.gameObject);
    }

    public void Cleanup()
    {
        CardEventBus.OnAttack -= OnAttack;
    }
}
