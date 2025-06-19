using UnityEngine;
using System.Collections;

/// <summary>
/// 카드(혹은 기타 오브젝트)가 공격 모션(앞으로 튀어나왔다가 돌아오기)을
/// 수행하도록 해 주는 공용 컴포넌트입니다.
/// </summary>
public class AttackMotion : MonoBehaviour
{
    [Header("→ 공격 모션 설정")]
    [Tooltip("앞으로 튀어나올 로컬 오프셋")]
    [SerializeField] private Vector3 attackOffset = new Vector3(0, 30, 0);
    [Tooltip("왕복하는 데 걸리는 전체 시간(초)")]
    [SerializeField] private float attackDuration = 0.3f;

    /// <summary>
    /// 공격 모션을 재생하는 코루틴을 반환합니다.
    /// StartCoroutine( PlayAttackMotion() ) 형태로 호출하세요.
    /// </summary>
    public IEnumerator PlayAttackMotion()
    {
        var tf = transform;
        Vector3 orig = tf.localPosition;
        Vector3 target = orig + attackOffset;

        float half = attackDuration * 0.5f;
        float t = 0f;

        // → 앞으로
        while (t < half)
        {
            tf.localPosition = Vector3.Lerp(orig, target, t / half);
            t += Time.deltaTime;
            yield return null;
        }
        tf.localPosition = target;

        // ← 뒤로
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
