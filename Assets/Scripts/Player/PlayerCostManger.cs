using UnityEngine;
using TMPro;
using System;

public class PlayerCostManager : MonoBehaviour
{
    [Header("----------코스트 설정------------")]
    [SerializeField] private int maxCost = 10;
    private int currentCost;

    [Header("-----------UI 연결 ------------")]
    [SerializeField] private TMP_Text costText;

    public int CurrentCost => currentCost;
    public int MaxCost => maxCost;

    private void OnEnable()
    {
        // 카드가 죽으면 한 번만 호출될 구독
        CardEventBus.OnDeath += HandleCardDeath;
    }

    private void OnDisable()
    {
        CardEventBus.OnDeath -= HandleCardDeath;
    }

    private void Start()
    {
        // 시작할 때 코스트 초기화
        currentCost = maxCost;
        UpdateCostUI();
    }

    /// <summary>
    /// 카드가 사망할 때마다 호출됩니다.
    /// Savage 종족이면 +2, 그 외엔 +1 코스트를 얻습니다.
    /// </summary>
    private void HandleCardDeath(FieldCard deadCard, CardData data)
    {
        //Savage 종족일시 코스트 +2 , 아닐시 +1 을 담당합니다.
        int gain = (data.Species == CardData.CardSpecies.Savage) ? 2 : 1;
        GainCost(gain);
        Debug.Log($"[HandleCardDeath] {data.CardName} ({data.Species}) 사망: +{gain} 코스트");
    }

    /// <summary>
    /// 코스트 얻는것을 담당합니다.
    /// </summary>
    /// <param name="amount"></param>
    public void GainCost(int amount)
    {
        currentCost = Mathf.Min(currentCost + amount, maxCost);
        Debug.Log($"코스트 획득: +{amount}, 현재 코스트: {currentCost}/{maxCost}");
        UpdateCostUI();
    }

    /// <summary>
    /// 코스트 없에는것을 담당합니다.
    /// </summary>
    /// <param name="amount"></param>
    /// <returns></returns>
    public bool RemoveCost(int amount)
    {
        if (currentCost >= amount)
        {
            currentCost -= amount;
            UpdateCostUI();
            return true;
        }
        Debug.LogWarning("코스트가 부족합니다!");
        return false;
    }

    /// <summary>
    /// 코스트를 10(최댓값)으로 초기화합니다.
    /// </summary>
    public void ResetCost()
    {
        currentCost = maxCost;
        UpdateCostUI();
    }

    /// <summary>
    /// 코스트 Ui를 업데이트 합니다.
    /// </summary>
    private void UpdateCostUI()
    {
        if (costText != null)
            costText.text = $"[ {currentCost} / {maxCost} ]";
    }
}
