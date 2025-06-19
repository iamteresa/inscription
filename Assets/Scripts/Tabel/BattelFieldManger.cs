using UnityEngine;
using TMPro; // TextMeshProUGUI (UI �ؽ�Ʈ) ����� ���� �ʿ�
using System.Collections; // IEnumerator (�ڷ�ƾ) ����� ���� �ʿ�
using System.Collections.Generic; // List (���) ����� ���� �ʿ�
using UnityEngine.UI; // RectTransform (UI ����� ũ�� �� ��ġ ����) ����� ���� �ʿ�

public class BattlefieldManager : MonoBehaviour
{
    [Header("---------- ī�� ��ġ ��ġ ---------")]
    [SerializeField] private List<Transform> spawnPoints = new List<Transform>();
    public List<Transform> SpawnPoints => spawnPoints;

    [Header("---------- ��ġ ���� �޽��� UI ---------")]
    [SerializeField] private TextMeshProUGUI placementInfoText;

    [Header("---------- �ʵ� ī�� ������ ---------")]
    [SerializeField] private GameObject fieldCardPrefab;

    [Header("---------- �ٸ� �Ŵ��� ���� ---------")]
    [SerializeField] private PlayerCostManager playerCostManager;
    [SerializeField] private HandManager handManager;

 
    // 'spawnPoints' ����Ʈ�� �� �ε����� �ش��ϴ� ���Կ� � ī�� GameObject�� ���� ��ġ�Ǿ� �ִ���
    // (�����ϰ� �ִ���) �����ϴ� �迭�Դϴ�.
    private GameObject[] occupiedSpawnPoints;

    // �÷��̾ ���п��� �ʵ忡 �������� ���� ���� ������(Ŭ����) ī�� GameObject�� �ӽ÷� �����մϴ�.
    private GameObject cardWaitingForPlacement = null;

    // MonoBehaviour�� ���� �ֱ� �޼���: ��ũ��Ʈ �ν��Ͻ��� �ε�� �� ȣ��˴ϴ�.
    void Awake()
    {
        // 1. 'spawnPoints' ����Ʈ�� ��ȿ�� �˻� �� 'occupiedSpawnPoints' �迭 �ʱ�ȭ
        if (spawnPoints.Count > 0)
            // 'spawnPoints'�� ������ŭ 'occupiedSpawnPoints' �迭�� �����մϴ�.
            occupiedSpawnPoints = new GameObject[spawnPoints.Count];
        else
        {
            // 'spawnPoints'�� �������� �ʾҴٸ� �ɰ��� ����, �� ��ũ��Ʈ�� ��Ȱ��ȭ�մϴ�.
            Debug.LogError("spawnPoints�� �������� �ʾҽ��ϴ�. �ʵ� ���� Transform�� �ν����Ϳ� �������ּ���.", this);
            enabled = false; // �ش� ��ũ��Ʈ ������Ʈ�� ��Ȱ��ȭ�մϴ�.
            return; 
        }

        // 2. �ٸ� �Ŵ��� ���� ���� (�ν����Ϳ� ������� ���� ���)
        // 'FindObjectOfType'�� �� ��ü�� Ž���ϹǷ� ���ɿ� ������ �� �� �ֽ��ϴ�.
        // �����ϸ� Unity �������� �ν����Ϳ��� ���� �����ϴ� ���� �� ȿ�����Դϴ�.
        if (playerCostManager == null)
            playerCostManager = FindObjectOfType<PlayerCostManager>();
        if (handManager == null)
            handManager = FindObjectOfType<HandManager>();

        // 3. 'placementInfoText' UI ��� ���� Ȯ��
        if (placementInfoText == null)
            Debug.LogError("Placement Info Text�� ������� �ʾҽ��ϴ�. UI Canvas�� TextMeshProUGUI ��Ҹ� �������ּ���.", this);

        // 4. �ʱ� ���� ����
        // ���� ���� �� ��ġ ���� UI�� ����ϴ�.
        HidePlacementInfo();
        // �ʵ� ���Ե��� �����մϴ� (BattlefieldSlot ������Ʈ �߰� �� �ε��� ����).
        SetupBattlefieldSlots();


    }

