using System.Collections;
using UnityEngine;
using UnityEngine.UI; // RectTransform�� ���� �߰�

public class HandAreaAnimator : MonoBehaviour
{
    [SerializeField] private float handDownAmount = 300f;
    [SerializeField] private float handMoveSpeed = 8f;

    private RectTransform handAreaRectTransform; // RectTransform ���� �߰�
    private Vector2 originalHandAreaAnchoredPosition; // anchoredPosition���� ����

    void Awake()
    {
        handAreaRectTransform = GetComponent<RectTransform>(); // RectTransform ��������
        if (handAreaRectTransform == null)
        {
            Debug.LogError("HandAreaAnimator: RectTransform�� ã�� �� �����ϴ�. �� ��ũ��Ʈ�� UI GameObject�� �پ�� �մϴ�.", this);
            enabled = false;
            return;
        }
        originalHandAreaAnchoredPosition = handAreaRectTransform.anchoredPosition; // anchoredPosition ����
    }

    public IEnumerator AnimateHandAreaDown()
    {
        Vector2 startPos = handAreaRectTransform.anchoredPosition;
        Vector2 targetPos = originalHandAreaAnchoredPosition + Vector2.down * handDownAmount; // Vector2.down ���

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