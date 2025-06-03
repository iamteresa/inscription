using System.Collections; // �ڷ�ƾ ����� ���� ����
using System.Collections.Generic; // List<T> ����� ���� ����
using UnityEngine; // Unity ������ �⺻ ��� ����� ���� ����

public class HandManager : MonoBehaviour
{
    // --- �ν����Ϳ��� ���� ������ ������ ---

    [Header("-------- ī�� �θ� ������Ʈ -------------")]
    [SerializeField] Transform handArea;
    [SerializeField] RectTransform mainCanvasRectTransform;

    [Header("-------- ī�� ������ -------------")]
    [SerializeField] GameObject cardPrefab; 

    [Header("-------- �� -------------")]
    [SerializeField] List<CardData> drawPile;       // ���� ���� �� �ִ� ī�� �����͵��� ����Ʈ

    [SerializeField] int maxHandSize = 7;           // �÷��̾� ������ �ִ� ī�� ��
    [SerializeField] float cardSpacing = 120f;      // ���� �� ī��� ������ ����


    private List<GameObject> handCards = new List<GameObject>();    // ���� ���п� �ִ� ī�� GameObject���� ����Ʈ
    private CardHover currentlyHoveredCard = null;                  // ���� ���콺�� �ö� �ִ� (ȣ����) CardHover ������Ʈ ����


    /// <summary>
    /// ������ ī�带 �� �� �̾� ���п� �߰��մϴ�.
    /// </summary>
    public void DrawCard()
    {
        // ���а� �ִ�ġ�� �����ߴ��� Ȯ��
        if (handCards.Count >= maxHandSize)
        {
            Debug.Log("���а� ���� á���ϴ�!");
            return; // ī�� �̱� �ߴ�
        }

        // ���� ������� Ȯ��
        if (drawPile.Count == 0)
        {
            Debug.Log("���� ������ϴ�!");
            return; // ī�� �̱� �ߴ�
        }

        // ������ ������ ī�� ����
        int index = Random.Range(0, drawPile.Count);
        CardData cardData = drawPile[index];

        // ī�� �������� handArea �Ʒ��� ����
        GameObject cardGO = Instantiate(cardPrefab, handArea);

        // ������ ī�忡 CardDisplay ������Ʈ�� �ִٸ� ī�� �����ͷ� UI ������Ʈ
        CardDisplay display = cardGO.GetComponent<CardDisplay>();
        if (display != null)
        {
            display.cardData = cardData;
            display.UpdateDisplay();
        }
        

        // ���� ����Ʈ�� ���� ���� ī�� �߰�
        handCards.Add(cardGO);
        // ������ ���� ī�� ������ ����
        drawPile.RemoveAt(index);

        // ������ ī����� �������մϴ�.
        ArrangeHandCards();
    }

    /// <summary>
    /// ���� ���п� �ִ� ī����� ������ ���ݿ� ���� �����մϴ�.
    /// </summary>
    private void ArrangeHandCards()
    {
        int count = handCards.Count;        // ���� ���п� �ִ� ī�� ��
        if (count == 0) return;             // ���а� ������ return

        // ī����� �߾ӿ� ���ĵǵ��� ù ī���� X ��ġ ���
        // (�� �ʺ� - ī�� �ϳ� �ʺ�) / 2 �� �ƴ�, ��ü ī����� �߽��� 0�� �ǵ��� ���
        float startX = -(cardSpacing * (count - 1)) / 2f;

        // �� ī�带 ��ȸ�ϸ� ��ġ ����
        for (int i = 0; i < count; i++)
        {
            GameObject card = handCards[i];
            // ���� ī���� ��ǥ X ��ġ ���
            Vector3 targetPos = new Vector3(startX + cardSpacing * i, 0, 0);

            // ī�� GameObject�� ���� ��ġ�� ��� ��ǥ ��ġ�� ����
            card.transform.localPosition = targetPos;

            // ī�忡 CardHover ������Ʈ�� �ִٸ�, �ش� ī���� '���� ���� ��ġ'�� ����
            // �� ��ġ�� CardHover�� ȣ��/�����ֱ� �ִϸ��̼��� ���������� ����մϴ�.
            CardHover hover = card.GetComponent<CardHover>();
            if (hover != null)
            {
                hover.SetOriginalPosition(targetPos);
            }
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
        if (isHovering)
        {
            currentlyHoveredCard = card;    // ���� ȣ���� ī�带 ����
            ApplySideShift();               // �ֺ� ī����� �����ֵ��� ����
        }
        else
        {
            currentlyHoveredCard = null;    // ȣ���� ī�� �������� ����
            ResetSideShift();               // ������� ī����� ���� ��ġ�� �ǵ���
        }
    }

    /// <summary>
    /// ���� ȣ���� ī�带 �������� �ֺ� ī����� ������ �����ֵ��� �մϴ�.
    /// </summary>
    private void ApplySideShift()
    {
        // ȣ���� ī�尡 ������ �ƹ��͵� ���� ����
        if (currentlyHoveredCard == null) return;

        int hoveredIndex = -1;      // ȣ���� ī���� ���� ����Ʈ �� �ε���
        // ���� ����Ʈ�� ��ȸ�ϸ� ȣ���� ī���� �ε��� ã��
        for (int i = 0; i < handCards.Count; i++)
        {
            // ���� ī�尡 ȣ���� ī��� �������� Ȯ��
            if (handCards[i].GetComponent<CardHover>() == currentlyHoveredCard)
            {
                hoveredIndex = i;           // �ε��� ����
                break;                      // ã������ �ݺ� �ߴ�
            }
        }

        // ȣ���� ī�带 ã�� �������� �ƹ��͵� ���� ���� (����)
        if (hoveredIndex == -1) return;

        // ȣ���� ī���� SideShiftAmount ���� ������ (CardHover ��ũ��Ʈ���� ����)
        float shiftAmount = currentlyHoveredCard.SideShiftAmount;

        // ��� ���� ī����� ��ȸ�ϸ� ��ġ ����
        for (int i = 0; i < handCards.Count; i++)
        {
            // ȣ���� ī�庸�� ���ʿ� �ִ� ī��� (�ε����� ���� ���)
            if (i < hoveredIndex)
            {
                // �ش� ī���� CardHover ������Ʈ�� ShiftPosition�� ȣ���Ͽ� �������� �̵�
                handCards[i].GetComponent<CardHover>()?.ShiftPosition(-shiftAmount);
            }
            // ȣ���� ī�庸�� �����ʿ� �ִ� ī��� (�ε����� ū ���)
            else if (i > hoveredIndex)
            {
                // �ش� ī���� CardHover ������Ʈ�� ShiftPosition�� ȣ���Ͽ� ���������� �̵�
                handCards[i].GetComponent<CardHover>()?.ShiftPosition(shiftAmount);
            }
            // ȣ���� ī�� �ڽ��� ��ġ ������ �ʿ� �����Ƿ� �� ��Ͽ� ���Ե��� ����
        }
    }

    /// <summary>
    /// ��� ������� ī����� ������ ���� ��ġ�� �ǵ����ϴ�.
    /// (���콺 ȣ���� �����Ǿ��� �� ȣ���)
    /// </summary>
    private void ResetSideShift()
    {
        // ��� ���� ī�� GameObject���� ��ȸ�ϸ� ResetShift ȣ��
        foreach (GameObject cardGO in handCards)
        {
            cardGO.GetComponent<CardHover>()?.ResetShift();
        }
    }
}