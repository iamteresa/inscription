// Assets/Scripts/Abilities/DeathrattleAbility.cs
using UnityEngine;

public class DeathrattleAbility : IAbility
{
    public string Id => "Deathrattle";
    private FieldCard _owner;
    private int _damage = 10;

    public void Initialize(FieldCard owner, CardData data)
    {
        _owner = owner;
        // CardData.AbilityValue 로 대미지를 조정하고 싶으면 여기를 바꾸세요.
        CardEventBus.OnDeath += OnDeath;
    }

    private void OnDeath(FieldCard dead, CardData data)
    {
        if (dead != _owner) return;

        // 1) 매니저 가져오기
        var bfm = Object.FindObjectOfType<BattlefieldManager>();
        var ecm = Object.FindObjectOfType<EnemyCardManager>();
        if (bfm == null || ecm == null) return;

        // 2) 죽은 카드의 슬롯 인덱스 찾기
        int idx;
        Transform targetSlot = null;

        if (_owner.faction == FieldCard.CardFaction.Player)
        {
            idx = bfm.GetSlotIndex(dead.gameObject);
            // 플레이어가 죽었으면, 적 필드의 같은 인덱스
            if (idx >= 0 && idx < ecm.EnemySpawnPoints.Count)
                targetSlot = ecm.EnemySpawnPoints[idx];
        }
        else
        {
            idx = ecm.GetSlotIndex(dead.gameObject);
            // 적이 죽었으면, 플레이어 필드의 같은 인덱스
            if (idx >= 0 && idx < bfm.SpawnPoints.Count)
                targetSlot = bfm.SpawnPoints[idx];
        }

        // 3) 그 슬롯에 카드가 있으면 한 장만 때리기
        if (targetSlot != null && targetSlot.childCount > 0)
        {
            var go = targetSlot.GetChild(0).gameObject;
            var fc = go.GetComponent<FieldCard>();
            if (fc != null)
            {
                fc.TakeDamage(_damage);
                Debug.Log($"[Deathrattle] {_owner.name} 사망 → {fc.name} 에게 {_damage} 대미지!");
            }
        }
    }

    public void Cleanup()
    {
        CardEventBus.OnDeath -= OnDeath;
    }
}
