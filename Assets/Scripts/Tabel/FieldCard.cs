using UnityEngine;
using UnityEngine.UI; // Text, Image ���� ����Ѵٸ� �ʿ�
using TMPro; // TextMeshPro�� ����Ѵٸ� �ʿ�
using System.Collections.Generic; // �ɷ� ������ �ʿ�

public class FieldCard : MonoBehaviour
{
    [Header("--------------ī�� ������---------------")]
    public CardData _cardData; // �� ī�尡 ������ CardData ScriptableObject

    [Header("-------------UI ����---------------")]
    [SerializeField] private TextMeshProUGUI _attackText; // ���ݷ� ǥ�� Text (TMP)
    [SerializeField] private TextMeshProUGUI _healthText; // ü�� ǥ�� Text (TMP)
    [SerializeField] private Image _cardImageDisplay; // ī�� �̹��� ǥ�� Image
    [SerializeField] private TextMeshProUGUI _abilityDescriptionText; // �ɷ� ���� Text (TMP)

    private int currentAttack;
    private int currentHealth;

    public int Attack => currentAttack;
    public int Health => currentHealth;

    // ī�尡 ��� �������� (�÷��̾�/��)
    public enum CardFaction { Player, Enemy }
    public CardFaction faction;

    void Awake()
    {
        // UI ��� ���� Ȯ��
        if (_attackText == null || _healthText == null || _cardImageDisplay == null || _abilityDescriptionText == null)
        {
            Debug.LogWarning("FieldCard: �Ϻ� UI ��Ұ� ������� �ʾҽ��ϴ�. �ν����Ϳ��� Ȯ�����ּ���.", this);
        }
    }

    /// <summary>
    /// ī���� CardData�� �����ϰ� ������ �ʱ�ȭ�մϴ�.
    /// �� �޼���� ī�尡 �ʵ忡 ��ȯ�� �� �ܺο��� ȣ��Ǿ�� �մϴ�.
    /// </summary>
    /// <param name="data">�� ī�忡 �Ҵ��� CardData</param>
    /// <param name="cardFaction">�� ī���� ���� (�÷��̾� �Ǵ� ��)</param>
    public void SetCardData(CardData data, CardFaction cardFaction)
    {
        _cardData = data;
        faction = cardFaction;

        if (_cardData != null)
        {
            currentAttack = _cardData.Attack; // CardData�� Attack ������Ƽ ���
            currentHealth = _cardData.Health; // CardData�� Health ������Ƽ ���
            UpdateCardUI();
        }
        else
        {
            Debug.LogError("FieldCard: CardData�� �Ҵ���� �ʾҽ��ϴ�!", this);
        }
    }

