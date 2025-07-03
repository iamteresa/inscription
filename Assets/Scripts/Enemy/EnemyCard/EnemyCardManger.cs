using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class EnemyCardManager : MonoBehaviour
{
    public enum Difficulty { Easy, Normal, Hard, Nightmare }

    [Header("=== 난이도별 덱 설정 (Inspector) ===")]
    [Tooltip("Easy 모드에서 사용할 카드들")]
    [SerializeField] private List<CardData> easyDeck = new List<CardData>();
    [Tooltip("Normal 모드에서 사용할 카드들")]
    [SerializeField] private List<CardData> normalDeck = new List<CardData>();
    [Tooltip("Hard 모드에서 사용할 카드들")]
    [SerializeField] private List<CardData> hardDeck = new List<CardData>();
    [Tooltip("Nightmare 모드에서 사용할 카드들")]
    [SerializeField] private List<CardData> nightmareDeck = new List<CardData>();

    [Header("----- 실제 소환에 사용하는 덱 -----")]
    private List<CardData> enemyDeck = new List<CardData>();

    [Header("----- 배치 슬롯(Inspector에서 연결) -----")]
    [Tooltip("적 필드 슬롯 Transform 리스트")]
    [SerializeField] private List<Transform> _enemySpawnPoints = new List<Transform>();

    /// <summary>외부에서 읽기 전용으로 접근할 수 있도록 공개 프로퍼티</summary>
    public IReadOnlyList<Transform> EnemySpawnPoints => _enemySpawnPoints;

    [Header("----- 필드 카드 Prefab -----")]
    [Tooltip("FieldCard가 붙은 카드 프리팹")]
    [SerializeField] private GameObject fieldCardPrefab;

    void Awake()
    {
        // 1) 전역 설정에서 난이도 가져오기
        Difficulty diff = (Difficulty)GameSettings.CurrentDifficulty;

        // 2) 알맞은 덱을 enemyDeck에 복사
        enemyDeck.Clear();
        switch (diff)
        {
            case Difficulty.Easy:
                enemyDeck.AddRange(easyDeck);
                break;
            case Difficulty.Normal:
                enemyDeck.AddRange(normalDeck);
                break;
            case Difficulty.Hard:
                enemyDeck.AddRange(hardDeck);
                break;
            case Difficulty.Nightmare:
                enemyDeck.AddRange(nightmareDeck);
                break;
        }

        // 덱에 null 카드가 있는지 초기 검사 (기존 오류 방지용)
        for (int i = enemyDeck.Count - 1; i >= 0; i--)
        {
            if (enemyDeck[i] == null)
            {
                Debug.LogWarning($"EnemyCardManager: Initial enemyDeck contains a null CardData at index {i}. Removing it.", this);
                enemyDeck.RemoveAt(i);
            }
        }
        // --- End of 추가 ---
    }

    /// <summary>
    /// 덱에서 랜덤으로 한 장을 뽑아 빈 슬롯에 소환하고 덱에서 제거합니다.
    /// </summary>
    public bool DrawAndSpawnEnemyCard()
    {
        // 0) 덱이 비었으면 실패
        if (enemyDeck == null || enemyDeck.Count == 0)
        {
            Debug.LogWarning("EnemyCardManager: 덱에 카드가 없습니다.");
            return false;
        }

        // 빈 슬롯을 랜덤으로 찾기 (카드를 스폰할 곳 랜덤으로 찾기)
        List<int> freeSlotIndices = new List<int>();
        for (int i = 0; i < EnemySpawnPoints.Count; i++)
        {
            if (EnemySpawnPoints[i].childCount == 0)
            {
                freeSlotIndices.Add(i);
            }
        }

        if (freeSlotIndices.Count == 0)
        {
            Debug.Log("EnemyCardManager: 빈 슬롯이 없습니다.");
            return false;
        }

        // 빈 슬롯들 중에서 랜덤으로 하나 선택
        int randomIndex = Random.Range(0, freeSlotIndices.Count);
        int freeIndex = freeSlotIndices[randomIndex];


        // 2) 덱에서 랜덤 카드 선택 및 덱에서 제거
        int deckIdx = Random.Range(0, enemyDeck.Count);
        CardData data = enemyDeck[deckIdx];
        enemyDeck.RemoveAt(deckIdx);

        //뽑은 카드가 null인지 확인 (이전 오류 방지용)
        if (data == null)
        {
            Debug.LogError("EnemyCardManager: 덱에서 null CardData가 뽑혔습니다. 덱 구성을 확인하세요.", this);
            return false;
        }


        // 3) 프리팹 인스턴스화
        Transform parent = EnemySpawnPoints[freeIndex];
        GameObject go = Instantiate(fieldCardPrefab, parent, false);

        // 4) FieldCard 초기화
        var fc = go.GetComponent<FieldCard>();
        if (fc != null)
            fc.Initialize(data, FieldCard.CardFaction.Enemy);
        else
            Debug.LogError("EnemyCardManager: fieldCardPrefab에 FieldCard가 없습니다.", go);

        // --- BattlefieldSlot의 배치 설정 적용 ---
        RectTransform cardRectTransform = go.GetComponent<RectTransform>();
        if (cardRectTransform != null)
        {
            BattlefieldSlot targetSlot = parent.GetComponent<BattlefieldSlot>();
            if (targetSlot != null)
            {
                targetSlot.ApplyPlacementTransform(cardRectTransform);
            }
            else
            {
                Debug.LogWarning($"EnemyCardManager: 슬롯 '{parent.name}'에 BattlefieldSlot 컴포넌트가 없습니다. " +
                    $"기본 크기와 위치가 적용됩니다.", parent);
                // 기본값 적용
                cardRectTransform.anchoredPosition = Vector2.zero;
                cardRectTransform.localScale = Vector3.one;
            }
        }
        else
        {
            Debug.LogWarning("EnemyCardManager: 소환된 카드 프리팹에 RectTransform이 없습니다.", go);
        }

        //배치 설정 적용
        Debug.Log($"EnemyCardManager: [{GameSettings.CurrentDifficulty}] 슬롯 {freeIndex}에 '{data.CardName}' 소환");
        return true;
    }
}