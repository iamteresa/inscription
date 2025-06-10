using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class CardSelector : MonoBehaviour, IPointerClickHandler
{
    private CardHover cardHover;
    private HandManager handManager; // HandManager 참조

    private bool isSelected = false; // 이 카드가 선택되었는지 여부

    // UI 연동을 위한 TextMeshProUGUI 변수. HandManager가 이 변수에 전역 UI 텍스트를 할당해 줄 것입니다.
    [SerializeField] private TextMeshProUGUI _selectionInfoText;

    // HandManager에서 이 텍스트 컴포넌트에 접근하고 설정할 수 있도록 public 프로퍼티 제공
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
            Debug.LogError("CardSelector: CardHover 컴포넌트를 찾을 수 없습니다.", this);
        }

        handManager = FindObjectOfType<HandManager>();
        if (handManager == null)
        {
            Debug.LogError("CardSelector: HandManager를 찾을 수 없습니다.", this);
        }

        // _selectionInfoText는 HandManager가 런타임에 설정할 것이므로,
        // 여기서는 null 체크나 HideSelectionInfo()를 호출하지 않습니다.
    }

    // 마우스 클릭 이벤트 처리
    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left) // 좌클릭 시
        {
            if (!isSelected)
            {
                SelectCard(); // 선택되지 않은 상태면 선택
            }
            // else {
            // 이미 선택된 상태에서 카드 자체를 좌클릭하는 동작은 이제 배치를 트리거하지 않습니다.
            // 대신 사용자가 전장 슬롯을 클릭하여 배치하도록 유도합니다.
            // 필요하다면, 이 else 블록에 카드 정보 상세 보기 등의 다른 기능을 추가할 수 있습니다.
            // }
        }
        else if (eventData.button == PointerEventData.InputButton.Right) // 우클릭 시
        {
            CancelSelection(); // 선택 및 호버 취소
        }
    }

    /// <summary>
    /// 카드를 선택 상태로 만듭니다.
    /// </summary>
    private void SelectCard()
    {
        if (handManager == null) return;

        // HandManager에게 다른 카드 선택 해제 요청
        handManager.DeselectAllCards(this.gameObject);

        isSelected = true;
        // 선택되면 CardHover가 호버 높이를 유지하도록 CardHover의 Update() 로직이 처리합니다.

        // HandManager에 선택된 카드 알림 (BattlefieldManager로 연결되어 배치 대기 상태로 설정)
        handManager.OnCardSelected(this.gameObject);
        Debug.Log(this.gameObject.name + " 카드가 선택되었습니다.");

        ShowSelectionInfo(); // UI 텍스트 표시
    }



    /// <summary>
    /// 카드의 선택 및 호버 상태를 취소합니다.
    /// </summary>
    private void CancelSelection()
    {
        if (handManager == null) return;

        isSelected = false;
        // CardHover의 호버 상태를 해제
        if (cardHover != null)
        {
            cardHover.OnPointerExit(null);
        }

        // HandManager에 선택 취소 알림 (BattlefieldManager에게 배치 대기 상태 해제 요청)
        handManager.OnCardDeselected(this.gameObject);
        Debug.Log(this.gameObject.name + " 카드의 선택 및 호버가 취소되었습니다.");

        HideSelectionInfo(); // UI 텍스트 숨기기
    }

    /// <summary>
    /// 이 카드의 선택 상태를 외부에 의해 해제할 때 호출됩니다.
    /// (주로 HandManager의 DeselectAllCards나 BattlefieldManager의 ClearCardWaitingForPlacement에서 호출)
    /// </summary>
    public void DeselectExternally()
    {
        if (isSelected)
        {
            isSelected = false;
            // CardHover의 호버 상태도 해제
            if (cardHover != null)
            {
                cardHover.OnPointerExit(null);
            }
            Debug.Log(this.gameObject.name + " 카드의 선택이 외부에서 해제되었습니다.");
            HideSelectionInfo(); // UI 텍스트 숨기기
        }
    }

    /// <summary>
    /// 이 카드가 현재 선택된 상태인지 외부에 알려줍니다.
    /// </summary>
    public bool IsSelected()
    {
        return isSelected;
    }

    // --- UI 텍스트 제어 메서드 ---

    /// <summary>
    /// 카드 선택 정보를 UI에 표시합니다.
    /// 이 텍스트는 HandManager에 의해 설정된 전역 UI 텍스트를 사용합니다.
    /// </summary>
    private void ShowSelectionInfo()
    {
        if (_selectionInfoText != null)
        {
            _selectionInfoText.text = "[좌클릭] : 배치 / [우클릭] : 취소"; // 사용자에게 보여줄 메시지
            _selectionInfoText.gameObject.SetActive(true);
        }
        else
        {
            Debug.LogWarning("CardSelector: _selectionInfoText가 할당되지 않았습니다. HandManager에서 설정되었는지 확인하세요.", this);
        }
    }

    /// <summary>
    /// 카드 선택 정보를 UI에서 숨깁니다.
    /// </summary>
    private void HideSelectionInfo()
    {
        if (_selectionInfoText != null)
        {
            _selectionInfoText.gameObject.SetActive(false);
        }
    }
}
