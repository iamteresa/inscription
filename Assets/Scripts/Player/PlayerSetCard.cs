using UnityEngine;
using UnityEngine.EventSystems; // Ŭ�� �̺�Ʈ�� ���� �ʿ�
using TMPro; // TextMeshPro�� ����Ѵٸ� �߰�

public class CardSelector : MonoBehaviour, IPointerClickHandler
{
    private CardHover cardHover; // ���� ������Ʈ�� �ִ� CardHover ����
    private HandManager handManager; // HandManager ����

    private bool isSelected = false; // �� ī�尡 ���õǾ����� ����

    //[Header("----------���� �� Y�� �̵�---------")]
    //[SerializeField] float selectedHeightOffset = 70f; // ���õ� �� �߰��� �ö� ���� (���� CardHover�� ó��)


    [Header("----------UI ����-------------")]
    [SerializeField] private TextMeshProUGUI _selectionInfoText; // TextMeshPro Text ������Ʈ ����

    public TextMeshProUGUI SelectionInfoText { get { return _selectionInfoText; } set { _selectionInfoText = value; } }

    void Awake()
    {
        // ���� ���� ������Ʈ�� �ִ� CardHover ������Ʈ�� �����ɴϴ�.
        cardHover = GetComponent<CardHover>();
        if (cardHover == null)
        {
            Debug.LogError("CardSelector: CardHover ������Ʈ�� ã�� �� �����ϴ�.", this);
        }

        // ������ HandManager�� ã���ϴ�.
        handManager = FindObjectOfType<HandManager>();
        if (handManager == null)
        {
            Debug.LogError("CardSelector: HandManager�� ã�� �� �����ϴ�.", this);
        }

        // UI �ؽ�Ʈ ������Ʈ�� ������� �ʾ��� ��� ���� �α�
        if (_selectionInfoText == null)
        {
            Debug.LogError("CardSelector: Selection Info Text�� ������� �ʾҽ��ϴ�.", this);
        }
        HideSelectionInfo(); // ������ �� �ؽ�Ʈ �����
    }


    // void Update() {}

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left) // ��Ŭ�� ��
        {
            if (!isSelected)
            {
                SelectCard(); // ���õ��� ���� ���¸� ����
            }
            else
            {
                PlaceCard(); // �̹� ���õ� ���¸� ��ġ
            }
        }
        else if (eventData.button == PointerEventData.InputButton.Right) // ��Ŭ�� ��
        {
            CancelSelection(); // ���� �� ȣ�� ���
        }
    }

    /// <summary>
    /// ī�带 ���� ���·� ����ϴ�.
    /// </summary>
    private void SelectCard()
    {
        if (handManager == null) return;

        // �ٸ� ī�尡 �̹� ���õǾ� �ִٸ�, �ش� ī���� ������ ���� �����մϴ�.
        handManager.DeselectAllCards(this.gameObject); // HandManager�� �ٸ� ī�� ���� ���� ��û

        isSelected = true;
        // ���õǸ� CardHover�� ȣ�� ���̸� �����ϵ��� CardHover�� Update() ������ ó���մϴ�.

        // HandManager�� ���õ� ī�� �˸�
        handManager.OnCardSelected(this.gameObject);
        Debug.Log(this.gameObject.name + " ī�尡 ���õǾ����ϴ�.");

        ShowSelectionInfo(); // UI �ؽ�Ʈ ǥ��
    }

    /// <summary>
    /// ī�带 ��ġ(�������)�մϴ�.
    /// </summary>
    private void PlaceCard()
    {
        if (handManager == null) return;

        // ���� ���� ����
        isSelected = false;
        // ȣ�� ���µ� ���� (HandManager�� �˸�)
        if (cardHover != null)
        {
            cardHover.OnPointerExit(null); // ���� EventData ����
        }

        // HandManager�� ī�� ��ġ �˸� (���п��� ����)
        handManager.OnCardPlaced(this.gameObject);
        Debug.Log(this.gameObject.name + " ī�尡 ��ġ�Ǿ����ϴ�.");
        // GameObject ��Ȱ��ȭ (�������)
        this.gameObject.SetActive(false);

        HideSelectionInfo(); // UI �ؽ�Ʈ �����
    }

    /// <summary>
    /// ī���� ���� �� ȣ�� ���¸� ����մϴ�.
    /// </summary>
    private void CancelSelection()
    {
        if (handManager == null) return;

        isSelected = false;
        // CardHover�� ȣ�� ���¸� ����
        if (cardHover != null)
        {
            cardHover.OnPointerExit(null); // ���� EventData ����
        }

        // HandManager�� ���� ��� �˸� (���õ� ī�� ������ �˸�)
        handManager.OnCardDeselected(this.gameObject);
        Debug.Log(this.gameObject.name + " ī���� ���� �� ȣ���� ��ҵǾ����ϴ�.");

        HideSelectionInfo(); // UI �ؽ�Ʈ �����
    }

    /// <summary>
    /// �� ī���� ���� ���¸� �ܺο� ���� ������ �� ȣ��˴ϴ�.
    /// </summary>
    public void DeselectExternally()
    {
        if (isSelected)
        {
            isSelected = false;
            // CardHover�� ȣ�� ���µ� ���� (�ʿ��ϴٸ�)
            if (cardHover != null)
            {
                cardHover.OnPointerExit(null); // ���� EventData ����
            }
            Debug.Log(this.gameObject.name + " ī���� ������ �ܺο��� �����Ǿ����ϴ�.");
            HideSelectionInfo(); // UI �ؽ�Ʈ �����
        }
    }

    /// <summary>
    /// �� ī�尡 ���� ���õ� �������� �ܺο� �˷��ݴϴ�.
    /// </summary>
    public bool IsSelected()
    {
        return isSelected;
    }

    // --- UI �ؽ�Ʈ ���� �޼��� ---

    /// <summary>
    /// ī�� ���� ������ UI�� ǥ���մϴ�.
    /// </summary>
    private void ShowSelectionInfo()
    {
        if (_selectionInfoText != null)
        { 
     
            _selectionInfoText.text = "Left: Setting Card | Right: Cancle";
            _selectionInfoText.gameObject.SetActive(true);
        }
    }

    /// <summary>
    /// ī�� ���� ������ UI���� ����ϴ�.
    /// </summary>
    private void HideSelectionInfo()
    {
        if (_selectionInfoText != null)
        {
            _selectionInfoText.gameObject.SetActive(false);
        }
    }
}