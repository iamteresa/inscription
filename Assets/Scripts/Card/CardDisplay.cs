using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class CardDisplay : MonoBehaviour
{
    // --- 기존 코드의 public 필드 (유지) ---
    public TMP_Text NameText;
    public TMP_Text SpeciesText;
    public TMP_Text AttackText;
    public TMP_Text HealthText;
    public Image CardImage;
    public Image CardSkillImage; // 능력 아이콘용 Image
    public TMP_Text Cost;

    // 런타임에 표시할 CardData
    public CardData cardData;           // 에디터 연결용
    private CardData _runtimeCardData;  // 동적 할당용

    void Start()
    {
        // 런타임 데이터가 있으면 우선 사용, 없으면 에디터 데이터 사용
        if (_runtimeCardData != null)
        {
            SetCardDisplay(_runtimeCardData);
        }
        else if (cardData != null)
        {
            _runtimeCardData = cardData;
            SetCardDisplay(_runtimeCardData);
        }
        else
        {
            Debug.LogWarning("CardDisplay: 표시할 CardData가 없습니다.", this);
        }
    }

    /// <summary>
    /// UI의 텍스트·이미지를 CardData로 업데이트합니다.
    /// </summary>
    public void UpdateDisplay()
    {
        var dataToUse = _runtimeCardData != null ? _runtimeCardData : cardData;
        if (dataToUse == null)
        {
            Debug.LogWarning("CardDisplay: UpdateDisplay 호출 시 cardData가 null입니다.", this);
            return;
        }

        // 기본 텍스트/이미지
        if (NameText != null) NameText.text = dataToUse.CardName;
        if (SpeciesText != null) SpeciesText.text = dataToUse.Species.ToString();
        if (AttackText != null) AttackText.text = dataToUse.Attack.ToString();
        if (HealthText != null) HealthText.text = dataToUse.Health.ToString();
        if (Cost != null) Cost.text = dataToUse.Cost.ToString();
        if (CardImage != null) CardImage.sprite = dataToUse.CardImage;

        // --- 능력 아이콘 활성화/비활성화 로직 추가 ---
        if (CardSkillImage != null)
        {
            if (dataToUse.CardSkillImage != null)
            {
                CardSkillImage.sprite = dataToUse.CardSkillImage;
                CardSkillImage.gameObject.SetActive(true);
            }
            else
            {
                CardSkillImage.gameObject.SetActive(false);
            }
        }
    }

    /// <summary>
    /// 외부에서 CardData를 지정하고 즉시 UI를 초기화합니다.
    /// </summary>
    public void SetCardDisplay(CardData data)
    {
        if (data == null)
        {
            Debug.LogError("CardDisplay: SetCardDisplay 호출 시 data가 null입니다.", this);
            return;
        }

        _runtimeCardData = data;

        // 기본 텍스트/이미지
        if (NameText != null) NameText.text = data.CardName;
        if (SpeciesText != null) SpeciesText.text = data.Species.ToString();
        if (AttackText != null) AttackText.text = data.Attack.ToString();
        if (HealthText != null) HealthText.text = data.Health.ToString();
        if (Cost != null) Cost.text = data.Cost.ToString();
        if (CardImage != null) CardImage.sprite = data.CardImage;

        // --- 능력 아이콘 활성화/비활성화 로직 추가 ---
        if (CardSkillImage != null)
        {
            if (data.CardSkillImage != null)
            {
                CardSkillImage.sprite = data.CardSkillImage;
                CardSkillImage.gameObject.SetActive(true);
            }
            else
            {
                CardSkillImage.gameObject.SetActive(false);
            }
        }
    }

    /// <summary>
    /// 체력/공격력만 업데이트할 때 호출합니다.
    /// </summary>
    public void UpdateStatsDisplay(int currentAttack, int currentHealth)
    {
        if (AttackText != null) AttackText.text = currentAttack.ToString();
        if (HealthText != null) HealthText.text = currentHealth.ToString();
    }

    /// <summary>
    /// 현재 연결된 CardData 반환
    /// </summary>
    public CardData GetCardData()
    {
        return _runtimeCardData != null ? _runtimeCardData : cardData;
    }
}
