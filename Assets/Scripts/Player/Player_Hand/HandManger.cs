using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandManager : MonoBehaviour
{
    // --- �ν����Ϳ��� ���� ������ ������ ---

    [Header("-------- ī�� �θ� ������Ʈ -------------")]
    [SerializeField] Transform handArea;
    // [SerializeField] RectTransform mainCanvasRectTransform; // CardZoom ������ ���ʿ�

    [Header("-------- ī�� ������ -------------")]
    [SerializeField] GameObject cardPrefab;

    [Header("-------- �� -------------")]
    [SerializeField] List<CardData> drawPile;

    [SerializeField] int maxHandSize = 7;
    [SerializeField] float cardSpacing = 120f;

    private List<GameObject> handCards = new List<GameObject>();
    private CardHover currentlyHoveredCard = null;

    // --- ���� �߰��� ���� ---
    private GameObject currentlySelectedCard = null; // ���� ���õ� ī�� GameObject

    /// <summary>
    /// ������ ī�带 �� �� �̾� ���п� �߰��մϴ�.
    /// </summary>
    public void DrawCard()
    {
        if (handCards.Count >= maxHandSize)
        {
            Debug.Log("���а� ���� á���ϴ�!");
            return;
        }

        if (drawPile.Count == 0)
        {
            Debug.Log("���� ������ϴ�!");
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

        // CardZoom ���� �ڵ�� ����
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
    /// ���� ���п� �ִ� ī����� ������ ���ݿ� ���� �����մϴ�.
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

            // ���� ���õ� ī����, ���� ��ġ�� ������ �߾����� ����
            // (���� ���°� �����ǵ���, �ٸ� ī��� �ٸ��� ��ġ�� �� ����)
            if (card == currentlySelectedCard)
            {
                // ���õ� ī���� ��ġ�� �߾ӿ� �����ϰ� �ʹٸ� �� ������ ���
                // targetPos.x = 0; // ����: �߾� ����
            }

            card.transform.localPosition = targetPos;

            CardHover hover = card.GetComponent<CardHover>();
            if (hover != null)
            {
                hover.SetOriginalPosition(targetPos);
            }

            // CardZoom ���� �ڵ�� ����
            // CardZoom cardZoom = cardGO.GetComponent<CardZoom>();
            // if (cardZoom != null)
            // {
            //     cardZoom.SetOriginalPositionAndScale(targetPos, card.transform.localScale);
            // }
        }
    }

    /// <summary>
    /// CardHover ��ũ��Ʈ�κ��� ���콺 ȣ�� �˸��� �޽��ϴ�.
    /// ȣ���� ī��� �ٸ� ī����� ��ġ�� �����ϴ� ������ �մϴ�.
    /// </summary>
    /// <param name="card">���� ȣ���ǰų� ȣ���� ������ CardHover ������Ʈ</param>
    /// <param name="isHovering">ȣ�� ���̸� true, ȣ�� ������ false</param>
    public void OnCardHovered(CardHover card, bool isHovering)
    {
        // �̹� ī�尡 ���õ� ���¶�� ȣ�� ������ �����մϴ�.
        // ���õ� ī���� ȣ�� ���´� CardSelector�� �����մϴ�.
        if (currentlySelectedCard != null && currentlySelectedCard.GetComponent<CardSelector>().IsSelected())
        {
            // ������, ���� ȣ���� ī�尡 ���õ� ī�� �ڽ��� ���� ����
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
    /// ���� ȣ���� ī�带 �������� �ֺ� ī����� ������ �����ֵ��� �մϴ�.
    /// </summary>
    private void ApplySideShift()
    {
        // ���� ���õ� ī�尡 �ִٸ�, �� ī�带 �������� �����ֱ� ������ ����
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
    /// ��� ������� ī����� ������ ���� ��ġ�� �ǵ����ϴ�.
    /// (���콺 ȣ���� �����Ǿ��� �� ȣ���)
    /// </summary>
    private void ResetSideShift()
    {
        // ���õ� ī�尡 �ִٸ�, �����ֱ� �������� ����
        if (currentlySelectedCard != null) return;

        foreach (GameObject cardGO in handCards)
        {
            cardGO.GetComponent<CardHover>()?.ResetShift();
        }
    }

    // --- ���� �߰��� �޼���� ---

    /// <summary>
    /// CardSelector�κ��� ī�� ���� �˸��� �޽��ϴ�.
    /// </summary>
    public void OnCardSelected(GameObject selectedCardGO)
    {
        // �ٸ� ī�尡 �̹� ���õǾ� �ִٸ�, �ش� ī���� ������ �����մϴ�.
        DeselectAllCards(selectedCardGO);

        currentlySelectedCard = selectedCardGO;
        // ���õ� ī�尡 ȣ���� ��ó�� ���̱� ���� ������ ApplySideShift ȣ��
        // (CardHover�� isHovered ���¸� isSelected ���·� �����ϵ��� CardHover�� �����ؾ� ��)
        // ���� ���������� `ApplySideShift`�� `currentlySelectedCard`�� �����ϵ��� ���������Ƿ� �ٽ� ȣ��
        ApplySideShift();
        Debug.Log("���� ���õ� ī��: " + currentlySelectedCard.name);

        // ���õ� ī��� ȣ�� �ִϸ��̼ǿ��� �� ���� Y ��ġ�� ������ �մϴ�.
        // �̴� CardHover�� Update() ������ isSelected�� �ν��Ͽ� ó���ؾ� �մϴ�.
    }

    /// <summary>
    /// ���õ� ī�带 ������ ��� ī���� ������ �����մϴ�.
    /// </summary>
    public void DeselectAllCards(GameObject exceptCard = null)
    {
        if (currentlySelectedCard != null && currentlySelectedCard != exceptCard)
        {
            CardSelector selector = currentlySelectedCard.GetComponent<CardSelector>();
            if (selector != null)
            {
                selector.DeselectExternally(); // �ܺο��� ���� ���� ȣ��
            }
            currentlySelectedCard = null;
            ResetSideShift(); // ���� ���� �Ŀ��� �ֺ� ī�� ��ġ�� �������
        }
    }

    /// <summary>
    /// CardSelector�κ��� ī�� ��ġ �˸��� �޽��ϴ�.
    /// </summary>
    public void OnCardPlaced(GameObject placedCardGO)
    {
        handCards.Remove(placedCardGO); // ���� ����Ʈ���� ī�� ����
        if (currentlySelectedCard == placedCardGO)
        {
            currentlySelectedCard = null; // ���õ� ī�忴�ٸ� �ʱ�ȭ
        }
        ResetSideShift(); // ī�� ��ġ �Ŀ��� �ֺ� ī�� ��ġ�� �������
        ArrangeHandCards(); // ���� ī�� ������
        Debug.Log(placedCardGO.name + " ī�尡 ��ġ�Ǿ� ���п��� ���ŵǾ����ϴ�.");
        // CardSelector���� GameObject.SetActive(false)�� ȣ���Ͽ� ������� ó��
    }

    /// <summary>
    /// CardSelector�κ��� ī�� ���� ��� �˸��� �޽��ϴ�.
    /// </summary>
    public void OnCardDeselected(GameObject deselectedCardGO)
    {
        if (currentlySelectedCard == deselectedCardGO)
        {
            currentlySelectedCard = null; // ���õ� ī�� �ʱ�ȭ
        }
        ResetSideShift(); // ���� ��� �� �ֺ� ī�� ��ġ �������
        // OnPointerExit���� �̹� CardHover�� ȣ�� ���¸� �����ϹǷ� ���⼭ �߰����� �۾��� �ʿ� ����
        Debug.Log(deselectedCardGO.name + " ī���� ������ ��ҵǾ����ϴ�.");
    }
}