    /// <summary>
    /// �ʵ��� �� 'spawnPoint' GameObject�� 'BattlefieldSlot' ������Ʈ�� �߰��ϰų� �����ͼ�
    /// �ش� ������ ���� �ε����� �����մϴ�. �� 'BattlefieldSlot' ������Ʈ�� �ش� ������ Ŭ���Ǿ��� ��
    /// 'BattlefieldManager'�� �̺�Ʈ�� �����ϴ� ������ �մϴ�.
    /// </summary>
    private void SetupBattlefieldSlots()
    {
        for (int i = 0; i < spawnPoints.Count; i++)
        {
            var slotTransform = spawnPoints[i]; // ���� �ݺ� ���� ������ Transform ��������
            if (slotTransform == null)
            {
                // �ش� 'spawnPoint'�� �ν����Ϳ��� ������� ��� ��� �α��ϰ� �ǳʶݴϴ�.
                Debug.LogWarning($"SpawnPoints[{i}]�� �ν����Ϳ��� ����ֽ��ϴ�. �ش� ������ ������ �ʽ��ϴ�.");
                occupiedSpawnPoints[i] = null; // �ش� ������ �������� ���� ������ �ʱ� ���¸� ǥ���մϴ�.
                continue; // ���� �ݺ����� �Ѿ�ϴ�.
            }

            var slotGO = slotTransform.gameObject; // Transform���� GameObject ��������
            // 'slotGO'�� 'BattlefieldSlot' ������Ʈ�� �̹� �ִ��� Ȯ���ϰ�, ������ ���� �߰��մϴ�.
            var slot = slotGO.GetComponent<BattlefieldSlot>() ?? slotGO.AddComponent<BattlefieldSlot>();
            // 'BattlefieldSlot' ������Ʈ�� ���� ������ �迭 �ε����� �����մϴ�.
            slot.SetSlotIndex(i);
        }
    }

    /// <summary>
    /// ���� ȭ���� 'placementInfoText' UI�� �޽����� ǥ���ϰ�,
    /// ������ �ð�(3��) �Ŀ� �ڵ����� ���������� �ڷ�ƾ�� �����մϴ�.
    /// </summary>
    /// <param name="message">UI�� ǥ���� ���ڿ� �޽����Դϴ�.</param>
    public void ShowPlacementInfo(string message)
    {
        if (placementInfoText == null) return; // UI �ؽ�Ʈ ������Ʈ�� ����Ǿ� ���� ������ �ƹ��͵� ���� �ʽ��ϴ�.
        placementInfoText.text = message; // UI �ؽ�Ʈ�� ������ ���޹��� �޽����� �����մϴ�.
        placementInfoText.gameObject.SetActive(true); // UI �ؽ�Ʈ ���� ������Ʈ�� Ȱ��ȭ�Ͽ� ȭ�鿡 ���̰� �մϴ�.
        StopAllCoroutines(); // �� ��ũ��Ʈ���� ���� ���� ���� ��� �ڷ�ƾ(������ 'HidePlacementInfoAfterDelay')�� �����մϴ�.
        StartCoroutine(HidePlacementInfoAfterDelay(3f)); // 3�� �Ŀ� UI�� ����� �ڷ�ƾ�� ���� �����մϴ�.
    }

