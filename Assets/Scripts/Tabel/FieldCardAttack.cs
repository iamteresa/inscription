using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FieldCardAttack : MonoBehaviour
{
    [Header("아군 필드 슬롯 관리 매니저")]
    [SerializeField] private BattlefieldManager battlefieldManager;
    [Header("적 필드 슬롯 관리 매니저")]
    [SerializeField] private EnemyCardManager enemyCardManager;
    [Header("적 플레이어 HP 매니저 (PlayerHpManger 사용)")]
    [SerializeField] private PlayerHpManger enemyHpManager;

    [Header("공격 애니메이션 설정")]
    [Tooltip("애니메이션 재생 후 데미지 적용 전 대기 시간")]
    [SerializeField] private float attackAnimDuration = 0.3f;
    [Tooltip("각 카드 공격 후 다음 카드로 넘어가기 전 대기 시간")]
    [SerializeField] private float delayBetweenAttacks = 0.1f;

    void Awake()
    {
        if (battlefieldManager == null)
            battlefieldManager = FindObjectOfType<BattlefieldManager>();
        if (enemyCardManager == null)
            enemyCardManager = FindObjectOfType<EnemyCardManager>();
        if (enemyHpManager == null)
            enemyHpManager = FindObjectOfType<PlayerHpManger>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
            StartCoroutine(AttackLeftToRight());
    }

    private IEnumerator AttackLeftToRight()
    {
        var playerSlots = battlefieldManager.SpawnPoints;
        var enemySlots = enemyCardManager.EnemySpawnPoints;

        for (int i = 0; i < playerSlots.Count; i++)
        {
            var pSlot = playerSlots[i];
            if (pSlot == null || pSlot.childCount == 0) continue;

            var pGO = pSlot.GetChild(0).gameObject;
            var pFC = pGO.GetComponent<FieldCard>();
            if (pFC == null) continue;

            int dmg = pFC.GetAttackPower();

            // 애니메이터 트리거로 공격 모션 재생
            var anim = pGO.GetComponent<Animator>();
            if (anim != null)
            {
                // 진영에 따라 다른 트리거
                if (pFC.faction == FieldCard.CardFaction.Player)
                    anim.SetTrigger("Attack");
                //else
                    //anim.SetTrigger("EnemyAttack");                           === 적 공격 애니메이션 === (현재 비활성화)
            }

            // 애니메이션 재생 시간 만큼 대기
            yield return new WaitForSeconds(attackAnimDuration);

            // ▶▶ 데미지 적용
            bool hitEnemyCard = false;
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
                        Debug.Log($"[{i}] {pGO.name} → {eGO.name} 에 {dmg} 데미지");
                        hitEnemyCard = true;
                    }
                }
            }
            if (!hitEnemyCard && enemyHpManager != null)
            {
                enemyHpManager.TakeDamage(dmg);
                Debug.Log($"[{i}] {pGO.name} 가 적 플레이어에게 {dmg} 데미지");
            }

            // 다음 카드 공격 전 짧게 대기
            yield return new WaitForSeconds(delayBetweenAttacks);
        }
    }
}
