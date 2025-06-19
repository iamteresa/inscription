using UnityEngine;
using TMPro; // TextMeshProUGUI (UI 텍스트) 사용을 위해 필요
using System.Collections; // IEnumerator (코루틴) 사용을 위해 필요
using System.Collections.Generic; // List (목록) 사용을 위해 필요
using UnityEngine.UI; // RectTransform (UI 요소의 크기 및 위치 조작) 사용을 위해 필요

public class BattlefieldManager : MonoBehaviour
{
    [Header("---------- 카드 배치 위치 ---------")]
    [SerializeField] private List<Transform> spawnPoints = new List<Transform>();
    public List<Transform> SpawnPoints => spawnPoints;

    [Header("---------- 배치 관련 메시지 UI ---------")]
    [SerializeField] private TextMeshProUGUI placementInfoText;

    [Header("---------- 필드 카드 프리펩 ---------")]
    [SerializeField] private GameObject fieldCardPrefab;

    [Header("---------- 다른 매니저 참조 ---------")]
    [SerializeField] private PlayerCostManager playerCostManager;
    [SerializeField] private HandManager handManager;

 
    // 'spawnPoints' 리스트의 각 인덱스에 해당하는 슬롯에 어떤 카드 GameObject가 현재 배치되어 있는지
    // (점유하고 있는지) 추적하는 배열입니다.
    private GameObject[] occupiedSpawnPoints;

    // 플레이어가 손패에서 필드에 내려놓기 위해 현재 선택한(클릭한) 카드 GameObject를 임시로 저장합니다.
    private GameObject cardWaitingForPlacement = null;

    // MonoBehaviour의 생명 주기 메서드: 스크립트 인스턴스가 로드될 때 호출됩니다.
    void Awake()
    {
        // 1. 'spawnPoints' 리스트의 유효성 검사 및 'occupiedSpawnPoints' 배열 초기화
        if (spawnPoints.Count > 0)
            // 'spawnPoints'의 개수만큼 'occupiedSpawnPoints' 배열을 생성합니다.
            occupiedSpawnPoints = new GameObject[spawnPoints.Count];
        else
        {
            // 'spawnPoints'가 설정되지 않았다면 심각한 오류, 이 스크립트를 비활성화합니다.
            Debug.LogError("spawnPoints가 설정되지 않았습니다. 필드 슬롯 Transform을 인스펙터에 연결해주세요.", this);
            enabled = false; // 해당 스크립트 컴포넌트를 비활성화합니다.
            return; 
        }

        // 2. 다른 매니저 참조 연결 (인스펙터에 연결되지 않은 경우)
        // 'FindObjectOfType'은 씬 전체를 탐색하므로 성능에 영향을 줄 수 있습니다.
        // 가능하면 Unity 에디터의 인스펙터에서 직접 연결하는 것이 더 효율적입니다.
        if (playerCostManager == null)
            playerCostManager = FindObjectOfType<PlayerCostManager>();
        if (handManager == null)
            handManager = FindObjectOfType<HandManager>();

        // 3. 'placementInfoText' UI 요소 연결 확인
        if (placementInfoText == null)
            Debug.LogError("Placement Info Text가 연결되지 않았습니다. UI Canvas의 TextMeshProUGUI 요소를 연결해주세요.", this);

        // 4. 초기 상태 설정
        // 게임 시작 시 배치 정보 UI를 숨깁니다.
        HidePlacementInfo();
        // 필드 슬롯들을 설정합니다 (BattlefieldSlot 컴포넌트 추가 및 인덱스 설정).
        SetupBattlefieldSlots();


    }

    /// <summary>
    /// 필드의 각 'spawnPoint' GameObject에 'BattlefieldSlot' 컴포넌트를 추가하거나 가져와서
    /// 해당 슬롯의 고유 인덱스를 설정합니다. 이 'BattlefieldSlot' 컴포넌트는 해당 슬롯이 클릭되었을 때
    /// 'BattlefieldManager'로 이벤트를 전달하는 역할을 합니다.
    /// </summary>
    private void SetupBattlefieldSlots()
    {
        for (int i = 0; i < spawnPoints.Count; i++)
        {
            var slotTransform = spawnPoints[i]; // 현재 반복 중인 슬롯의 Transform 가져오기
            if (slotTransform == null)
            {
                // 해당 'spawnPoint'가 인스펙터에서 비어있을 경우 경고를 로깅하고 건너뜁니다.
                Debug.LogWarning($"SpawnPoints[{i}]가 인스펙터에서 비어있습니다. 해당 슬롯은 사용되지 않습니다.");
                occupiedSpawnPoints[i] = null; // 해당 슬롯은 점유되지 않은 것으로 초기 상태를 표시합니다.
                continue; // 다음 반복으로 넘어갑니다.
            }

            var slotGO = slotTransform.gameObject; // Transform에서 GameObject 가져오기
            // 'slotGO'에 'BattlefieldSlot' 컴포넌트가 이미 있는지 확인하고, 없으면 새로 추가합니다.
            var slot = slotGO.GetComponent<BattlefieldSlot>() ?? slotGO.AddComponent<BattlefieldSlot>();
            // 'BattlefieldSlot' 컴포넌트에 현재 슬롯의 배열 인덱스를 설정합니다.
            slot.SetSlotIndex(i);
        }
    }

