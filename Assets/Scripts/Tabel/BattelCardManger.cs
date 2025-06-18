using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ���忡 ��ġ�� ī�� ��ϸ� ����ϸ�,
/// CardStatsManager�� ���� Stat �ʱ�ȭ �� ������ �����մϴ�.
/// </summary>
public class BattleCardManager : MonoBehaviour
{
    [Header("-- CardStatsManager ���� --")]
    [SerializeField] private CardStatsManager statsManager;

    // ���忡 ��ġ�� ī�� ���
    private List<GameObject> battleCards = new List<GameObject>();

    /// <summary>
    /// ī�尡 ���忡 ��ȯ�� �� ȣ���մϴ�.
    /// CardStatsManager���� �ʱ� Stat ����� �����մϴ�.
    /// </summary>
    public void RegisterCard(GameObject cardGO)
    {
        if (battleCards.Contains(cardGO)) return;

        // CardDisplay���� CardData ��ȸ
        var display = cardGO.GetComponent<CardDisplay>();
        var data = display?.GetCardData();
        if (data == null)
        {
            Debug.LogWarning($"BattleCardManager: CardData�� ���� ����� �� �����ϴ�. {cardGO.name}");
            return;
        }

        // �⺻ Stat�� ScriptableObject �������� ������ ��Ÿ�ӿ� ���
        var baseData = statsManager.GetStats(data.CardName);
        if (baseData == null)
        {
            Debug.LogWarning($"CardStatsManager: '{data.CardName}' Stat ������ �����ϴ�.");
            return;
        }

        statsManager.RegisterRuntimeStats(cardGO, baseData);

        // ��Ͽ� �߰�
        battleCards.Add(cardGO);
        Debug.Log($"[BattleCardManager] ī�� ���: {data.CardName}");
    }

    /// <summary>
    /// ī�尡 ���忡�� ���ŵ� �� ȣ���մϴ�.
    /// </summary>
    public void UnregisterCard(GameObject cardGO)
    {
        if (!battleCards.Remove(cardGO)) return;

        statsManager.UnregisterRuntimeStats(cardGO);
        Debug.Log($"[BattleCardManager] ī�� ����: {cardGO.name}");
    }

    /// <summary>
    /// ���忡 ��ϵ� ��� ī�� GameObject�� ��ȯ�մϴ�.
    /// </summary>
    public IEnumerable<GameObject> GetAllBattleCards() => battleCards;
}
