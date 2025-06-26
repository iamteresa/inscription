// Assets/Scripts/Abilities/PoisonerAbility.cs
using UnityEngine;

/// <summary>
/// 독살자 능력:
///  - 이 카드가 공격할 때 상대 FieldCard에
///    지정된 턴 수만큼 매 턴 1데미지 + 공격력 1 감소 효과를 부여합니다.
/// </summary>
public class PoisonerAbility : IAbility
{
    public string Id => "Poisoner";

    FieldCard _owner;
    int _durationTurns;

    public void Initialize(FieldCard owner, CardData data)
    {
        _owner = owner;
        _durationTurns = data.AbilityValue;   // 스크립터블에 설정된 턴 수
        CardEventBus.OnAttack += OnAttack;
    }

    void OnAttack(FieldCard attacker, FieldCard target)
    {
        if (attacker != _owner || target == null) return;

        // 공격할 때만 효과 붙이기
        var effect = new TemporaryPoisonEffect(_durationTurns);
        // IAbility.Initialize 시 signature 에 맞춰 호출하되 data는 무시
        effect.Initialize(target, null);
        target.RegisterAbility(effect);
    }

    public void Cleanup()
    {
        CardEventBus.OnAttack -= OnAttack;
    }

    /// <summary>
    /// 실제로 매 턴마다 독 효과를 처리하는 내부 클래스
    /// </summary>
    private class TemporaryPoisonEffect : IAbility
    {
        public string Id => "PoisonEffect";

        FieldCard _target;
        int _turnsRemaining;

        // 생성자에서 턴 수를 받아 저장
        public TemporaryPoisonEffect(int turns)
        {
            _turnsRemaining = turns;
        }

        // IAbility.Initialize(…) 시 호출됩니다.
        public void Initialize(FieldCard owner, CardData data)
        {
            _target = owner;
            CardEventBus.OnTurnStart += OnTurnStart;
        }

        void OnTurnStart(FieldCard.CardFaction f)
        {
            // 내 진영 턴에만 작동
            if (f != _target.faction) return;

            // 1 데미지 + 공격력 1 감소
            _target.TakeDamage(1);
            int newAtk = Mathf.Max(0, _target.GetAttackPower() - 1);
            _target.SetAttackPower(newAtk);

            _turnsRemaining--;
            if (_turnsRemaining <= 0)
                Cleanup();
        }

        public void Cleanup()
        {
            CardEventBus.OnTurnStart -= OnTurnStart;
        }
    }
}
