using System.Collections;
using UnityEngine;
using UnityEngine.UI; // RectTransform을 위해 추가

public class HandAreaAnimator : MonoBehaviour
{
    [SerializeField] private float handDownAmount = 300f;
    [SerializeField] private float handMoveSpeed = 8f;

    private RectTransform handAreaRectTransform; // RectTransform 참조 추가
    private Vector2 originalHandAreaAnchoredPosition; // anchoredPosition으로 저장

    void Awake()
    {
        handAreaRectTransform = GetComponent<RectTransform>(); // RectTransform 가져오기
        if (handAreaRectTransform == null)
        {
            Debug.LogError("HandAreaAnimator: RectTransform을 찾을 수 없습니다. 이 스크립트는 UI GameObject에 붙어야 합니다.", this);
            enabled = false;
            return;
        }
        originalHandAreaAnchoredPosition = handAreaRectTransform.anchoredPosition; // anchoredPosition 저장
    }

    public IEnumerator AnimateHandAreaDown()
    {
        Vector2 startPos = handAreaRectTransform.anchoredPosition;
        Vector2 targetPos = originalHandAreaAnchoredPosition + Vector2.down * handDownAmount; // Vector2.down 사용

        float elapsedTime = 0f;
        float duration = 1f / handMoveSpeed;

        while (elapsedTime < duration)
        {
            handAreaRectTransform.anchoredPosition = Vector2.Lerp(startPos, targetPos, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        handAreaRectTransform.anchoredPosition = targetPos;
    }

    public IEnumerator AnimateHandAreaUp()
    {
        Vector2 startPos = handAreaRectTransform.anchoredPosition;
        Vector2 targetPos = originalHandAreaAnchoredPosition;

        float elapsedTime = 0f;
        float duration = 1f / handMoveSpeed;

        while (elapsedTime < duration)
        {
            handAreaRectTransform.anchoredPosition = Vector2.Lerp(startPos, targetPos, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        handAreaRectTransform.anchoredPosition = targetPos;
    }
}