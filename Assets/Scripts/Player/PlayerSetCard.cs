using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class CardSelector : MonoBehaviour, IPointerClickHandler
{
    private CardHover cardHover;
    private HandManager handManager; // HandManager ����

    private bool isSelected = false; // �� ī�尡 ���õǾ����� ����

    // UI ������ ���� TextMeshProUGUI ����.
    // HandManager�� �� ������ ���� UI �ؽ�Ʈ�� �Ҵ��� �� ���Դϴ�.
    [SerializeField] private TextMeshProUGUI _selectionInfoText;

    // HandManager���� �� �ؽ�Ʈ ������Ʈ�� �����ϰ� ������ �� �ֵ��� public ������Ƽ ����
    public TextMeshProUGUI SelectionInfoText
    {
        get { return _selectionInfoText; }
        set { _selectionInfoText = value; }
    }

    void Awake()
    {
        cardHover = GetComponent<CardHover>();
        if (cardHover == null)
        {
            Debug.LogError("CardSelector: CardHover ������Ʈ�� ã�� �� �����ϴ�.", this);
        }

        // HandManager�� ã�� ���� �ʼ����Դϴ�.
        handManager = FindObjectOfType<HandManager>();
        if (handManager == null)
        {
            Debug.LogError("CardSelector: HandManager�� ã�� �� �����ϴ�.", this);
        }

        // _selectionInfoText�� HandManager�� ��Ÿ�ӿ� ������ ���̹Ƿ�,
        // ���⼭�� null üũ�� HideSelectionInfo()�� ȣ������ �ʽ��ϴ�.
    }

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
                TryToPlaceCard(); // �̹� ���õ� ���¸� ��ġ �õ�
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

        // HandManager���� �ٸ� ī�� ���� ���� ��û
        handManager.DeselectAllCards(this.gameObject);

        isSelected = true;
        // ���õǸ� CardHover�� ȣ�� ���̸� �����ϵ��� CardHover�� Update() ������ ó���մϴ�.

        // HandManager�� ���õ� ī�� �˸� (BattlefieldManager�� ����� �� ����)
        handManager.OnCardSelected(this.gameObject);
        Debug.Log(this.gameObject.name + " ī�尡 ���õǾ����ϴ�.");

        ShowSelectionInfo(); // UI �ؽ�Ʈ ǥ��
    }

    /// <summary>
    /// ī�带 ��ġ�Ϸ��� �õ��մϴ�. HandManager���� �� ��û�� �����ϴ�.
    /// </summary>
    private void TryToPlaceCard()
    {
        if (handManager == null) return;

        // HandManager���� ī�� ��ġ �õ��� �˸��ϴ�.
        // ���� Cost Ȯ�� �� ��ġ ������ BattlefieldManager�� �����մϴ�.
        handManager.OnCardPlaced(this.gameObject);

        // ��ġ �õ� �Ŀ��� �� ī���� ���� ���¸� �����ϰ� UI�� ����ϴ�.
        // ���� ���ο� ������� CardSelector�� ������ ���⼭ �����ϴ�.
        // ���� ��ġ�� �����Ͽ� ī�带 �ٽ� ���з� ���������� �Ѵٸ�,
        // �̴� HandManager �Ǵ� BattlefieldManager���� �߰����� �������� ó���ؾ� �մϴ�.
        isSelected = false;
        if (cardHover != null)
        {
            cardHover.OnPointerExit(null); // ȣ�� ���µ� ����
        }
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

        // HandManager�� ���� ��� �˸�
        handManager.OnCardDeselected(this.gameObject);
        Debug.Log(this.gameObject.name + " ī���� ���� �� ȣ���� ��ҵǾ����ϴ�.");

        HideSelectionInfo(); // UI �ؽ�Ʈ �����
    }

    /// <summary>
    /// �� ī���� ���� ���¸� �ܺο� ���� ������ �� ȣ��˴ϴ�.
    /// (�ַ� HandManager�� DeselectAllCards���� ȣ��)
    /// </summary>
    public void DeselectExternally()
    {
        if (isSelected)
        {
            isSelected = false;
            // CardHover�� ȣ�� ���µ� ����
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
    /// �� �ؽ�Ʈ�� HandManager�� ���� ������ ���� UI �ؽ�Ʈ�� ����մϴ�.
    /// </summary>
    private void ShowSelectionInfo()
    {
        if (_selectionInfoText != null)
        {
            _selectionInfoText.text = "��Ŭ��: ī�� ��ġ | ��Ŭ��: ���� ���"; // ����ڿ��� ������ �޽���
            _selectionInfoText.gameObject.SetActive(true);
        }
        else
        {
            Debug.LogWarning("CardSelector: _selectionInfoText�� �Ҵ���� �ʾҽ��ϴ�. HandManager���� �����Ǿ����� Ȯ���ϼ���.", this);
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