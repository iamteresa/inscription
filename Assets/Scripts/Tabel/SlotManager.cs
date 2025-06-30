using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 여러 슬롯(Transform)들에 '카드'(FieldCard)를 배치·해제하고,
/// 빈 슬롯 검색, 인덱스 조회 등을 통일된 인터페이스로 제공합니다.
/// </summary>
public class SlotManager : MonoBehaviour
{
    [Header("슬롯으로 사용할 Transform들")]
    [Tooltip("카드가 꽂힐 슬롯 Transform 리스트")]
    [SerializeField] private List<Transform> slots = new List<Transform>();

    // 슬롯별로 현재 배치된 카드 GameObject 참조
    private GameObject[] occupied;

    void Awake()
    {
        occupied = new GameObject[slots.Count];
        // (옵션) 씬에 미리 붙어 있던 카드 제거
        for (int i = 0; i < slots.Count; i++)
        {
            // 슬롯에 FieldCard가 붙어 있으면 모두 언등록
            for (int c = slots[i].childCount - 1; c >= 0; c--)
            {
                var child = slots[i].GetChild(c).gameObject;
                if (child.GetComponent<CardDisplay>() != null)
                    Destroy(child);
            }
        }
    }

    /// <summary>
    /// 첫 번째 빈 슬롯 인덱스를 반환합니다. 없으면 -1.
    /// </summary>
    public int GetFirstFreeIndex()
    {
        for (int i = 0; i < slots.Count; i++)
        {
            // 실제 FieldCard가 붙은 자식이 없을 때 빈 슬롯으로 간주
            bool hasCard = false;
            for (int c = 0; c < slots[i].childCount; c++)
                if (slots[i].GetChild(c).GetComponent<FieldCard>() != null)
                {
                    hasCard = true;
                    break;
                }
            if (!hasCard) return i;
        }
        return -1;
    }

    /// <summary>
    /// 이 카드를 지정한 슬롯에 고정(부모)하고 내부 점유 배열에도 기록합니다.
    /// 슬롯이 유효하지 않거나 이미 차 있으면 false.
    /// </summary>
    public bool TryOccupySlot(int slotIndex, GameObject cardGO)
    {
        if (slotIndex < 0 || slotIndex >= slots.Count) return false;
        // 진짜 FieldCard가 없으면
        if (GetSlotIndex(cardGO) >= 0) return false;

        // 슬롯에 FieldCard 붙어 있는지 다시 확인
        foreach (Transform child in slots[slotIndex])
            if (child.GetComponent<FieldCard>() != null)
                return false;

        // 부모 설정
        cardGO.transform.SetParent(slots[slotIndex], false);
        occupied[slotIndex] = cardGO;
        return true;
    }

    /// <summary>
    /// 이 카드가 어느 슬롯에 들어가 있는지 반환합니다. 없으면 -1.
    /// </summary>
    public int GetSlotIndex(GameObject cardGO)
    {
        for (int i = 0; i < slots.Count; i++)
            if (occupied[i] == cardGO)
                return i;
        return -1;
    }

    /// <summary>
    /// 이 카드를 슬롯에서 해제(occupied null)하고, 오브젝트도 파괴합니다.
    /// </summary>
    public void ReleaseSlot(GameObject cardGO)
    {
        int idx = GetSlotIndex(cardGO);
        if (idx >= 0)
        {
            occupied[idx] = null;
            Destroy(cardGO);
        }
    }

    /// <summary>
    /// 빈 슬롯이 하나라도 남아있는지 여부.
    /// </summary>
    public bool HasFreeSlot() => GetFirstFreeIndex() >= 0;

    /// <summary>
    /// 슬롯의 Transform 리스트에 직접 접근해야 할 때.
    /// </summary>
    public List<Transform> Slots => slots;
}