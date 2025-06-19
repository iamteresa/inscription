using UnityEngine;
using System.Collections.Generic;

public class FieldCardAttack : MonoBehaviour
{
    [Header("-----------아군 필드 슬롯 관리 매니저-----------")]
    [SerializeField] private BattlefieldManager battlefieldManager;
    [Header("-----------적 필드 슬롯 관리 매니저---------")]
    [SerializeField] private EnemyCardManager enemyCardManager;
    [Header("-----------적 플레이어 HP 매니저------------")]
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
        // 1) 아군 슬롯
        List<Transform> playerSlots = battlefieldManager.SpawnPoints;
        // 2) 적 슬롯
        List<Transform> enemySlots = enemyCardManager.EnemySpawnPoints;

        for (int i = 0; i < playerSlots.Count; i++)
        {
            // 아군 슬롯, 적 슬롯 길이는 같다고 가정
            Transform pSlot = playerSlots[i];
            Transform eSlot = (i < enemySlots.Count ? enemySlots[i] : null);

            if (pSlot == null || pSlot.childCount == 0)
                continue;

            // 아군 카드
            var pCardGO = pSlot.GetChild(0).gameObject;
            var pFC = pCardGO.GetComponent<FieldCard>();
            if (pFC == null)
                continue;

            int damage = pFC.GetAttackPower();

            // 적 카드가 있으면 피해
            if (eSlot != null && eSlot.childCount > 0)
            {
                var eCardGO = eSlot.GetChild(0).gameObject;
                var eFC = eCardGO.GetComponent<FieldCard>();
                if (eFC != null)
                {
                    eFC.TakeDamage(damage);
                    Debug.Log($"[{i}] {pCardGO.name}→{eCardGO.name} 에 {damage} 데미지");
                    continue;
                }
            }

            // 아니면 적 플레이어에게 직접 데미지
            if (enemyHpManager != null)
            {
                enemyHpManager.TakeDamage(damage);
                Debug.Log($"[{i}] {pCardGO.name} 가 적 플레이어에게 {damage} 데미지");
            }
        }
    }
}
