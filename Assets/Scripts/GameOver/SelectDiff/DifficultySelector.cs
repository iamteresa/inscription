// Assets/Scripts/UI/DifficultySelector.cs
using UnityEngine;
using UnityEngine.SceneManagement;

public class DifficultySelector : MonoBehaviour
{
    [Tooltip("플레이 씬 이름을 정확히 입력하세요")]
    [SerializeField] private string playSceneName = "GamePlayScene";

    /// <summary>
    /// 이지 모드로 설정하고 플레이 씬으로 이동
    /// </summary>
    public void OnEasyButton()
    {
        GameSettings.SetDifficulty(GameSettings.Difficulty.Easy);
        LoadPlayScene();
    }

    /// <summary>
    /// 노말 모드로 설정하고 플레이 씬으로 이동
    /// </summary>
    public void OnNormalButton()
    {
        GameSettings.SetDifficulty(GameSettings.Difficulty.Normal);
        LoadPlayScene();
    }

    /// <summary>
    /// 하드 모드로 설정하고 플레이 씬으로 이동
    /// </summary>
    public void OnHardButton()
    {
        GameSettings.SetDifficulty(GameSettings.Difficulty.Hard);
        LoadPlayScene();
    }

    /// <summary>
    /// 나이트메어 모드로 설정하고 플레이 씬으로 이동
    /// </summary>
    public void OnNightmareButton()
    {
        GameSettings.SetDifficulty(GameSettings.Difficulty.Nightmare);
        LoadPlayScene();
    }

    private void LoadPlayScene()
    {
        if (string.IsNullOrEmpty(playSceneName))
        {
            Debug.LogError("DifficultySelector: playSceneName이 설정되지 않았습니다.");
            return;
        }
        SceneManager.LoadScene(playSceneName);
    }
}
