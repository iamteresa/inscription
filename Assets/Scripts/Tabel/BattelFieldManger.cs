using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic; // List�� ����ϱ� ���� �߰�

public class BattlefieldManager : MonoBehaviour
{
    [Header("---------- ī�� ��ġ ��ġ ---------")]
    [SerializeField] private List<Transform> spawnPoints = new List<Transform>();

    [Header("---------- ��ġ ���� �޽��� UI ---------")]
    [SerializeField] private TextMeshProUGUI placementInfoText;

    [Header("---------- ��ġ�� ī���� �Ӽ� ---------")]
    [SerializeField] private Vector3 placedCardScale = new Vector3(1f, 1f, 1f); // ��ġ�� ī���� ũ�� (�ν����Ϳ��� ����)
    [SerializeField] private Vector3 cardLocalOffset = Vector3.zero; // ��ġ�� ī���� ���� ������ (��������Ʈ ����)

 
    [Header("---------- �ٸ� �Ŵ��� ���� ---------")]
    [SerializeField] private PlayerCostManager playerCostManager;
    [SerializeField] private HandManager handManager;

    private GameObject[] occupiedSpawnPoints;

    // ���� ��ġ ��� ���� ī��
    private GameObject cardWaitingForPlacement = null;

    void Awake()
    {
        if (spawnPoints.Count > 0)
        {
            occupiedSpawnPoints = new GameObject[spawnPoints.Count];
        }
        else
        {
            Debug.LogError("spawnPoints�� �������� �ʾҽ��ϴ�.", this);
            enabled = false;
            return;
        }

        if (playerCostManager == null)
        {
            playerCostManager = FindObjectOfType<PlayerCostManager>();
            if (playerCostManager == null)
            {
                Debug.LogError("PlayerCostManager�� ������� �ʾҰų� ������ ã�� �� �����ϴ�.", this);
            }
        }
        if (handManager == null)
        {
            handManager = FindObjectOfType<HandManager>();
            if (handManager == null)
            {
                Debug.LogWarning("HandManager�� ������� �ʾҰų� ������ ã�� �� �����ϴ�.", this);
            }   
        }

        if (placementInfoText == null)
        {
            Debug.LogError("Placement Info Text�� ������� �ʾҽ��ϴ�.", this);
        }

        HidePlacementInfo();

        // �� spawnPoints GameObject�� BattlefieldSlot ��ũ��Ʈ�� �����մϴ�.
        // �ݶ��̴��� ���� �� �޼��忡�� �ڵ����� �߰����� �ʽ��ϴ�. �������� �߰��ؾ� �մϴ�.
        SetupBattlefieldSlots();
    }

    /// <summary>
    /// �� ���� ����Ʈ GameObject�� BattlefieldSlot ��ũ��Ʈ�� �����մϴ�.
    /// �ݶ��̴��� ���� �������� �߰��ؾ� �մϴ�.
    /// </summary>
    private void SetupBattlefieldSlots()
    {
        for (int i = 0; i < spawnPoints.Count; i++)
        {
            Transform slotTransform = spawnPoints[i];
            // NULL ���� ���� ������ ���� �κ� (��������Ʈ ����Ʈ�� ������� ��)
            if (slotTransform == null)
            {
                Debug.LogWarning($"SpawnPoints ����Ʈ�� {i}��° ������ ����ֽ��ϴ�." +
                    $" �ش� ������ �ǳʶݴϴ�. �ν����Ϳ��� Transform�� �������ּ���.", this);
                occupiedSpawnPoints[i] = null;
                continue;
            }

            GameObject slotGO = slotTransform.gameObject;
            BattlefieldSlot slot = slotGO.GetComponent<BattlefieldSlot>();
            if (slot == null)
            {
                slot = slotGO.AddComponent<BattlefieldSlot>();
            }
            slot.slotIndex = i; // ���� �ε��� �Ҵ�

            
        }
    }


    /// <summary>
    /// ī�� ��ġ ���� ������ UI�� ǥ���մϴ�.
    /// </summary>
    /// <param name="message">ǥ���� �޽���</param>
    public void ShowPlacementInfo(string message)
    {
        if (placementInfoText != null)
        {
            placementInfoText.text = message;
            placementInfoText.gameObject.SetActive(true);
            StopAllCoroutines();
            StartCoroutine(HidePlacementInfoAfterDelay(3f)); // 3�� �� �������
        }
    }

    /// <summary>
    /// ī�� ��ġ ���� ������ UI���� ����ϴ�.
    /// </summary>
    public void HidePlacementInfo()
    {
        if (placementInfoText != null)
        {
            placementInfoText.gameObject.SetActive(false);
        }
    }

