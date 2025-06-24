public interface IAbility
{
    CardData.CardAbilityType Id { get; }
    /// <summary>카드에 능력을 붙이고, 필요한 이벤트를 구독합니다.</summary>
    void Initialize(FieldCard owner, CardData data);
    /// <summary>카드가 제거될 때 호출, 이벤트 언구독 등 정리.</summary>
    void Cleanup();
}