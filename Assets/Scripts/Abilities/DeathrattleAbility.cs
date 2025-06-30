// Assets/Scripts/Abilities/DeathrattleAbility.cs
using UnityEngine;

public class DeathrattleAbility : IAbility
{
    public string Id => "Deathrattle";
    FieldCard _owner;
    BattlefieldManager _bfm;
    EnemyCardManager _ecm;
    int _damage = 10;

    public void Initialize(FieldCard owner, CardData data)
    {
        _owner = owner;
        _bfm = Object.FindObjectOfType<BattlefieldManager>();
        _ecm = Object.FindObjectOfType<EnemyCardManager>();
        CardEventBus.OnDeath += OnDeath;
    }

    void OnDeath(FieldCard dead, CardData data)
    {
        // 사망 카드가 플레이어 진영이면 상대편(EnemyField) 같은 인덱스 슬롯 공략
        Transform targetSlot = null;
        if (_owner.faction == FieldCard.CardFaction.Player && _ecm != null)
        {
            // dead.gameObject.transform.parent 가 EnemySpawnPoints 리스트의 어떤 원소인지 찾아보자
            var parent = dead.gameObject.transform.parent;
            int idx = _ecm.EnemySpawnPoints.IndexOf(parent);
            if (idx >= 0 && idx < _ecm.EnemySpawnPoints.Count)
                targetSlot = _ecm.EnemySpawnPoints[idx];
        }
        // 반대로 적 진영 카드가 죽었을 때 플레이어 필드 타겟
        else if (_owner.faction == FieldCard.CardFaction.Enemy && _bfm != null)
        {
            var parent = dead.gameObject.transform.parent;
            int idx = _bfm.SpawnPoints.IndexOf(parent);
            if (idx >= 0 && idx < _bfm.SpawnPoints.Count)
                targetSlot = _bfm.SpawnPoints[idx];
        }

        // 그 슬롯에 카드가 있으면 한 장만 타격
        if (targetSlot != null && targetSlot.childCount > 0)
        {
            var go = targetSlot.GetChild(0).gameObject;
            var fc = go.GetComponent<FieldCard>();
            if (fc != null)
            {
                fc.TakeDamage(_damage);
                Debug.Log($"[Deathrattle] {_owner.name} 사망 → {fc.name} 에게 {_damage} 데미지!");
            }
        }
    }

    public void Cleanup()
    {
        CardEventBus.OnDeath -= OnDeath;
    }
}
