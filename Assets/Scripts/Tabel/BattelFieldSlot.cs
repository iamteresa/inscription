// BattlefieldSlot.cs

using UnityEngine;
using UnityEngine.EventSystems; // IPointerClickHandler�� ���� �ʿ��մϴ�.

public class BattlefieldSlot : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private int slotIndex; // �ν����Ϳ��� ���� �ʼ�!

    [Header("---------- ��ġ�� ī���� ���� ������ �� ������ ---------")]
    [SerializeField] private Vector2 placedCardLocalOffset = Vector2.zero; // �� ���Կ� ��ġ�� ī���� ���� ������
    [SerializeField] private Vector3 placedCardScale = Vector3.one;       // �� ���Կ� ��ġ�� ī���� ũ��

    private BattlefieldManager battlefieldManager;
    
    void Awake()
    {
        battlefieldManager = FindObjectOfType<BattlefieldManager>();
        if (battlefieldManager == null)
        {
            Debug.LogError("BattlefieldSlot: BattlefieldManager�� ã�� �� �����ϴ�. ���� BattlefieldManager ��ũ��Ʈ�� ���� GameObject�� �ִ��� Ȯ���ϼ���.", this);
        }

        // Ȥ�� ��������Ʈ�� RectTransform�� ������ ��� (UI ��ҿ��� ��)
        if (GetComponent<RectTransform>() == null)
        {
            Debug.LogWarning($"BattlefieldSlot: '{gameObject.name}'�� RectTransform�� �����ϴ�. UI ��Ҹ� Ŭ���Ϸ��� RectTransform�� �ʿ��մϴ�.", this);
        }
    }

    // ���콺 Ŭ�� �̺�Ʈ ó��
    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left) // ��Ŭ�� ��
        {
            if (battlefieldManager != null)
            {
                // BattlefieldManager�� OnSlotClicked �޼��带 ȣ���Ͽ� ī�带 ��ġ ��û
                battlefieldManager.OnSlotClicked(slotIndex); // OnSlotClicked�� TryPlaceCard�� ������ �� ���Դϴ�.
            }
            else
            {
                Debug.LogWarning("BattlefieldSlot: BattlefieldManager�� �Ҵ���� �ʾ� Ŭ�� �̺�Ʈ�� ó���� �� �����ϴ�.");
            }
        }
    }

    /// <summary>
    /// ��ġ�� ī�忡 �� ������ Ư�� ���� �����°� �������� �����մϴ�.
    /// </summary>
    /// <param name="cardRect">��ġ�� ī���� RectTransform</param>
    public void ApplyPlacementTransform(RectTransform cardRect)
    {
        cardRect.localPosition = placedCardLocalOffset; // anchoredPosition ��� localPosition ���
        cardRect.localScale = placedCardScale;
        cardRect.localRotation = Quaternion.identity; // ȸ���� �ʱ�ȭ (�ʿ��ϴٸ� �ɼ����� �߰�)

        Debug.Log($"Applied custom placement transform to {cardRect.name} on slot " +
            $"{slotIndex}. Offset: {placedCardLocalOffset}, Scale: {placedCardScale}");
    }


    // �ܺο��� �� ������ �ε����� �����ؾ� �� ��츦 ����� Setter (BattlefieldManager���� ���)
    public void SetSlotIndex(int index)
    {
        slotIndex = index;
    }

    // �ܺο��� �� ������ �ε����� �����;� �� ��츦 ����� Getter (���� ����)
    public int GetSlotIndex()
    {
        return slotIndex;
    }
}