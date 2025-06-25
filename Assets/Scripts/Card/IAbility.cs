public interface IAbility
{
    /// <summary>이 능력의 고유 타입 (종족 or 추가능력)</summary>
    string Id { get; }

    /// <summary>카드 소환 직후 호출. 이벤트 구독 등 초기화</summary>
    void Initialize(FieldCard owner, CardData data);

    /// <summary>카드 제거 직전 호출. 이벤트 언구독 등 정리</summary>
    void Cleanup();
}