using UnityEngine;
using UnityEngine.EventSystems; // 클릭 이벤트 처리를 위해 필요

public class BattlefieldSlot : MonoBehaviour, IPointerClickHandler
{
    // 이 슬롯의 고유 인덱스. BattlefieldManager가 설정합니다.
    public int slotIndex;

    private BattlefieldManager battlefieldManager;

    void Awake()
    {
        battlefieldManager = FindObjectOfType<BattlefieldManager>();
        if (battlefieldManager == null)
        {
            Debug.LogError($"{gameObject.name}에서 BattlefieldManager를 찾을 수 없습니다. " +
                $"씬에 BattlefieldManager 스크립트가 부착된 오브젝트가 있는지 확인하세요.", this);
            enabled = false; // 매니저 없으면 스크립트 비활성화
            return;
        }
    }

    void Start() // Start 메서드에서 콜라이더를 확인
    {
        // 2D 게임용 콜라이더 검사 (Collider2D)
        Collider2D collider2D = GetComponent<Collider2D>();
        if (collider2D == null)
        {
            Debug.LogWarning($"={gameObject.name}에 BoxCollider2D가 없습니다. " +
                $"Is Trigger'를 체크해주세요.", this);
        }
        else if (!collider2D.isTrigger) // isTrigger가 설정 안 되어 있다면 경고
        {
            Debug.LogWarning($"{gameObject.name}의 BoxCollider2D에 'Is Trigger'가 " +
                $"체크되어 있지 않습니다.", this);
        }
    }


    // 마우스 클릭 이벤트 처리
    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left) // 좌클릭 시
        {
            if (battlefieldManager != null)
            {
                battlefieldManager.OnSlotClicked(slotIndex);
            }
        }
    }
}
