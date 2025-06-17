using UnityEngine;
using UnityEngine.SceneManagement; // 씬 관리를 위해 필요
using UnityEngine.UI; // UI Button 컴포넌트를 사용하기 위해 필요

public class GameOverManager : MonoBehaviour
{
    [Header("다시 시작할 게임 씬 이름")]
    [SerializeField] private string gameSceneName = "GameScene"; // 초기 게임 씬의 정확한 이름

    // 유니티 에디터의 인스펙터에서 버튼을 이 필드에 연결해야 합니다.
    [Header("UI 버튼")]
    [SerializeField] private Button restartButton;

    void Awake()
    {
       
        // 버튼이 할당되었는지 확인하고 클릭 이벤트에 메서드를 연결합니다.
        if (restartButton != null)
        {
            restartButton.onClick.AddListener(RestartGame);
        }
        else
        {
            Debug.LogError("GameOverManager: Restart Button이 연결되지 않았습니다. 인스펙터에서 할당해주세요.", this);
        }

        // 게임 씬 이름이 비어있는지 확인 (선택 사항이지만, 오타 방지에 도움)
        if (string.IsNullOrEmpty(gameSceneName))
        {
            Debug.LogWarning("GameOverManager: 다시 시작할 게임 씬 이름이 지정되지 않았습니다. 인스펙터에서 'Game Scene Name' 필드를 확인해주세요.", this);
        }
    }

    /// <summary>
    /// 버튼 클릭 시 게임 씬을 다시 로드하는 메서드.
    /// </summary>
    public void RestartGame()
    {
        if (!string.IsNullOrEmpty(gameSceneName))
        {
            Debug.Log($"게임을 다시 시작합니다: '{gameSceneName}' 씬 로드");
            SceneManager.LoadScene(gameSceneName); // 지정된 게임 씬 로드
        }
        else
        {
            Debug.LogError("게임을 다시 시작할 씬 이름이 유효하지 않습니다. 씬 이름을 확인해주세요.");
        }
    }
}