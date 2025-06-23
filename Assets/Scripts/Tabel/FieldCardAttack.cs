using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// A 키를 누르면
/// 1) 아군 카드가 순차적으로 앞→뒤 공격 애니메이션(Attack)을 재생하고 데미지를 주고,
/// 2) 잠시 대기 후
/// 3) 적 카드가 순차적으로 앞→뒤 공격 애니메이션(EnemyAttack)을 재생하고 대응 아군 카드 또는 플레이어에게 데미지를 주는
/// 턴 단위 시퀀스를 처리합니다.
/// </summary>
public class FieldCardAttack : MonoBehaviour
{
    [Header("매니저 참조")]
    [SerializeField] private BattlefieldManager battlefieldManager;
    [SerializeField] private EnemyCardManager   enemyCardManager;
    [SerializeField] private PlayerHpManger     playerHpManager;
    [SerializeField] private PlayerHpManger     enemyHpManager;

    [Header("공격 애니메이션 및 딜레이 설정")]
    [Tooltip("애니메이션 재생 후 데미지 적용 전 대기 시간")]
    [SerializeField] private float attackAnimDuration  = 0.3f;
    [Tooltip("카드 간 공격 사이 대기 시간")]
    [SerializeField] private float delayBetweenAttacks = 0.1f;
    [Tooltip("아군 턴이 끝난 후 적 턴 시작 전 대기 시간")]
    [SerializeField] private float delayAfterPlayerTurn = 0.5f;

    void Awake()
    {
        if (battlefieldManager == null)
            battlefieldManager   = FindObjectOfType<BattlefieldManager>();
        if (enemyCardManager == null)
            enemyCardManager     = FindObjectOfType<EnemyCardManager>();
        if (playerHpManager == null)
            playerHpManager      = FindObjectOfType<PlayerHpManger>();
        if (enemyHpManager == null)
            enemyHpManager       = FindObjectOfType<PlayerHpManger>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
            StartCoroutine(AttackSequence());
    }

    private IEnumerator AttackSequence()
    {
        var playerSlots = battlefieldManager.SpawnPoints;
        var enemySlots  = enemyCardManager.EnemySpawnPoints;

        // ─── 1) 아군 공격 턴 ─────────────────────────────────────
        for (int i = 0; i < playerSlots.Count; i++)
        {
            var pSlot = playerSlots[i];
            if (pSlot == null || pSlot.childCount == 0)
                continue;

            var pGO = pSlot.GetChild(0).gameObject;
            var pFC = pGO.GetComponent<FieldCard>();
            if (pFC == null) continue;

            // 애니메이터 트리거
            var pAnim = pGO.GetComponent<Animator>();
            if (pAnim != null)
            {
                pAnim.ResetTrigger("Attack");
                pAnim.SetTrigger("Attack");
            }

            // 애니메이션 재생 대기
            yield return new WaitForSeconds(attackAnimDuration);

            // 데미지 적용
            int dmg = pFC.GetAttackPower();
            bool hitEnemy = false;
            if (i < enemySlots.Count)
            {
                var eSlot = enemySlots[i];
                if (eSlot != null && eSlot.childCount > 0)
                {
                    var eGO = eSlot.GetChild(0).gameObject;
                    var eFC = eGO.GetComponent<FieldCard>();
                    if (eFC != null)
                    {
                        eFC.TakeDamage(dmg);
                        Debug.Log($"[플레이어 턴] {pGO.name} → {eGO.name} 에 {dmg} 데미지");
                        hitEnemy = true;
                    }
                }
            }
            if (!hitEnemy && enemyHpManager != null)
            {
                enemyHpManager.TakeDamage(dmg);
                Debug.Log($"[플레이어 턴] {pGO.name} 가 적 플레이어에게 {dmg} 데미지");
            }

            yield return new WaitForSeconds(delayBetweenAttacks);
        }

        // ─── 아군 턴 끝, 잠시 대기 ────────────────────────────────
        yield return new WaitForSeconds(delayAfterPlayerTurn);

        // ─── 2) 적 공격 턴 ───────────────────────────────────────
        for (int i = 0; i < enemySlots.Count; i++)
        {
            var eSlot = enemySlots[i];
            if (eSlot == null || eSlot.childCount == 0)
                continue;

            var eGO = eSlot.GetChild(0).gameObject;
            var eFC = eGO.GetComponent<FieldCard>();
            if (eFC == null) continue;

            // 애니메이터 트리거
            var eAnim = eGO.GetComponent<Animator>();
            if (eAnim != null)
            {
                eAnim.ResetTrigger("EnemyAttack");
                eAnim.SetTrigger("EnemyAttack");
            }

            // 애니메이션 재생 대기
            yield return new WaitForSeconds(attackAnimDuration);

            // 데미지 적용
            int dmg = eFC.GetAttackPower();
            bool hitPlayerCard = false;
            if (i < playerSlots.Count)
            {
                var pSlot = playerSlots[i];
                if (pSlot != null && pSlot.childCount > 0)
                {
                    var pGO = pSlot.GetChild(0).gameObject;
                    var pFC = pGO.GetComponent<FieldCard>();
                    if (pFC != null)
                    {
                        pFC.TakeDamage(dmg);
                        Debug.Log($"[적 턴] {eGO.name} → {pGO.name} 에 {dmg} 데미지");
                        hitPlayerCard = true;
                    }
                }
            }
            if (!hitPlayerCard && playerHpManager != null)
            {
                playerHpManager.TakeDamage(dmg);
                Debug.Log($"[적 턴] {eGO.name} 가 플레이어에게 {dmg} 데미지");
            }

            yield return new WaitForSeconds(delayBetweenAttacks);
        }
    }
}
