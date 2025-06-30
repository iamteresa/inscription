// Assets/Scripts/Enemy/EnemyCardManager.cs
using UnityEngine;
using System.Collections.Generic;

public class EnemyCardManager : MonoBehaviour
{
    [Header("----- 덱(Inspector에서 관리) -----")]
    [Tooltip("적이 사용할 카드 데이터 리스트")]
    public List<CardData> enemyDeck = new List<CardData>();

    [Header("----- 배치 슬롯(Inspector에서 연결) -----")]
    [Tooltip("적 필드 슬롯 Transform 리스트")]
    public List<Transform> EnemySpawnPoints = new List<Transform>();

    [Header("----- 필드 카드 Prefab -----")]
    [Tooltip("FieldCard가 붙은 카드 프리팹")]
    public GameObject fieldCardPrefab;

    private GameObject[] _occupiedSlots;

    void Awake()
    {
        _occupiedSlots = new GameObject[EnemySpawnPoints.Count];
    }

    void OnEnable()
    {
        CardEventBus.OnDeath += HandleCardDeath;
    }

    void OnDisable()
    {
        CardEventBus.OnDeath -= HandleCardDeath;
    }

    /// <summary>
    /// 덱에서 랜덤으로 한 장을 뽑아,
    /// 빈 슬롯(중복 없는) 중 랜덤한 위치에 소환 후 덱에서 제거합니다.
    /// 없으면 false, 성공하면 true 반환.
    /// </summary>
    public bool DrawAndSpawnEnemyCard()
    {
        // 0) 덱이 비었으면 실패
        if (enemyDeck == null || enemyDeck.Count == 0)
        {
            Debug.LogWarning("EnemyCardManager: 덱에 카드가 없습니다.");
            return false;
        }

        // 1) 빈 슬롯 인덱스 모두 수집
        var freeIndices = new List<int>();
        for (int i = 0; i < _occupiedSlots.Length; i++)
        {
            if (_occupiedSlots[i] == null)
                freeIndices.Add(i);
        }

        if (freeIndices.Count == 0)
        {
            Debug.Log("EnemyCardManager: 빈 슬롯이 없습니다.");
            return false;
        }

        // 2) 덱에서 랜덤 카드 선택 및 덱에서 제거
        int deckIdx = Random.Range(0, enemyDeck.Count);
        CardData data = enemyDeck[deckIdx];
        enemyDeck.RemoveAt(deckIdx);

        // 3) 빈 슬롯 중 랜덤 선택
        int slotIndex = freeIndices[Random.Range(0, freeIndices.Count)];

        // 4) 카드 프리팹 인스턴스화 (부모는 선택된 슬롯)
        Transform parent = EnemySpawnPoints[slotIndex];
        GameObject go = Instantiate(fieldCardPrefab, parent, false);

        // 5) 슬롯 컴포넌트 있으면 위치·크기 조정
        var slotComp = parent.GetComponent<BattlefieldSlot>();
        var rect = go.GetComponent<RectTransform>();
        if (slotComp != null && rect != null)
            slotComp.ApplyPlacementTransform(rect);

        // 6) FieldCard 초기화
        var fc = go.GetComponent<FieldCard>();
        if (fc == null)
        {
            Debug.LogError("EnemyCardManager: fieldCardPrefab에 FieldCard 컴포넌트가 없습니다!", go);
            Destroy(go);
            return false;
        }
        fc.Initialize(data, FieldCard.CardFaction.Enemy);

        // 7) 내부 점유 배열에 등록
        _occupiedSlots[slotIndex] = go;

        Debug.Log($"EnemyCardManager: 슬롯 {slotIndex}에 '{data.CardName}' 랜덤 소환");
        return true;
    }

    private void HandleCardDeath(FieldCard deadCard, CardData cardData)
    {
        for (int i = 0; i < _occupiedSlots.Length; i++)
        {
            if (_occupiedSlots[i] != null &&
                _occupiedSlots[i].GetComponent<FieldCard>() == deadCard)
            {
                _occupiedSlots[i] = null;
                Debug.Log($"EnemyCardManager: 슬롯 {i} 해제 (카드 사망)");
                break;
            }
        }
    }
}
