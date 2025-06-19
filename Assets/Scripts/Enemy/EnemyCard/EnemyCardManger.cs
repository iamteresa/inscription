using UnityEngine;
using UnityEngine.UI;               // RectTransform 사용을 위해
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 적 전용 필드 카드 관리 매니저
/// 4개의 슬롯에 적 카드를 소환·제거하며, 슬롯에 맞춰 위치·크기까지 자동 조절합니다.
/// </summary>
public class EnemyCardManager : MonoBehaviour
{
    [Header("----- 적 카드 배치 슬롯(4개) -----")]
    [SerializeField] private List<Transform> enemySpawnPoints = new List<Transform>();

    [Header("----- 필드 카드 프리팹 -----")]
    [SerializeField] private GameObject fieldCardPrefab;

    [Header("----- 적 덱 (CardData 리스트) -----")]
    [SerializeField] private List<CardData> enemyDeck = new List<CardData>();

    [Header("=== 카드 위치 & 크기 설정 ===")]
    [Tooltip("소환 시 슬롯 위치에 더해질 오프셋")]
    [SerializeField] private Vector3 cardPositionOffset = Vector3.zero;
    [Tooltip("카드의 최종 localScale")]
    [SerializeField] private Vector3 cardScale = Vector3.one;

    // 슬롯 점유 상태 추적용
    private GameObject[] occupiedEnemySlots;

    void Awake()
    {
        if (enemySpawnPoints == null || enemySpawnPoints.Count == 0)
        {
            Debug.LogError("EnemyCardManager: 슬롯이 설정되지 않았습니다!", this);
            enabled = false;
            return;
        }
        occupiedEnemySlots = new GameObject[enemySpawnPoints.Count];
    }

    /// <summary>
    /// 덱에서 랜덤으로 한 장을 뽑아 빈 슬롯에 소환합니다.
    /// 슬롯이 없거나 덱이 비어 있으면 false를 반환합니다.
    /// </summary>
    public bool DrawAndSpawnEnemyCard()
    {
        if (enemyDeck == null || enemyDeck.Count == 0)
        {
            Debug.LogWarning("EnemyCardManager: 덱에 카드가 없습니다.");
            return false;
        }

        // 1) 빈 슬롯 찾기
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
            Debug.Log("EnemyCardManager: 빈 슬롯이 없습니다.");
            return false;
        }

        // 2) 덱에서 랜덤 카드 선택 및 덱에서 제거
        int idx = Random.Range(0, enemyDeck.Count);
        CardData data = enemyDeck[idx];
        enemyDeck.RemoveAt(idx);

        // 3) 프리팹 인스턴스화 (부모는 슬롯 Transform)
        GameObject go = Instantiate(fieldCardPrefab, enemySpawnPoints[freeIndex], false);

        // 4) 슬롯용 BattlefieldSlot 컴포넌트를 사용해 위치·크기 적용
        var slotComp = enemySpawnPoints[freeIndex].GetComponent<BattlefieldSlot>();
        var rect = go.GetComponent<RectTransform>();
        if (slotComp != null && rect != null)
        {
            slotComp.ApplyPlacementTransform(rect);
        }
        else
        {
            // 슬롯 컴포넌트나 RectTransform이 없으면 기본 오프셋/스케일만 적용
            go.transform.localPosition += cardPositionOffset;
            go.transform.localScale = cardScale;
        }

        // 5) FieldCard 초기화
        FieldCard fc = go.GetComponent<FieldCard>();
        if (fc == null)
        {
            Debug.LogError("EnemyCardManager: FieldCard 컴포넌트가 없습니다!", go);
            Destroy(go);
            return false;
        }
        fc.Initialize(data, FieldCard.CardFaction.Enemy);

        // 6) 슬롯 점유 상태 등록
        occupiedEnemySlots[freeIndex] = go;
        Debug.Log($"EnemyCardManager: 슬롯 {freeIndex}에 '{data.CardName}' 소환 (offset:{cardPositionOffset}, scale:{cardScale})");

        return true;
    }

    /// <summary>
    /// 특정 적 카드를 슬롯에서 제거하고 오브젝트도 파괴합니다.
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
                Debug.Log($"EnemyCardManager: 슬롯 {i}의 카드를 제거했습니다.");
                return;
            }
        }
        Debug.LogWarning("EnemyCardManager: 슬롯에서 해당 카드를 찾을 수 없습니다.", cardGO);
    }

    /// <summary>
    /// 현재 소환된 모든 적 카드 GameObject를 반환합니다.
    /// </summary>
    public IEnumerable<GameObject> GetAllEnemyCards()
    {
        foreach (var go in occupiedEnemySlots)
            if (go != null)
                yield return go;
    }
}
