using UnityEngine;
using System.Collections.Generic;

public class FieldCardAttack : MonoBehaviour
{
    [Header("← 공격 순서를 관리하는 BattlefieldManager")]
    [SerializeField] private BattlefieldManager battlefieldManager;

    void Awake()
    {
        if (battlefieldManager == null)
            battlefieldManager = FindObjectOfType<BattlefieldManager>();
    }

    void Update()
    {
        // 예: A 키를 누르면 한 턴 공격을 실행
        if (Input.GetKeyDown(KeyCode.A))
        {
            AttackLeftToRight();
        }
    }

    /// <summary>
    /// 왼쪽(인덱스 0)부터 오른쪽까지 슬롯별로 FieldCard가 있으면
    /// FieldCard.GetAttackPower()를 호출하여 로그로 찍습니다.
    /// </summary>
    public void AttackLeftToRight()
    {
        // BattlefieldManager에 슬롯 리스트를 노출하는 프로퍼티가 필요합니다.
        // 아래 코드가 컴파일되지 않는다면, BattlefieldManager에
        //   public List<Transform> SpawnPoints => spawnPoints;
        // 처럼 접근자(Property)를 추가해주세요.

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
                Debug.Log($"{i}번 슬롯의 {cardGO.name} → 공격력 {atk}로 공격!");
            }
        }
    }
}
