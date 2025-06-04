using UnityEngine;
using UnityEngine.EventSystems; // 클릭 이벤트를 위해 필요
using TMPro; // TextMeshPro를 사용한다면 추가

public class CardSelector : MonoBehaviour, IPointerClickHandler
{
    private CardHover cardHover; // 같은 오브젝트에 있는 CardHover 참조
    private HandManager handManager; // HandManager 참조

    private bool isSelected = false; // 이 카드가 선택되었는지 여부

    //[Header("----------선택 시 Y축 이동---------")]
    //[SerializeField] float selectedHeightOffset = 70f; // 선택될 때 추가로 올라갈 높이 (현재 CardHover가 처리)


    [Header("----------UI 연동-------------")]
    [SerializeField] private TextMeshProUGUI _selectionInfoText; // TextMeshPro Text 컴포넌트 연결

    public TextMeshProUGUI SelectionInfoText { get { return _selectionInfoText; } set { _selectionInfoText = value; } }

    void Awake()
    {
        // 같은 게임 오브젝트에 있는 CardHover 컴포넌트를 가져옵니다.
        cardHover = GetComponent<CardHover>();
        if (cardHover == null)
        {
            Debug.LogError("CardSelector: CardHover 컴포넌트를 찾을 수 없습니다.", this);
        }

        // 씬에서 HandManager를 찾습니다.
        handManager = FindObjectOfType<HandManager>();
        if (handManager == null)
        {
            Debug.LogError("CardSelector: HandManager를 찾을 수 없습니다.", this);
        }

        // UI 텍스트 컴포넌트가 연결되지 않았을 경우 에러 로그
        if (_selectionInfoText == null)
        {
            Debug.LogError("CardSelector: Selection Info Text가 연결되지 않았습니다.", this);
        }
        HideSelectionInfo(); // 시작할 때 텍스트 숨기기
    }


    // void Update() {}

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left) // 좌클릭 시
        {
            if (!isSelected)
            {
                SelectCard(); // 선택되지 않은 상태면 선택
            }
            else
            {
                PlaceCard(); // 이미 선택된 상태면 배치
            }
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

        // 다른 카드가 이미 선택되어 있다면, 해당 카드의 선택을 먼저 해제합니다.
        handManager.DeselectAllCards(this.gameObject); // HandManager에 다른 카드 선택 해제 요청

        isSelected = true;
        // 선택되면 CardHover가 호버 높이를 유지하도록 CardHover의 Update() 로직이 처리합니다.

        // HandManager에 선택된 카드 알림
        handManager.OnCardSelected(this.gameObject);
        Debug.Log(this.gameObject.name + " 카드가 선택되었습니다.");

        ShowSelectionInfo(); // UI 텍스트 표시
    }

    /// <summary>
    /// 카드를 배치(사라지게)합니다.
    /// </summary>
    private void PlaceCard()
    {
        if (handManager == null) return;

        // 선택 상태 해제
        isSelected = false;
        // 호버 상태도 해제 (HandManager에 알림)
        if (cardHover != null)
        {
            cardHover.OnPointerExit(null); // 더미 EventData 전달
        }

        // HandManager에 카드 배치 알림 (손패에서 제거)
        handManager.OnCardPlaced(this.gameObject);
        Debug.Log(this.gameObject.name + " 카드가 배치되었습니다.");
        // GameObject 비활성화 (사라지게)
        this.gameObject.SetActive(false);

        HideSelectionInfo(); // UI 텍스트 숨기기
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
            cardHover.OnPointerExit(null); // 더미 EventData 전달
        }

        // HandManager에 선택 취소 알림 (선택된 카드 없음을 알림)
        handManager.OnCardDeselected(this.gameObject);
        Debug.Log(this.gameObject.name + " 카드의 선택 및 호버가 취소되었습니다.");

        HideSelectionInfo(); // UI 텍스트 숨기기
    }

    /// <summary>
    /// 이 카드의 선택 상태를 외부에 의해 해제할 때 호출됩니다.
    /// </summary>
    public void DeselectExternally()
    {
        if (isSelected)
        {
            isSelected = false;
            // CardHover의 호버 상태도 해제 (필요하다면)
            if (cardHover != null)
            {
                cardHover.OnPointerExit(null); // 더미 EventData 전달
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