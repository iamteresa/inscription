using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandManager : MonoBehaviour
{
    [Header("-------- 카드 부모 오브젝트 -------------")]
    [SerializeField] Transform handArea;

    [Header("-------- 카드 프리팹 -------------")]
    [SerializeField] GameObject cardPrefab;

    [Header("-------- 덱 -------------")]
    [SerializeField] List<CardData> drawPile;

    [SerializeField] int maxHandSize = 7;
    [SerializeField] float cardSpacing = 120f;
    // [SerializeField] float sideShiftSpeed = 5f; // 이 변수는 현재 사용되지 않으므로 제거하거나 주석 처리해도 됩니다.

    private List<GameObject> handCards = new List<GameObject>();
    private CardHover currentlyHoveredCard = null;

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

        handCards.Add(cardGO);
        drawPile.RemoveAt(index);

        ArrangeHandCards();
    }

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

    public void OnCardHovered(CardHover card, bool isHovering)
    {
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

    private void ApplySideShift()
    {
        if (currentlyHoveredCard == null) return;

        int hoveredIndex = -1;
        for (int i = 0; i < handCards.Count; i++)
        {
            if (handCards[i].GetComponent<CardHover>() == currentlyHoveredCard)
            {
                hoveredIndex = i;
                break;
            }
        }

        if (hoveredIndex == -1) return;

        float shiftAmount = currentlyHoveredCard.SideShiftAmount;

        for (int i = 0; i < handCards.Count; i++)
        {
            if (i < hoveredIndex)
            {
                handCards[i].GetComponent<CardHover>()?.ShiftPosition(-shiftAmount);
            }
            else if (i > hoveredIndex)
            {
                handCards[i].GetComponent<CardHover>()?.ShiftPosition(shiftAmount);
            }
        }
    }

    private void ResetSideShift()
    {
        foreach (GameObject cardGO in handCards)
        {
            cardGO.GetComponent<CardHover>()?.ResetShift();
        }
    }
}