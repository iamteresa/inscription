using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// CardData ScriptableObject�� �������� �⺻ ������ �����ϰ�,
/// ��Ÿ�� ���� ���/���� �� ��ȸ ����� �����մϴ�.
/// </summary>
public class CardStatsManager : MonoBehaviour
{
    [Header("-- Base CardData List --")]
    [Tooltip("GameSettings/CardData ������ ScriptableObject ����Ʈ")]
    public List<CardData> baseCardDataList = new List<CardData>();

    // ��Ÿ�� ���� ����: ī�� GameObject ����
    private Dictionary<GameObject, RuntimeStats> runtimeStats = new Dictionary<GameObject, RuntimeStats>();

    /// <summary>
    /// CardData �̸��� �������� baseCardDataList���� �ش� ScriptableObject�� ã���ϴ�.
    /// </summary>
    public CardData GetStats(string cardName)
    {
        return baseCardDataList.Find(cd => cd.CardName == cardName);
    }

    /// <summary>
    /// ��Ÿ�ӿ� �ش� ī��(GameObject)�� ������ ����մϴ�.
    /// CardData�� Attack/Health ���� �ʱⰪ���� ����մϴ�.
    /// </summary>
    /// <param name="cardGO">����� ī�� GameObject</param>
    /// <param name="cardData">�ش� ī���� ScriptableObject</param>
    public void RegisterRuntimeStats(GameObject cardGO, CardData cardData)
    {
        if (cardData == null)
        {
            Debug.LogError("CardStatsManager: cardData�� null�Դϴ�.", this);
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
    /// ��Ÿ�� ������ ���� �� �����մϴ�.
    /// </summary>
    public void UnregisterRuntimeStats(GameObject cardGO)
    {
        runtimeStats.Remove(cardGO);
    }

    /// <summary>
    /// ��Ÿ�� ��ϵ� ī���� ������ �����ɴϴ�.
    /// </summary>
    public RuntimeStats GetRuntimeStats(GameObject cardGO)
    {
        runtimeStats.TryGetValue(cardGO, out var stats);
        return stats;
    }

    /// <summary>
    /// ��Ÿ�� ���� ����ü: ���� ü��/���ݷ°� ���� CardData ����
    /// </summary>
    public class RuntimeStats
    {
        public CardData CardData;
        public int CurrentAttack;
        public int CurrentHealth;
    }
}
