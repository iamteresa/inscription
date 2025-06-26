/// <summary>
/// 카드에 붙일 수 있는 능력(종족·스킬)을 정의하는 인터페이스
/// </summary>
public interface ICardAbility
{
    /// <summary>능력 고유 ID (중복 방지용)</summary>
    string Id { get; }

    /// <summary>
    /// 카드가 전장에 소환될 때(Initialize 호출) 능력 초기 설정
    /// </summary>
    /// <param name="owner">이 능력이 붙는 FieldCard</param>
    /// <param name="data">카드의 원본 ScriptableObject</param>
    void Initialize(FieldCard owner, CardData data);

    /// <summary>
    /// 카드가 전장에서 제거되기 직전에 호출
    /// (이벤트 구독 해제 등 정리 작업)</summary>
    void Cleanup();
}