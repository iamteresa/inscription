using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 전장에 배치된 카드 등록만 담당하며,
/// CardStatsManager를 통해 Stat 초기화 및 해제를 위임합니다.
/// </summary>
public class BattleCardManager : MonoBehaviour
{
    [Header("-- CardStatsManager 참조 --")]
    [SerializeField] private CardStatsManager statsManager;

    // 전장에 배치된 카드 목록
    private List<GameObject> battleCards = new List<GameObject>();

    /// <summary>
    /// 카드가 전장에 소환될 때 호출합니다.
    /// CardStatsManager에서 초기 Stat 등록을 수행합니다.
    /// </summary>
    public void RegisterCard(GameObject cardGO)
    {
        if (battleCards.Contains(cardGO)) return;

        // CardDisplay에서 CardData 조회
        var display = cardGO.GetComponent<CardDisplay>();
        var data = display?.GetCardData();
        if (data == null)
        {
            Debug.LogWarning($"BattleCardManager: CardData가 없어 등록할 수 없습니다. {cardGO.name}");
            return;
        }

        // 기본 Stat을 ScriptableObject 기준으로 가져와 런타임에 등록
        var baseData = statsManager.GetStats(data.CardName);
        if (baseData == null)
        {
            Debug.LogWarning($"CardStatsManager: '{data.CardName}' Stat 정보가 없습니다.");
            return;
        }

        statsManager.RegisterRuntimeStats(cardGO, baseData);

        // 목록에 추가
        battleCards.Add(cardGO);
        Debug.Log($"[BattleCardManager] 카드 등록: {data.CardName}");
    }

    /// <summary>
    /// 카드가 전장에서 제거될 때 호출합니다.
    /// </summary>
    public void UnregisterCard(GameObject cardGO)
    {
        if (!battleCards.Remove(cardGO)) return;

        statsManager.UnregisterRuntimeStats(cardGO);
        Debug.Log($"[BattleCardManager] 카드 해제: {cardGO.name}");
    }

    /// <summary>
    /// 전장에 등록된 모든 카드 GameObject를 반환합니다.
    /// </summary>
    public IEnumerable<GameObject> GetAllBattleCards() => battleCards;
}
