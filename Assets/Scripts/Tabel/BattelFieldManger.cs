using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic; // List를 사용하기 위해 추가

public class BattlefieldManager : MonoBehaviour
{
    [Header("---------- 카드 배치 위치 ---------")]
    // Transform 배열 대신 List를 사용하면 유니티 에디터에서 더 유연하게 관리할 수 있습니다.
    [SerializeField] private List<Transform> spawnPoints = new List<Transform>();

    [Header("---------- 배치 관련 메시지 UI ---------")]
    [SerializeField] private TextMeshProUGUI placementInfoText; // 배치 관련 메시지 표시 UI (TextMeshPro)

    // 다른 매니저 참조 (Awake 또는 Start에서 FindObjectOfType 대신 SerializeField로 연결하는 것이 더 안정적입니다.)
    [SerializeField] private PlayerCostManager playerCostManager; // 인스펙터에서 직접 연결
    [SerializeField] private HandManager handManager;             // 인스펙터에서 직접 연결 (옵션, 필요한 경우)


    // 각 스폰 포인트에 배치된 카드 GameObject를 관리 (슬롯 점유 여부)
    private GameObject[] occupiedSpawnPoints;

    void Awake()
    {
        // 필드 초기화 (List 크기 기반)
        if (spawnPoints.Count > 0)
        {
            occupiedSpawnPoints = new GameObject[spawnPoints.Count];
        }
        else
        {
            Debug.LogError("BattlefieldManager: spawnPoints가 설정되지 않았습니다. 최소 1개 이상의 배치 위치가 필요합니다.", this);
            enabled = false; // 스크립트 비활성화
            return;
        }

        // 필수 컴포넌트 연결 확인 (인스펙터에서 연결하는 것이 가장 좋습니다.)
        if (playerCostManager == null)
        {
            playerCostManager = FindObjectOfType<PlayerCostManager>(); // 폴백: 인스펙터 연결 안 됐을 시 씬에서 찾기
            if (playerCostManager == null)
            {
                Debug.LogError("BattlefieldManager: PlayerCostManager가 연결되지 않았거나 씬에서 찾을 수 없습니다.", this);
            }
        }
        if (handManager == null)
        {
            handManager = FindObjectOfType<HandManager>(); // 폴백
            if (handManager == null)
            {
                Debug.LogWarning("BattlefieldManager: HandManager가 연결되지 않았거나 씬에서 찾을 수 없습니다. (필요 시 연결)", this);
            }
        }


        if (placementInfoText == null)
        {
            Debug.LogError("BattlefieldManager: Placement Info Text가 연결되지 않았습니다.", this);
        }

        HidePlacementInfo(); // 시작 시 배치 정보 UI 숨기기
    }

    /// <summary>
    /// 카드 배치 관련 정보를 UI에 표시합니다.
    /// </summary>
    /// <param name="message">표시할 메시지</param>
    public void ShowPlacementInfo(string message) // 다른 스크립트에서도 호출할 수 있도록 public
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
    public void HidePlacementInfo() // 다른 스크립트에서도 호출할 수 있도록 public
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
    /// 선택된 카드를 배치하려는 시도를 처리합니다.
    /// 이 메서드는 HandManager의 OnCardPlaced에서 호출될 것으로 예상됩니다.
    /// </summary>
    /// <param name="selectedCardGO">선택된 카드 GameObject</param>
    /// <returns>카드가 성공적으로 배치되었으면 true, 아니면 false</returns>
    public bool TryPlaceCard(GameObject selectedCardGO)
    {
        CardDisplay cardDisplay = selectedCardGO.GetComponent<CardDisplay>();
        if (cardDisplay == null || cardDisplay.cardData == null)
        {
            Debug.LogError("BattlefieldManager: 배치하려는 카드에 CardDisplay 또는 CardData가 없습니다.", selectedCardGO);
            return false;
        }

        int cardCost = cardDisplay.cardData.Cost; // CardData의 Cost 프로퍼티 사용

        // 1. PlayerCostManager가 초기화되었는지 확인
        if (playerCostManager == null)
        {
            Debug.LogError("BattlefieldManager: PlayerCostManager가 초기화되지 않았습니다. 카드를 배치할 수 없습니다.", this);
            ShowPlacementInfo("시스템 오류: 코스트 관리자 없음.");
            return false;
        }

        // 2. Cost 확인
        if (playerCostManager.CurrentCost < cardCost)
        {
            ShowPlacementInfo($"코스트 부족! ({cardCost - playerCostManager.CurrentCost} 더 필요)");
            Debug.Log($"코스트 부족: {selectedCardGO.name} (필요 코스트: {cardCost}, 현재 코스트: {playerCostManager.CurrentCost})");
            return false;
        }

        // 3. 배치할 빈 위치 찾기
        int targetSpawnIndex = -1;
        for (int i = 0; i < spawnPoints.Count; i++)
        {
            if (occupiedSpawnPoints[i] == null) // 빈 슬롯 찾기
            {
                targetSpawnIndex = i;
                break;
            }
        }

        if (targetSpawnIndex == -1)
        {
            ShowPlacementInfo("배치할 빈 공간이 없습니다!");
            Debug.Log("모든 배치 공간이 이미 사용 중입니다.");
            return false;
        }

        // 4. Cost 소모
        bool costRemoved = playerCostManager.RemoveCost(cardCost);
        if (!costRemoved) // 이중 체크 (사실 Cost가 부족했다면 위에서 이미 return 됨)
        {
            Debug.LogError("BattlefieldManager: 코스트 소모 중 예상치 못한 오류 발생.", this);
            ShowPlacementInfo("코스트 소모 오류!");
            return false;
        }

        // 5. 카드 배치
        // 카드를 손패에서 전장으로 이동시키는 로직 (부모 변경)
        selectedCardGO.transform.SetParent(spawnPoints[targetSpawnIndex]);
        selectedCardGO.transform.localPosition = Vector3.zero; // 배치 위치의 로컬 원점으로 이동
        selectedCardGO.transform.localRotation = Quaternion.identity; // 로컬 회전 초기화
        selectedCardGO.transform.localScale = Vector3.one; // 로컬 스케일 초기화 (필요하다면 카드별로 조정)

        // 스폰 포인트에 카드 참조 저장
        occupiedSpawnPoints[targetSpawnIndex] = selectedCardGO;

        ShowPlacementInfo($"{selectedCardGO.name}을(를) 배치했습니다! (-{cardCost} 코스트)");
        Debug.Log($"{selectedCardGO.name} 카드가 {spawnPoints[targetSpawnIndex].name}에 배치되었습니다. 남은 코스트: {playerCostManager.CurrentCost}");

        // 카드 배치가 성공했으므로, CardSelector에서 이 카드를 비활성화하거나 파괴하도록 HandManager에게 알림
        // HandManager의 OnCardPlaced가 이 카드 GameObject를 HandCards 리스트에서 제거하고 SetActive(false)를 처리할 것임.
        return true; // 배치 성공
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
                occupiedSpawnPoints[i] = null; // 슬롯 비우기
                // Destroy(cardGO); // 실제 게임에서는 파괴하거나 오브젝트 풀로 반환
                Debug.Log($"{cardGO.name}가 전장에서 제거되었습니다.");
                return;
            }
        }
        Debug.LogWarning($"BattlefieldManager: 전장에서 제거하려는 카드 '{cardGO.name}'를 찾을 수 없습니다.");
    }
}