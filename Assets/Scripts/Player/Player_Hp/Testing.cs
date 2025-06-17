using UnityEngine;

public class DamageTest : MonoBehaviour
{
    [SerializeField] private PlayerHpManger _playerHpManger; // PlayerHealth 스크립트 참조
    [SerializeField] private int testDamage = 1; // 테스트용 데미지 양

    void Update()
    {
        // 스페이스바를 누르면 플레이어에게 데미지를 줍니다.
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (_playerHpManger != null)
            {
                _playerHpManger.TakeDamage(testDamage);
            }
            else
            {
                Debug.LogError("PlayerHealth가 연결되지 않았습니다! 인스펙터에서 할당해주세요.");
            }
        }
        // H 키를 누르면 플레이어 체력을 회복합니다.
        if (Input.GetKeyDown(KeyCode.H))
        {
            if (_playerHpManger != null|| _playerHpManger.CurrentHp < 10)
            {
                _playerHpManger.Heal(testDamage); // 테스트 데미지 양만큼 회복
            }
        }
    }
}