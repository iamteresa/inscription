using UnityEngine;
using System.Collections.Generic; // List�� ����ϱ� ���� �ʿ�

public class FieldCardDamageTester : MonoBehaviour
{
    [Header("------------�׽�Ʈ ����--------------")]
    [SerializeField] private int testDamageAmount = 1; // �����̽��ٸ� ���� �� �� ������ ��

    void Update()
    {
        // �����̽��ٸ� ������ �ʵ� ���� ��� ī�忡 �������� �ݴϴ�.
        if (Input.GetKeyDown(KeyCode.Space))
        {
            ApplyDamageToAllFieldCards(testDamageAmount);
        }
    }

    /// <summary>
    /// ���� ���� �ִ� ��� FieldCard�� �������� �����մϴ�.
    /// </summary>
    /// <param name="damageAmount">������ ������ ��</param>
    private void ApplyDamageToAllFieldCards(int damageAmount)
    {
        // ���� ��� FieldCard ������Ʈ�� ã���ϴ�.
        // ����: �� ����� ���� FieldCard�� ���������� ���ɿ� ������ �� �� �ֽ��ϴ�.
        // ���� ���ӿ����� GameManager�� BattlefieldManager�� FieldCard ����� �����ϴ� ���� �����ϴ�.
        FieldCard[] allFieldCards = FindObjectsOfType<FieldCard>();

        if (allFieldCards.Length == 0)
        {
            Debug.Log("�ʵ忡 �������� �� FieldCard�� �����ϴ�.");
            return;
        }

        Debug.Log($"�����̽��� ����! �ʵ��� ��� ī�忡 {damageAmount} ������ ���� ��...");

        foreach (FieldCard card in allFieldCards)
        {
            // ī�� ������ ���� Ư�� �������� �������� �� ���� �ֽ��ϴ�.
            // ���� ���, �÷��̾� ī�忡�� �������� �ַ���:
            // if (card.faction == FieldCard.CardFaction.Player)
            // {
            //     card.TakeDamage(damageAmount);
            // }

            // ����� ��� FieldCard�� ������ ����
            card.TakeDamage(damageAmount);
        }
    }
}