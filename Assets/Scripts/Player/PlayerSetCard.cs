using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class CardSelector : MonoBehaviour, IPointerClickHandler
{
    private CardHover cardHover;
    private HandManager handManager; // HandManager 참조

    private bool isSelected = false; // 이 카드가 선택되었는지 여부

    // UI 연동을 위한 TextMeshProUGUI 변수.
    // HandManager가 이 변수에 전역 UI 텍스트를 할당해 줄 것입니다.
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

        // HandManager를 찾는 것은 필수적입니다.
        handManager = FindObjectOfType<HandManager>();
        if (handManager == null)
        {
            Debug.LogError("CardSelector: HandManager를 찾을 수 없습니다.", this);
        }

        // _selectionInfoText는 HandManager가 런타임에 설정할 것이므로,
        // 여기서는 null 체크나 HideSelectionInfo()를 호출하지 않습니다.
    }

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
                TryToPlaceCard(); // 이미 선택된 상태면 배치 시도
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

        // HandManager에게 다른 카드 선택 해제 요청
        handManager.DeselectAllCards(this.gameObject);

        isSelected = true;
        // 선택되면 CardHover가 호버 높이를 유지하도록 CardHover의 Update() 로직이 처리합니다.

        // HandManager에 선택된 카드 알림 (BattlefieldManager로 연결될 수 있음)
        handManager.OnCardSelected(this.gameObject);
        Debug.Log(this.gameObject.name + " 카드가 선택되었습니다.");

        ShowSelectionInfo(); // UI 텍스트 표시
    }

    /// <summary>
    /// 카드를 배치하려고 시도합니다. HandManager에게 이 요청을 보냅니다.
    /// </summary>
    private void TryToPlaceCard()
    {
        if (handManager == null) return;

        // HandManager에게 카드 배치 시도를 알립니다.
        // 실제 Cost 확인 및 배치 로직은 BattlefieldManager가 수행합니다.
        handManager.OnCardPlaced(this.gameObject);

        // 배치 시도 후에는 이 카드의 선택 상태를 해제하고 UI를 숨깁니다.
        // 성공 여부와 관계없이 CardSelector의 역할은 여기서 끝납니다.
        // 만약 배치가 실패하여 카드를 다시 손패로 돌려보내야 한다면,
        // 이는 HandManager 또는 BattlefieldManager에서 추가적인 로직으로 처리해야 합니다.
        isSelected = false;
        if (cardHover != null)
        {
            cardHover.OnPointerExit(null); // 호버 상태도 해제
        }
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

        // HandManager에 선택 취소 알림
        handManager.OnCardDeselected(this.gameObject);
        Debug.Log(this.gameObject.name + " 카드의 선택 및 호버가 취소되었습니다.");

        HideSelectionInfo(); // UI 텍스트 숨기기
    }

    /// <summary>
    /// 이 카드의 선택 상태를 외부에 의해 해제할 때 호출됩니다.
    /// (주로 HandManager의 DeselectAllCards에서 호출)
    /// </summary>
    public void DeselectExternally()
    {
        if (isSelected)
        {
            isSelected = false;
            // CardHover의 호버 상태도 해제
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
    /// 이 텍스트는 HandManager에 의해 설정된 전역 UI 텍스트를 사용합니다.
    /// </summary>
    private void ShowSelectionInfo()
    {
        if (_selectionInfoText != null)
        {
            _selectionInfoText.text = "좌클릭: 카드 배치 | 우클릭: 선택 취소"; // 사용자에게 보여줄 메시지
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