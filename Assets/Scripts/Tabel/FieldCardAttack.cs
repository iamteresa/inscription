using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FieldCardAttack : MonoBehaviour
{
    [Header("�Ʊ� �ʵ� ���� ���� �Ŵ���")]
    [SerializeField] private BattlefieldManager battlefieldManager;
    [Header("�� �ʵ� ���� ���� �Ŵ���")]
    [SerializeField] private EnemyCardManager enemyCardManager;
    [Header("�� �÷��̾� HP �Ŵ��� (PlayerHpManger ���)")]
    [SerializeField] private PlayerHpManger enemyHpManager;

    [Header("���� �ִϸ��̼� ����")]
    [Tooltip("�ִϸ��̼� ��� �� ������ ���� �� ��� �ð�")]
    [SerializeField] private float attackAnimDuration = 0.3f;
    [Tooltip("�� ī�� ���� �� ���� ī��� �Ѿ�� �� ��� �ð�")]
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

            // �ִϸ����� Ʈ���ŷ� ���� ��� ���
            var anim = pGO.GetComponent<Animator>();
            if (anim != null)
            {
                // ������ ���� �ٸ� Ʈ����
                if (pFC.faction == FieldCard.CardFaction.Player)
                    anim.SetTrigger("Attack");
                //else
                    //anim.SetTrigger("EnemyAttack");                           === �� ���� �ִϸ��̼� === (���� ��Ȱ��ȭ)
            }

            // �ִϸ��̼� ��� �ð� ��ŭ ���
            yield return new WaitForSeconds(attackAnimDuration);

            // ���� ������ ����
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
                        Debug.Log($"[{i}] {pGO.name} �� {eGO.name} �� {dmg} ������");
                        hitEnemyCard = true;
                    }
                }
            }
            if (!hitEnemyCard && enemyHpManager != null)
            {
                enemyHpManager.TakeDamage(dmg);
                Debug.Log($"[{i}] {pGO.name} �� �� �÷��̾�� {dmg} ������");
            }

            // ���� ī�� ���� �� ª�� ���
            yield return new WaitForSeconds(delayBetweenAttacks);
        }
    }
}
