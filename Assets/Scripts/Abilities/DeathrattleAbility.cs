// Assets/Scripts/Abilities/DeathrattleAbility.cs
using UnityEngine;
using System.Collections.Generic;

public class DeathrattleAbility : IAbility
{
    public string Id => "Deathrattle";
    private FieldCard _owner;
    private int _damage = 10; // 고정 10 데미지

    public void Initialize(FieldCard owner, CardData data)
    {
        _owner = owner;
        // 카드가 죽을 때 불러줄 이벤트 (CardEventBus.OnDeath(Card, CardData) 를 사용중이라 가정)
        CardEventBus.OnDeath += OnDeath;
    }

    private void OnDeath(FieldCard dead, CardData data)
    {
        if (dead != _owner) return;  // 자기 자신이 죽었을 때만 발동

        // 내 카드인지 상대 카드인지에 따라 타겟 필드 결정
        bool imPlayer = _owner.faction == FieldCard.CardFaction.Player;
        List<Transform> slots;

        if (imPlayer)
        {
            // 내가 플레이어일 때는 적 진영을 노린다
            var ecm = Object.FindObjectOfType<EnemyCardManager>();
            if (ecm == null) return;
            slots = (List<Transform>)ecm.EnemySpawnPoints;
        }
        else
        {
            // 내가 적일 때는 플레이어 진영을 노린다
            var bfm = Object.FindObjectOfType<BattlefieldManager>();
            if (bfm == null) return;
            slots = bfm.SpawnPoints;
        }

        // “앞에서 첫 번째로 자리 잡고 있는 카드” 한 장을 찾아서 데미지
        foreach (var slot in slots)
        {
            if (slot.childCount > 0)
            {
                var targetGO = slot.GetChild(0).gameObject;
                var fc = targetGO.GetComponent<FieldCard>();
                if (fc != null)
                {
                    fc.TakeDamage(_damage);
                    Debug.Log($"[Deathrattle] {_owner.name} 사망 → {fc.name}에 {_damage} 데미지!");
                }
                break;
            }
        }
    }

    public void Cleanup()
    {
        CardEventBus.OnDeath -= OnDeath;
    }
}
