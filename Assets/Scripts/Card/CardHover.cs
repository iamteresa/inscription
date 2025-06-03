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

    [Header("-------- 카드 애니메이션 ----------")]
    [SerializeField] float hoverHeight = 50f;
    [SerializeField] float moveSpeed = 10f;

    [field: SerializeField] public float SideShiftAmount { get; private set; } = 80f;

    void Awake()
    {
        handManager = FindObjectOfType<HandManager>();
        if (handManager == null)
        {
            Debug.LogError("HandManager를 찾을 수 없습니다. 씬에 HandManager가 있는지 확인하세요.");
        }
    }

    // HandManager가 정해준 카드의 초기 정렬 위치를 설정합니다.
    public void SetOriginalPosition(Vector3 pos)
    {
        originalArrangedPosition = pos;
        currentBasePosition = pos; // 초기에는 정렬 위치가 기본 위치가 됩니다.
        //transform.localPosition = pos; // 즉시 위치를 설정하는 경우 (선택 사항)
    }

    void Update()
    {
        // 호버 중인 경우: currentBasePosition (정렬 또는 비켜난 위치) + 위로 hoverHeight
        // 호버 중이 아닌 경우: currentBasePosition (정렬 또는 비켜난 위치)
        Vector3 targetPosForHover = isHovered
            ? currentBasePosition + Vector3.up * hoverHeight
            : currentBasePosition;

        // 현재 localPosition에서 targetPosForHover로 부드럽게 이동
        transform.localPosition = Vector3.Lerp(transform.localPosition, targetPosForHover, Time.deltaTime * moveSpeed);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (isHovered) return;

        isHovered = true;
        if (handManager != null)
        {
            handManager.OnCardHovered(this, true);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!isHovered) return;

        isHovered = false;
        if (handManager != null)
        {
            handManager.OnCardHovered(this, false);
        }
    }

    // 이 카드의 '기본 위치'를 옆으로 이동시킵니다.
    // 이 메서드가 호출될 때, Update()가 참조할 currentBasePosition을 변경합니다.
    public void ShiftPosition(float shiftAmount)
    {
        // 원래 정렬 위치(originalArrangedPosition)에서 shiftAmount만큼 이동한 곳이 새로운 기본 위치가 됩니다.
        currentBasePosition = originalArrangedPosition + new Vector3(shiftAmount, 0, 0);
       
    }

    // 이 카드의 '기본 위치'를 원래 정렬 위치로 되돌립니다.
    public void ResetShift()
    {
        // 원래 정렬 위치가 새로운 기본 위치가 됩니다.
        currentBasePosition = originalArrangedPosition;
        
    }
}