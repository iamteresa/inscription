using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HandManager : MonoBehaviour
{
    // --- 인스펙터에서 설정 가능한 변수들 ---
    [Header("-------- 카드 부모 오브젝트 -------------")]
    [SerializeField] Transform handArea;

    [Header("-------- 카드 프리팹 -------------")]
    [SerializeField] GameObject cardPrefab;

    [Header("-------- 덱 -------------")]
    [SerializeField] List<CardData> drawPile;

    [SerializeField] int maxHandSize = 7;
    [SerializeField] float cardSpacing = 120f;

    // --- 참조 매니저들 (인스펙터에서 직접 연결하는 것이 좋습니다.) ---
    [SerializeField] private BattlefieldManager battlefieldManager;
    [SerializeField] private PlayerCostManager playerCostManager; // HandManager가 PlayerCostManager를 직접 참조하는 경우는 드뭅니다. (주로 BattlefieldManager가 참조)

    // HandManager가 관리할 전역 UI 텍스트 (CardSelector에 주입)
    [Header("-------- UI 연동 -------------")]
    [SerializeField] private TextMeshProUGUI _globalSelectionInfoText;

    private List<GameObject> handCards = new List<GameObject>();
    private CardHover currentlyHoveredCard = null;
    private GameObject currentlySelectedCard = null; // 현재 선택된 카드 GameObject

    void Awake()
    {
        // 매니저 참조 초기화 (인스펙터 연결이 우선, 아니면 FindObjectOfType)
        if (battlefieldManager == null)
        {
            battlefieldManager = FindObjectOfType<BattlefieldManager>();
            if (battlefieldManager == null)
            {
                Debug.LogError("HandManager: BattlefieldManager가 연결되지 않았거나 씬에서 찾을 수 없습니다.", this);
            }
        }
        if (playerCostManager == null) // HandManager가 Cost를 직접 다룰 일은 적지만, 필요한 경우를 대비
        {
            playerCostManager = FindObjectOfType<PlayerCostManager>();
            if (playerCostManager == null)
            {
                Debug.LogWarning("HandManager: PlayerCostManager가 연결되지 않았거나 씬에서 찾을 수 없습니다.", this);
            }
        }

        if (_globalSelectionInfoText == null)
        {
            Debug.LogError("HandManager: Global Selection Info Text가 연결되지 않았습니다. 인스펙터에서 TextMeshProUGUI 컴포넌트를 연결해주세요.", this);
        }
        HideGlobalSelectionInfo();
    }

    /// <summary>
    /// 덱에서 카드를 한 장 뽑아 손패에 추가합니다.
    /// </summary>
    public void DrawCard()
    {
        if (handCards.Count >= maxHandSize)
        {
            Debug.Log("손패가 가득 찼습니다!");
            return;
        }

        if (drawPile.Count == 0)
        {
            Debug.Log("덱이 비었습니다!");
            return;
        }

        int index = Random.Range(0, drawPile.Count);
        CardData cardData = drawPile[index];

        GameObject cardGO = Instantiate(cardPrefab, handArea);

        CardDisplay display = cardGO.GetComponent<CardDisplay>();
        if (display != null)
        {
            display.cardData = cardData;
            display.UpdateDisplay();
        }

        CardSelector selector = cardGO.GetComponent<CardSelector>();
        if (selector != null)
        {
            // CardSelector에 HandManager의 전역 UI 텍스트를 할당
            selector.SelectionInfoText = _globalSelectionInfoText;
        }

        handCards.Add(cardGO);
        drawPile.RemoveAt(index);

        ArrangeHandCards();
    }

    /// <summary>
    /// 현재 손패에 있는 카드들을 정해진 간격에 맞춰 정렬합니다.
    /// </summary>
    private void ArrangeHandCards()
    {
        int count = handCards.Count;
        if (count == 0) return;

        float startX = -(cardSpacing * (count - 1)) / 2f;

        for (int i = 0; i < count; i++)
        {
            GameObject card = handCards[i];
            Vector3 targetPos = new Vector3(startX + cardSpacing * i, 0, 0);

            // 선택된 카드는 호버 위치에 고정될 것이므로, 정렬에서 제외하거나 특별 처리
            // CardHover가 이미 이를 처리하므로 여기서는 원래 위치만 설정
            card.transform.localPosition = targetPos;

            CardHover hover = card.GetComponent<CardHover>();
            if (hover != null)
            {
                hover.SetOriginalPosition(targetPos);
            }
        }
    }

    /// <summary>
    /// CardHover 스크립트로부터 마우스 호버 알림을 받습니다.
    /// 호버된 카드와 다른 카드들의 위치를 조정하는 역할을 합니다.
    /// </summary>
    /// <param name="card">현재 호버되거나 호버가 해제된 CardHover 컴포넌트</param>
    /// <param name="isHovering">호버 중이면 true, 호버 해제면 false</param>
    public void OnCardHovered(CardHover card, bool isHovering)
    {
        // 이미 카드가 선택된 상태라면 호버 로직을 무시합니다.
        // 선택된 카드의 호버 상태는 CardSelector가 관리합니다.
        if (currentlySelectedCard != null && currentlySelectedCard.GetComponent<CardSelector>().IsSelected())
        {
            // 하지만, 현재 호버된 카드가 선택된 카드 자신인 경우는 예외
            if (currentlySelectedCard != card.gameObject)
            {
                return;
            }
        }

        if (isHovering)
        {
            currentlyHoveredCard = card;
            ApplySideShift();
        }
        else
        {
            currentlyHoveredCard = null;
            ResetSideShift();
        }
    }

    /// <summary>
    /// 현재 호버된 카드를 기준으로 주변 카드들을 옆으로 비켜주도록 합니다.
    /// </summary>
    private void ApplySideShift()
    {
        // 현재 선택된 카드가 있다면, 그 카드를 기준으로 비켜주기 로직을 적용
        GameObject referenceCard = currentlySelectedCard != null ? currentlySelectedCard : (currentlyHoveredCard != null ? currentlyHoveredCard.gameObject : null);

        if (referenceCard == null) return;

        CardHover referenceHover = referenceCard.GetComponent<CardHover>();
        if (referenceHover == null) return;

        int referenceIndex = -1;
        for (int i = 0; i < handCards.Count; i++)
        {
            if (handCards[i] == referenceCard)
            {
                referenceIndex = i;
                break;
            }
        }

        if (referenceIndex == -1) return;

        float shiftAmount = referenceHover.SideShiftAmount;

        for (int i = 0; i < handCards.Count; i++)
        {
            if (i < referenceIndex)
            {
                handCards[i].GetComponent<CardHover>()?.ShiftPosition(-shiftAmount);
            }
            else if (i > referenceIndex)
            {
                handCards[i].GetComponent<CardHover>()?.ShiftPosition(shiftAmount);
            }
        }
    }

    /// <summary>
    /// 모든 비켜줬던 카드들을 원래의 정렬 위치로 되돌립니다.
    /// (마우스 호버가 해제되었을 때 호출됨)
    /// </summary>
    private void ResetSideShift()
    {
        // 선택된 카드가 있다면, 비켜주기 해제하지 않음
        if (currentlySelectedCard != null) return;

        foreach (GameObject cardGO in handCards)
        {
            cardGO.GetComponent<CardHover>()?.ResetShift();
        }
    }

    // --- CardSelector와 연동되는 메서드들 ---

    /// <summary>
    /// CardSelector로부터 카드 선택 알림을 받습니다.
    /// </summary>
    public void OnCardSelected(GameObject selectedCardGO)
    {
        // 다른 카드가 이미 선택되어 있다면, 해당 카드의 선택을 해제합니다.
        DeselectAllCards(selectedCardGO);

        currentlySelectedCard = selectedCardGO;
        ApplySideShift(); // 선택된 카드를 기준으로 주변 카드 이동
        Debug.Log("현재 선택된 카드: " + currentlySelectedCard.name);

        // UI 텍스트 업데이트는 CardSelector가 직접 처리합니다. (ShowSelectionInfo() 호출)
    }

    /// <summary>
    /// 선택된 카드를 제외한 모든 카드의 선택을 해제합니다.
    /// </summary>
    public void DeselectAllCards(GameObject exceptCard = null)
    {
        // handCards 리스트에 있는 모든 카드를 순회하며 DeselectExternally 호출
        foreach (GameObject cardGO in handCards)
        {
            if (cardGO != exceptCard && cardGO != null) // exceptCard는 선택될 새 카드이므로 제외
            {
                CardSelector selector = cardGO.GetComponent<CardSelector>();
                if (selector != null && selector.IsSelected())
                {
                    selector.DeselectExternally(); // 외부에서 선택 해제 호출
                }
            }
        }

        // 이 로직은 DeselectExternally가 이미 isSelected를 false로 만들고 UI를 숨기므로,
        // 현재 선택된 카드만 초기화하고 ResetSideShift를 호출합니다.
        if (currentlySelectedCard != null && currentlySelectedCard != exceptCard)
        {
            currentlySelectedCard = null;
            ResetSideShift(); // 선택 해제 후에는 주변 카드 위치도 원래대로
        }
        // 만약 exceptCard가 null (모든 카드 해제)이고, 현재 선택된 카드가 없으면 UI도 숨김
        else if (exceptCard == null && currentlySelectedCard == null)
        {
            HideGlobalSelectionInfo(); // 전역 UI 텍스트 숨김 (CardSelector에 주입된 텍스트는 해당 카드가 처리)
        }
    }

    /// <summary>
    /// CardSelector로부터 카드 배치 시도 알림을 받습니다.
    /// 이 메서드는 BattlefieldManager에게 배치를 요청하고, 성공 시 손패에서 카드를 제거합니다.
    /// </summary>
    public void OnCardPlaced(GameObject placedCardGO)
    {
        if (battlefieldManager == null)
        {
            Debug.LogError("HandManager: BattlefieldManager가 없어 카드를 배치할 수 없습니다.", this);
            return;
        }

        // BattlefieldManager에게 카드 배치 요청
        bool placedSuccessfully = battlefieldManager.TryPlaceCard(placedCardGO);

        if (placedSuccessfully)
        {
            // 성공적으로 배치되면 손패에서 카드 제거
            handCards.Remove(placedCardGO);
            if (currentlySelectedCard == placedCardGO)
            {
                currentlySelectedCard = null; // 선택된 카드였다면 초기화
            }
            ResetSideShift(); // 카드 배치 후에는 주변 카드 위치를 원래대로
            ArrangeHandCards(); // 남은 카드 재정렬
            Debug.Log(placedCardGO.name + " 카드가 배치되어 손패에서 제거되었습니다.");
            // CardSelector에서 GameObject.SetActive(false)를 이미 호출합니다.
        }
        else
        {
            // 배치 실패 시:
            // - 카드의 isSelected 상태는 이미 CardSelector에서 TryToPlaceCard 호출 후 false로 설정됩니다.
            // - CardHover의 OnPointerExit(null)도 호출됩니다.
            // - UI 텍스트도 CardSelector에서 HideSelectionInfo()로 숨겨집니다.
            // 따라서 여기서 추가적인 처리 (예: SetActive(true)로 되돌리기)는 필요하지 않을 수 있습니다.
            // 만약 카드를 원래 손패 위치로 되돌리고 싶다면, 여기에서 CardSelector.DeselectExternally() 등을 다시 호출하고,
            // CardHover의 ResetPosition을 호출하는 로직을 추가해야 합니다.
            Debug.Log($"{placedCardGO.name} 카드 배치 실패 (BattlefieldManager에서 원인 확인).");
        }
    }

    /// <summary>
    /// CardSelector로부터 카드 선택 취소 알림을 받습니다.
    /// </summary>
    public void OnCardDeselected(GameObject deselectedCardGO)
    {
        if (currentlySelectedCard == deselectedCardGO)
        {
            currentlySelectedCard = null; // 선택된 카드 초기화
        }
        ResetSideShift(); // 선택 취소 후 주변 카드 위치 원래대로
        Debug.Log(deselectedCardGO.name + " 카드의 선택이 취소되었습니다.");
        // UI 텍스트는 CardSelector에서 HideSelectionInfo()를 호출하여 숨겨집니다.
    }

    // --- 전역 UI 텍스트 제어 메서드 (HandManager에서만 호출, CardSelector에 주입할 텍스트를 관리) ---

    // 이 메서드들은 HandManager가 CardSelector에 주입할 TextMeshProUGUI 인스턴스를 관리하고
    // 필요에 따라 초기 상태를 제어하는 데 사용됩니다.
    // 개별 카드의 선택 정보 표시는 CardSelector가 직접 수행합니다.
    private void ShowGlobalSelectionInfo(string message)
    {
        if (_globalSelectionInfoText != null)
        {
            _globalSelectionInfoText.text = message;
            _globalSelectionInfoText.gameObject.SetActive(true);
        }
    }

    private void HideGlobalSelectionInfo()
    {
        if (_globalSelectionInfoText != null)
        {
            _globalSelectionInfoText.gameObject.SetActive(false);
        }
    }
}