    /// <summary>
    /// 게임 화면의 'placementInfoText' UI에 메시지를 표시하고,
    /// 지정된 시간(3초) 후에 자동으로 숨겨지도록 코루틴을 시작합니다.
    /// </summary>
    /// <param name="message">UI에 표시할 문자열 메시지입니다.</param>
    public void ShowPlacementInfo(string message)
    {
        if (placementInfoText == null) return; // UI 텍스트 컴포넌트가 연결되어 있지 않으면 아무것도 하지 않습니다.
        placementInfoText.text = message; // UI 텍스트의 내용을 전달받은 메시지로 설정합니다.
        placementInfoText.gameObject.SetActive(true); // UI 텍스트 게임 오브젝트를 활성화하여 화면에 보이게 합니다.
        StopAllCoroutines(); // 이 스크립트에서 현재 실행 중인 모든 코루틴(이전의 'HidePlacementInfoAfterDelay')을 중지합니다.
        StartCoroutine(HidePlacementInfoAfterDelay(3f)); // 3초 후에 UI를 숨기는 코루틴을 새로 시작합니다.
    }

    /// <summary>
    /// 지정된 'delay' 시간만큼 대기한 후, 'placementInfoText' UI를 숨기는 코루틴입니다.
    /// </summary>
    /// <param name="delay">UI를 숨기기 전까지 대기할 시간(초)입니다.</param>
    private IEnumerator HidePlacementInfoAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay); // 'delay' 시간만큼 실행을 일시 중지합니다.
        HidePlacementInfo(); // 대기 후 UI를 숨기는 메서드를 호출합니다.
    }

    /// <summary>
    /// 'placementInfoText' UI를 즉시 숨깁니다.
    /// </summary>
    public void HidePlacementInfo()
    {
        if (placementInfoText != null)
            placementInfoText.gameObject.SetActive(false); // UI 텍스트 게임 오브젝트를 비활성화하여 화면에서 숨깁니다.
    }

    /// <summary>
    /// 플레이어가 손패에서 필드에 내려놓기 위해 선택한 카드 GameObject를 설정합니다.
    /// 이 메서드는 일반적으로 손패에 있는 카드가 클릭되었을 때 'CardSelector' 또는 'HandManager'에서 호출됩니다.
    /// </summary>
    /// <param name="cardGO">필드에 배치할 (손패의) 카드 GameObject입니다.</param>
    public void SetCardWaitingForPlacement(GameObject cardGO)
    {
        cardWaitingForPlacement = cardGO; // 전달받은 카드를 배치 대기 중인 카드로 설정합니다.
        // 사용자에게 현재 카드를 필드 슬롯에 배치해야 함을 알리는 메시지를 UI에 표시합니다.
        ShowPlacementInfo($"'{cardGO.name}' 카드 배치 대기 중. 슬롯을 클릭하세요.");
    }

    /// <summary>
    /// 필드의 슬롯이 클릭되었을 때 'BattlefieldSlot' 컴포넌트로부터 호출되는 주요 메서드입니다.
    /// 이 메서드 내에서 카드 배치에 필요한 모든 유효성 검사 및 실제 배치 로직이 수행됩니다.
    /// </summary>
    /// <param name="clickedSlotIndex">클릭된 필드 슬롯의 고유 인덱스입니다.</param>
    public void OnSlotClicked(int clickedSlotIndex)
    {
        // 1. 배치 대기 중인 카드가 있는지 확인합니다.
        if (cardWaitingForPlacement == null)
        {
            ShowPlacementInfo("먼저 손패에서 카드를 선택하세요!"); // 카드를 선택하지 않았다면 메시지 표시
            return; // 이후 로직을 실행하지 않고 함수를 종료합니다.
        }

        // 2. 클릭된 슬롯 인덱스의 유효성을 검사합니다.
        // 인덱스가 유효한 범위 내에 있고, 해당 'spawnPoint'가 실제로 존재하는지 확인합니다.
        if (clickedSlotIndex < 0 || clickedSlotIndex >= spawnPoints.Count || spawnPoints[clickedSlotIndex] == null)
        {
            ShowPlacementInfo("유효하지 않은 슬롯입니다."); // 잘못된 슬롯 클릭 시 메시지 표시
            ClearCardWaitingForPlacement(); // 배치 대기 상태를 해제합니다.
            return;
        }

        // 3. 해당 슬롯이 이미 다른 카드로 점유되어 있는지 확인합니다.
        if (occupiedSpawnPoints[clickedSlotIndex] != null)
        {
            ShowPlacementInfo("이미 다른 카드가 있습니다!"); // 슬롯이 이미 차있다면 메시지 표시
            return;
        }

        // 4. 배치 대기 중인 카드로부터 'CardDisplay' 컴포넌트와 'CardData'를 가져옵니다.
        // 손패 카드는 'CardDisplay' 컴포넌트만 가지고 있고, 그 안에 'CardData'가 저장되어 있다고 가정합니다.
        var originalDisplay = cardWaitingForPlacement.GetComponent<CardDisplay>();
        if (originalDisplay == null || originalDisplay.GetCardData() == null) // 'GetCardData()' 메서드를 사용하여 'CardData'를 가져옵니다.
        {
            Debug.LogError("CardDisplay 또는 CardData가 손패 카드에 없습니다. 손패 카드의 설정 및 데이터 할당을 확인하세요.", cardWaitingForPlacement);
            ClearCardWaitingForPlacement(); // 오류 발생 시 배치 대기 상태 해제
            return;
        }
        CardData cardDataToPlay = originalDisplay.GetCardData(); // 실제 필드에 소환할 카드의 데이터를 가져옵니다.

        // 5. 카드 소환에 필요한 코스트(자원)를 확인합니다.
        int cost = cardDataToPlay.Cost; // 'CardData'에 정의된 카드의 비용을 가져옵니다.
        if (playerCostManager == null)
        {
            Debug.LogError("PlayerCostManager가 연결되지 않았습니다. 코스트 관리자를 설정해주세요.");
            ShowPlacementInfo("코스트 관리자 없음. 설정이 필요합니다.");
            ClearCardWaitingForPlacement();
            return;
        }

        if (playerCostManager.CurrentCost < cost) // 플레이어의 현재 코스트가 카드 비용보다 적은지 확인
        {
            ShowPlacementInfo($"코스트 부족! ({cost - playerCostManager.CurrentCost} 필요)"); // 코스트 부족 메시지 표시
            return;
        }

        // 6. 플레이어의 코스트를 소모합니다.
        if (!playerCostManager.RemoveCost(cost)) // 'PlayerCostManager'를 통해 코스트를 소모 시도
        {
            ShowPlacementInfo("코스트 소모 중 오류가 발생했습니다!"); // 코스트 소모 실패 시 메시지 표시
            return;
        }

        // --- 여기부터는 카드 배치에 성공했을 때의 핵심 게임 로직입니다. ---

        // 1) 'fieldCardPrefab'을 인스턴스화하여 필드에 새로운 카드 오브젝트를 생성합니다.
        // 'spawnPoints[clickedSlotIndex]'를 부모 Transform으로 설정하여 해당 슬롯의 위치에 생성합니다.
        // 'false'는 Instantiate 시 월드 공간에서의 회전과 크기를 유지하지 않고, 부모에 상대적으로 설정함을 의미합니다 (UI 요소에 적합).
        var newFieldCardGO = Instantiate(fieldCardPrefab, spawnPoints[clickedSlotIndex], false);

        // 2) 새로 생성된 필드 카드의 'FieldCard' 컴포넌트를 가져와 초기화합니다.
        // 'fieldCardPrefab'에는 'FieldCard.cs' 스크립트가 미리 부착되어 있어야 합니다.
        FieldCard newFieldCard = newFieldCardGO.GetComponent<FieldCard>();
        if (newFieldCard != null)
        {
            // 'FieldCard'의 'SetCardData' 메서드를 호출하여, 손패 카드로부터 가져온 'CardData'를 전달합니다.
            // 이 카드('newFieldCard')의 진영을 플레이어 진영으로 설정합니다.
            newFieldCard.Initialize(cardDataToPlay, FieldCard.CardFaction.Player);
            // 카드가 필드에 소환되었을 때 발동하는 능력(예: 전함)을 처리하는 메서드를 호출합니다.

        }
        else
        {
            // 만약 'fieldCardPrefab'에 'FieldCard' 컴포넌트가 없다면 심각한 오류를 로깅합니다.
            Debug.LogError($"BattlefieldManager: 생성된 필드 카드 '{newFieldCardGO.name}'에 FieldCard 컴포넌트가 없습니다! 'fieldCardPrefab' 설정을 확인해주세요.", newFieldCardGO);
            Destroy(newFieldCardGO); // 잘못 생성된 카드 오브젝트를 즉시 파괴합니다.
            ClearCardWaitingForPlacement(); // 배치 대기 상태를 해제합니다.
            return;
        }

        // 3) 슬롯별 Transform 적용 (만약 'BattlefieldSlot'에 커스텀 배치 로직이 있다면)
        // 'newFieldCardGO'가 UI 요소(Canvas의 자식)라면 'RectTransform'이 필요합니다.
        var fieldRect = newFieldCardGO.GetComponent<RectTransform>();
        var slotComp = spawnPoints[clickedSlotIndex].GetComponent<BattlefieldSlot>();
        if (slotComp != null && fieldRect != null)
            // 'BattlefieldSlot'에 정의된 'ApplyPlacementTransform' 메서드를 호출하여
            // 카드의 위치나 크기를 슬롯에 맞게 조정합니다.
            slotComp.ApplyPlacementTransform(fieldRect);

        // 4) 'occupiedSpawnPoints' 배열에 새로 배치된 필드 카드 GameObject를 등록하여 슬롯이 점유되었음을 표시합니다.
        occupiedSpawnPoints[clickedSlotIndex] = newFieldCardGO;

        // 5) 손패에 있던 원본 카드 제거 및 'HandManager'에 배치 성공 알림
        // 'handManager'가 유효한지 확인합니다.
        if (handManager != null)
            // 'HandManager'의 'NotifyCardPlacedSuccessfully' 메서드를 호출하여,
            // 손패에서 해당 카드 GameObject를 제거하고 손패를 재정렬하도록 알립니다.
            handManager.NotifyCardPlacedSuccessfully(cardWaitingForPlacement);

        // 필드에 새로운 카드를 배치했으므로, 손패에 있던 원본 카드 GameObject를 파괴합니다.
        Destroy(cardWaitingForPlacement);

        // 6. 배치 완료 메시지 표시 및 상태 초기화
        ShowPlacementInfo($"{cardDataToPlay.CardName} 배치 완료! (-{cost} 코스트)");
        ClearCardWaitingForPlacement(); // 카드 배치 대기 상태를 해제하고 UI를 숨깁니다.
    }

    /// <summary>
    /// 'cardWaitingForPlacement' 변수를 초기화(null로 설정)하고,
    /// 관련 UI(배치 정보 텍스트)를 숨기는 메서드입니다.
    /// 또한, 이전에 선택되었던 카드(손패 카드)의 선택 상태를 해제합니다.
    /// </summary>
    public void ClearCardWaitingForPlacement()
    {
        // 배치 대기 중인 카드가 있다면, 해당 카드의 'CardSelector' 컴포넌트를 찾아 선택 상태를 해제합니다.
        if (cardWaitingForPlacement != null)
        {
            var selector = cardWaitingForPlacement.GetComponent<CardSelector>();
            if (selector != null && selector.IsSelected())
                selector.DeselectExternally(); // 'CardSelector'가 외부에서 선택 해제되도록 요청합니다.
        }
        cardWaitingForPlacement = null; // 'cardWaitingForPlacement' 변수를 null로 설정하여 참조를 해제합니다.
        HidePlacementInfo(); // 배치 정보 UI를 숨깁니다.
    }

    /// <summary>
    /// 필드에서 특정 카드 GameObject를 제거하고 해당 슬롯의 점유 상태를 해제합니다.
    /// 이 메서드는 보통 필드 카드가 사망했을 때 'FieldCard.Die()' 메서드에서
    /// 'BattlefieldManager'를 호출하여 사용될 수 있습니다.
    /// </summary>
    /// <param name="cardGO">필드에서 제거할 카드 GameObject입니다.</param>
    public void RemoveCardFromBattlefield(GameObject cardGO)
    {
        for (int i = 0; i < occupiedSpawnPoints.Length; i++)
        {
            if (occupiedSpawnPoints[i] == cardGO) // 'occupiedSpawnPoints' 배열에서 해당 카드를 찾습니다.
            {
                occupiedSpawnPoints[i] = null; // 해당 슬롯의 점유 상태를 해제합니다 (null로 설정).
                Debug.Log($"{cardGO.name}이(가) 필드에서 성공적으로 제거됨."); 
                return;
            }
        }
        // 배열에서 카드를 찾지 못했을 경우 경고를 로깅합니다.
        Debug.LogWarning($"{cardGO.name}은(는) 필드에 등록된 카드가 아니므로 제거할 수 없습니다.");
    }
}