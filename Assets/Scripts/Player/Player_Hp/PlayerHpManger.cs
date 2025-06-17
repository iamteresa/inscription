using UnityEngine;
using UnityEngine.UI; // UI Image ������Ʈ�� ����ϱ� ���� �ʿ�
using UnityEngine.SceneManagement; // �� ������ ���� �߰�

public class PlayerHpManger : MonoBehaviour
{
    [Header("----------------HP ����-------------")]
    [SerializeField] private int maxHealth = 10; // �÷��̾��� �ִ� ü��
    private int _currentHealth; // �÷��̾��� ���� ü��
    public int CurrentHp => _currentHealth;

    [Header("--------------UI ����---------------")]
    [SerializeField] private Image hpFillImage; // HP ���� fill Image ������Ʈ

    // �÷��̾ �������� ���� �� ȣ���� �̺�Ʈ (���� ����: ���߿� �̺�Ʈ �ý��� ���� �� ����)
    // public event System.Action<int> OnHealthChanged;

    [Header("--------------��� �� �� ��ȯ---------------")]
    [SerializeField] private string sceneToLoadOnDeath; // ��� �� �ε��� ���� �̸�

    void Awake()
    {
        // ���� ���� �� ���� ü���� �ִ� ü������ ����
        _currentHealth = maxHealth;

        // HP �� UI�� ����Ǿ����� Ȯ��
        if (hpFillImage == null)
        {
            Debug.LogError("PlayerHealth: HP Fill Image�� ������� �ʾҽ��ϴ�. �ν����Ϳ��� �Ҵ����ּ���.", this);
        }

        // �ʱ� HP UI ������Ʈ
        UpdateHealthUI();
    }

    /// <summary>
    /// �÷��̾ �������� �޽��ϴ�.
    /// </summary>
    /// <param name="damageAmount">���� ������ ��</param>
    public void TakeDamage(int damageAmount)
    {
        if (damageAmount < 0)
        {
            Debug.LogWarning("������ ���� ������ �� �� �����ϴ�. TakeDamage ��� Heal�� ����ϼ���.");
            return;
        }

        _currentHealth -= damageAmount; // ���� ü�¿��� ������ ����
        _currentHealth = Mathf.Max(_currentHealth, 0); // ü���� 0 �̸����� �������� �ʵ��� ����

        Debug.Log($"�÷��̾ {damageAmount} �������� �޾ҽ��ϴ�. ���� ü��: {_currentHealth}");

        UpdateHealthUI(); // UI ������Ʈ

        if (_currentHealth <= 0)
        {
            Die(); // ü���� 0 ���ϸ� ��� ó��
        }
    }

    /// <summary>
    /// �÷��̾ ü���� ȸ���մϴ�.
    /// </summary>
    /// <param name="healAmount">ȸ���� ü�� ��</param>
    public void Heal(int healAmount)
    {
        if (healAmount < 0)
        {
            Debug.LogWarning("ȸ�� ���� ������ �� �� �����ϴ�. Heal ��� TakeDamage�� ����ϼ���.");
            return;
        }

        _currentHealth += healAmount; // ���� ü�¿� ȸ���� �߰�
        _currentHealth = Mathf.Min(_currentHealth, maxHealth); // ü���� �ִ� ü���� �ʰ����� �ʵ��� ����

        Debug.Log($"�÷��̾ {healAmount} ü���� ȸ���߽��ϴ�. ���� ü��: {_currentHealth}");

        UpdateHealthUI(); // UI ������Ʈ
    }

    /// <summary>
    /// HP �� UI�� Fill Amount�� ������Ʈ�մϴ�.
    /// </summary>
    private void UpdateHealthUI()
    {
        if (hpFillImage != null)
        {
            // ���� ü���� �ִ� ü������ ���� ������ Fill Amount ����
            hpFillImage.fillAmount = (float)_currentHealth / maxHealth;
        }
        // OnHealthChanged?.Invoke(currentHealth); // �̺�Ʈ �߻� (���� ����)
    }

    /// <summary>
    /// �÷��̾� ��� �� ȣ��Ǵ� �޼���. ������ ������ ��ȯ�մϴ�.
    /// </summary>
    private void Die()
    {
        Debug.Log("�÷��̾ ����߽��ϴ�!");
        if (!string.IsNullOrEmpty(sceneToLoadOnDeath)) // �� �̸��� ������� ������ Ȯ��
        {
            Debug.Log($"�÷��̾� ������� ���� �� '{sceneToLoadOnDeath}'���� �̵��մϴ�.");
            SceneManager.LoadScene(sceneToLoadOnDeath); // ������ �� �̸����� �� �ε�
        }
        else
        {
            Debug.LogWarning("��� �� �ε��� �� �̸��� �������� �ʾҽ��ϴ�. " +
                "'Scene To Load On Death' �ʵ带 Ȯ�����ּ���.", this);
            gameObject.SetActive(false);
        }
    }

    // �ܺο��� ���� ü�°� �ִ� ü���� ���� �� �ִ� ������Ƽ
    public int GetCurrentHealth()
    {
        return _currentHealth;
    }

    public int GetMaxHealth()
    {
        return maxHealth;
    }
}