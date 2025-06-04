using System.Collections;
using UnityEngine;

public class HandAreaAnimator : MonoBehaviour
{
    [SerializeField] private float handDownAmount = 300f; // 손패가 들어올 때 이동할 거리.

    [SerializeField] private float handMoveSpeed = 8f;    // 손패가 이동하는 속도.
    
    private Vector3 originalHandAreaLocalPosition;
    // 손패 영역의 초기 위치를 저장. 


    void Awake()
    {
        // 이 스크립트가 부착된 GameObject(즉, Hand Area)의 현재 로컬 위치를 저장합니다.
        // 이 위치가 손패가 카드 플레이 후에 돌아올 '원래 위치'가 됩니다.
        originalHandAreaLocalPosition = transform.localPosition;
    }


    /// <summary>
    /// 손패 영역을 화면 아래로 부드럽게 내리는 애니메이션을 시작합니다.
    /// </summary>
    public IEnumerator AnimateHandAreaDown()
    {
        Vector3 startPos = transform.localPosition;         // 애니메이션 시작 위치 (현재 손패의 위치)

        // 애니메이션 목표 위치: 원래 위치에서 'handDownAmount'만큼 아래로 이동
        Vector3 targetPos = originalHandAreaLocalPosition + Vector3.down * handDownAmount;

        float elapsedTime = 0f;                     // 경과 시간 초기화
        float duration = 1f / handMoveSpeed;        // 애니메이션 총 지속 시간 계산 (속도에 반비례)


        while (elapsedTime < duration)
        {
            // Vector3.Lerp를 사용하여 시작 위치에서 목표 위치로 선형 보간(점진적으로 부드럽게 이동)합니다.
            transform.localPosition = Vector3.Lerp(startPos, targetPos, elapsedTime / duration);

            elapsedTime += Time.deltaTime;      // 프레임당 경과 시간 업데이트
            yield return null;                  // 다음 프레임까지 대기 (코루틴 일시 정지)
        }
        // 애니메이션이 끝나면 목표 위치에 정확히 도달하도록 최종 위치를 설정하여 오차를 보정합니다.
        transform.localPosition = targetPos;
    }

    /// <summary>
    /// 손패 영역을 원래 위치(화면 위)로 부드럽게 올리는 애니메이션을 시작합니다.
    /// </summary>
    public IEnumerator AnimateHandAreaUp()
    {
        Vector3 startPos = transform.localPosition;         // 애니메이션 시작 위치 (현재 손패의 위치)
        Vector3 targetPos = originalHandAreaLocalPosition;  // 애니메이션 목표 위치 (Awake에서 저장된 원래 위치)

        float elapsedTime = 0f;                 // 경과 시간 초기화
        float duration = 1f / handMoveSpeed;    // 애니메이션 총 지속 시간 계산

        // 애니메이션 루프
        while (elapsedTime < duration)
        {
            // 시작 위치에서 원래 위치로 선형 보간하여 이동
            transform.localPosition = Vector3.Lerp(startPos, targetPos, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        // 애니메이션이 끝나면 목표 위치에 정확히 도달하도록 최종 위치를 설정
        transform.localPosition = targetPos;
    }
}