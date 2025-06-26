// Assets/Scripts/CardEventBus.cs
using System;

public static class CardEventBus
{
    // --- 카드 소환 ---
    public static event Action<FieldCard> OnSummon;
    public static void Summon(FieldCard c) => OnSummon?.Invoke(c);

    // --- 공격 ---
    public static event Action<FieldCard, FieldCard> OnAttack;
    public static void Attack(FieldCard attacker, FieldCard target) => OnAttack?.Invoke(attacker, target);

    // --- 데미지 ---
    public static event Action<FieldCard, int> OnDamaged;
    public static void Damaged(FieldCard target, int dmg) => OnDamaged?.Invoke(target, dmg);

    // --- 사망 (CardData까지 함께 전달) ---
    public static event Action<FieldCard, CardData> OnDeath;
    public static void Death(FieldCard c, CardData data) => OnDeath?.Invoke(c, data);

    // --- 회복 ---
    public static event Action<FieldCard, int> OnHeal;
    public static void Heal(FieldCard c, int amount) => OnHeal?.Invoke(c, amount);

    // --- 이동 (Mover 능력용) ---
    public static event Action<FieldCard, int> OnMove;
    public static void Move(FieldCard c, int steps) => OnMove?.Invoke(c, steps);

    // --- 턴 시작/종료 (Turn-based abilities) ---
    public static event Action<FieldCard.CardFaction> OnTurnStart;
    public static void TurnStart(FieldCard.CardFaction f) => OnTurnStart?.Invoke(f);

    public static event Action<FieldCard.CardFaction> OnTurnEnd;
    public static void TurnEnd(FieldCard.CardFaction f) => OnTurnEnd?.Invoke(f);
}
