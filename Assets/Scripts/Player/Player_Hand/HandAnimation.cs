using System.Collections;
using UnityEngine;

public class HandAreaAnimator : MonoBehaviour
{
    [SerializeField] private float handDownAmount = 300f; // 손패가 아래로 내려가는 양
    [SerializeField] private float handMoveSpeed = 8f;   // 손패 애니메이션 속도

    private Vector3 originalHandAreaLocalPosition; // 손패 영역의 원래 위치

    void Awake()
    {
        originalHandAreaLocalPosition = transform.localPosition;
    }

    // 손패 영역을 아래로 내리는 애니메이션 시작
    public IEnumerator AnimateHandAreaDown()
    {
        Vector3 startPos = transform.localPosition;
        Vector3 targetPos = originalHandAreaLocalPosition + Vector3.down * handDownAmount;

        float elapsedTime = 0f;
        float duration = 1f / handMoveSpeed;

        while (elapsedTime < duration)
        {
            transform.localPosition = Vector3.Lerp(startPos, targetPos, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        transform.localPosition = targetPos;
    }

    // 손패 영역을 원래 위치로 올리는 애니메이션 시작
    public IEnumerator AnimateHandAreaUp()
    {
        Vector3 startPos = transform.localPosition;
        Vector3 targetPos = originalHandAreaLocalPosition;

        float elapsedTime = 0f;
        float duration = 1f / handMoveSpeed;

        while (elapsedTime < duration)
        {
            transform.localPosition = Vector3.Lerp(startPos, targetPos, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        transform.localPosition = targetPos;
    }
}