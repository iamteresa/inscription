// BattlefieldManager.cs

using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI; // RectTransform을 사용하기 위해 이 줄이 필요합니다.

public class BattlefieldManager : MonoBehaviour
{
    [Header("---------- 카드 배치 위치 ---------")]
    [SerializeField] private List<Transform> spawnPoints = new List<Transform>();

    [Header("---------- 배치 관련 메시지 UI ---------")]
    [SerializeField] private TextMeshProUGUI placementInfoText;

    // 이 변수들은 이제 BattlefieldSlot에서 관리되므로 주석 처리하거나 제거할 수 있습니다.
    // [Header("---------- 배치될 카드의 속성 ---------")]
    // [SerializeField] private Vector3 placedCardScale = new Vector3(1f, 1f, 1f);
    // [SerializeField] private Vector3 cardLocalOffset = Vector3.zero;


    [Header("---------- 다른 매니저 참조 ---------")]
    [SerializeField] private PlayerCostManager playerCostManager;
    [SerializeField] private HandManager handManager;

    private GameObject[] occupiedSpawnPoints;

    private GameObject cardWaitingForPlacement = null;

    void Awake()
    {
        if (spawnPoints.Count > 0)
        {
            occupiedSpawnPoints = new GameObject[spawnPoints.Count];
        }
        else
        {
            Debug.LogError("spawnPoints가 설정되지 않았습니다.", this);
            enabled = false;
            return;
        }

        if (playerCostManager == null)
        {
            playerCostManager = FindObjectOfType<PlayerCostManager>();
            if (playerCostManager == null)
            {
                Debug.LogError("PlayerCostManager가 연결되지 않았거나 씬에서 찾을 수 없습니다.", this);
            }
        }
        if (handManager == null)
        {
            handManager = FindObjectOfType<HandManager>();
            if (handManager == null)
            {
                Debug.LogWarning("HandManager가 연결되지 않았거나 씬에서 찾을 수 없습니다.", this);
            }
        }

        if (placementInfoText == null)
        {
            Debug.LogError("Placement Info Text가 연결되지 않았습니다.", this);
        }

        HidePlacementInfo();

        SetupBattlefieldSlots(); // BattlefieldSlot 컴포넌트를 스폰포인트에 동적으로 추가하고 인덱스 설정
    }

    private void SetupBattlefieldSlots()
    {
        for (int i = 0; i < spawnPoints.Count; i++)
        {
            Transform slotTransform = spawnPoints[i];
            if (slotTransform == null)
            {
                Debug.LogWarning($"SpawnPoints 리스트의 {i}번째 슬롯이 비어있습니다." +
                    $" 해당 슬롯은 건너뜜니다. 인스펙터에서 Transform을 연결해주세요.", this);
                occupiedSpawnPoints[i] = null;
                continue;
            }

            GameObject slotGO = slotTransform.gameObject;
            BattlefieldSlot slot = slotGO.GetComponent<BattlefieldSlot>();
            if (slot == null)
            {
                slot = slotGO.AddComponent<BattlefieldSlot>();
            }
            slot.SetSlotIndex(i); // BattlefieldSlot의 SetSlotIndex 메서드를 사용
        }
    }

    public void ShowPlacementInfo(string message)
    {
        if (placementInfoText != null)
        {
            placementInfoText.text = message;
            placementInfoText.gameObject.SetActive(true);
            StopAllCoroutines();
            StartCoroutine(HidePlacementInfoAfterDelay(3f));
        }
    }

    public void HidePlacementInfo()
    {
        if (placementInfoText != null)
        {
            placementInfoText.gameObject.SetActive(false);
        }
    }

