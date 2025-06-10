using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic; // List를 사용하기 위해 추가

public class BattlefieldManager : MonoBehaviour
{
    [Header("---------- 카드 배치 위치 ---------")]
    [SerializeField] private List<Transform> spawnPoints = new List<Transform>();

    [Header("---------- 배치 관련 메시지 UI ---------")]
    [SerializeField] private TextMeshProUGUI placementInfoText;

    [Header("---------- 배치될 카드의 속성 ---------")]
    [SerializeField] private Vector3 placedCardScale = new Vector3(1f, 1f, 1f); // 배치될 카드의 크기 (인스펙터에서 조절)
    [SerializeField] private Vector3 cardLocalOffset = Vector3.zero; // 배치될 카드의 로컬 오프셋 (스폰포인트 기준)

 
    [Header("---------- 다른 매니저 참조 ---------")]
    [SerializeField] private PlayerCostManager playerCostManager;
    [SerializeField] private HandManager handManager;

    private GameObject[] occupiedSpawnPoints;

    // 현재 배치 대기 중인 카드
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

        // 각 spawnPoints GameObject에 BattlefieldSlot 스크립트를 설정합니다.
        // 콜라이더는 이제 이 메서드에서 자동으로 추가하지 않습니다. 수동으로 추가해야 합니다.
        SetupBattlefieldSlots();
    }

    /// <summary>
    /// 각 스폰 포인트 GameObject에 BattlefieldSlot 스크립트를 설정합니다.
    /// 콜라이더는 이제 수동으로 추가해야 합니다.
    /// </summary>
    private void SetupBattlefieldSlots()
    {
        for (int i = 0; i < spawnPoints.Count; i++)
        {
            Transform slotTransform = spawnPoints[i];
            // NULL 참조 오류 방지를 위한 부분 (스폰포인트 리스트가 비어있을 때)
            if (slotTransform == null)
            {
                Debug.LogWarning($"SpawnPoints 리스트의 {i}번째 슬롯이 비어있습니다." +
                    $" 해당 슬롯은 건너뜁니다. 인스펙터에서 Transform을 연결해주세요.", this);
                occupiedSpawnPoints[i] = null;
                continue;
            }

            GameObject slotGO = slotTransform.gameObject;
            BattlefieldSlot slot = slotGO.GetComponent<BattlefieldSlot>();
            if (slot == null)
            {
                slot = slotGO.AddComponent<BattlefieldSlot>();
            }
            slot.slotIndex = i; // 슬롯 인덱스 할당

            
        }
    }


    /// <summary>
    /// 카드 배치 관련 정보를 UI에 표시합니다.
    /// </summary>
    /// <param name="message">표시할 메시지</param>
    public void ShowPlacementInfo(string message)
    {
        if (placementInfoText != null)
        {
            placementInfoText.text = message;
            placementInfoText.gameObject.SetActive(true);
            StopAllCoroutines();
            StartCoroutine(HidePlacementInfoAfterDelay(3f)); // 3초 후 사라지게
        }
    }

    /// <summary>
    /// 카드 배치 관련 정보를 UI에서 숨깁니다.
    /// </summary>
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

    /// <summary>
    /// HandManager로부터 카드가 선택되어 배치 대기 상태임을 알립니다.
    /// </summary>
    /// <param name="cardGO">배치 대기 중인 카드 GameObject</param>
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
    public void OnSlotClicked(int clickedSlotIndex)
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
        card.transform.SetParent(spawnPoints[clickedSlotIndex]);
        card.transform.localPosition = cardLocalOffset; // <--- 수정: 시리얼라이즈된 오프셋 사용
        card.transform.localRotation = Quaternion.identity;
        card.transform.localScale = placedCardScale; // <--- 수정: 시리얼라이즈된 스케일 사용

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
