using TMPro;
using UnityEngine;
using UnityEngine.UI; // Text 사용 시 필요

public class PlayerCostManager : MonoBehaviour
{
    [Header("----------코스트 설정------------")]
    [SerializeField] private int maxCost = 10;
    [SerializeField] private int currentCost;

    [Header("-----------UI 연결 ------------")]
    [SerializeField] private TMP_Text costText;

    public int CurrentCost => currentCost;
    public int MaxCost => maxCost;

    private void Start()
    {
        UpdateCostUI();
    }

    public void GainCost(int amount)
    {
        currentCost += amount;
        currentCost = Mathf.Min(currentCost, maxCost);
        Debug.Log($"코스트 획득: +{amount}, 현재 코스트: {currentCost}/{maxCost}");
        UpdateCostUI();
    }

    public bool RemoveCost(int amount)
    {
        if (currentCost >= amount)
        {
            currentCost -= amount;
            Debug.Log($"코스트 사용: -{amount}, 현재 코스트: {currentCost}/{maxCost}");
            UpdateCostUI();
            return true;
        }
        else
        {
            Debug.LogWarning("코스트가 부족합니다!");
            return false;
        }
    }

    public void ResetCost()
    {
        currentCost = maxCost;
        UpdateCostUI();
    }
    
    private void UpdateCostUI()
    {
        if (costText != null)
        {
            costText.text = $"[ {currentCost} / {maxCost} ]";
        }
    }
}