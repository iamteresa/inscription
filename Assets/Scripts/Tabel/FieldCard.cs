using UnityEngine;
using UnityEngine.UI; // Text, Image 등을 사용한다면 필요
using TMPro; // TextMeshPro를 사용한다면 필요
using System.Collections.Generic; // 능력 구현에 필요

public class FieldCard : MonoBehaviour
{
    [Header("--------------카드 데이터---------------")]
    public CardData _cardData; // 이 카드가 참조할 CardData ScriptableObject

    [Header("-------------UI 연동---------------")]
    [SerializeField] private TextMeshProUGUI _attackText; // 공격력 표시 Text (TMP)
    [SerializeField] private TextMeshProUGUI _healthText; // 체력 표시 Text (TMP)
    [SerializeField] private Image _cardImageDisplay; // 카드 이미지 표시 Image
    [SerializeField] private TextMeshProUGUI _abilityDescriptionText; // 능력 설명 Text (TMP)

    private int currentAttack;
    private int currentHealth;

    public int Attack => currentAttack;
    public int Health => currentHealth;

    // 카드가 어느 진영인지 (플레이어/적)
    public enum CardFaction { Player, Enemy }
    public CardFaction faction;

    void Awake()
    {
        // UI 요소 연결 확인
        if (_attackText == null || _healthText == null || _cardImageDisplay == null || _abilityDescriptionText == null)
        {
            Debug.LogWarning("FieldCard: 일부 UI 요소가 연결되지 않았습니다. 인스펙터에서 확인해주세요.", this);
        }
    }

    /// <summary>
    /// 카드의 CardData를 설정하고 스탯을 초기화합니다.
    /// 이 메서드는 카드가 필드에 소환될 때 외부에서 호출되어야 합니다.
    /// </summary>
    /// <param name="data">이 카드에 할당할 CardData</param>
    /// <param name="cardFaction">이 카드의 진영 (플레이어 또는 적)</param>
    public void SetCardData(CardData data, CardFaction cardFaction)
    {
        _cardData = data;
        faction = cardFaction;

        if (_cardData != null)
        {
            currentAttack = _cardData.Attack; // CardData의 Attack 프로퍼티 사용
            currentHealth = _cardData.Health; // CardData의 Health 프로퍼티 사용
            UpdateCardUI();
        }
        else
        {
            Debug.LogError("FieldCard: CardData가 할당되지 않았습니다!", this);
        }
    }

    /// <summary>
    /// 카드의 UI를 업데이트합니다 (공격력, 체력, 이미지, 능력 설명).
    /// </summary>
    private void UpdateCardUI()
    {
        if (_attackText != null)
        {
            _attackText.text = currentAttack.ToString();
        }
        if (_healthText != null)
        {
            _healthText.text = currentHealth.ToString();
        }
        if (_cardImageDisplay != null && _cardData.CardImage != null)
        {
            _cardImageDisplay.sprite = _cardData.CardImage;
        }
        if (_abilityDescriptionText != null)
        {
            // 능력이 없다면 설명을 비우거나 숨김
            if (_cardData.AbilityType == CardData.CardAbilityType.None || string.IsNullOrEmpty(_cardData.AbilityDescription))
            {
                _abilityDescriptionText.text = "";
                // _abilityDescriptionText.gameObject.SetActive(false); // UI를 완전히 숨기려면
            }
            else
            {
                _abilityDescriptionText.text = _cardData.AbilityDescription;
                // _abilityDescriptionText.gameObject.SetActive(true);
            }
        }
    }