    private IEnumerator HidePlacementInfoAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        HidePlacementInfo();
    }

    public void SetCardWaitingForPlacement(GameObject cardGO)
    {
        cardWaitingForPlacement = cardGO;
        ShowPlacementInfo($"'{cardGO.name}' 카드 배치 대기 중. 전장 슬롯을 클릭하세요.");
        Debug.Log($"BattlefieldManager: '{cardGO.name}' 카드 배치 대기 중. 슬롯 클릭을 기다립니다.");
    }

    /// <summary>
    /// BattlefieldSlot으로부터 슬롯 클릭 알림을 받습니다.
    /// 이 메서드에서 Cost 확인 및 카드 배치 로직을 수행합니다.
    /// </summary>
    /// <param name="clickedSlotIndex">클릭된 슬롯의 인덱스</param>
    public void OnSlotClicked(int clickedSlotIndex) // BattlefieldSlot에서 이 메서드를 호출합니다.
    {
        if (cardWaitingForPlacement == null)
        {
            ShowPlacementInfo("먼저 손패에서 카드를 선택하세요!");
            Debug.Log("BattlefieldManager: 배치할 카드가 선택되지 않았습니다.");
            return;
        }

        if (clickedSlotIndex < 0 || clickedSlotIndex >= spawnPoints.Count || spawnPoints[clickedSlotIndex] == null)
        {
            Debug.LogError($"BattlefieldManager: 유효하지 않거나 비어있는 슬롯 인덱스: {clickedSlotIndex}", this);
            ShowPlacementInfo("유효하지 않은 슬롯입니다.");
            ClearCardWaitingForPlacement();
            return;
        }

        if (occupiedSpawnPoints[clickedSlotIndex] != null)
        {
            ShowPlacementInfo("이미 다른 카드가 있습니다!");
            Debug.Log($"BattlefieldManager: 슬롯 {clickedSlotIndex}는 이미 점유 중입니다.");
            return;
        }

        CardDisplay cardDisplay = cardWaitingForPlacement.GetComponent<CardDisplay>();
        if (cardDisplay == null || cardDisplay.cardData == null)
        {
            Debug.LogError("배치하려는 카드에 CardDisplay 또는 CardData가 없습니다.", cardWaitingForPlacement);
            ClearCardWaitingForPlacement();
            return;
        }

        int cardCost = cardDisplay.cardData.Cost;

        if (playerCostManager == null)
        {
            Debug.LogError("PlayerCostManager가 초기화되지 않았습니다. 카드를 배치할 수 없습니다.", this);
            ShowPlacementInfo("코스트 관리자 없음.");
            ClearCardWaitingForPlacement();
            return;
        }

        if (playerCostManager.CurrentCost < cardCost)
        {
            ShowPlacementInfo($"코스트 부족! ({cardCost - playerCostManager.CurrentCost} 더 필요)");
            Debug.Log($"코스트 부족: {cardWaitingForPlacement.name} (필요 코스트: {cardCost}, 현재 코스트: {playerCostManager.CurrentCost})");
            return;
        }

        bool costRemoved = playerCostManager.RemoveCost(cardCost);
        if (!costRemoved)
        {
            Debug.LogError("코스트 소모 중 예상치 못한 오류 발생.", this);
            ShowPlacementInfo("코스트 소모 오류!");
            return;
        }

        GameObject card = cardWaitingForPlacement;
        RectTransform cardRect = card.GetComponent<RectTransform>();

        if (cardRect == null)
        {
            Debug.LogError("BattlefieldManager: 배치할 카드에 RectTransform이 없습니다.", card);
            ClearCardWaitingForPlacement();
            return;
        }

        // 1. 카드를 슬롯의 자식으로 설정합니다. (worldPositionStays = false)
        // 이렇게 하면 카드의 localPosition이 새로운 부모를 기준으로 재계산됩니다.
        card.transform.SetParent(spawnPoints[clickedSlotIndex], false);

        // 2. 해당 슬롯의 BattlefieldSlot 컴포넌트에서 카드 변환을 적용합니다.
        BattlefieldSlot targetBattlefieldSlot = spawnPoints[clickedSlotIndex].GetComponent<BattlefieldSlot>();
        if (targetBattlefieldSlot != null)
        {
            targetBattlefieldSlot.ApplyPlacementTransform(cardRect); // <--- BattlefieldSlot에서 오프셋/스케일 적용
        }
        else
        {
            Debug.LogWarning($"BattlefieldManager: {spawnPoints[clickedSlotIndex].name}에 BattlefieldSlot 컴포넌트가 없습니다. 기본값으로 배치합니다.", this);
            // BattlefieldSlot이 없는 경우를 대비한 기본 배치 로직
            cardRect.localPosition = Vector2.zero; // 중앙으로 설정
            cardRect.localScale = Vector3.one;     // 기본 스케일
            cardRect.localRotation = Quaternion.identity; // 회전 초기화
        }

        // ***** 불필요한 컴포넌트 제거 *****
        // 배치된 카드에서 더 이상 필요 없는 컴포넌트들을 제거합니다.
        // CardDisplay는 남겨둡니다.

        // CardHover 컴포넌트 제거
        CardHover cardHover = card.GetComponent<CardHover>();
        if (cardHover != null)
        {
            Destroy(cardHover);
            Debug.Log($"Removed CardHover from {card.name}");
        }

        // CardSelector 컴포넌트 제거 (카드를 클릭하여 선택하는 기능을 담당했다면)
        CardSelector cardSelector = card.GetComponent<CardSelector>();
        if (cardSelector != null)
        {
            Destroy(cardSelector);
            Debug.Log($"Removed CardSelector from {card.name}");
        }

        // 혹시 드래그 기능을 하는 스크립트가 있다면 (예: CardDragHandler, IBeginDragHandler, IDragHandler, IEndDragHandler 구현 스크립트)
        // 여기에 추가하여 제거할 수 있습니다.
        // 예를 들어:
        // YourCardDragHandler dragHandler = card.GetComponent<YourCardDragHandler>();
        // if (dragHandler != null)
        // {
        //     Destroy(dragHandler);
        //     Debug.Log($"Removed YourCardDragHandler from {card.name}");
        // }


        occupiedSpawnPoints[clickedSlotIndex] = card;

        ShowPlacementInfo($"{card.name}을(를) {clickedSlotIndex}번 슬롯에 배치했습니다! (-{cardCost} 코스트)");
        Debug.Log($"{card.name} 카드가 {spawnPoints[clickedSlotIndex].name}에 배치되었습니다. 남은 코스트: {playerCostManager.CurrentCost}");

        if (handManager != null)
        {
            handManager.NotifyCardPlacedSuccessfully(card);
        }

        ClearCardWaitingForPlacement();
    }

    /// <summary>
    /// 배치 대기 상태를 해제하고 관련 UI를 숨깁니다.
    /// (배치 성공 또는 실패/취소 시 호출)
    /// </summary>
    public void ClearCardWaitingForPlacement()
    {
        if (cardWaitingForPlacement != null)
        {
            CardSelector selector = cardWaitingForPlacement.GetComponent<CardSelector>();
            if (selector != null && selector.IsSelected())
            {
                selector.DeselectExternally();
            }
        }
        cardWaitingForPlacement = null;
        HidePlacementInfo();
    }

    /// <summary>
    /// 지정된 스폰 포인트에서 카드를 제거합니다. (예: 카드 파괴 시)
    /// </summary>
    /// <param name="cardGO">제거할 카드 GameObject</param>
    public void RemoveCardFromBattlefield(GameObject cardGO)
    {
        for (int i = 0; i < occupiedSpawnPoints.Length; i++)
        {
            if (occupiedSpawnPoints[i] == cardGO)
            {
                occupiedSpawnPoints[i] = null;
                Debug.Log($"{cardGO.name}가 테이블에서 제거되었습니다.");
                return;
            }
        }
        Debug.LogWarning($"테이블에서 제거하려는 카드 '{cardGO.name}'를 찾을 수 없습니다.");
    }
}