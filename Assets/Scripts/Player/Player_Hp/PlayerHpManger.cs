using UnityEngine;
using UnityEngine.UI; // UI Image 컴포넌트를 사용하기 위해 필요
using UnityEngine.SceneManagement; // 씬 관리를 위해 추가

public class PlayerHpManger : MonoBehaviour
{
    [Header("----------------HP 설정-------------")]
    [SerializeField] private int maxHealth = 10; // 플레이어의 최대 체력
    private int _currentHealth; // 플레이어의 현재 체력
    public int CurrentHp => _currentHealth;

    [Header("--------------UI 연동---------------")]
    [SerializeField] private Image hpFillImage; // HP 바의 fill Image 컴포넌트

    // 플레이어가 데미지를 받을 때 호출할 이벤트 (선택 사항: 나중에 이벤트 시스템 구현 시 유용)
    // public event System.Action<int> OnHealthChanged;

    [Header("--------------사망 시 씬 전환---------------")]
    [SerializeField] private string sceneToLoadOnDeath; // 사망 시 로드할 씬의 이름

    void Awake()
    {
        // 게임 시작 시 현재 체력을 최대 체력으로 설정
        _currentHealth = maxHealth;

        // HP 바 UI가 연결되었는지 확인
        if (hpFillImage == null)
        {
            Debug.LogError("PlayerHealth: HP Fill Image가 연결되지 않았습니다. 인스펙터에서 할당해주세요.", this);
        }

        // 초기 HP UI 업데이트
        UpdateHealthUI();
    }

    /// <summary>
    /// 플레이어가 데미지를 받습니다.
    /// </summary>
    /// <param name="damageAmount">받을 데미지 양</param>
    public void TakeDamage(int damageAmount)
    {
        if (damageAmount < 0)
        {
            Debug.LogWarning("데미지 양은 음수가 될 수 없습니다. TakeDamage 대신 Heal을 사용하세요.");
            return;
        }

        _currentHealth -= damageAmount; // 현재 체력에서 데미지 감소
        _currentHealth = Mathf.Max(_currentHealth, 0); // 체력이 0 미만으로 내려가지 않도록 보정

        Debug.Log($"플레이어가 {damageAmount} 데미지를 받았습니다. 현재 체력: {_currentHealth}");

        UpdateHealthUI(); // UI 업데이트

        if (_currentHealth <= 0)
        {
            Die(); // 체력이 0 이하면 사망 처리
        }
    }

    /// <summary>
    /// 플레이어가 체력을 회복합니다.
    /// </summary>
    /// <param name="healAmount">회복할 체력 양</param>
    public void Heal(int healAmount)
    {
        if (healAmount < 0)
        {
            Debug.LogWarning("회복 양은 음수가 될 수 없습니다. Heal 대신 TakeDamage를 사용하세요.");
            return;
        }

        _currentHealth += healAmount; // 현재 체력에 회복량 추가
        _currentHealth = Mathf.Min(_currentHealth, maxHealth); // 체력이 최대 체력을 초과하지 않도록 보정

        Debug.Log($"플레이어가 {healAmount} 체력을 회복했습니다. 현재 체력: {_currentHealth}");

        UpdateHealthUI(); // UI 업데이트
    }

    /// <summary>
    /// HP 바 UI의 Fill Amount를 업데이트합니다.
    /// </summary>
    private void UpdateHealthUI()
    {
        if (hpFillImage != null)
        {
            // 현재 체력을 최대 체력으로 나눈 비율로 Fill Amount 설정
            hpFillImage.fillAmount = (float)_currentHealth / maxHealth;
        }
        // OnHealthChanged?.Invoke(currentHealth); // 이벤트 발생 (선택 사항)
    }

    /// <summary>
    /// 플레이어 사망 시 호출되는 메서드. 지정된 씬으로 전환합니다.
    /// </summary>
    private void Die()
    {
        Debug.Log("플레이어가 사망했습니다!");
        if (!string.IsNullOrEmpty(sceneToLoadOnDeath)) // 씬 이름이 비어있지 않은지 확인
        {
            Debug.Log($"플레이어 사망으로 인해 씬 '{sceneToLoadOnDeath}'으로 이동합니다.");
            SceneManager.LoadScene(sceneToLoadOnDeath); // 지정된 씬 이름으로 씬 로드
        }
        else
        {
            Debug.LogWarning("사망 시 로드할 씬 이름이 지정되지 않았습니다. " +
                "'Scene To Load On Death' 필드를 확인해주세요.", this);
            gameObject.SetActive(false);
        }
    }

    // 외부에서 현재 체력과 최대 체력을 얻을 수 있는 프로퍼티
    public int GetCurrentHealth()
    {
        return _currentHealth;
    }

    public int GetMaxHealth()
    {
        return maxHealth;
    }
}