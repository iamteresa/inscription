using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CardHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private Vector3 originalArrangedPosition; // HandManager가 정해준 초기 정렬 위치
    private Vector3 currentBasePosition;      // 카드가 현재 있어야 할 기본 위치 (옆으로 비켜났을 수도 있음)
    private bool isHovered = false;
    private HandManager handManager;
    private CardSelector cardSelector; // CardSelector 참조 추가

    [Header("-------- 카드 애니메이션 ----------")]
    [SerializeField] float _hoverHeight = 50f;
    [SerializeField] float _moveSpeed = 10f;
    [SerializeField] float _selectedHeight = 100f; // 선택되었을 때 추가로 올라갈 높이

    [field: SerializeField] public float SideShiftAmount { get; private set; } = 80f;

    void Awake()
    {
        handManager = FindObjectOfType<HandManager>();
        if (handManager == null)
        {
            Debug.LogError("HandManager를 찾을 수 없습니다.");
        }
        cardSelector = GetComponent<CardSelector>(); // CardSelector 컴포넌트 가져오기
        if (cardSelector == null)
        {
            Debug.LogError("CardHover: CardSelector 컴포넌트를 찾을 수 없습니다. " +
                "CardHover와 CardSelector는 같은 게임 오브젝트에 있어야 합니다.", this);
        }
    }

    // HandManager가 정해준 카드의 초기 정렬 위치를 설정합니다.
    public void SetOriginalPosition(Vector3 pos)
    {
        originalArrangedPosition = pos;
        currentBasePosition = pos;      // 초기에는 정렬 위치가 기본 위치가 됩니다.
    }

    void Update()
    {
        Vector3 targetPosForHover;

        // CardSelector가 이 카드를 선택했다고 알려주면 선택된 높이를 유지
        if (cardSelector != null && cardSelector.IsSelected())
        {
            targetPosForHover = currentBasePosition + Vector3.up * _selectedHeight;
        }

        // 선택되지 않았고 호버 중인 경우
        else if (isHovered)
        {
            targetPosForHover = currentBasePosition + Vector3.up * _hoverHeight;
        }

        // 그 외의 경우 (선택되지도 호버 중도 아닌 경우)
        else
        {
            targetPosForHover = currentBasePosition;
        }
        

        transform.localPosition = Vector3.Lerp(transform.localPosition, targetPosForHover, Time.deltaTime * _moveSpeed);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        // 카드가 이미 선택된 상태라면 호버 효과를 적용하지 않습니다. (선택 상태가 우선)
        if (cardSelector != null && cardSelector.IsSelected()) return;

        if (isHovered) return;

        isHovered = true;
        if (handManager != null)
        {
            handManager.OnCardHovered(this, true);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        // 카드가 이미 선택된 상태라면 호버 해제 효과를 적용하지 않습니다.
        if (cardSelector != null && cardSelector.IsSelected()) return;

        if (!isHovered) return;

        isHovered = false;
        if (handManager != null)
        {
            handManager.OnCardHovered(this, false);
        }
    }

    /// <summary>
    /// 카드의 기본 위치를 shiftAmount만큼 이동시킨다.
    /// </summary>
    /// <param name="shiftAmount"></param>
    public void ShiftPosition(float shiftAmount)
    {
        currentBasePosition = originalArrangedPosition + new Vector3(shiftAmount, 0, 0);
    }
    
    /// <summary>
    /// 카드를 기본 위치로 되돌린다.
    /// </summary>
    public void ResetShift()
    {
        currentBasePosition = originalArrangedPosition;
    }
    
}