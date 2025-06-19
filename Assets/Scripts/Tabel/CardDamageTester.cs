using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// �����̽��� �Է� �� ���� ��ġ�� FieldCard �ν��Ͻ��� �������� �����մϴ�.
/// ����(������) ������ �����ϱ� ���� �� ������Ʈ�� ������� ����ϴ�.
/// </summary>
public class FieldCardDamageTester : MonoBehaviour
{
    [Header("------------�׽�Ʈ ����--------------")]
    [Tooltip("�����̽��ٸ� ������ �� ������ ������ ��")]
    [SerializeField] private int testDamageAmount = 1;
    [SerializeField] EnemyCardManager _enemyCardManger;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            ApplyDamageToAllFieldCards(testDamageAmount);
        }
        if (Input.GetKeyDown(KeyCode.T))
        {
            _enemyCardManger.DrawAndSpawnEnemyCard();
        }
    }

    /// <summary>
    /// ���� �ִ� ��� FieldCard�� �������� �����մϴ�.
    /// </summary>
    private void ApplyDamageToAllFieldCards(int damageAmount)
    {
        // ���� ��ġ�� FieldCard �ν��Ͻ��� ������
        FieldCard[] allFieldCards = FindObjectsOfType<FieldCard>();
        bool anyHit = false;

        foreach (FieldCard fc in allFieldCards)
        {
            // ���� ���� �ִ� ������Ʈ���� Ȯ���ϰ�, �÷��� ������� Ȯ��
            if (fc.gameObject.scene.IsValid() && Application.isPlaying)
            {
                fc.TakeDamage(damageAmount);
                Debug.Log($"{fc.gameObject.name}�� {damageAmount} ������ ����. ���� ü��: {fc.GetCurrentHealth()}");
                anyHit = true;
            }
        }

        if (!anyHit)
        {
            Debug.Log("�ʵ忡 �������� �� FieldCard�� �����ϴ�.");
        }
    }
}