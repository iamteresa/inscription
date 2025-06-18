using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// CardData ScriptableObject를 기준으로 기본 스탯을 관리하고,
/// 런타임 스탯 등록/해제 및 조회 기능을 제공합니다.
/// </summary>
public class CardStatsManager : MonoBehaviour
{
    [Header("-- Base CardData List --")]
    [Tooltip("GameSettings/CardData 형태의 ScriptableObject 리스트")]
    public List<CardData> baseCardDataList = new List<CardData>();

    // 런타임 스탯 저장: 카드 GameObject 기준
    private Dictionary<GameObject, RuntimeStats> runtimeStats = new Dictionary<GameObject, RuntimeStats>();

    /// <summary>
    /// CardData 이름을 기준으로 baseCardDataList에서 해당 ScriptableObject를 찾습니다.
    /// </summary>
    public CardData GetStats(string cardName)
    {
        return baseCardDataList.Find(cd => cd.CardName == cardName);
    }

    /// <summary>
    /// 런타임에 해당 카드(GameObject)의 스탯을 등록합니다.
    /// CardData의 Attack/Health 값을 초기값으로 사용합니다.
    /// </summary>
    /// <param name="cardGO">등록할 카드 GameObject</param>
    /// <param name="cardData">해당 카드의 ScriptableObject</param>
    public void RegisterRuntimeStats(GameObject cardGO, CardData cardData)
    {
        if (cardData == null)
        {
            Debug.LogError("CardStatsManager: cardData가 null입니다.", this);
            return;
        }
        if (runtimeStats.ContainsKey(cardGO)) return;

        runtimeStats[cardGO] = new RuntimeStats
        {
            CardData = cardData,
            CurrentAttack = cardData.Attack,
            CurrentHealth = cardData.Health
        };
    }

    /// <summary>
    /// 런타임 스탯을 해제 및 삭제합니다.
    /// </summary>
    public void UnregisterRuntimeStats(GameObject cardGO)
    {
        runtimeStats.Remove(cardGO);
    }

    /// <summary>
    /// 런타임 등록된 카드의 스탯을 가져옵니다.
    /// </summary>
    public RuntimeStats GetRuntimeStats(GameObject cardGO)
    {
        runtimeStats.TryGetValue(cardGO, out var stats);
        return stats;
    }

    /// <summary>
    /// 런타임 스탯 구조체: 현재 체력/공격력과 원본 CardData 참조
    /// </summary>
    public class RuntimeStats
    {
        public CardData CardData;
        public int CurrentAttack;
        public int CurrentHealth;
    }
}
