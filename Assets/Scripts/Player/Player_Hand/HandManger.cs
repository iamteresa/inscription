using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandManager : MonoBehaviour
{
    // --- 인스펙터에서 설정 가능한 변수들 ---

    [Header("-------- 카드 부모 오브젝트 -------------")]
    [SerializeField] Transform handArea;
    // [SerializeField] RectTransform mainCanvasRectTransform; // CardZoom 삭제로 불필요

    [Header("-------- 카드 프리팹 -------------")]
    [SerializeField] GameObject cardPrefab;

    [Header("-------- 덱 -------------")]
    [SerializeField] List<CardData> drawPile;

    [SerializeField] int maxHandSize = 7;
    [SerializeField] float cardSpacing = 120f;

    private List<GameObject> handCards = new List<GameObject>();
    private CardHover currentlyHoveredCard = null;

    // --- 새로 추가된 변수 ---
    private GameObject currentlySelectedCard = null; // 현재 선택된 카드 GameObject

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
            //selector.SelectionInfoText =  ;

        }

        // CardZoom 관련 코드는 제거
        // CardZoom cardZoom = cardGO.GetComponent<CardZoom>();
        // if(cardZoom != null)
        // {
        // cardZoom.SetMainCanvasRectTransform(mainCanvasRectTransform);
        // }

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

            // 현재 선택된 카드라면, 정렬 위치를 강제로 중앙으로 유지
            // (선택 상태가 유지되도록, 다른 카드와 다르게 배치할 수 있음)
            if (card == currentlySelectedCard)
            {
                // 선택된 카드의 위치를 중앙에 고정하고 싶다면 이 로직을 사용
                // targetPos.x = 0; // 예시: 중앙 고정
            }

            card.transform.localPosition = targetPos;

            CardHover hover = card.GetComponent<CardHover>();
            if (hover != null)
            {
                hover.SetOriginalPosition(targetPos);
            }

            // CardZoom 관련 코드는 제거
            // CardZoom cardZoom = cardGO.GetComponent<CardZoom>();
            // if (cardZoom != null)
            // {
            //     cardZoom.SetOriginalPositionAndScale(targetPos, card.transform.localScale);
            // }
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

    // --- 새로 추가된 메서드들 ---

    /// <summary>
    /// CardSelector로부터 카드 선택 알림을 받습니다.
    /// </summary>
    public void OnCardSelected(GameObject selectedCardGO)
    {
        // 다른 카드가 이미 선택되어 있다면, 해당 카드의 선택을 해제합니다.
        DeselectAllCards(selectedCardGO);

        currentlySelectedCard = selectedCardGO;
        // 선택된 카드가 호버된 것처럼 보이기 위해 강제로 ApplySideShift 호출
        // (CardHover의 isHovered 상태를 isSelected 상태로 제어하도록 CardHover를 수정해야 함)
        // 현재 로직에서는 `ApplySideShift`가 `currentlySelectedCard`를 참조하도록 수정했으므로 다시 호출
        ApplySideShift();
        Debug.Log("현재 선택된 카드: " + currentlySelectedCard.name);

        // 선택된 카드는 호버 애니메이션에서 더 높은 Y 위치를 가져야 합니다.
        // 이는 CardHover의 Update() 로직이 isSelected를 인식하여 처리해야 합니다.
    }

    /// <summary>
    /// 선택된 카드를 제외한 모든 카드의 선택을 해제합니다.
    /// </summary>
    public void DeselectAllCards(GameObject exceptCard = null)
    {
        if (currentlySelectedCard != null && currentlySelectedCard != exceptCard)
        {
            CardSelector selector = currentlySelectedCard.GetComponent<CardSelector>();
            if (selector != null)
            {
                selector.DeselectExternally(); // 외부에서 선택 해제 호출
            }
            currentlySelectedCard = null;
            ResetSideShift(); // 선택 해제 후에는 주변 카드 위치도 원래대로
        }
    }

    /// <summary>
    /// CardSelector로부터 카드 배치 알림을 받습니다.
    /// </summary>
    public void OnCardPlaced(GameObject placedCardGO)
    {
        handCards.Remove(placedCardGO); // 손패 리스트에서 카드 제거
        if (currentlySelectedCard == placedCardGO)
        {
            currentlySelectedCard = null; // 선택된 카드였다면 초기화
        }
        ResetSideShift(); // 카드 배치 후에는 주변 카드 위치를 원래대로
        ArrangeHandCards(); // 남은 카드 재정렬
        Debug.Log(placedCardGO.name + " 카드가 배치되어 손패에서 제거되었습니다.");
        // CardSelector에서 GameObject.SetActive(false)를 호출하여 사라지게 처리
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
        // OnPointerExit에서 이미 CardHover의 호버 상태를 해제하므로 여기서 추가적인 작업은 필요 없음
        Debug.Log(deselectedCardGO.name + " 카드의 선택이 취소되었습니다.");
    }
}