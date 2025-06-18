using UnityEngine;
using UnityEngine.UI; 
using TMPro; 
using System.Collections;
using System.Collections.Generic;

public class CardDisplay : MonoBehaviour
{
    // --- ���� �ڵ��� public �ʵ� (����) ---
    // �� �ʵ���� �ν����Ϳ��� ���� �����ϴ� UI ��ҵ��Դϴ�.
    public TMP_Text NameText;
    public TMP_Text SpeciesText;
    public TMP_Text AttackText;
    public TMP_Text HealthText;
    public Image CardImage;
    public Image CardSkillImage; // ���� CardSkillImage �ʵ�
    public TMP_Text Cost;

    // �� cardData �ʵ�� Unity �����Ϳ��� ���� CardData ScriptableObject�� ������ �� ���˴ϴ�.
    // ��Ÿ�ӿ� CardData�� �������� �Ҵ��Ϸ��� _runtimeCardData�� ����մϴ�.
    public CardData cardData; // Unity �����Ϳ��� �����տ� �⺻ CardData�� �Ҵ��� ��� ��� (�ɼ�)


    // ��Ÿ�ӿ� �� ���÷��̰� ǥ���� ���� CardData�� �����ϴ� private �ʵ�
    private CardData _runtimeCardData;

    void Start()
    {
        //  _runtimeCardData�� �켱������ �����ϴ�.
        // ���� Start ������ �̹� _runtimeCardData�� SetCardDisplay()�� ���� �Ҵ�Ǿ��ٸ�, �� �����͸� ����մϴ�.
        // �׷��� �ʰ� public cardData �ʵ尡 �Ҵ�Ǿ� �ִٸ� �װ��� ����մϴ�.
        // �� �� ���ٸ� ��� ���ϴ�.
        if (_runtimeCardData != null)
        {
            // _runtimeCardData�� �̹� �����Ǿ��ٸ�, �װ����� UI ������Ʈ
            SetCardDisplay(_runtimeCardData);
        }
        else if (cardData != null) // ���� public cardData�� �ν����Ϳ� ����Ǿ� �ִٸ�
        {
            _runtimeCardData = cardData; // _runtimeCardData�� �Ҵ��ϰ�
            SetCardDisplay(_runtimeCardData); // �װ����� UI ������Ʈ
        }
        else
        {
            Debug.LogWarning("CardDisplay: Start ������ ǥ���� CardData�� �����ϴ�. " +
                "SetCardDisplay()�� ȣ��� ������ UI�� ����ְų� �⺻���� ǥ���մϴ�.", this);
        }
    }

    /// <summary>
    /// ���� UpdateDisplay �޼���. public cardData �ʵ带 ����Ͽ� UI�� ������Ʈ�մϴ�.
    /// </summary>
    public void UpdateDisplay()
    {
        // _runtimeCardData�� �Ҵ�Ǿ� �ִٸ� �װ��� �켱 ����մϴ�.
        CardData dataToUse = (_runtimeCardData != null) ? _runtimeCardData : cardData;

        if (dataToUse == null)
        {
            Debug.LogWarning("CardDisplay: UpdateDisplay ȣ�� �� cardData�� null�Դϴ�. UI ������Ʈ�� �ǳʶݴϴ�.", this);
            return;
        }

        // ���� �ʵ���� ����Ͽ� UI ������Ʈ
        if (NameText != null) NameText.text = dataToUse.CardName;
        if (SpeciesText != null) SpeciesText.text = dataToUse.Species.ToString();
        if (AttackText != null) AttackText.text = dataToUse.Attack.ToString();
        if (HealthText != null) HealthText.text = dataToUse.Health.ToString();
        if (Cost != null) Cost.text = dataToUse.Cost.ToString();

        if (CardImage != null) CardImage.sprite = dataToUse.CardImage;

        // CardSkillImage�� ���� ������ CardData�� �ش� �ʵ尡 �ִٸ� ���⿡ �߰� ������ �����ؾ� �մϴ�.
        // ��: if (CardSkillImage != null && dataToUse.CardSkillImage != null) CardSkillImage.sprite = dataToUse.CardSkillImage;
    }

    // --- ���� �߰��ǰų� ������ ��� �޼��� ---

    /// <summary>
    /// CardData ��ü�� �޾� UI�� �ʱ�ȭ�ϴ� �޼���.
    /// �� �޼���� ī�� ������Ʈ�� ������ �� �ܺ�(��: HandManager, CardSelector, FieldCard)���� ȣ��˴ϴ�.
    /// </summary>
    /// <param name="data">ǥ���� CardData</param>
    public void SetCardDisplay(CardData data)
    {
        if (data == null)
        {
            Debug.LogError("CardDisplay: ������ CardData�� null�Դϴ�.", this);
            return;
        }

        _runtimeCardData = data; // ��Ÿ�ӿ� �� ���÷��̰� ������ CardData ����

        // ��� UI ��� ������Ʈ (null üũ �ʼ�)
        if (NameText != null) NameText.text = _runtimeCardData.CardName; // ī�� �̸� �ؽ�Ʈ ����
        if (SpeciesText != null) SpeciesText.text = _runtimeCardData.Species.ToString(); // ���� �ؽ�Ʈ ����
        if (Cost != null) Cost.text = _runtimeCardData.Cost.ToString(); // ��� �ؽ�Ʈ ����
        if (CardImage != null) CardImage.sprite = _runtimeCardData.CardImage; // ī�� ��Ʈ��ũ �̹��� ����

        // CardSkillImage ó�� (CardData�� SkillImage �ʵ尡 �ִٸ�)
        // if (CardSkillImage != null && _runtimeCardData.CardSkillImage != null) CardSkillImage.sprite = _runtimeCardData.CardSkillImage;
        // else if (CardSkillImage != null) CardSkillImage.gameObject.SetActive(false); // ��ų �̹����� ������ ��Ȱ��ȭ

        // AttackText�� HealthText�� �ʱⰪ���� ����
        if (AttackText != null) AttackText.text = _runtimeCardData.Attack.ToString();
        if (HealthText != null) HealthText.text = _runtimeCardData.Health.ToString();
    }

    /// <summary>
    /// ī���� ���� ����(���ݷ�, ü��)�� ������Ʈ�ϴ� �޼���.
    /// �������� �ްų� ȸ������ �� FieldCard���� ȣ��˴ϴ�.
    /// </summary>
    /// <param name="currentAttack">���� ���ݷ�</param>
    /// <param name="currentHealth">���� ü��</param>
    public void UpdateStatsDisplay(int currentAttack, int currentHealth)
    {
        if (AttackText != null)
        {
            AttackText.text = currentAttack.ToString();
        }
        if (HealthText != null)
        {
            HealthText.text = currentHealth.ToString();
        }
    }

    /// <summary>
    /// ���� �� ���÷��̰� ǥ���ϴ� CardData�� �����ɴϴ�.
    /// CardSelector ��� �� ī���� CardData�� ������ �� ���˴ϴ�.
    /// </summary>
    /// <returns>���� CardData</returns>
    public CardData GetCardData()
    {
        // ��Ÿ�� �����Ͱ� �ִٸ� �װ��� ��ȯ, ������ public���� ����� cardData ��ȯ
        return _runtimeCardData != null ? _runtimeCardData : cardData;
    }
}