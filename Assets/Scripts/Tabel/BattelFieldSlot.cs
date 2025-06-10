using UnityEngine;
using UnityEngine.EventSystems; // Ŭ�� �̺�Ʈ ó���� ���� �ʿ�

public class BattlefieldSlot : MonoBehaviour, IPointerClickHandler
{
    // �� ������ ���� �ε���. BattlefieldManager�� �����մϴ�.
    public int slotIndex;

    private BattlefieldManager battlefieldManager;

    void Awake()
    {
        battlefieldManager = FindObjectOfType<BattlefieldManager>();
        if (battlefieldManager == null)
        {
            Debug.LogError($"{gameObject.name}���� BattlefieldManager�� ã�� �� �����ϴ�. " +
                $"���� BattlefieldManager ��ũ��Ʈ�� ������ ������Ʈ�� �ִ��� Ȯ���ϼ���.", this);
            enabled = false; // �Ŵ��� ������ ��ũ��Ʈ ��Ȱ��ȭ
            return;
        }
    }

    void Start() // Start �޼��忡�� �ݶ��̴��� Ȯ��
    {
        // 2D ���ӿ� �ݶ��̴� �˻� (Collider2D)
        Collider2D collider2D = GetComponent<Collider2D>();
        if (collider2D == null)
        {
            Debug.LogWarning($"={gameObject.name}�� BoxCollider2D�� �����ϴ�. " +
                $"Is Trigger'�� üũ���ּ���.", this);
        }
        else if (!collider2D.isTrigger) // isTrigger�� ���� �� �Ǿ� �ִٸ� ���
        {
            Debug.LogWarning($"{gameObject.name}�� BoxCollider2D�� 'Is Trigger'�� " +
                $"üũ�Ǿ� ���� �ʽ��ϴ�.", this);
        }
    }


    // ���콺 Ŭ�� �̺�Ʈ ó��
    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left) // ��Ŭ�� ��
        {
            if (battlefieldManager != null)
            {
                battlefieldManager.OnSlotClicked(slotIndex);
            }
        }
    }
}