    /// <summary>
    /// 이 카드가 데미지를 받습니다.
    /// </summary>
    /// <param name="damageAmount">받을 데미지 양</param>
    public void TakeDamage(int damageAmount)
    {
        if (damageAmount < 0)
        {
            Debug.LogWarning("데미지 양은 음수가 될 수 없습니다. Heal 대신 TakeDamage를 사용하세요.");
            return;
        }

        currentHealth -= damageAmount;
        currentHealth = Mathf.Max(currentHealth, 0); // 체력이 0 미만으로 내려가지 않도록

        Debug.Log($"{_cardData.CardName}이(가) {damageAmount} 데미지를 받았습니다. 현재 체력: {currentHealth}");
        
        UpdateCardUI(); // UI 업데이트

        if (currentHealth <= 0)
        {
            Die(); // 체력이 0 이하면 사망 처리
        }
    }

    /// <summary>
    /// 이 카드가 체력을 회복합니다.
    /// </summary>
    /// <param name="healAmount">회복할 체력 양</param>
    public void Heal(int healAmount)
    {
        if (healAmount < 0)
        {
            Debug.LogWarning("회복 양은 음수가 될 수 없습니다. Heal 대신 TakeDamage를 사용하세요.");
            return;
        }

        currentHealth += healAmount;
        // 회복은 기본 체력(최대 체력)을 넘지 않도록
        currentHealth = Mathf.Min(currentHealth, _cardData.Health);

        Debug.Log($"{_cardData.CardName}이(가) {healAmount} 체력을 회복했습니다. 현재 체력: {currentHealth}");

        UpdateCardUI();
    }

    /// <summary>
    /// 이 카드가 다른 유닛을 공격합니다.
    /// </summary>
    /// <param name="targetCard">공격할 대상 카드</param>
    public void AttackTarget(FieldCard targetCard)
    {
        if (targetCard == null)
        {
            Debug.LogWarning($"{_cardData.CardName}이(가) 공격할 대상이 없습니다.");
            return;
        }

        Debug.Log($"{_cardData.CardName}이(가) {targetCard._cardData.CardName}을(를) 공격합니다! 데미지: {currentAttack}");
        targetCard.TakeDamage(currentAttack); // 대상에게 데미지 부여

        // 공격 후 자신도 반격 데미지를 받는 경우 (하스스톤 등)
        if (targetCard.Attack > 0) // 대상의 공격력이 0보다 커야 반격
        {
            TakeDamage(targetCard.Attack); // 대상의 공격력만큼 자신도 데미지
        }

        // --- 생명력 흡수 능력 처리 ---
        if (_cardData.AbilityType == CardData.CardAbilityType.Lifesteal)
        {
            Heal(currentAttack); // 공격 데미지만큼 체력 회복
            Debug.Log($"{_cardData.CardName}이(가) 생명력 흡수로 {currentAttack} 체력을 회복했습니다.");
        }
    }

    /// <summary>
    /// 카드가 사망했을 때 호출됩니다.
    /// </summary>
    private void Die()
    {
        Debug.Log($"{_cardData.CardName}이(가) 사망했습니다!");

        // --- 죽음의 메아리 능력 처리 ---
        if (_cardData.AbilityType == CardData.CardAbilityType.Deathrattle)
        {
            Debug.Log($"{_cardData.CardName}의 죽음의 메아리가 발동합니다!");
            ActivateAbility(_cardData.AbilityType, _cardData.AbilityValue);
        }

        // 이 카드를 필드에서 제거하는 로직 (예: 부모 오브젝트의 리스트에서 제거, 오브젝트 파괴 등)
        Destroy(gameObject); // 이 카드 게임 오브젝트 파괴
    }

    /// <summary>
    /// 카드가 필드에 소환될 때 호출됩니다.
    /// 전함(Battlecry)과 같은 능력 발동에 사용될 수 있습니다.
    /// </summary>
    public void OnSummoned()
    {
        Debug.Log($"{_cardData.CardName}이(가) 소환되었습니다!");

      
    }

