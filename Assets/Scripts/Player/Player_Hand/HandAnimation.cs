using System.Collections;
using UnityEngine;

public class HandAreaAnimator : MonoBehaviour
{
    [SerializeField] private float handDownAmount = 300f; // ���а� ���� �� �̵��� �Ÿ�.

    [SerializeField] private float handMoveSpeed = 8f;    // ���а� �̵��ϴ� �ӵ�.
    
    private Vector3 originalHandAreaLocalPosition;
    // ���� ������ �ʱ� ��ġ�� ����. 


    void Awake()
    {
        // �� ��ũ��Ʈ�� ������ GameObject(��, Hand Area)�� ���� ���� ��ġ�� �����մϴ�.
        // �� ��ġ�� ���а� ī�� �÷��� �Ŀ� ���ƿ� '���� ��ġ'�� �˴ϴ�.
        originalHandAreaLocalPosition = transform.localPosition;
    }


    /// <summary>
    /// ���� ������ ȭ�� �Ʒ��� �ε巴�� ������ �ִϸ��̼��� �����մϴ�.
    /// </summary>
    public IEnumerator AnimateHandAreaDown()
    {
        Vector3 startPos = transform.localPosition;         // �ִϸ��̼� ���� ��ġ (���� ������ ��ġ)

        // �ִϸ��̼� ��ǥ ��ġ: ���� ��ġ���� 'handDownAmount'��ŭ �Ʒ��� �̵�
        Vector3 targetPos = originalHandAreaLocalPosition + Vector3.down * handDownAmount;

        float elapsedTime = 0f;                     // ��� �ð� �ʱ�ȭ
        float duration = 1f / handMoveSpeed;        // �ִϸ��̼� �� ���� �ð� ��� (�ӵ��� �ݺ��)


        while (elapsedTime < duration)
        {
            // Vector3.Lerp�� ����Ͽ� ���� ��ġ���� ��ǥ ��ġ�� ���� ����(���������� �ε巴�� �̵�)�մϴ�.
            transform.localPosition = Vector3.Lerp(startPos, targetPos, elapsedTime / duration);

            elapsedTime += Time.deltaTime;      // �����Ӵ� ��� �ð� ������Ʈ
            yield return null;                  // ���� �����ӱ��� ��� (�ڷ�ƾ �Ͻ� ����)
        }
        // �ִϸ��̼��� ������ ��ǥ ��ġ�� ��Ȯ�� �����ϵ��� ���� ��ġ�� �����Ͽ� ������ �����մϴ�.
        transform.localPosition = targetPos;
    }

    /// <summary>
    /// ���� ������ ���� ��ġ(ȭ�� ��)�� �ε巴�� �ø��� �ִϸ��̼��� �����մϴ�.
    /// </summary>
    public IEnumerator AnimateHandAreaUp()
    {
        Vector3 startPos = transform.localPosition;         // �ִϸ��̼� ���� ��ġ (���� ������ ��ġ)
        Vector3 targetPos = originalHandAreaLocalPosition;  // �ִϸ��̼� ��ǥ ��ġ (Awake���� ����� ���� ��ġ)

        float elapsedTime = 0f;                 // ��� �ð� �ʱ�ȭ
        float duration = 1f / handMoveSpeed;    // �ִϸ��̼� �� ���� �ð� ���

        // �ִϸ��̼� ����
        while (elapsedTime < duration)
        {
            // ���� ��ġ���� ���� ��ġ�� ���� �����Ͽ� �̵�
            transform.localPosition = Vector3.Lerp(startPos, targetPos, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        // �ִϸ��̼��� ������ ��ǥ ��ġ�� ��Ȯ�� �����ϵ��� ���� ��ġ�� ����
        transform.localPosition = targetPos;
    }
}