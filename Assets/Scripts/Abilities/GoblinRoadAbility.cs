// Assets/Scripts/Abilities/GoblinRoadAbility.cs
using UnityEngine;

/// <summary>
/// GoblinRoadAbility: 이 카드를 소환하면 양 옆 슬롯에 배치된 카드들의 공격력을 영구 +1합니다.
/// </summary>
public class GoblinRoadAbility : IAbility
{
    public string Id => "GoblinRoad";
    private FieldCard _owner;
    private BattlefieldManager _bfm;

    public void Initialize(FieldCard owner, CardData data)
    {
        _owner = owner;
        // 슬롯 인덱스를 가져오기 위한 매니저
        _bfm = Object.FindObjectOfType<BattlefieldManager>();
        // 소환 이벤트 구독
        CardEventBus.OnSummon += OnSummon;
    }

    private void OnSummon(FieldCard summoned)
    {
        // “내” 카드가 소환된 경우에만 실행
        if (summoned != _owner || _bfm == null) return;

        // 내 슬롯 인덱스 조회
        int idx = _bfm.GetSlotIndex(_owner.gameObject);
        var slots = _bfm.SpawnPoints;

        // 왼쪽(-1), 오른쪽(+1) 두 방향 체크
        for (int dir = -1; dir <= 1; dir += 2)
        {
            int ni = idx + dir;
            if (ni < 0 || ni >= slots.Count) continue;          // 범위 밖
            var slot = slots[ni];
            if (slot.childCount == 0) continue;                 // 빈 슬롯은 패스

            // 슬롯의 카드 가져오기
            var neighborGO = slot.GetChild(0).gameObject;
            var neighborFC = neighborGO.GetComponent<FieldCard>();
            if (neighborFC == null) continue;

            // 공격력 +1 (영구 반영)
            int oldAtk = neighborFC.GetAttackPower();
            neighborFC.SetAttackPower(oldAtk + 1);
            Debug.Log($"[GoblinRoad] {_owner.name} 소환으로 {neighborGO.name} 공격력 {oldAtk} → {oldAtk + 1}");
        }
    }

    public void Cleanup()
    {
        // 구독 해제
        CardEventBus.OnSummon -= OnSummon;
    }
}
