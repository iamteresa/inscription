using UnityEngine;

/// <summary>
/// ���忡 ��ġ�� ī���� �����͸� �����ϰ�, �ð��� ǥ�ø� ����մϴ�.
/// ü��, ���ݷ� �� ��Ÿ�� ���� ������ BattleCardManager�� ����մϴ�.
/// </summary>
[RequireComponent(typeof(CardDisplay))]
public class FieldCard : MonoBehaviour
{
    [Header("---------�ʵ� ī�� ����---------")]
    [SerializeField] private CardDisplay cardDisplay;      // ī�� UI ǥ�ÿ�
    [SerializeField] private CardFaction faction;         // ī�� ���� (�÷��̾� �Ǵ� ��)

    public enum CardFaction { Player, Enemy }

    /// <summary>
    /// ī�� �����͸� �Ҵ��ϰ�, CardDisplay�� ������Ʈ�մϴ�.
    /// BattleCardManager.RegisterCard�� �ܺο��� ȣ�����ּ���.
    /// </summary>
    public void SetCardData(CardData data, CardFaction cardFaction)
    {
        if (data == null)
        {
            Debug.LogError("FieldCard: ������ CardData�� null�Դϴ�.", this);
            return;
        }

        faction = cardFaction;

        // ī�� UI ����
        if (cardDisplay == null)
            cardDisplay = GetComponent<CardDisplay>();

        cardDisplay.SetCardDisplay(data);
    }

    /// <summary>
    /// BattleCardManager�� �ܺ� �ý����� ī�� ���� �� ȣ���մϴ�.
    /// </summary>
    public void RemoveFromField()
    {
        Destroy(gameObject);
    }
}
