using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic; // List�� ����ϱ� ���� �߰�

public class BattlefieldManager : MonoBehaviour
{
    [Header("---------- ī�� ��ġ ��ġ ---------")]
    // Transform �迭 ��� List�� ����ϸ� ����Ƽ �����Ϳ��� �� �����ϰ� ������ �� �ֽ��ϴ�.
    [SerializeField] private List<Transform> spawnPoints = new List<Transform>();

    [Header("---------- ��ġ ���� �޽��� UI ---------")]
    [SerializeField] private TextMeshProUGUI placementInfoText; // ��ġ ���� �޽��� ǥ�� UI (TextMeshPro)

    // �ٸ� �Ŵ��� ���� (Awake �Ǵ� Start���� FindObjectOfType ��� SerializeField�� �����ϴ� ���� �� �������Դϴ�.)
    [SerializeField] private PlayerCostManager playerCostManager; // �ν����Ϳ��� ���� ����
    [SerializeField] private HandManager handManager;             // �ν����Ϳ��� ���� ���� (�ɼ�, �ʿ��� ���)


    // �� ���� ����Ʈ�� ��ġ�� ī�� GameObject�� ���� (���� ���� ����)
    private GameObject[] occupiedSpawnPoints;

    void Awake()
    {
        // �ʵ� �ʱ�ȭ (List ũ�� ���)
        if (spawnPoints.Count > 0)
        {
            occupiedSpawnPoints = new GameObject[spawnPoints.Count];
        }
        else
        {
            Debug.LogError("BattlefieldManager: spawnPoints�� �������� �ʾҽ��ϴ�. �ּ� 1�� �̻��� ��ġ ��ġ�� �ʿ��մϴ�.", this);
            enabled = false; // ��ũ��Ʈ ��Ȱ��ȭ
            return;
        }

        // �ʼ� ������Ʈ ���� Ȯ�� (�ν����Ϳ��� �����ϴ� ���� ���� �����ϴ�.)
        if (playerCostManager == null)
        {
            playerCostManager = FindObjectOfType<PlayerCostManager>(); // ����: �ν����� ���� �� ���� �� ������ ã��
            if (playerCostManager == null)
            {
                Debug.LogError("BattlefieldManager: PlayerCostManager�� ������� �ʾҰų� ������ ã�� �� �����ϴ�.", this);
            }
        }
        if (handManager == null)
        {
            handManager = FindObjectOfType<HandManager>(); // ����
            if (handManager == null)
            {
                Debug.LogWarning("BattlefieldManager: HandManager�� ������� �ʾҰų� ������ ã�� �� �����ϴ�. (�ʿ� �� ����)", this);
            }
        }


        if (placementInfoText == null)
        {
            Debug.LogError("BattlefieldManager: Placement Info Text�� ������� �ʾҽ��ϴ�.", this);
        }

        HidePlacementInfo(); // ���� �� ��ġ ���� UI �����
    }

