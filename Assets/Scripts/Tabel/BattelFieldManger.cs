using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI; // RectTransform 사용을 위해 필요

public class BattlefieldManager : MonoBehaviour
{
    [Header("---------- 카드 배치 위치 ---------")]
    [SerializeField] private List<Transform> spawnPoints = new List<Transform>();

    [Header("---------- 배치 관련 메시지 UI ---------")]
    [SerializeField] private TextMeshProUGUI placementInfoText;

    [Header("---------- 필드 카드 프리펩 ---------")]
    [SerializeField] private GameObject fieldCardPrefab;

    [Header("---------- 다른 매니저 참조 ---------")]
    [SerializeField] private PlayerCostManager playerCostManager;
    [SerializeField] private HandManager handManager;

    private GameObject[] occupiedSpawnPoints;
    private GameObject cardWaitingForPlacement = null;

    void Awake()
    {
        if (spawnPoints.Count > 0)
            occupiedSpawnPoints = new GameObject[spawnPoints.Count];
        else
        {
            Debug.LogError("spawnPoints가 설정되지 않았습니다.", this);
            enabled = false;
            return;
        }

        if (playerCostManager == null)
            playerCostManager = FindObjectOfType<PlayerCostManager>();
        if (handManager == null)
            handManager = FindObjectOfType<HandManager>();

        if (placementInfoText == null)
            Debug.LogError("Placement Info Text가 연결되지 않았습니다.", this);

        HidePlacementInfo();
        SetupBattlefieldSlots();
    }

    private void SetupBattlefieldSlots()
    {
        for (int i = 0; i < spawnPoints.Count; i++)
        {
            var slotTransform = spawnPoints[i];
            if (slotTransform == null)
            {
                Debug.LogWarning($"SpawnPoints[{i}]가 비어있습니다.");
                occupiedSpawnPoints[i] = null;
                continue;
            }

            var slotGO = slotTransform.gameObject;
            var slot = slotGO.GetComponent<BattlefieldSlot>() ?? slotGO.AddComponent<BattlefieldSlot>();
            slot.SetSlotIndex(i);
        }
    }

    public void ShowPlacementInfo(string message)
    {
        if (placementInfoText == null) return;
        placementInfoText.text = message;
        placementInfoText.gameObject.SetActive(true);
        StopAllCoroutines();
        StartCoroutine(HidePlacementInfoAfterDelay(3f));
    }

    private IEnumerator HidePlacementInfoAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        HidePlacementInfo();
    }

    public void HidePlacementInfo()
    {
        if (placementInfoText != null)
            placementInfoText.gameObject.SetActive(false);
    }

    public void SetCardWaitingForPlacement(GameObject cardGO)
    {
        cardWaitingForPlacement = cardGO;
        ShowPlacementInfo($"'{cardGO.name}' 카드 배치 대기 중. 슬롯을 클릭하세요.");
    }

    public void OnSlotClicked(int clickedSlotIndex)
    {
        if (cardWaitingForPlacement == null)
        {
            ShowPlacementInfo("먼저 손패에서 카드를 선택하세요!");
            return;
        }

        if (clickedSlotIndex < 0 || clickedSlotIndex >= spawnPoints.Count || spawnPoints[clickedSlotIndex] == null)
        {
            ShowPlacementInfo("유효하지 않은 슬롯입니다.");
            ClearCardWaitingForPlacement();
            return;
        }

        if (occupiedSpawnPoints[clickedSlotIndex] != null)
        {
            ShowPlacementInfo("이미 다른 카드가 있습니다!");
            return;
        }

        var originalDisplay = cardWaitingForPlacement.GetComponent<CardDisplay>();
        if (originalDisplay == null || originalDisplay.cardData == null)
        {
            Debug.LogError("CardDisplay 또는 CardData 없음", cardWaitingForPlacement);
            ClearCardWaitingForPlacement();
            return;
        }

        int cost = originalDisplay.cardData.Cost;
        if (playerCostManager == null)
        {
            ShowPlacementInfo("코스트 관리자 없음.");
            ClearCardWaitingForPlacement();
            return;
        }

        if (playerCostManager.CurrentCost < cost)
        {
            ShowPlacementInfo($"코스트 부족! ({cost - playerCostManager.CurrentCost} 필요)");
            return;
        }

        if (!playerCostManager.RemoveCost(cost))
        {
            ShowPlacementInfo("코스트 소모 오류!");
            return;
        }

        // 1) 필드 카드 프리팹 인스턴스화
        var fieldCard = Instantiate(fieldCardPrefab, spawnPoints[clickedSlotIndex], false);

        // 2) 원본 카드의 cardData 복사 및 디스플레이 업데이트
        var newDisplay = fieldCard.GetComponent<CardDisplay>();
        if (newDisplay != null)
        {
            newDisplay.cardData = originalDisplay.cardData;
            newDisplay.UpdateDisplay();
        }

        // 3) 슬롯별 Transform 적용
        var fieldRect = fieldCard.GetComponent<RectTransform>();
        var slotComp = spawnPoints[clickedSlotIndex].GetComponent<BattlefieldSlot>();
        if (slotComp != null && fieldRect != null)
            slotComp.ApplyPlacementTransform(fieldRect);

        occupiedSpawnPoints[clickedSlotIndex] = fieldCard;

        // 4) 원본 손패 카드 제거 및 HandManager 알림
        if (handManager != null)
            handManager.NotifyCardPlacedSuccessfully(cardWaitingForPlacement);
        Destroy(cardWaitingForPlacement);

        ShowPlacementInfo($"{originalDisplay.cardData.CardName} 배치 완료! (-{cost} 코스트)");
        ClearCardWaitingForPlacement();
    }

    public void ClearCardWaitingForPlacement()
    {
        if (cardWaitingForPlacement != null)
        {
            var selector = cardWaitingForPlacement.GetComponent<CardSelector>();
            if (selector != null && selector.IsSelected())
                selector.DeselectExternally();
        }
        cardWaitingForPlacement = null;
        HidePlacementInfo();
    }

    public void RemoveCardFromBattlefield(GameObject cardGO)
    {
        for (int i = 0; i < occupiedSpawnPoints.Length; i++)
        {
            if (occupiedSpawnPoints[i] == cardGO)
            {
                occupiedSpawnPoints[i] = null;
                Debug.Log($"{cardGO.name} 제거됨.");
                return;
            }
        }
        Debug.LogWarning($"{cardGO.name}을(를) 찾을 수 없습니다.");
    }
}
