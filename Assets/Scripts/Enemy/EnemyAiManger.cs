// Assets/Scripts/Enemy/EnemyAiManager.cs
using UnityEngine;
using System.Collections;

public class EnemyAiManager : MonoBehaviour
{
    [Header("AI 설정")]
    [Tooltip("한 턴에 몇 장 소환할지")]
    [SerializeField] private int spawnPerTurn = 1;
    [Tooltip("소환 후 다음 소환까지 대기할 시간")]
    [SerializeField] private float spawnDelay = 0.3f;

    [Header("참조")]
    [SerializeField] private EnemyCardManager enemyCardManager;
    [SerializeField] private FieldCardAttack cardAttack;   // 적 공격 코루틴
    [SerializeField] private HandManager handManager;      // 플레이어 카드 드로우용

    /// <summary>
    /// 외부에서 호출 : 한 턴 전체 흐름
    /// 1) 플레이어 공격 → 2) AI 카드 소환 → 3) AI 공격 → 4) 플레이어 카드 1장 드로우
    /// </summary>
    public IEnumerator PlayTurn()
    {
        // 1) 플레이어 공격
        yield return StartCoroutine(cardAttack.AttackSequence());

        // 2) AI 카드 소환
        for (int i = 0; i < spawnPerTurn; i++)
        {
            if (!enemyCardManager.DrawAndSpawnEnemyCard())
                break; // 덱이나 슬롯 없으면 중단

            yield return new WaitForSeconds(spawnDelay);
        }

        // 3) AI 공격 (AttackSequence 안에 이미 적 공격 로직 포함되어 있다면 다시 부르면 안 됩니다.
        //    예: AttackSequence() 내부가 양쪽 모두 처리한다면 여기서는 따로 호출하지 않아야 하고,
        //    만약 플레이어/적을 분리해 PlayerAttackRoutine(), EnemyAttackRoutine() 쓰셨다면
        //    yield return StartCoroutine(cardAttack.EnemyAttackRoutine());
        //    로 대체해주세요.

        // (여기선 AttackSequence만 쓴다고 가정)
        // yield return StartCoroutine(cardAttack.EnemyAttackRoutine());

        // 4) 플레이어 카드 1장 드로우
        handManager.DrawCard();
    }   
}