    private IEnumerator HidePlacementInfoAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        HidePlacementInfo();
    }

    /// <summary>
    /// HandManager�κ��� ī�尡 ���õǾ� ��ġ ��� �������� �˸��ϴ�.
    /// </summary>
    /// <param name="cardGO">��ġ ��� ���� ī�� GameObject</param>
    public void SetCardWaitingForPlacement(GameObject cardGO)
    {
        cardWaitingForPlacement = cardGO;
        ShowPlacementInfo($"'{cardGO.name}' ī�� ��ġ ��� ��. ���� ������ Ŭ���ϼ���.");
        Debug.Log($"BattlefieldManager: '{cardGO.name}' ī�� ��ġ ��� ��. ���� Ŭ���� ��ٸ��ϴ�.");
    }

    /// <summary>
    /// BattlefieldSlot���κ��� ���� Ŭ�� �˸��� �޽��ϴ�.
    /// �� �޼��忡�� Cost Ȯ�� �� ī�� ��ġ ������ �����մϴ�.
    /// </summary>
    /// <param name="clickedSlotIndex">Ŭ���� ������ �ε���</param>
    public void OnSlotClicked(int clickedSlotIndex)
    {
        if (cardWaitingForPlacement == null)
        {
            ShowPlacementInfo("���� ���п��� ī�带 �����ϼ���!");
            Debug.Log("BattlefieldManager: ��ġ�� ī�尡 ���õ��� �ʾҽ��ϴ�.");
            return;
        }

        if (clickedSlotIndex < 0 || clickedSlotIndex >= spawnPoints.Count || spawnPoints[clickedSlotIndex] == null)
        {
            Debug.LogError($"BattlefieldManager: ��ȿ���� �ʰų� ����ִ� ���� �ε���: {clickedSlotIndex}", this);
            ShowPlacementInfo("��ȿ���� ���� �����Դϴ�.");
            ClearCardWaitingForPlacement();
            return;
        }

        if (occupiedSpawnPoints[clickedSlotIndex] != null)
        {
            ShowPlacementInfo("�̹� �ٸ� ī�尡 �ֽ��ϴ�!");
            Debug.Log($"BattlefieldManager: ���� {clickedSlotIndex}�� �̹� ���� ���Դϴ�.");
            return;
        }

        CardDisplay cardDisplay = cardWaitingForPlacement.GetComponent<CardDisplay>();
        if (cardDisplay == null || cardDisplay.cardData == null)
        {
            Debug.LogError("��ġ�Ϸ��� ī�忡 CardDisplay �Ǵ� CardData�� �����ϴ�.", cardWaitingForPlacement);
            ClearCardWaitingForPlacement();
            return;
        }

        int cardCost = cardDisplay.cardData.Cost;

        if (playerCostManager == null)
        {
            Debug.LogError("PlayerCostManager�� �ʱ�ȭ���� �ʾҽ��ϴ�. ī�带 ��ġ�� �� �����ϴ�.", this);
            ShowPlacementInfo("�ڽ�Ʈ ������ ����.");
            ClearCardWaitingForPlacement();
            return;
        }

        if (playerCostManager.CurrentCost < cardCost)
        {
            ShowPlacementInfo($"�ڽ�Ʈ ����! ({cardCost - playerCostManager.CurrentCost} �� �ʿ�)");
            Debug.Log($"�ڽ�Ʈ ����: {cardWaitingForPlacement.name} (�ʿ� �ڽ�Ʈ: {cardCost}, ���� �ڽ�Ʈ: {playerCostManager.CurrentCost})");
            return;
        }

        bool costRemoved = playerCostManager.RemoveCost(cardCost);
        if (!costRemoved)
        {
            Debug.LogError("�ڽ�Ʈ �Ҹ� �� ����ġ ���� ���� �߻�.", this);
            ShowPlacementInfo("�ڽ�Ʈ �Ҹ� ����!");
            return;
        }

        GameObject card = cardWaitingForPlacement;
        card.transform.SetParent(spawnPoints[clickedSlotIndex]);
        card.transform.localPosition = cardLocalOffset; // <--- ����: �ø��������� ������ ���
        card.transform.localRotation = Quaternion.identity;
        card.transform.localScale = placedCardScale; // <--- ����: �ø��������� ������ ���

        occupiedSpawnPoints[clickedSlotIndex] = card;

        ShowPlacementInfo($"{card.name}��(��) {clickedSlotIndex}�� ���Կ� ��ġ�߽��ϴ�! (-{cardCost} �ڽ�Ʈ)");
        Debug.Log($"{card.name} ī�尡 {spawnPoints[clickedSlotIndex].name}�� ��ġ�Ǿ����ϴ�. ���� �ڽ�Ʈ: {playerCostManager.CurrentCost}");

        if (handManager != null)
        {
            handManager.NotifyCardPlacedSuccessfully(card);
        }

        ClearCardWaitingForPlacement();
    }

    /// <summary>
    /// ��ġ ��� ���¸� �����ϰ� ���� UI�� ����ϴ�.
    /// (��ġ ���� �Ǵ� ����/��� �� ȣ��)
    /// </summary>
    public void ClearCardWaitingForPlacement()
    {
        if (cardWaitingForPlacement != null)
        {
            CardSelector selector = cardWaitingForPlacement.GetComponent<CardSelector>();
            if (selector != null && selector.IsSelected())
            {
                selector.DeselectExternally();
            }
        }
        cardWaitingForPlacement = null;
        HidePlacementInfo();
    }

    /// <summary>
    /// ������ ���� ����Ʈ���� ī�带 �����մϴ�. (��: ī�� �ı� ��)
    /// </summary>
    /// <param name="cardGO">������ ī�� GameObject</param>
    public void RemoveCardFromBattlefield(GameObject cardGO)
    {
        for (int i = 0; i < occupiedSpawnPoints.Length; i++)
        {
            if (occupiedSpawnPoints[i] == cardGO)
            {
                occupiedSpawnPoints[i] = null;
                Debug.Log($"{cardGO.name}�� ���̺��� ���ŵǾ����ϴ�.");
                return;
            }
        }
        Debug.LogWarning($"���̺��� �����Ϸ��� ī�� '{cardGO.name}'�� ã�� �� �����ϴ�.");
    }
}
