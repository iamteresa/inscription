using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 스페이스바 입력 시 씬에 배치된 FieldCard 인스턴스에 데미지를 적용합니다.
/// 에셋(프리팹) 에러를 방지하기 위해 씬 오브젝트만 대상으로 삼습니다.
/// </summary>
public class FieldCardDamageTester : MonoBehaviour
{
    [Header("------------테스트 설정--------------")]
    [Tooltip("스페이스바를 눌렀을 때 적용할 데미지 양")]
    [SerializeField] private int testDamageAmount = 1;
    [SerializeField] EnemyCardManager _enemyCardManger;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            ApplyDamageToAllFieldCards(testDamageAmount);
        }
        if (Input.GetKeyDown(KeyCode.T))
        {
            _enemyCardManger.DrawAndSpawnEnemyCard();
        }
    }

    /// <summary>
    /// 씬에 있는 모든 FieldCard에 데미지를 적용합니다.
    /// </summary>
    private void ApplyDamageToAllFieldCards(int damageAmount)
    {
        // 씬에 배치된 FieldCard 인스턴스만 가져옴
        FieldCard[] allFieldCards = FindObjectsOfType<FieldCard>();
        bool anyHit = false;

        foreach (FieldCard fc in allFieldCards)
        {
            // 씬에 속해 있는 오브젝트인지 확인하고, 플레이 모드인지 확인
            if (fc.gameObject.scene.IsValid() && Application.isPlaying)
            {
                fc.TakeDamage(damageAmount);
                Debug.Log($"{fc.gameObject.name}에 {damageAmount} 데미지 적용. 현재 체력: {fc.GetCurrentHealth()}");
                anyHit = true;
            }
        }

        if (!anyHit)
        {
            Debug.Log("필드에 데미지를 줄 FieldCard가 없습니다.");
        }
    }
}