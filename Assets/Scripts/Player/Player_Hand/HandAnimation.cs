using System.Collections;
using UnityEngine;

public class HandAreaAnimator : MonoBehaviour
{
    [SerializeField] private float handDownAmount = 300f; // ���а� �Ʒ��� �������� ��
    [SerializeField] private float handMoveSpeed = 8f;   // ���� �ִϸ��̼� �ӵ�

    private Vector3 originalHandAreaLocalPosition; // ���� ������ ���� ��ġ

    void Awake()
    {
        originalHandAreaLocalPosition = transform.localPosition;
    }

    // ���� ������ �Ʒ��� ������ �ִϸ��̼� ����
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

    // ���� ������ ���� ��ġ�� �ø��� �ִϸ��̼� ����
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