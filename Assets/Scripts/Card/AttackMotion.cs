using UnityEngine;
using System.Collections;

/// <summary>
/// ī��(Ȥ�� ��Ÿ ������Ʈ)�� ���� ���(������ Ƣ��Դٰ� ���ƿ���)��
/// �����ϵ��� �� �ִ� ���� ������Ʈ�Դϴ�.
/// </summary>
public class AttackMotion : MonoBehaviour
{
    [Header("�� ���� ��� ����")]
    [Tooltip("������ Ƣ��� ���� ������")]
    [SerializeField] private Vector3 attackOffset = new Vector3(0, 30, 0);
    [Tooltip("�պ��ϴ� �� �ɸ��� ��ü �ð�(��)")]
    [SerializeField] private float attackDuration = 0.3f;

    /// <summary>
    /// ���� ����� ����ϴ� �ڷ�ƾ�� ��ȯ�մϴ�.
    /// StartCoroutine( PlayAttackMotion() ) ���·� ȣ���ϼ���.
    /// </summary>
    public IEnumerator PlayAttackMotion()
    {
        var tf = transform;
        Vector3 orig = tf.localPosition;
        Vector3 target = orig + attackOffset;

        float half = attackDuration * 0.5f;
        float t = 0f;

        // �� ������
        while (t < half)
        {
            tf.localPosition = Vector3.Lerp(orig, target, t / half);
            t += Time.deltaTime;
            yield return null;
        }
        tf.localPosition = target;

        // �� �ڷ�
        t = 0f;
        while (t < half)
        {
            tf.localPosition = Vector3.Lerp(target, orig, t / half);
            t += Time.deltaTime;
            yield return null;
        }
        tf.localPosition = orig;
    }
}
