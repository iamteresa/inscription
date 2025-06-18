using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI; // RectTransform ����� ���� �ʿ�

public class BattlefieldManager : MonoBehaviour
{
    [Header("---------- ī�� ��ġ ��ġ ---------")]
    [SerializeField] private List<Transform> spawnPoints = new List<Transform>();

    [Header("---------- ��ġ ���� �޽��� UI ---------")]
    [SerializeField] private TextMeshProUGUI placementInfoText;

    [Header("---------- �ʵ� ī�� ������ ---------")]
    [SerializeField] private GameObject fieldCardPrefab;

    [Header("---------- �ٸ� �Ŵ��� ���� ---------")]
    [SerializeField] private PlayerCostManager playerCostManager;
    [SerializeField] private HandManager handManager;

    private GameObject[] occupiedSpawnPoints;
    private GameObject cardWaitingForPlacement = null;

    void Awake()
    {
        if (spawnPoints.Count > 0)
            occupiedSpawnPoints = new GameObject[spawnPoints.Count];
        else
        {
            Debug.LogError("spawnPoints�� �������� �ʾҽ��ϴ�.", this);
            enabled = false;
            return;
        }

        if (playerCostManager == null)
            playerCostManager = FindObjectOfType<PlayerCostManager>();
        if (handManager == null)
            handManager = FindObjectOfType<HandManager>();

        if (placementInfoText == null)
            Debug.LogError("Placement Info Text�� ������� �ʾҽ��ϴ�.", this);

        HidePlacementInfo();
        SetupBattlefieldSlots();
    }

    private void SetupBattlefieldSlots()
    {
        for (int i = 0; i < spawnPoints.Count; i++)
        {
            var slotTransform = spawnPoints[i];
            if (slotTransform == null)
            {
                Debug.LogWarning($"SpawnPoints[{i}]�� ����ֽ��ϴ�.");
                occupiedSpawnPoints[i] = null;
                continue;
            }

            var slotGO = slotTransform.gameObject;
            var slot = slotGO.GetComponent<BattlefieldSlot>() ?? slotGO.AddComponent<BattlefieldSlot>();
            slot.SetSlotIndex(i);
        }
    }

    public void ShowPlacementInfo(string message)
    {
        if (placementInfoText == null) return;
        placementInfoText.text = message;
        placementInfoText.gameObject.SetActive(true);
        StopAllCoroutines();
        StartCoroutine(HidePlacementInfoAfterDelay(3f));
    }

    private IEnumerator HidePlacementInfoAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        HidePlacementInfo();
    }

    public void HidePlacementInfo()
    {
        if (placementInfoText != null)
            placementInfoText.gameObject.SetActive(false);
    }

    public void SetCardWaitingForPlacement(GameObject cardGO)
    {
        cardWaitingForPlacement = cardGO;
        ShowPlacementInfo($"'{cardGO.name}' ī�� ��ġ ��� ��. ������ Ŭ���ϼ���.");
    }

    public void OnSlotClicked(int clickedSlotIndex)
    {
        if (cardWaitingForPlacement == null)
        {
            ShowPlacementInfo("���� ���п��� ī�带 �����ϼ���!");
            return;
        }

        if (clickedSlotIndex < 0 || clickedSlotIndex >= spawnPoints.Count || spawnPoints[clickedSlotIndex] == null)
        {
            ShowPlacementInfo("��ȿ���� ���� �����Դϴ�.");
            ClearCardWaitingForPlacement();
            return;
        }

        if (occupiedSpawnPoints[clickedSlotIndex] != null)
        {
            ShowPlacementInfo("�̹� �ٸ� ī�尡 �ֽ��ϴ�!");
            return;
        }

        var originalDisplay = cardWaitingForPlacement.GetComponent<CardDisplay>();
        if (originalDisplay == null || originalDisplay.cardData == null)
        {
            Debug.LogError("CardDisplay �Ǵ� CardData ����", cardWaitingForPlacement);
            ClearCardWaitingForPlacement();
            return;
        }

        int cost = originalDisplay.cardData.Cost;
        if (playerCostManager == null)
        {
            ShowPlacementInfo("�ڽ�Ʈ ������ ����.");
            ClearCardWaitingForPlacement();
            return;
        }

        if (playerCostManager.CurrentCost < cost)
        {
            ShowPlacementInfo($"�ڽ�Ʈ ����! ({cost - playerCostManager.CurrentCost} �ʿ�)");
            return;
        }

        if (!playerCostManager.RemoveCost(cost))
        {
            ShowPlacementInfo("�ڽ�Ʈ �Ҹ� ����!");
            return;
        }

        // 1) �ʵ� ī�� ������ �ν��Ͻ�ȭ
        var fieldCard = Instantiate(fieldCardPrefab, spawnPoints[clickedSlotIndex], false);

        // 2) ���� ī���� cardData ���� �� ���÷��� ������Ʈ
        var newDisplay = fieldCard.GetComponent<CardDisplay>();
        if (newDisplay != null)
        {
            newDisplay.cardData = originalDisplay.cardData;
            newDisplay.UpdateDisplay();
        }

        // 3) ���Ժ� Transform ����
        var fieldRect = fieldCard.GetComponent<RectTransform>();
        var slotComp = spawnPoints[clickedSlotIndex].GetComponent<BattlefieldSlot>();
        if (slotComp != null && fieldRect != null)
            slotComp.ApplyPlacementTransform(fieldRect);

        occupiedSpawnPoints[clickedSlotIndex] = fieldCard;

        // 4) ���� ���� ī�� ���� �� HandManager �˸�
        if (handManager != null)
            handManager.NotifyCardPlacedSuccessfully(cardWaitingForPlacement);
        Destroy(cardWaitingForPlacement);

        ShowPlacementInfo($"{originalDisplay.cardData.CardName} ��ġ �Ϸ�! (-{cost} �ڽ�Ʈ)");
        ClearCardWaitingForPlacement();
    }

    public void ClearCardWaitingForPlacement()
    {
        if (cardWaitingForPlacement != null)
        {
            var selector = cardWaitingForPlacement.GetComponent<CardSelector>();
            if (selector != null && selector.IsSelected())
                selector.DeselectExternally();
        }
        cardWaitingForPlacement = null;
        HidePlacementInfo();
    }

    public void RemoveCardFromBattlefield(GameObject cardGO)
    {
        for (int i = 0; i < occupiedSpawnPoints.Length; i++)
        {
            if (occupiedSpawnPoints[i] == cardGO)
            {
                occupiedSpawnPoints[i] = null;
                Debug.Log($"{cardGO.name} ���ŵ�.");
                return;
            }
        }
        Debug.LogWarning($"{cardGO.name}��(��) ã�� �� �����ϴ�.");
    }
}
