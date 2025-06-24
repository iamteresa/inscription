using System;
using UnityEngine;

public static class CardEventBus
{
    // 카드 소환 시
    public static event Action<FieldCard> OnSummon;

    // 카드가 공격할 때 (공격자, 피격자)
    public static event Action<FieldCard, FieldCard> OnAttack;

    // 카드가 피해를 받을 때 (피해 대상, 피해량)
    public static event Action<FieldCard, int> OnDamaged;

    // 카드가 사망할 때
    public static event Action<FieldCard> OnDeath;

    // 특정 대상에게 피해를 줄 때 (예: Revenger)
    public static event Action<FieldCard, int> OnDamageTarget;

    // 호출용 래퍼 
    public static void Summon(FieldCard card) => OnSummon?.Invoke(card);
    public static void Attack(FieldCard attacker, FieldCard defender)
                                                        => OnAttack?.Invoke(attacker, defender);
    public static void Damaged(FieldCard target, int dmg) => OnDamaged?.Invoke(target, dmg);
    public static void Death(FieldCard card) => OnDeath?.Invoke(card);
    public static void DamageTarget(FieldCard target, int dmg)
                                                        => OnDamageTarget?.Invoke(target, dmg);
}
