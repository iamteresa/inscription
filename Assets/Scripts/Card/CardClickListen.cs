using UnityEngine;
using UnityEngine.EventSystems; 
using System; 

public class CardClickListen : MonoBehaviour, IPointerClickHandler
{
    // 카드가 클릭되었을 때 외부(CardPlayManager)에 알리는 이벤트
    public static event Action<GameObject> OnCardClickedForPlay;

    // 이 카드가 현재 클릭 가능한 상태인지 여부 (CardPlayManager가 제어)
    private bool isClickable = true;

    // 외부에서 클릭 가능 여부를 설정
    public void SetClickable(bool enable)
    {
        isClickable = enable;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        // 클릭 불가능 상태이거나 좌클릭이 아니면 무시
        if (!isClickable || eventData.button != PointerEventData.InputButton.Left) return;

        // 클릭 이벤트를 발생시켜 CardPlayManager에게 어떤 카드가 클릭되었는지 알림
        OnCardClickedForPlay?.Invoke(this.gameObject);

        // 카드가 클릭되면, 이 카드는 일시적으로 클릭 불가능하게 만듭니다.
        // CardPlayManager가 플레이 프로세스 전반을 제어할 것입니다.
        SetClickable(false);
    }
}
