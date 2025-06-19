using UnityEngine;
using UnityEngine.UI;               // RectTransform ����� ����
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// �� ���� �ʵ� ī�� ���� �Ŵ���
/// 4���� ���Կ� �� ī�带 ��ȯ�������ϸ�, ���Կ� ���� ��ġ��ũ����� �ڵ� �����մϴ�.
/// </summary>
public class EnemyCardManager : MonoBehaviour
{
    [Header("----- �� ī�� ��ġ ����(4��) -----")]
    [SerializeField] private List<Transform> enemySpawnPoints = new List<Transform>();

    [Header("----- �ʵ� ī�� ������ -----")]
    [SerializeField] private GameObject fieldCardPrefab;

    [Header("----- �� �� (CardData ����Ʈ) -----")]
    [SerializeField] private List<CardData> enemyDeck = new List<CardData>();

    [Header("=== ī�� ��ġ & ũ�� ���� ===")]
    [Tooltip("��ȯ �� ���� ��ġ�� ������ ������")]
    [SerializeField] private Vector3 cardPositionOffset = Vector3.zero;
    [Tooltip("ī���� ���� localScale")]
    [SerializeField] private Vector3 cardScale = Vector3.one;

    // ���� ���� ���� ������
    private GameObject[] occupiedEnemySlots;

    void Awake()
    {
        if (enemySpawnPoints == null || enemySpawnPoints.Count == 0)
        {
            Debug.LogError("EnemyCardManager: ������ �������� �ʾҽ��ϴ�!", this);
            enabled = false;
            return;
        }
        occupiedEnemySlots = new GameObject[enemySpawnPoints.Count];
    }

    /// <summary>
    /// ������ �������� �� ���� �̾� �� ���Կ� ��ȯ�մϴ�.
    /// ������ ���ų� ���� ��� ������ false�� ��ȯ�մϴ�.
    /// </summary>
    public bool DrawAndSpawnEnemyCard()
    {
        if (enemyDeck == null || enemyDeck.Count == 0)
        {
            Debug.LogWarning("EnemyCardManager: ���� ī�尡 �����ϴ�.");
            return false;
        }

        // 1) �� ���� ã��
        int freeIndex = -1;
        for (int i = 0; i < occupiedEnemySlots.Length; i++)
        {
            if (occupiedEnemySlots[i] == null)
            {
                freeIndex = i;
                break;
            }
        }
        if (freeIndex < 0)
        {
            Debug.Log("EnemyCardManager: �� ������ �����ϴ�.");
            return false;
        }

        // 2) ������ ���� ī�� ���� �� ������ ����
        int idx = Random.Range(0, enemyDeck.Count);
        CardData data = enemyDeck[idx];
        enemyDeck.RemoveAt(idx);

        // 3) ������ �ν��Ͻ�ȭ (�θ�� ���� Transform)
        GameObject go = Instantiate(fieldCardPrefab, enemySpawnPoints[freeIndex], false);

        // 4) ���Կ� BattlefieldSlot ������Ʈ�� ����� ��ġ��ũ�� ����
        var slotComp = enemySpawnPoints[freeIndex].GetComponent<BattlefieldSlot>();
        var rect = go.GetComponent<RectTransform>();
        if (slotComp != null && rect != null)
        {
            slotComp.ApplyPlacementTransform(rect);
        }
        else
        {
            // ���� ������Ʈ�� RectTransform�� ������ �⺻ ������/�����ϸ� ����
            go.transform.localPosition += cardPositionOffset;
            go.transform.localScale = cardScale;
        }

        // 5) FieldCard �ʱ�ȭ
        FieldCard fc = go.GetComponent<FieldCard>();
        if (fc == null)
        {
            Debug.LogError("EnemyCardManager: FieldCard ������Ʈ�� �����ϴ�!", go);
            Destroy(go);
            return false;
        }
        fc.Initialize(data, FieldCard.CardFaction.Enemy);

        // 6) ���� ���� ���� ���
        occupiedEnemySlots[freeIndex] = go;
        Debug.Log($"EnemyCardManager: ���� {freeIndex}�� '{data.CardName}' ��ȯ (offset:{cardPositionOffset}, scale:{cardScale})");

        return true;
    }

    /// <summary>
    /// Ư�� �� ī�带 ���Կ��� �����ϰ� ������Ʈ�� �ı��մϴ�.
    /// </summary>
    public void RemoveEnemyCard(GameObject cardGO)
    {
        for (int i = 0; i < occupiedEnemySlots.Length; i++)
        {
            if (occupiedEnemySlots[i] == cardGO)
            {
                occupiedEnemySlots[i] = null;
                if (Application.isPlaying)
                    Destroy(cardGO);
                Debug.Log($"EnemyCardManager: ���� {i}�� ī�带 �����߽��ϴ�.");
                return;
            }
        }
        Debug.LogWarning("EnemyCardManager: ���Կ��� �ش� ī�带 ã�� �� �����ϴ�.", cardGO);
    }

    /// <summary>
    /// ���� ��ȯ�� ��� �� ī�� GameObject�� ��ȯ�մϴ�.
    /// </summary>
    public IEnumerable<GameObject> GetAllEnemyCards()
    {
        foreach (var go in occupiedEnemySlots)
            if (go != null)
                yield return go;
    }
}
