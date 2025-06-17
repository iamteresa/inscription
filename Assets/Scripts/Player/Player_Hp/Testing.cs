using UnityEngine;

public class DamageTest : MonoBehaviour
{
    [SerializeField] private PlayerHpManger _playerHpManger; // PlayerHealth ��ũ��Ʈ ����
    [SerializeField] private int testDamage = 1; // �׽�Ʈ�� ������ ��

    void Update()
    {
        // �����̽��ٸ� ������ �÷��̾�� �������� �ݴϴ�.
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (_playerHpManger != null)
            {
                _playerHpManger.TakeDamage(testDamage);
            }
            else
            {
                Debug.LogError("PlayerHealth�� ������� �ʾҽ��ϴ�! �ν����Ϳ��� �Ҵ����ּ���.");
            }
        }
        // H Ű�� ������ �÷��̾� ü���� ȸ���մϴ�.
        if (Input.GetKeyDown(KeyCode.H))
        {
            if (_playerHpManger != null|| _playerHpManger.CurrentHp < 10)
            {
                _playerHpManger.Heal(testDamage); // �׽�Ʈ ������ �縸ŭ ȸ��
            }
        }
    }
}