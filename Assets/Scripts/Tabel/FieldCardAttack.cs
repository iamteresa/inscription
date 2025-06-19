using UnityEngine;
using System.Collections.Generic;

public class FieldCardAttack : MonoBehaviour
{
    [Header("-----------�Ʊ� �ʵ� ���� ���� �Ŵ���-----------")]
    [SerializeField] private BattlefieldManager battlefieldManager;
    [Header("-----------�� �ʵ� ���� ���� �Ŵ���---------")]
    [SerializeField] private EnemyCardManager enemyCardManager;
    [Header("-----------�� �÷��̾� HP �Ŵ���------------")]
    [SerializeField] private PlayerHpManger enemyHpManager;

    void Awake()
    {
        if (battlefieldManager == null)
            battlefieldManager = FindObjectOfType<BattlefieldManager>();
        if (enemyCardManager == null)
            enemyCardManager = FindObjectOfType<EnemyCardManager>();
        
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
            AttackLeftToRight();
    }

    public void AttackLeftToRight()
    {
        // 1) �Ʊ� ����
        List<Transform> playerSlots = battlefieldManager.SpawnPoints;
        // 2) �� ����
        List<Transform> enemySlots = enemyCardManager.EnemySpawnPoints;

        for (int i = 0; i < playerSlots.Count; i++)
        {
            // �Ʊ� ����, �� ���� ���̴� ���ٰ� ����
            Transform pSlot = playerSlots[i];
            Transform eSlot = (i < enemySlots.Count ? enemySlots[i] : null);

            if (pSlot == null || pSlot.childCount == 0)
                continue;

            // �Ʊ� ī��
            var pCardGO = pSlot.GetChild(0).gameObject;
            var pFC = pCardGO.GetComponent<FieldCard>();
            if (pFC == null)
                continue;

            int damage = pFC.GetAttackPower();

            // �� ī�尡 ������ ����
            if (eSlot != null && eSlot.childCount > 0)
            {
                var eCardGO = eSlot.GetChild(0).gameObject;
                var eFC = eCardGO.GetComponent<FieldCard>();
                if (eFC != null)
                {
                    eFC.TakeDamage(damage);
                    Debug.Log($"[{i}] {pCardGO.name}��{eCardGO.name} �� {damage} ������");
                    continue;
                }
            }

            // �ƴϸ� �� �÷��̾�� ���� ������
            if (enemyHpManager != null)
            {
                enemyHpManager.TakeDamage(damage);
                Debug.Log($"[{i}] {pCardGO.name} �� �� �÷��̾�� {damage} ������");
            }
        }
    }
}
