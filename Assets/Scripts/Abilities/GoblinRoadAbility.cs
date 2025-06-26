// Assets/Scripts/Abilities/GoblinRoadAbility.cs
using System.Collections.Generic;
using UnityEngine;               // ← 이거 반드시 추가


public class GoblinRoadAbility : IAbility
{
    public string Id => "GoblinRoad";

    FieldCard _owner;
    BattlefieldManager _bfm;

    [Header("GoblinRoad 설정")]
    [SerializeField] private CardData _goblinCardData;
    [SerializeField] private GameObject _goblinPrefab;

    public void Initialize(FieldCard owner, CardData data)
    {
        _owner = owner;
        // FindObjectOfType 은 UnityEngine.Object 의 확장 메서드이므로 위에 using UnityEngine; 이 있어야 인식됩니다
        _bfm = Object.FindObjectOfType<BattlefieldManager>();

        CardEventBus.OnSummon += OnSummon;
    }

    void OnSummon(FieldCard summoned)
    {
        if (summoned != _owner) return;

        int idx = _bfm.GetSlotIndex(_owner.gameObject);
        var slots = _bfm.SpawnPoints;

        for (int dir = -1; dir <= 1; dir += 2)
        {
            int ni = idx + dir;
            if (ni < 0 || ni >= slots.Count) continue;
            if (slots[ni].childCount != 0) continue;

            var go = GameObject.Instantiate(_goblinPrefab, slots[ni], false);
            var fc = go.GetComponent<FieldCard>();
            // 여기서 _goblinCardData 를 넘겨주셔야 고블린도 스탯이 따라옵니다
            fc.Initialize(_goblinCardData, _owner.faction);
            _bfm.RegisterCardAtSlot(ni, go);
        }
    }

    public void Cleanup()
    {
        CardEventBus.OnSummon -= OnSummon;
    }
}