    /// <summary>
    /// ī�� ��ġ ���� ������ UI�� ǥ���մϴ�.
    /// </summary>
    /// <param name="message">ǥ���� �޽���</param>
    public void ShowPlacementInfo(string message) // �ٸ� ��ũ��Ʈ������ ȣ���� �� �ֵ��� public
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
    public void HidePlacementInfo() // �ٸ� ��ũ��Ʈ������ ȣ���� �� �ֵ��� public
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
    /// ���õ� ī�带 ��ġ�Ϸ��� �õ��� ó���մϴ�.
    /// �� �޼���� HandManager�� OnCardPlaced���� ȣ��� ������ ����˴ϴ�.
    /// </summary>
    /// <param name="selectedCardGO">���õ� ī�� GameObject</param>
    /// <returns>ī�尡 ���������� ��ġ�Ǿ����� true, �ƴϸ� false</returns>
    public bool TryPlaceCard(GameObject selectedCardGO)
    {
        CardDisplay cardDisplay = selectedCardGO.GetComponent<CardDisplay>();
        if (cardDisplay == null || cardDisplay.cardData == null)
        {
            Debug.LogError("BattlefieldManager: ��ġ�Ϸ��� ī�忡 CardDisplay �Ǵ� CardData�� �����ϴ�.", selectedCardGO);
            return false;
        }

        int cardCost = cardDisplay.cardData.Cost; // CardData�� Cost ������Ƽ ���

        // 1. PlayerCostManager�� �ʱ�ȭ�Ǿ����� Ȯ��
        if (playerCostManager == null)
        {
            Debug.LogError("BattlefieldManager: PlayerCostManager�� �ʱ�ȭ���� �ʾҽ��ϴ�. ī�带 ��ġ�� �� �����ϴ�.", this);
            ShowPlacementInfo("�ý��� ����: �ڽ�Ʈ ������ ����.");
            return false;
        }

        // 2. Cost Ȯ��
        if (playerCostManager.CurrentCost < cardCost)
        {
            ShowPlacementInfo($"�ڽ�Ʈ ����! ({cardCost - playerCostManager.CurrentCost} �� �ʿ�)");
            Debug.Log($"�ڽ�Ʈ ����: {selectedCardGO.name} (�ʿ� �ڽ�Ʈ: {cardCost}, ���� �ڽ�Ʈ: {playerCostManager.CurrentCost})");
            return false;
        }

        // 3. ��ġ�� �� ��ġ ã��
        int targetSpawnIndex = -1;
        for (int i = 0; i < spawnPoints.Count; i++)
        {
            if (occupiedSpawnPoints[i] == null) // �� ���� ã��
            {
                targetSpawnIndex = i;
                break;
            }
        }

        if (targetSpawnIndex == -1)
        {
            ShowPlacementInfo("��ġ�� �� ������ �����ϴ�!");
            Debug.Log("��� ��ġ ������ �̹� ��� ���Դϴ�.");
            return false;
        }

        // 4. Cost �Ҹ�
        bool costRemoved = playerCostManager.RemoveCost(cardCost);
        if (!costRemoved) // ���� üũ (��� Cost�� �����ߴٸ� ������ �̹� return ��)
        {
            Debug.LogError("BattlefieldManager: �ڽ�Ʈ �Ҹ� �� ����ġ ���� ���� �߻�.", this);
            ShowPlacementInfo("�ڽ�Ʈ �Ҹ� ����!");
            return false;
        }

        // 5. ī�� ��ġ
        // ī�带 ���п��� �������� �̵���Ű�� ���� (�θ� ����)
        selectedCardGO.transform.SetParent(spawnPoints[targetSpawnIndex]);
        selectedCardGO.transform.localPosition = Vector3.zero; // ��ġ ��ġ�� ���� �������� �̵�
        selectedCardGO.transform.localRotation = Quaternion.identity; // ���� ȸ�� �ʱ�ȭ
        selectedCardGO.transform.localScale = Vector3.one; // ���� ������ �ʱ�ȭ (�ʿ��ϴٸ� ī�庰�� ����)

        // ���� ����Ʈ�� ī�� ���� ����
        occupiedSpawnPoints[targetSpawnIndex] = selectedCardGO;

        ShowPlacementInfo($"{selectedCardGO.name}��(��) ��ġ�߽��ϴ�! (-{cardCost} �ڽ�Ʈ)");
        Debug.Log($"{selectedCardGO.name} ī�尡 {spawnPoints[targetSpawnIndex].name}�� ��ġ�Ǿ����ϴ�. ���� �ڽ�Ʈ: {playerCostManager.CurrentCost}");

        // ī�� ��ġ�� ���������Ƿ�, CardSelector���� �� ī�带 ��Ȱ��ȭ�ϰų� �ı��ϵ��� HandManager���� �˸�
        // HandManager�� OnCardPlaced�� �� ī�� GameObject�� HandCards ����Ʈ���� �����ϰ� SetActive(false)�� ó���� ����.
        return true; // ��ġ ����
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
                occupiedSpawnPoints[i] = null; // ���� ����
                // Destroy(cardGO); // ���� ���ӿ����� �ı��ϰų� ������Ʈ Ǯ�� ��ȯ
                Debug.Log($"{cardGO.name}�� ���忡�� ���ŵǾ����ϴ�.");
                return;
            }
        }
        Debug.LogWarning($"BattlefieldManager: ���忡�� �����Ϸ��� ī�� '{cardGO.name}'�� ã�� �� �����ϴ�.");
    }
}