    /// <summary>
    /// ī���� UI�� ������Ʈ�մϴ� (���ݷ�, ü��, �̹���, �ɷ� ����).
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
            // �ɷ��� ���ٸ� ������ ���ų� ����
            if (_cardData.AbilityType == CardData.CardAbilityType.None || string.IsNullOrEmpty(_cardData.AbilityDescription))
            {
                _abilityDescriptionText.text = "";
                // _abilityDescriptionText.gameObject.SetActive(false); // UI�� ������ �������
            }
            else
            {
                _abilityDescriptionText.text = _cardData.AbilityDescription;
                // _abilityDescriptionText.gameObject.SetActive(true);
            }
        }
    }

    /// <summary>
    /// �� ī�尡 �������� �޽��ϴ�.
    /// </summary>
    /// <param name="damageAmount">���� ������ ��</param>
    public void TakeDamage(int damageAmount)
    {
        if (damageAmount < 0)
        {
            Debug.LogWarning("������ ���� ������ �� �� �����ϴ�. Heal ��� TakeDamage�� ����ϼ���.");
            return;
        }

        currentHealth -= damageAmount;
        currentHealth = Mathf.Max(currentHealth, 0); // ü���� 0 �̸����� �������� �ʵ���

        Debug.Log($"{_cardData.CardName}��(��) {damageAmount} �������� �޾ҽ��ϴ�. ���� ü��: {currentHealth}");
        
        UpdateCardUI(); // UI ������Ʈ

        if (currentHealth <= 0)
        {
            Die(); // ü���� 0 ���ϸ� ��� ó��
        }
    }

    /// <summary>
    /// �� ī�尡 ü���� ȸ���մϴ�.
    /// </summary>
    /// <param name="healAmount">ȸ���� ü�� ��</param>
    public void Heal(int healAmount)
    {
        if (healAmount < 0)
        {
            Debug.LogWarning("ȸ�� ���� ������ �� �� �����ϴ�. Heal ��� TakeDamage�� ����ϼ���.");
            return;
        }

        currentHealth += healAmount;
        // ȸ���� �⺻ ü��(�ִ� ü��)�� ���� �ʵ���
        currentHealth = Mathf.Min(currentHealth, _cardData.Health);

        Debug.Log($"{_cardData.CardName}��(��) {healAmount} ü���� ȸ���߽��ϴ�. ���� ü��: {currentHealth}");

        UpdateCardUI();
    }

    /// <summary>
    /// �� ī�尡 �ٸ� ������ �����մϴ�.
    /// </summary>
    /// <param name="targetCard">������ ��� ī��</param>
    public void AttackTarget(FieldCard targetCard)
    {
        if (targetCard == null)
        {
            Debug.LogWarning($"{_cardData.CardName}��(��) ������ ����� �����ϴ�.");
            return;
        }

        Debug.Log($"{_cardData.CardName}��(��) {targetCard._cardData.CardName}��(��) �����մϴ�! ������: {currentAttack}");
        targetCard.TakeDamage(currentAttack); // ��󿡰� ������ �ο�

        // ���� �� �ڽŵ� �ݰ� �������� �޴� ��� (�Ͻ����� ��)
        if (targetCard.Attack > 0) // ����� ���ݷ��� 0���� Ŀ�� �ݰ�
        {
            TakeDamage(targetCard.Attack); // ����� ���ݷ¸�ŭ �ڽŵ� ������
        }

        // --- ����� ��� �ɷ� ó�� ---
        if (_cardData.AbilityType == CardData.CardAbilityType.Lifesteal)
        {
            Heal(currentAttack); // ���� ��������ŭ ü�� ȸ��
            Debug.Log($"{_cardData.CardName}��(��) ����� ����� {currentAttack} ü���� ȸ���߽��ϴ�.");
        }
    }

    /// <summary>
    /// ī�尡 ������� �� ȣ��˴ϴ�.
    /// </summary>
    private void Die()
    {
        Debug.Log($"{_cardData.CardName}��(��) ����߽��ϴ�!");

        // --- ������ �޾Ƹ� �ɷ� ó�� ---
        if (_cardData.AbilityType == CardData.CardAbilityType.Deathrattle)
        {
            Debug.Log($"{_cardData.CardName}�� ������ �޾Ƹ��� �ߵ��մϴ�!");
            ActivateAbility(_cardData.AbilityType, _cardData.AbilityValue);
        }

        // �� ī�带 �ʵ忡�� �����ϴ� ���� (��: �θ� ������Ʈ�� ����Ʈ���� ����, ������Ʈ �ı� ��)
        Destroy(gameObject); // �� ī�� ���� ������Ʈ �ı�
    }

    /// <summary>
    /// ī�尡 �ʵ忡 ��ȯ�� �� ȣ��˴ϴ�.
    /// ����(Battlecry)�� ���� �ɷ� �ߵ��� ���� �� �ֽ��ϴ�.
    /// </summary>
    public void OnSummoned()
    {
        Debug.Log($"{_cardData.CardName}��(��) ��ȯ�Ǿ����ϴ�!");

      
    }

    /// <summary>
    /// ī���� Ư�� �ɷ��� Ȱ��ȭ�մϴ�.
    /// </summary>
    /// <param name="ability">Ȱ��ȭ�� �ɷ� Ÿ��</param>
    /// <param name="value">�ɷ¿� ���� ��</param>
    private void ActivateAbility(CardData.CardAbilityType ability, int value)
    {
        switch (ability)
        {
            case CardData.CardAbilityType.Killer:
                // ���θ� �ɷ¿� ���� �߰����� �����̳� ȿ���� �ִٸ� ���⿡ ����.
                // ���� ���, _cardData.abilityValue�� ���� Ư�� ���� �ߵ�.
                if (_cardData.AbilityDescription.Contains("���ݹ��� ���� ���.")) // ����: ���� ���� �ɷ� �б� (���� ����� �ƴ�)
                {
                    Debug.Log("����: ��� �Ʊ� ü�� " + value + " ȸ��!");
                    // ���� ����: GameManager�� BattlefieldManager�� ���� �Ʊ� ī�� ã�Ƽ� Heal ȣ��
                    // Example: GameManager.Instance.HealAllFriendlyMinions(faction, value);
                }
                else if (_cardData.AbilityDescription.Contains("ī�� ��ο�"))
                {
                    Debug.Log("����: ī�� " + value + "�� ��ο�!");
                    // Example: HandManager.Instance.DrawCards(value);
                }
                break;
            case CardData.CardAbilityType.Deathrattle:
                // ������ �޾Ƹ� �ɷ¿� ���� �߰����� �����̳� ȿ���� �ִٸ� ���⿡ ����.
                if (_cardData.AbilityDescription.Contains("��� ������ ������")) // ����
                {
                    Debug.Log("������ �޾Ƹ�: ��� ������ " + value + " ������!");
                    // ���� ����: GameManager�� BattlefieldManager�� ���� �� ī�� ã�Ƽ� TakeDamage ȣ��
                    // Example: GameManager.Instance.DamageAllOpponentMinions(faction, value);
                }
                break;
            case CardData.CardAbilityType.Mover:
                // ���� ī��� Ư���� ��Ƽ�� ������ �ƴ�, �ٸ� ī���� ���� Ÿ���ÿ� ������ �ݴϴ�.
                // �̴� ���� ��� ���� �������� �����Ǿ�� �մϴ�.
                Debug.Log($"{_cardData.CardName}��(��) ������ ������ �ֽ��ϴ�!");
                break;
            case CardData.CardAbilityType.Lifesteal:
                // ����� ����� AttackTarget �޼��忡�� �̹� ó���˴ϴ�.
                Debug.Log($"{_cardData.CardName}��(��) ����� ��� �ɷ��� ������ �ֽ��ϴ�.");
                break;
            case CardData.CardAbilityType.Defender:
                Debug.Log($"ī�� {value}�� ��ο�!");
                // ���� ��ο� ������ HandManager���� �����Ǿ�� �մϴ�.
                // HandManager.Instance.DrawCards(value);
                break;
            case CardData.CardAbilityType.Flyer:
                Debug.Log($"{_cardData.CardName}��(��) õ���� ��ȣ���� ������ϴ�.");
                // ���� ����: ������ �Ҹ��� �÷��׸� �ΰų�, TakeDamage �������� ù �������� �����ϵ��� ó��
                break;
            case CardData.CardAbilityType.Diver:
                Debug.Log($"��� ������ {value} �������� �ݴϴ�!");
                // GameManager�� �ʵ� �����ڸ� ���� ��� �� FieldCard�� ã�� TakeDamage ȣ��
                break;
            case CardData.CardAbilityType.GoblinRoad:
                Debug.Log($"��� �Ʊ� ü���� {value} ȸ���մϴ�!");
                // GameManager�� �ʵ� �����ڸ� ���� ��� �Ʊ� FieldCard�� ã�� Heal ȣ��
                break;
            // ... �ٸ� �ɷ� ���� 
            case CardData.CardAbilityType.None:
            default:
                Debug.Log($"{_cardData.CardName}��(��) Ư���� Ȱ��ȭ �ɷ��� �����ϴ�.");
                break;
        }
    }


    // �ܺο��� ���� ���ݷ°� ü���� ���� �� �ִ� ������Ƽ (�߰�: �ʿ��ϴٸ�)
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