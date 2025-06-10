using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HandManager : MonoBehaviour
{
    // --- �ν����Ϳ��� ���� ������ ������ ---
    [Header("-------- ī�� �θ� ������Ʈ -------------")]
    [SerializeField] Transform handArea;

    [Header("-------- ī�� ������ -------------")]
    [SerializeField] GameObject cardPrefab;

    [Header("-------- �� -------------")]
    [SerializeField] List<CardData> drawPile;

    [SerializeField] int maxHandSize = 7;
    [SerializeField] float cardSpacing = 120f;

    // --- ���� �Ŵ����� (�ν����Ϳ��� ���� �����ϴ� ���� �����ϴ�.) ---
    [Header("---------- �ٸ� �Ŵ��� ���� ---------")]
    [SerializeField] private BattlefieldManager battlefieldManager;
    [SerializeField] private PlayerCostManager playerCostManager; 

    // HandManager�� ������ ���� UI �ؽ�Ʈ (CardSelector�� ����)
    [Header("-------- UI ���� -------------")]
    [SerializeField] private TextMeshProUGUI _globalSelectionInfoText;

    private List<GameObject> handCards = new List<GameObject>();
    private CardHover currentlyHoveredCard = null;
    private GameObject currentlySelectedCard = null; // ���� ���õ� ī�� GameObject

    void Awake()
    {
        // �Ŵ��� ���� �ʱ�ȭ (�ν����� ������ �켱, �ƴϸ� FindObjectOfType)
        if (battlefieldManager == null)
        {
            battlefieldManager = FindObjectOfType<BattlefieldManager>();
            if (battlefieldManager == null)
            {
                Debug.LogError("HandManager: BattlefieldManager�� ������� �ʾҰų� ������ ã�� �� �����ϴ�.", this);
            }
        }
        if (playerCostManager == null) 
        {
            playerCostManager = FindObjectOfType<PlayerCostManager>();
            if (playerCostManager == null)
            {
                Debug.LogWarning("HandManager: PlayerCostManager�� ������� �ʾҰų� ������ ã�� �� �����ϴ�. (�ʿ� �� ����)", this);
            }
        }

        if (_globalSelectionInfoText == null)
        {
            Debug.LogError("HandManager: Global Selection Info Text�� ������� �ʾҽ��ϴ�. �ν����Ϳ��� TextMeshProUGUI ������Ʈ�� �������ּ���.", this);
        }
        HideGlobalSelectionInfo();
    }

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
            // CardSelector�� HandManager�� ���� UI �ؽ�Ʈ�� �Ҵ�
            selector.SelectionInfoText = _globalSelectionInfoText;
        }

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

            card.transform.localPosition = targetPos;

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

    // --- CardSelector�� �����Ǵ� �޼���� ---

    /// <summary>
    /// CardSelector�κ��� ī�� ���� �˸��� �޽��ϴ�.
    /// ���õ� ī�带 BattlefieldManager���� �˸���, ��ġ ��� ���·� ����ϴ�.
    /// </summary>
    public void OnCardSelected(GameObject selectedCardGO)
    {
        DeselectAllCards(selectedCardGO); // �ٸ� ī�� ���� ����

        currentlySelectedCard = selectedCardGO;
        ApplySideShift(); // ���õ� ī�带 �������� �ֺ� ī�� �̵�
        Debug.Log("���� ���õ� ī��: " + currentlySelectedCard.name);

        // BattlefieldManager���� ���� �� ī�带 ��ġ ��� ���·� �����ϵ��� �˸��ϴ�.
        if (battlefieldManager != null)
        {
            battlefieldManager.SetCardWaitingForPlacement(currentlySelectedCard);
        }

        // UI �ؽ�Ʈ ������Ʈ�� CardSelector�� ���� ó���մϴ�. (ShowSelectionInfo() ȣ��)
    }

    /// <summary>
    /// ���õ� ī�带 ������ ��� ī���� ������ �����մϴ�.
    /// �� �޼���� ���� ���õ� ī�常 �ʱ�ȭ�ϰ�, ���� ī���� DeselectExternally�� CardSelector�� ȣ���մϴ�.
    /// </summary>
    public void DeselectAllCards(GameObject exceptCard = null)
    {
        // ���� ����Ʈ�� �ִ� ��� ī�带 ��ȸ�ϸ� ���� ����
        foreach (GameObject cardGO in handCards)
        {
            if (cardGO != exceptCard && cardGO != null) 
            {
                CardSelector selector = cardGO.GetComponent<CardSelector>();
                if (selector != null && selector.IsSelected())
                {
                    selector.DeselectExternally(); // �ܺο��� ���� ���� ȣ��
                }
            }
        }

        // ���� ���õ� ī�嵵 ���� ���� (�ٸ� ī�带 �����߰ų� ��� �����ϴ� ���)
        if (currentlySelectedCard != null && currentlySelectedCard != exceptCard)
        {
             currentlySelectedCard = null;
             ResetSideShift(); // ���� ���� �Ŀ��� �ֺ� ī�� ��ġ�� �������
        }

        // BattlefieldManager���Ե� ī�� ��ġ ��� ���¸� �����ϵ��� �˸��ϴ�.
        if (battlefieldManager != null)
        {
            battlefieldManager.ClearCardWaitingForPlacement();
        }
    }

    /// <summary>
    /// BattlefieldManager�κ��� ī�尡 ���������� ��ġ�Ǿ����� �˸��ϴ�.
    /// </summary>
    /// <param name="placedCardGO">���������� ��ġ�� ī�� GameObject</param>
    public void NotifyCardPlacedSuccessfully(GameObject placedCardGO)
    {
        handCards.Remove(placedCardGO); // ���� ����Ʈ���� ī�� ����
        if (currentlySelectedCard == placedCardGO)
        {
            currentlySelectedCard = null; // ���õ� ī�忴�ٸ� �ʱ�ȭ
        }
        ResetSideShift(); // ī�� ��ġ �Ŀ��� �ֺ� ī�� ��ġ�� �������
        ArrangeHandCards(); // ���� ī�� ������
        Debug.Log(placedCardGO.name + " ī�尡 ���������� ��ġ�Ǿ� ���п��� ���ŵǾ����ϴ�.");
        // CardSelector���� GameObject.SetActive(false)�� �̹� ȣ���մϴ�.
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
        Debug.Log(deselectedCardGO.name + " ī���� ������ ��ҵǾ����ϴ�.");

        // BattlefieldManager���Ե� ī�� ��ġ ��� ���¸� �����ϵ��� �˸��ϴ�.
        if (battlefieldManager != null)
        {
            battlefieldManager.ClearCardWaitingForPlacement();
        }
    }

    // --- ���� UI �ؽ�Ʈ ���� �޼��� ---

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