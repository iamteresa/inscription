using UnityEngine;
using System.Collections.Generic;

public class FieldCardAttack : MonoBehaviour
{
    [Header("�� ���� ������ �����ϴ� BattlefieldManager")]
    [SerializeField] private BattlefieldManager battlefieldManager;

    void Awake()
    {
        if (battlefieldManager == null)
            battlefieldManager = FindObjectOfType<BattlefieldManager>();
    }

    void Update()
    {
        // ��: A Ű�� ������ �� �� ������ ����
        if (Input.GetKeyDown(KeyCode.A))
        {
            AttackLeftToRight();
        }
    }

    /// <summary>
    /// ����(�ε��� 0)���� �����ʱ��� ���Ժ��� FieldCard�� ������
    /// FieldCard.GetAttackPower()�� ȣ���Ͽ� �α׷� ����ϴ�.
    /// </summary>
    public void AttackLeftToRight()
    {
        // BattlefieldManager�� ���� ����Ʈ�� �����ϴ� ������Ƽ�� �ʿ��մϴ�.
        // �Ʒ� �ڵ尡 �����ϵ��� �ʴ´ٸ�, BattlefieldManager��
        //   public List<Transform> SpawnPoints => spawnPoints;
        // ó�� ������(Property)�� �߰����ּ���.

        List<Transform> slots = battlefieldManager.SpawnPoints;
        for (int i = 0; i < slots.Count; i++)
        {
            Transform slot = slots[i];
            if (slot.childCount == 0) continue;

            GameObject cardGO = slot.GetChild(0).gameObject;
            FieldCard fc = cardGO.GetComponent<FieldCard>();
            if (fc != null)
            {
                int atk = fc.GetAttackPower();
                Debug.Log($"{i}�� ������ {cardGO.name} �� ���ݷ� {atk}�� ����!");
            }
        }
    }
}
