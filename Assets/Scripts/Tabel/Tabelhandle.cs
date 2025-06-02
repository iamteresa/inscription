using UnityEngine;
using UnityEngine.EventSystems;
using System; // Action 사용

public class TabelHandle : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private int areaIndex; // 이 영역을 식별하는 고유 인덱스 (1, 2, 3, 4 등)

    // 외부에서 이벤트를 구독할 수 있도록 Action 정의
    public static event Action<int> OnTableAreaClicked;

    private bool isListening = false; // 클릭 감지 활성화/비활성화 플래그

    public void SetListening(bool enable)
    {
        isListening = enable;
        // 선택 사항: 클릭 가능할 때 시각적 피드백 제공 (예: 하이라이트)
        // GetComponent<UnityEngine.UI.Image>()?.CrossFadeAlpha(enable ? 0.5f : 1f, 0.2f, true); 
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        // 클릭 감지 비활성화 상태이거나 좌클릭이 아니면 무시
        if (!isListening || eventData.button != PointerEventData.InputButton.Left) return;

        // 이벤트를 발생시켜 CardPlayManager에게 어떤 영역이 클릭되었는지 알림
        OnTableAreaClicked?.Invoke(areaIndex);

        // 클릭 후 즉시 리스닝을 비활성화 (CardPlayManager가 다른 영역도 비활성화할 것임)
        SetListening(false);
    }
}