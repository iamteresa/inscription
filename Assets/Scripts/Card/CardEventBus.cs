using System;

public static class CardEventBus
{
    public static event Action<FieldCard, CardData> OnDeath;

    public static void Death(FieldCard card, CardData data) => OnDeath?.Invoke(card, data);

    //public static event Action<FieldCard> OnDeath;

    //// 이 메서드를 통해 외부에서 죽음 이벤트를 발행한다
    //public static void Death(FieldCard c) => OnDeath?.Invoke(c);

    // ● 카드 소환 직후(전장에 배치된 직후)
    public static event Action<FieldCard> OnSummon;
    public static void Summon(FieldCard c) => OnSummon?.Invoke(c);

    // ● 공격 직후 (공격자, 방어자)
    public static event Action<FieldCard, FieldCard> OnAttack;
    public static void Attack(FieldCard a, FieldCard d) => OnAttack?.Invoke(a, d);

    // ● 피격 직후 (피해 받은 카드, 입은 데미지)
    public static event Action<FieldCard, int> OnDamaged;
    public static void Damaged(FieldCard t, int dmg) => OnDamaged?.Invoke(t, dmg);

    // ● 사망 직후 (죽은 카드)
    //public static event Action<FieldCard> OnDeath;
    //public static void Death(FieldCard c) => OnDeath?.Invoke(c);

    // ▼ 추가된 부분 ▼

    // ● 회복 직후 (회복된 카드, 회복량)
    public static event Action<FieldCard, int> OnHeal;
    public static void Heal(FieldCard c, int amt) => OnHeal?.Invoke(c, amt);

    // ● 이동 직후 (이동한 카드, 이동 칸 수)
    public static event Action<FieldCard, int> OnMove;
    public static void Move(FieldCard c, int steps) => OnMove?.Invoke(c, steps);

    // ● 턴이 시작될 때 (누구 턴인지: Player/Enemy)
    public static event Action<FieldCard.CardFaction> OnTurnStart;
    public static void TurnStart(FieldCard.CardFaction faction) => OnTurnStart?.Invoke(faction);

    // ● 턴이 끝날 때 (누구 턴이 끝났는지)
    public static event Action<FieldCard.CardFaction> OnTurnEnd;
    public static void TurnEnd(FieldCard.CardFaction faction) => OnTurnEnd?.Invoke(faction);
}
