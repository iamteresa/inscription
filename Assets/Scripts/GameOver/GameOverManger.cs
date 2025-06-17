using UnityEngine;
using UnityEngine.SceneManagement; // �� ������ ���� �ʿ�
using UnityEngine.UI; // UI Button ������Ʈ�� ����ϱ� ���� �ʿ�

public class GameOverManager : MonoBehaviour
{
    [Header("�ٽ� ������ ���� �� �̸�")]
    [SerializeField] private string gameSceneName = "GameScene"; // �ʱ� ���� ���� ��Ȯ�� �̸�

    // ����Ƽ �������� �ν����Ϳ��� ��ư�� �� �ʵ忡 �����ؾ� �մϴ�.
    [Header("UI ��ư")]
    [SerializeField] private Button restartButton;

    void Awake()
    {
       
        // ��ư�� �Ҵ�Ǿ����� Ȯ���ϰ� Ŭ�� �̺�Ʈ�� �޼��带 �����մϴ�.
        if (restartButton != null)
        {
            restartButton.onClick.AddListener(RestartGame);
        }
        else
        {
            Debug.LogError("GameOverManager: Restart Button�� ������� �ʾҽ��ϴ�. �ν����Ϳ��� �Ҵ����ּ���.", this);
        }

        // ���� �� �̸��� ����ִ��� Ȯ�� (���� ����������, ��Ÿ ������ ����)
        if (string.IsNullOrEmpty(gameSceneName))
        {
            Debug.LogWarning("GameOverManager: �ٽ� ������ ���� �� �̸��� �������� �ʾҽ��ϴ�. �ν����Ϳ��� 'Game Scene Name' �ʵ带 Ȯ�����ּ���.", this);
        }
    }

    /// <summary>
    /// ��ư Ŭ�� �� ���� ���� �ٽ� �ε��ϴ� �޼���.
    /// </summary>
    public void RestartGame()
    {
        if (!string.IsNullOrEmpty(gameSceneName))
        {
            Debug.Log($"������ �ٽ� �����մϴ�: '{gameSceneName}' �� �ε�");
            SceneManager.LoadScene(gameSceneName); // ������ ���� �� �ε�
        }
        else
        {
            Debug.LogError("������ �ٽ� ������ �� �̸��� ��ȿ���� �ʽ��ϴ�. �� �̸��� Ȯ�����ּ���.");
        }
    }
}