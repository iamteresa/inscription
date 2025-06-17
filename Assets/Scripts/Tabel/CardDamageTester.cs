using UnityEngine;
using System.Collections.Generic; // List를 사용하기 위해 필요

public class FieldCardDamageTester : MonoBehaviour
{
    [Header("------------테스트 설정--------------")]
    [SerializeField] private int testDamageAmount = 1; // 스페이스바를 누를 때 줄 데미지 양

    void Update()
    {
        // 스페이스바를 누르면 필드 위의 모든 카드에 데미지를 줍니다.
        if (Input.GetKeyDown(KeyCode.Space))
        {
            ApplyDamageToAllFieldCards(testDamageAmount);
        }
    }

    /// <summary>
    /// 현재 씬에 있는 모든 FieldCard에 데미지를 적용합니다.
    /// </summary>
    /// <param name="damageAmount">적용할 데미지 양</param>
    private void ApplyDamageToAllFieldCards(int damageAmount)
    {
        // 씬의 모든 FieldCard 컴포넌트를 찾습니다.
        // 주의: 이 방식은 씬에 FieldCard가 많아질수록 성능에 영향을 줄 수 있습니다.
        // 실제 게임에서는 GameManager나 BattlefieldManager가 FieldCard 목록을 관리하는 것이 좋습니다.
        FieldCard[] allFieldCards = FindObjectsOfType<FieldCard>();

        if (allFieldCards.Length == 0)
        {
            Debug.Log("필드에 데미지를 줄 FieldCard가 없습니다.");
            return;
        }

        Debug.Log($"스페이스바 눌림! 필드의 모든 카드에 {damageAmount} 데미지 적용 중...");

        foreach (FieldCard card in allFieldCards)
        {
            // 카드 진영에 따라 특정 진영에만 데미지를 줄 수도 있습니다.
            // 예를 들어, 플레이어 카드에만 데미지를 주려면:
            // if (card.faction == FieldCard.CardFaction.Player)
            // {
            //     card.TakeDamage(damageAmount);
            // }

            // 현재는 모든 FieldCard에 데미지 적용
            card.TakeDamage(damageAmount);
        }
    }
}