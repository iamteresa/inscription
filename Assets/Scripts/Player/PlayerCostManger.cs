using TMPro;
using UnityEngine;
using UnityEngine.UI; // Text ��� �� �ʿ�

public class PlayerCostManager : MonoBehaviour
{
    [Header("----------�ڽ�Ʈ ����------------")]
    [SerializeField] private int maxCost = 10;
    [SerializeField] private int currentCost;

    [Header("-----------UI ���� ------------")]
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
        Debug.Log($"�ڽ�Ʈ ȹ��: +{amount}, ���� �ڽ�Ʈ: {currentCost}/{maxCost}");
        UpdateCostUI();
    }

    public bool RemoveCost(int amount)
    {
        if (currentCost >= amount)
        {
            currentCost -= amount;
            Debug.Log($"�ڽ�Ʈ ���: -{amount}, ���� �ڽ�Ʈ: {currentCost}/{maxCost}");
            UpdateCostUI();
            return true;
        }
        else
        {
            Debug.LogWarning("�ڽ�Ʈ�� �����մϴ�!");
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