    /// <summary>
    /// ������ 'delay' �ð���ŭ ����� ��, 'placementInfoText' UI�� ����� �ڷ�ƾ�Դϴ�.
    /// </summary>
    /// <param name="delay">UI�� ����� ������ ����� �ð�(��)�Դϴ�.</param>
    private IEnumerator HidePlacementInfoAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay); // 'delay' �ð���ŭ ������ �Ͻ� �����մϴ�.
        HidePlacementInfo(); // ��� �� UI�� ����� �޼��带 ȣ���մϴ�.
    }

    /// <summary>
    /// 'placementInfoText' UI�� ��� ����ϴ�.
    /// </summary>
    public void HidePlacementInfo()
    {
        if (placementInfoText != null)
            placementInfoText.gameObject.SetActive(false); // UI �ؽ�Ʈ ���� ������Ʈ�� ��Ȱ��ȭ�Ͽ� ȭ�鿡�� ����ϴ�.
    }

    /// <summary>
    /// �÷��̾ ���п��� �ʵ忡 �������� ���� ������ ī�� GameObject�� �����մϴ�.
    /// �� �޼���� �Ϲ������� ���п� �ִ� ī�尡 Ŭ���Ǿ��� �� 'CardSelector' �Ǵ� 'HandManager'���� ȣ��˴ϴ�.
    /// </summary>
    /// <param name="cardGO">�ʵ忡 ��ġ�� (������) ī�� GameObject�Դϴ�.</param>
    public void SetCardWaitingForPlacement(GameObject cardGO)
    {
        cardWaitingForPlacement = cardGO; // ���޹��� ī�带 ��ġ ��� ���� ī��� �����մϴ�.
        // ����ڿ��� ���� ī�带 �ʵ� ���Կ� ��ġ�ؾ� ���� �˸��� �޽����� UI�� ǥ���մϴ�.
        ShowPlacementInfo($"'{cardGO.name}' ī�� ��ġ ��� ��. ������ Ŭ���ϼ���.");
    }

    /// <summary>
    /// �ʵ��� ������ Ŭ���Ǿ��� �� 'BattlefieldSlot' ������Ʈ�κ��� ȣ��Ǵ� �ֿ� �޼����Դϴ�.
    /// �� �޼��� ������ ī�� ��ġ�� �ʿ��� ��� ��ȿ�� �˻� �� ���� ��ġ ������ ����˴ϴ�.
    /// </summary>
    /// <param name="clickedSlotIndex">Ŭ���� �ʵ� ������ ���� �ε����Դϴ�.</param>
    public void OnSlotClicked(int clickedSlotIndex)
    {
        // 1. ��ġ ��� ���� ī�尡 �ִ��� Ȯ���մϴ�.
        if (cardWaitingForPlacement == null)
        {
            ShowPlacementInfo("���� ���п��� ī�带 �����ϼ���!"); // ī�带 �������� �ʾҴٸ� �޽��� ǥ��
            return; // ���� ������ �������� �ʰ� �Լ��� �����մϴ�.
        }

        // 2. Ŭ���� ���� �ε����� ��ȿ���� �˻��մϴ�.
        // �ε����� ��ȿ�� ���� ���� �ְ�, �ش� 'spawnPoint'�� ������ �����ϴ��� Ȯ���մϴ�.
        if (clickedSlotIndex < 0 || clickedSlotIndex >= spawnPoints.Count || spawnPoints[clickedSlotIndex] == null)
        {
            ShowPlacementInfo("��ȿ���� ���� �����Դϴ�."); // �߸��� ���� Ŭ�� �� �޽��� ǥ��
            ClearCardWaitingForPlacement(); // ��ġ ��� ���¸� �����մϴ�.
            return;
        }

        // 3. �ش� ������ �̹� �ٸ� ī��� �����Ǿ� �ִ��� Ȯ���մϴ�.
        if (occupiedSpawnPoints[clickedSlotIndex] != null)
        {
            ShowPlacementInfo("�̹� �ٸ� ī�尡 �ֽ��ϴ�!"); // ������ �̹� ���ִٸ� �޽��� ǥ��
            return;
        }

        // 4. ��ġ ��� ���� ī��κ��� 'CardDisplay' ������Ʈ�� 'CardData'�� �����ɴϴ�.
        // ���� ī��� 'CardDisplay' ������Ʈ�� ������ �ְ�, �� �ȿ� 'CardData'�� ����Ǿ� �ִٰ� �����մϴ�.
        var originalDisplay = cardWaitingForPlacement.GetComponent<CardDisplay>();
        if (originalDisplay == null || originalDisplay.GetCardData() == null) // 'GetCardData()' �޼��带 ����Ͽ� 'CardData'�� �����ɴϴ�.
        {
            Debug.LogError("CardDisplay �Ǵ� CardData�� ���� ī�忡 �����ϴ�. ���� ī���� ���� �� ������ �Ҵ��� Ȯ���ϼ���.", cardWaitingForPlacement);
            ClearCardWaitingForPlacement(); // ���� �߻� �� ��ġ ��� ���� ����
            return;
        }
        CardData cardDataToPlay = originalDisplay.GetCardData(); // ���� �ʵ忡 ��ȯ�� ī���� �����͸� �����ɴϴ�.

        // 5. ī�� ��ȯ�� �ʿ��� �ڽ�Ʈ(�ڿ�)�� Ȯ���մϴ�.
        int cost = cardDataToPlay.Cost; // 'CardData'�� ���ǵ� ī���� ����� �����ɴϴ�.
        if (playerCostManager == null)
        {
            Debug.LogError("PlayerCostManager�� ������� �ʾҽ��ϴ�. �ڽ�Ʈ �����ڸ� �������ּ���.");
            ShowPlacementInfo("�ڽ�Ʈ ������ ����. ������ �ʿ��մϴ�.");
            ClearCardWaitingForPlacement();
            return;
        }

        if (playerCostManager.CurrentCost < cost) // �÷��̾��� ���� �ڽ�Ʈ�� ī�� ��뺸�� ������ Ȯ��
        {
            ShowPlacementInfo($"�ڽ�Ʈ ����! ({cost - playerCostManager.CurrentCost} �ʿ�)"); // �ڽ�Ʈ ���� �޽��� ǥ��
            return;
        }

        // 6. �÷��̾��� �ڽ�Ʈ�� �Ҹ��մϴ�.
        if (!playerCostManager.RemoveCost(cost)) // 'PlayerCostManager'�� ���� �ڽ�Ʈ�� �Ҹ� �õ�
        {
            ShowPlacementInfo("�ڽ�Ʈ �Ҹ� �� ������ �߻��߽��ϴ�!"); // �ڽ�Ʈ �Ҹ� ���� �� �޽��� ǥ��
            return;
        }

        // --- ������ʹ� ī�� ��ġ�� �������� ���� �ٽ� ���� �����Դϴ�. ---

        // 1) 'fieldCardPrefab'�� �ν��Ͻ�ȭ�Ͽ� �ʵ忡 ���ο� ī�� ������Ʈ�� �����մϴ�.
        // 'spawnPoints[clickedSlotIndex]'�� �θ� Transform���� �����Ͽ� �ش� ������ ��ġ�� �����մϴ�.
        // 'false'�� Instantiate �� ���� ���������� ȸ���� ũ�⸦ �������� �ʰ�, �θ� ��������� �������� �ǹ��մϴ� (UI ��ҿ� ����).
        var newFieldCardGO = Instantiate(fieldCardPrefab, spawnPoints[clickedSlotIndex], false);

        // 2) ���� ������ �ʵ� ī���� 'FieldCard' ������Ʈ�� ������ �ʱ�ȭ�մϴ�.
        // 'fieldCardPrefab'���� 'FieldCard.cs' ��ũ��Ʈ�� �̸� �����Ǿ� �־�� �մϴ�.
        FieldCard newFieldCard = newFieldCardGO.GetComponent<FieldCard>();
        if (newFieldCard != null)
        {
            // 'FieldCard'�� 'SetCardData' �޼��带 ȣ���Ͽ�, ���� ī��κ��� ������ 'CardData'�� �����մϴ�.
            // �� ī��('newFieldCard')�� ������ �÷��̾� �������� �����մϴ�.
            newFieldCard.Initialize(cardDataToPlay, FieldCard.CardFaction.Player);
            // ī�尡 �ʵ忡 ��ȯ�Ǿ��� �� �ߵ��ϴ� �ɷ�(��: ����)�� ó���ϴ� �޼��带 ȣ���մϴ�.

        }
        else
        {
            // ���� 'fieldCardPrefab'�� 'FieldCard' ������Ʈ�� ���ٸ� �ɰ��� ������ �α��մϴ�.
            Debug.LogError($"BattlefieldManager: ������ �ʵ� ī�� '{newFieldCardGO.name}'�� FieldCard ������Ʈ�� �����ϴ�! 'fieldCardPrefab' ������ Ȯ�����ּ���.", newFieldCardGO);
            Destroy(newFieldCardGO); // �߸� ������ ī�� ������Ʈ�� ��� �ı��մϴ�.
            ClearCardWaitingForPlacement(); // ��ġ ��� ���¸� �����մϴ�.
            return;
        }

        // 3) ���Ժ� Transform ���� (���� 'BattlefieldSlot'�� Ŀ���� ��ġ ������ �ִٸ�)
        // 'newFieldCardGO'�� UI ���(Canvas�� �ڽ�)��� 'RectTransform'�� �ʿ��մϴ�.
        var fieldRect = newFieldCardGO.GetComponent<RectTransform>();
        var slotComp = spawnPoints[clickedSlotIndex].GetComponent<BattlefieldSlot>();
        if (slotComp != null && fieldRect != null)
            // 'BattlefieldSlot'�� ���ǵ� 'ApplyPlacementTransform' �޼��带 ȣ���Ͽ�
            // ī���� ��ġ�� ũ�⸦ ���Կ� �°� �����մϴ�.
            slotComp.ApplyPlacementTransform(fieldRect);

        // 4) 'occupiedSpawnPoints' �迭�� ���� ��ġ�� �ʵ� ī�� GameObject�� ����Ͽ� ������ �����Ǿ����� ǥ���մϴ�.
        occupiedSpawnPoints[clickedSlotIndex] = newFieldCardGO;

        // 5) ���п� �ִ� ���� ī�� ���� �� 'HandManager'�� ��ġ ���� �˸�
        // 'handManager'�� ��ȿ���� Ȯ���մϴ�.
        if (handManager != null)
            // 'HandManager'�� 'NotifyCardPlacedSuccessfully' �޼��带 ȣ���Ͽ�,
            // ���п��� �ش� ī�� GameObject�� �����ϰ� ���и� �������ϵ��� �˸��ϴ�.
            handManager.NotifyCardPlacedSuccessfully(cardWaitingForPlacement);

        // �ʵ忡 ���ο� ī�带 ��ġ�����Ƿ�, ���п� �ִ� ���� ī�� GameObject�� �ı��մϴ�.
        Destroy(cardWaitingForPlacement);

        // 6. ��ġ �Ϸ� �޽��� ǥ�� �� ���� �ʱ�ȭ
        ShowPlacementInfo($"{cardDataToPlay.CardName} ��ġ �Ϸ�! (-{cost} �ڽ�Ʈ)");
        ClearCardWaitingForPlacement(); // ī�� ��ġ ��� ���¸� �����ϰ� UI�� ����ϴ�.
    }

    /// <summary>
    /// 'cardWaitingForPlacement' ������ �ʱ�ȭ(null�� ����)�ϰ�,
    /// ���� UI(��ġ ���� �ؽ�Ʈ)�� ����� �޼����Դϴ�.
    /// ����, ������ ���õǾ��� ī��(���� ī��)�� ���� ���¸� �����մϴ�.
    /// </summary>
    public void ClearCardWaitingForPlacement()
    {
        // ��ġ ��� ���� ī�尡 �ִٸ�, �ش� ī���� 'CardSelector' ������Ʈ�� ã�� ���� ���¸� �����մϴ�.
        if (cardWaitingForPlacement != null)
        {
            var selector = cardWaitingForPlacement.GetComponent<CardSelector>();
            if (selector != null && selector.IsSelected())
                selector.DeselectExternally(); // 'CardSelector'�� �ܺο��� ���� �����ǵ��� ��û�մϴ�.
        }
        cardWaitingForPlacement = null; // 'cardWaitingForPlacement' ������ null�� �����Ͽ� ������ �����մϴ�.
        HidePlacementInfo(); // ��ġ ���� UI�� ����ϴ�.
    }

    /// <summary>
    /// �ʵ忡�� Ư�� ī�� GameObject�� �����ϰ� �ش� ������ ���� ���¸� �����մϴ�.
    /// �� �޼���� ���� �ʵ� ī�尡 ������� �� 'FieldCard.Die()' �޼��忡��
    /// 'BattlefieldManager'�� ȣ���Ͽ� ���� �� �ֽ��ϴ�.
    /// </summary>
    /// <param name="cardGO">�ʵ忡�� ������ ī�� GameObject�Դϴ�.</param>
    public void RemoveCardFromBattlefield(GameObject cardGO)
    {
        for (int i = 0; i < occupiedSpawnPoints.Length; i++)
        {
            if (occupiedSpawnPoints[i] == cardGO) // 'occupiedSpawnPoints' �迭���� �ش� ī�带 ã���ϴ�.
            {
                occupiedSpawnPoints[i] = null; // �ش� ������ ���� ���¸� �����մϴ� (null�� ����).
                Debug.Log($"{cardGO.name}��(��) �ʵ忡�� ���������� ���ŵ�."); 
                return;
            }
        }
        // �迭���� ī�带 ã�� ������ ��� ��� �α��մϴ�.
        Debug.LogWarning($"{cardGO.name}��(��) �ʵ忡 ��ϵ� ī�尡 �ƴϹǷ� ������ �� �����ϴ�.");
    }
}