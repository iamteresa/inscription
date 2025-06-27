using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FieldCardAttack : MonoBehaviour
{
    [Header("매니저 참조")]
    [SerializeField] private BattlefieldManager battlefieldManager;
    [SerializeField] private EnemyCardManager enemyCardManager;
    [SerializeField] private PlayerHpManger playerHpManager;
    [SerializeField] private PlayerHpManger enemyHpManager;

    [Header("공격 애니메이션 및 딜레이 설정")]
    [Tooltip("애니메이션 재생 후 데미지 적용 전 대기 시간")]
    [SerializeField] private float attackAnimDuration = 0.3f;
    [Tooltip("카드 간 공격 사이 대기 시간")]
    [SerializeField] private float delayBetweenAttacks = 0.1f;
    [Tooltip("아군 턴이 끝난 후 적 턴 시작 전 대기 시간")]
    [SerializeField] private float delayAfterPlayerTurn = 0.5f;

    void Awake()
    {
        battlefieldManager = battlefieldManager ?? FindObjectOfType<BattlefieldManager>();
        enemyCardManager = enemyCardManager ?? FindObjectOfType<EnemyCardManager>();
        playerHpManager = playerHpManager ?? FindObjectOfType<PlayerHpManger>();
        enemyHpManager = enemyHpManager ?? FindObjectOfType<PlayerHpManger>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
            StartCoroutine(AttackSequence());
    }

    private IEnumerator AttackSequence()
    {
        var playerSlots = battlefieldManager.SpawnPoints;
        var enemySlots = enemyCardManager.EnemySpawnPoints;

        // ─── 1) 아군 공격 턴 ─────────────────────────────────────
        for (int i = 0; i < playerSlots.Count; i++)
        {
            var pSlot = playerSlots[i];
            if (pSlot == null || pSlot.childCount == 0)
                continue;

            var pGO = pSlot.GetChild(0).gameObject;
            var pFC = pGO.GetComponent<FieldCard>();
            if (pFC == null)
                continue;

            // (A) 공격 애니메이션 재생
            var pAnim = pGO.GetComponent<Animator>();
            if (pAnim != null)
            {
                pAnim.ResetTrigger("Attack");
                pAnim.SetTrigger("Attack");
            }
            yield return new WaitForSeconds(attackAnimDuration);

            // (B) 공격력 읽기
            int dmg = pFC.GetAttackPower();

            // (C) Flyer 능력: 적 슬롯 검사 없이 적 플레이어만 관통 피해
            if (pFC.AbilityType == CardData.CardAbilityType.Flyer)
            {
                CardEventBus.Attack(pFC, null);
                enemyHpManager.TakeDamage(dmg);
                Debug.Log($"[플라이어 관통] {pGO.name} → 적 플레이어에 {dmg} 데미지");
                yield return new WaitForSeconds(delayBetweenAttacks);
                continue;
            }

            // (D) 일반 타겟(슬롯 i)에서 FieldCard 찾기
            FieldCard targetFC = null;
            GameObject targetGO = null;
            if (i < enemySlots.Count)
            {
                var eSlot = enemySlots[i];
                if (eSlot != null && eSlot.childCount > 0)
                {
                    targetGO = eSlot.GetChild(0).gameObject;
                    targetFC = targetGO.GetComponent<FieldCard>();
                }
            }

            // (E) Diver 능력: 방어자가 Diver면 피해를 무시하고 플레이어 관통
            if (targetFC != null && targetFC.AbilityType == CardData.CardAbilityType.Diver)
            {
                CardEventBus.Attack(pFC, null);
                enemyHpManager.TakeDamage(dmg);
                Debug.Log($"[다이버 관통] {pGO.name} → 적 플레이어에 {dmg} 데미지");
                yield return new WaitForSeconds(delayBetweenAttacks);
                continue;
            }

            // (F) OnAttack 이벤트 발행
            CardEventBus.Attack(pFC, targetFC);

            // (G) 실제 데미지 적용
            if (targetFC != null)
            {
                targetFC.TakeDamage(dmg);
                Debug.Log($"[플레이어 턴] {pGO.name} → {targetGO.name} 에 {dmg} 데미지");
            }
            else
            {
                enemyHpManager.TakeDamage(dmg);
                Debug.Log($"[플레이어 턴] {pGO.name} → 적 플레이어에 {dmg} 데미지");
            }

            yield return new WaitForSeconds(delayBetweenAttacks);
        }

        // ─── 아군 턴 종료 후 대기 ──────────────────────────────────
        yield return new WaitForSeconds(delayAfterPlayerTurn);

        // ─── 2) 적 공격 턴 ───────────────────────────────────────
        for (int i = 0; i < enemySlots.Count; i++)
        {
            var eSlot = enemySlots[i];
            if (eSlot == null || eSlot.childCount == 0)
                continue;

            var eGO = eSlot.GetChild(0).gameObject;
            var eFC = eGO.GetComponent<FieldCard>();
            if (eFC == null)
                continue;

            // (H) 적 공격 애니메이션
            var eAnim = eGO.GetComponent<Animator>();
            if (eAnim != null)
            {
                eAnim.ResetTrigger("EnemyAttack");
                eAnim.SetTrigger("EnemyAttack");
            }
            yield return new WaitForSeconds(attackAnimDuration);

            // (I) 공격력 읽기
            int edmg = eFC.GetAttackPower();

            // (J) Flyer 능력: 아군 카드 검사 없이 플레이어 관통 피해
            if (eFC.AbilityType == CardData.CardAbilityType.Flyer)
            {
                CardEventBus.Attack(eFC, null);
                playerHpManager.TakeDamage(edmg);
                Debug.Log($"[적 플라이어 관통] {eGO.name} → 플레이어에 {edmg} 데미지");
                yield return new WaitForSeconds(delayBetweenAttacks);
                continue;
            }

            // (K) 일반 타겟(슬롯 i)에서 FieldCard 찾기
            FieldCard defendFC = null;
            GameObject defendGO = null;
            if (i < playerSlots.Count)
            {
                var pSlot = playerSlots[i];
                if (pSlot != null && pSlot.childCount > 0)
                {
                    defendGO = pSlot.GetChild(0).gameObject;
                    defendFC = defendGO.GetComponent<FieldCard>();
                }
            }

            // (L) Diver 능력: 방어자가 Diver면 플레이어 관통 피해
            if (defendFC != null && defendFC.AbilityType == CardData.CardAbilityType.Diver)
            {
                CardEventBus.Attack(eFC, null);
                playerHpManager.TakeDamage(edmg);
                Debug.Log($"[적 다이버 관통] {eGO.name} → 플레이어에 {edmg} 데미지");
                yield return new WaitForSeconds(delayBetweenAttacks);
                continue;
            }

            // (M) OnAttack 이벤트 발행
            CardEventBus.Attack(eFC, defendFC);

            // (N) 실제 데미지 적용
            if (defendFC != null)
            {
                defendFC.TakeDamage(edmg);
                Debug.Log($"[적 턴] {eGO.name} → {defendGO.name} 에 {edmg} 데미지");
            }
            else
            {
                playerHpManager.TakeDamage(edmg);
                Debug.Log($"[적 턴] {eGO.name} → 플레이어에 {edmg} 데미지");
            }

            yield return new WaitForSeconds(delayBetweenAttacks);
        }
    }
}
