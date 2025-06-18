// BattlefieldSlot.cs

using UnityEngine;
using UnityEngine.EventSystems; // IPointerClickHandler를 위해 필요합니다.

public class BattlefieldSlot : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private int slotIndex; // 인스펙터에서 설정 필수!

    [Header("---------- 배치될 카드의 로컬 오프셋 및 스케일 ---------")]
    [SerializeField] private Vector2 placedCardLocalOffset = Vector2.zero; // 이 슬롯에 배치될 카드의 로컬 오프셋
    [SerializeField] private Vector3 placedCardScale = Vector3.one;       // 이 슬롯에 배치될 카드의 크기

    private BattlefieldManager battlefieldManager;
    
    void Awake()
    {
        battlefieldManager = FindObjectOfType<BattlefieldManager>();
        if (battlefieldManager == null)
        {
            Debug.LogError("BattlefieldSlot: BattlefieldManager를 찾을 수 없습니다. 씬에 BattlefieldManager 스크립트가 붙은 GameObject가 있는지 확인하세요.", this);
        }

        // 혹시 스폰포인트에 RectTransform이 없으면 경고 (UI 요소여야 함)
        if (GetComponent<RectTransform>() == null)
        {
            Debug.LogWarning($"BattlefieldSlot: '{gameObject.name}'에 RectTransform이 없습니다. UI 요소를 클릭하려면 RectTransform이 필요합니다.", this);
        }
    }

    // 마우스 클릭 이벤트 처리
    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left) // 좌클릭 시
        {
            if (battlefieldManager != null)
            {
                // BattlefieldManager의 OnSlotClicked 메서드를 호출하여 카드를 배치 요청
                battlefieldManager.OnSlotClicked(slotIndex); // OnSlotClicked가 TryPlaceCard의 역할을 할 것입니다.
            }
            else
            {
                Debug.LogWarning("BattlefieldSlot: BattlefieldManager가 할당되지 않아 클릭 이벤트를 처리할 수 없습니다.");
            }
        }
    }

    /// <summary>
    /// 배치될 카드에 이 슬롯의 특정 로컬 오프셋과 스케일을 적용합니다.
    /// </summary>
    /// <param name="cardRect">배치될 카드의 RectTransform</param>
    public void ApplyPlacementTransform(RectTransform cardRect)
    {
        cardRect.localPosition = placedCardLocalOffset; // anchoredPosition 대신 localPosition 사용
        cardRect.localScale = placedCardScale;
        cardRect.localRotation = Quaternion.identity; // 회전은 초기화 (필요하다면 옵션으로 추가)

        Debug.Log($"Applied custom placement transform to {cardRect.name} on slot " +
            $"{slotIndex}. Offset: {placedCardLocalOffset}, Scale: {placedCardScale}");
    }


    // 외부에서 이 슬롯의 인덱스를 설정해야 할 경우를 대비한 Setter (BattlefieldManager에서 사용)
    public void SetSlotIndex(int index)
    {
        slotIndex = index;
    }

    // 외부에서 이 슬롯의 인덱스를 가져와야 할 경우를 대비한 Getter (선택 사항)
    public int GetSlotIndex()
    {
        return slotIndex;
    }
}