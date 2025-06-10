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
    [Header("---------- 다른 매니저 참조 ---------")]
    [SerializeField] private BattlefieldManager battlefieldManager;
    [SerializeField] private PlayerCostManager playerCostManager; 

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
        if (playerCostManager == null) 
        {
            playerCostManager = FindObjectOfType<PlayerCostManager>();
            if (playerCostManager == null)
            {
                Debug.LogWarning("HandManager: PlayerCostManager가 연결되지 않았거나 씬에서 찾을 수 없습니다. (필요 시 연결)", this);
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
    /// 선택된 카드를 BattlefieldManager에게 알리고, 배치 대기 상태로 만듭니다.
    /// </summary>
    public void OnCardSelected(GameObject selectedCardGO)
    {
        DeselectAllCards(selectedCardGO); // 다른 카드 선택 해제

        currentlySelectedCard = selectedCardGO;
        ApplySideShift(); // 선택된 카드를 기준으로 주변 카드 이동
        Debug.Log("현재 선택된 카드: " + currentlySelectedCard.name);

        // BattlefieldManager에게 현재 이 카드를 배치 대기 상태로 설정하도록 알립니다.
        if (battlefieldManager != null)
        {
            battlefieldManager.SetCardWaitingForPlacement(currentlySelectedCard);
        }

        // UI 텍스트 업데이트는 CardSelector가 직접 처리합니다. (ShowSelectionInfo() 호출)
    }

    /// <summary>
    /// 선택된 카드를 제외한 모든 카드의 선택을 해제합니다.
    /// 이 메서드는 현재 선택된 카드만 초기화하고, 실제 카드의 DeselectExternally는 CardSelector가 호출합니다.
    /// </summary>
    public void DeselectAllCards(GameObject exceptCard = null)
    {
        // 손패 리스트에 있는 모든 카드를 순회하며 선택 해제
        foreach (GameObject cardGO in handCards)
        {
            if (cardGO != exceptCard && cardGO != null) 
            {
                CardSelector selector = cardGO.GetComponent<CardSelector>();
                if (selector != null && selector.IsSelected())
                {
                    selector.DeselectExternally(); // 외부에서 선택 해제 호출
                }
            }
        }

        // 현재 선택된 카드도 상태 정리 (다른 카드를 선택했거나 모두 해제하는 경우)
        if (currentlySelectedCard != null && currentlySelectedCard != exceptCard)
        {
             currentlySelectedCard = null;
             ResetSideShift(); // 선택 해제 후에는 주변 카드 위치도 원래대로
        }

        // BattlefieldManager에게도 카드 배치 대기 상태를 해제하도록 알립니다.
        if (battlefieldManager != null)
        {
            battlefieldManager.ClearCardWaitingForPlacement();
        }
    }

    /// <summary>
    /// BattlefieldManager로부터 카드가 성공적으로 배치되었음을 알립니다.
    /// </summary>
    /// <param name="placedCardGO">성공적으로 배치된 카드 GameObject</param>
    public void NotifyCardPlacedSuccessfully(GameObject placedCardGO)
    {
        handCards.Remove(placedCardGO); // 손패 리스트에서 카드 제거
        if (currentlySelectedCard == placedCardGO)
        {
            currentlySelectedCard = null; // 선택된 카드였다면 초기화
        }
        ResetSideShift(); // 카드 배치 후에는 주변 카드 위치를 원래대로
        ArrangeHandCards(); // 남은 카드 재정렬
        Debug.Log(placedCardGO.name + " 카드가 성공적으로 배치되어 손패에서 제거되었습니다.");
        // CardSelector에서 GameObject.SetActive(false)를 이미 호출합니다.
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

        // BattlefieldManager에게도 카드 배치 대기 상태를 해제하도록 알립니다.
        if (battlefieldManager != null)
        {
            battlefieldManager.ClearCardWaitingForPlacement();
        }
    }

    // --- 전역 UI 텍스트 제어 메서드 ---

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