using UnityEngine;

/// <summary>
/// 전장에 배치된 카드의 데이터를 설정하고, 시각적 표시만 담당합니다.
/// 체력, 공격력 등 런타임 스탯 관리는 BattleCardManager가 담당합니다.
/// </summary>
[RequireComponent(typeof(CardDisplay))]
public class FieldCard : MonoBehaviour
{
    [Header("---------필드 카드 설정---------")]
    [SerializeField] private CardDisplay cardDisplay;      // 카드 UI 표시용
    [SerializeField] private CardFaction faction;         // 카드 진영 (플레이어 또는 적)

    public enum CardFaction { Player, Enemy }

    /// <summary>
    /// 카드 데이터를 할당하고, CardDisplay를 업데이트합니다.
    /// BattleCardManager.RegisterCard는 외부에서 호출해주세요.
    /// </summary>
    public void SetCardData(CardData data, CardFaction cardFaction)
    {
        if (data == null)
        {
            Debug.LogError("FieldCard: 설정할 CardData가 null입니다.", this);
            return;
        }

        faction = cardFaction;

        // 카드 UI 갱신
        if (cardDisplay == null)
            cardDisplay = GetComponent<CardDisplay>();

        cardDisplay.SetCardDisplay(data);
    }

    /// <summary>
    /// BattleCardManager나 외부 시스템이 카드 제거 시 호출합니다.
    /// </summary>
    public void RemoveFromField()
    {
        Destroy(gameObject);
    }
}