    /// <summary>
    /// 카드의 특정 능력을 활성화합니다.
    /// </summary>
    /// <param name="ability">활성화할 능력 타입</param>
    /// <param name="value">능력에 사용될 값</param>
    private void ActivateAbility(CardData.CardAbilityType ability, int value)
    {
        switch (ability)
        {
            case CardData.CardAbilityType.Killer:
                // 살인마 능력에 대한 추가적인 조건이나 효과가 있다면 여기에 구현.
                // 예를 들어, _cardData.abilityValue에 따라 특정 전함 발동.
                if (_cardData.AbilityDescription.Contains("공격받은 상대방 즉사.")) // 예시: 설명에 따라 능력 분기 (좋은 방법은 아님)
                {
                    Debug.Log("전함: 모든 아군 체력 " + value + " 회복!");
                    // 실제 구현: GameManager나 BattlefieldManager를 통해 아군 카드 찾아서 Heal 호출
                    // Example: GameManager.Instance.HealAllFriendlyMinions(faction, value);
                }
                else if (_cardData.AbilityDescription.Contains("카드 드로우"))
                {
                    Debug.Log("전함: 카드 " + value + "장 드로우!");
                    // Example: HandManager.Instance.DrawCards(value);
                }
                break;
            case CardData.CardAbilityType.Deathrattle:
                // 죽음의 메아리 능력에 대한 추가적인 조건이나 효과가 있다면 여기에 구현.
                if (_cardData.AbilityDescription.Contains("모든 적에게 데미지")) // 예시
                {
                    Debug.Log("죽음의 메아리: 모든 적에게 " + value + " 데미지!");
                    // 실제 구현: GameManager나 BattlefieldManager를 통해 적 카드 찾아서 TakeDamage 호출
                    // Example: GameManager.Instance.DamageAllOpponentMinions(faction, value);
                }
                break;
            case CardData.CardAbilityType.Mover:
                // 도발 카드는 특별한 액티브 로직이 아닌, 다른 카드의 공격 타겟팅에 영향을 줍니다.
                // 이는 공격 대상 선택 로직에서 구현되어야 합니다.
                Debug.Log($"{_cardData.CardName}이(가) 도발을 가지고 있습니다!");
                break;
            case CardData.CardAbilityType.Lifesteal:
                // 생명력 흡수는 AttackTarget 메서드에서 이미 처리됩니다.
                Debug.Log($"{_cardData.CardName}이(가) 생명력 흡수 능력을 가지고 있습니다.");
                break;
            case CardData.CardAbilityType.Defender:
                Debug.Log($"카드 {value}장 드로우!");
                // 실제 드로우 로직은 HandManager에서 구현되어야 합니다.
                // HandManager.Instance.DrawCards(value);
                break;
            case CardData.CardAbilityType.Flyer:
                Debug.Log($"{_cardData.CardName}이(가) 천상의 보호막을 얻었습니다.");
                // 실제 구현: 별도의 불리언 플래그를 두거나, TakeDamage 로직에서 첫 데미지를 무시하도록 처리
                break;
            case CardData.CardAbilityType.Diver:
                Debug.Log($"모든 적에게 {value} 데미지를 줍니다!");
                // GameManager나 필드 관리자를 통해 모든 적 FieldCard를 찾아 TakeDamage 호출
                break;
            case CardData.CardAbilityType.GoblinRoad:
                Debug.Log($"모든 아군 체력을 {value} 회복합니다!");
                // GameManager나 필드 관리자를 통해 모든 아군 FieldCard를 찾아 Heal 호출
                break;
            // ... 다른 능력 구현 
            case CardData.CardAbilityType.None:
            default:
                Debug.Log($"{_cardData.CardName}은(는) 특별한 활성화 능력이 없습니다.");
                break;
        }
    }


    // 외부에서 현재 공격력과 체력을 얻을 수 있는 프로퍼티 (추가: 필요하다면)
    public int GetCurrentAttack()
    {
        return currentAttack;
    }

    public int GetCurrentHealth()
    {
        return currentHealth;
    }

    public CardData GetCardData()
    {
        return _cardData;
    }
}