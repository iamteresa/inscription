using UnityEngine;
using UnityEngine.UI; 
using TMPro; 
using System.Collections;
using System.Collections.Generic;

public class CardDisplay : MonoBehaviour
{
    // --- 기존 코드의 public 필드 (유지) ---
    // 이 필드들은 인스펙터에서 직접 연결하는 UI 요소들입니다.
    public TMP_Text NameText;
    public TMP_Text SpeciesText;
    public TMP_Text AttackText;
    public TMP_Text HealthText;
    public Image CardImage;
    public Image CardSkillImage; // 기존 CardSkillImage 필드
    public TMP_Text Cost;

    // 이 cardData 필드는 Unity 에디터에서 직접 CardData ScriptableObject를 연결할 때 사용됩니다.
    // 런타임에 CardData를 동적으로 할당하려면 _runtimeCardData를 사용합니다.
    public CardData cardData; // Unity 에디터에서 프리팹에 기본 CardData를 할당할 경우 사용 (옵션)


    // 런타임에 이 디스플레이가 표시할 실제 CardData를 저장하는 private 필드
    private CardData _runtimeCardData;

    void Start()
    {
        //  _runtimeCardData가 우선순위를 가집니다.
        // 만약 Start 시점에 이미 _runtimeCardData가 SetCardDisplay()를 통해 할당되었다면, 그 데이터를 사용합니다.
        // 그렇지 않고 public cardData 필드가 할당되어 있다면 그것을 사용합니다.
        // 둘 다 없다면 경고를 띄웁니다.
        if (_runtimeCardData != null)
        {
            // _runtimeCardData가 이미 설정되었다면, 그것으로 UI 업데이트
            SetCardDisplay(_runtimeCardData);
        }
        else if (cardData != null) // 기존 public cardData가 인스펙터에 연결되어 있다면
        {
            _runtimeCardData = cardData; // _runtimeCardData에 할당하고
            SetCardDisplay(_runtimeCardData); // 그것으로 UI 업데이트
        }
        else
        {
            Debug.LogWarning("CardDisplay: Start 시점에 표시할 CardData가 없습니다. " +
                "SetCardDisplay()가 호출될 때까지 UI는 비어있거나 기본값을 표시합니다.", this);
        }
    }

    /// <summary>
    /// 기존 UpdateDisplay 메서드. public cardData 필드를 사용하여 UI를 업데이트합니다.
    /// </summary>
    public void UpdateDisplay()
    {
        // _runtimeCardData가 할당되어 있다면 그것을 우선 사용합니다.
        CardData dataToUse = (_runtimeCardData != null) ? _runtimeCardData : cardData;

        if (dataToUse == null)
        {
            Debug.LogWarning("CardDisplay: UpdateDisplay 호출 시 cardData가 null입니다. UI 업데이트를 건너뜁니다.", this);
            return;
        }

        // 기존 필드들을 사용하여 UI 업데이트
        if (NameText != null) NameText.text = dataToUse.CardName;
        if (SpeciesText != null) SpeciesText.text = dataToUse.Species.ToString();
        if (AttackText != null) AttackText.text = dataToUse.Attack.ToString();
        if (HealthText != null) HealthText.text = dataToUse.Health.ToString();
        if (Cost != null) Cost.text = dataToUse.Cost.ToString();

        if (CardImage != null) CardImage.sprite = dataToUse.CardImage;

        // CardSkillImage에 대한 로직은 CardData에 해당 필드가 있다면 여기에 추가 로직을 구현해야 합니다.
        // 예: if (CardSkillImage != null && dataToUse.CardSkillImage != null) CardSkillImage.sprite = dataToUse.CardSkillImage;
    }

    // --- 새로 추가되거나 수정된 기능 메서드 ---

    /// <summary>
    /// CardData 전체를 받아 UI를 초기화하는 메서드.
    /// 이 메서드는 카드 오브젝트가 생성된 후 외부(예: HandManager, CardSelector, FieldCard)에서 호출됩니다.
    /// </summary>
    /// <param name="data">표시할 CardData</param>
    public void SetCardDisplay(CardData data)
    {
        if (data == null)
        {
            Debug.LogError("CardDisplay: 설정할 CardData가 null입니다.", this);
            return;
        }

        _runtimeCardData = data; // 런타임에 이 디스플레이가 참조할 CardData 저장

        // 모든 UI 요소 업데이트 (null 체크 필수)
        if (NameText != null) NameText.text = _runtimeCardData.CardName; // 카드 이름 텍스트 설정
        if (SpeciesText != null) SpeciesText.text = _runtimeCardData.Species.ToString(); // 종족 텍스트 설정
        if (Cost != null) Cost.text = _runtimeCardData.Cost.ToString(); // 비용 텍스트 설정
        if (CardImage != null) CardImage.sprite = _runtimeCardData.CardImage; // 카드 아트워크 이미지 설정

        // CardSkillImage 처리 (CardData에 SkillImage 필드가 있다면)
        // if (CardSkillImage != null && _runtimeCardData.CardSkillImage != null) CardSkillImage.sprite = _runtimeCardData.CardSkillImage;
        // else if (CardSkillImage != null) CardSkillImage.gameObject.SetActive(false); // 스킬 이미지가 없으면 비활성화

        // AttackText와 HealthText는 초기값으로 설정
        if (AttackText != null) AttackText.text = _runtimeCardData.Attack.ToString();
        if (HealthText != null) HealthText.text = _runtimeCardData.Health.ToString();
    }

    /// <summary>
    /// 카드의 현재 스탯(공격력, 체력)만 업데이트하는 메서드.
    /// 데미지를 받거나 회복했을 때 FieldCard에서 호출됩니다.
    /// </summary>
    /// <param name="currentAttack">현재 공격력</param>
    /// <param name="currentHealth">현재 체력</param>
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
    /// 현재 이 디스플레이가 표시하는 CardData를 가져옵니다.
    /// CardSelector 등에서 이 카드의 CardData를 참조할 때 사용됩니다.
    /// </summary>
    /// <returns>현재 CardData</returns>
    public CardData GetCardData()
    {
        // 런타임 데이터가 있다면 그것을 반환, 없으면 public으로 연결된 cardData 반환
        return _runtimeCardData != null ? _runtimeCardData : cardData;
    }
}