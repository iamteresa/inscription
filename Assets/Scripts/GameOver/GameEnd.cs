using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class GameEnd : MonoBehaviour
{
    // 버튼 OnClick 에 이 함수를 연결하세요.
    public void Quit()
    {
        // 에디터에서 플레이 모드를 멈추고,
        // 빌드된 게임에서는 정상 종료합니다.
#if UNITY_EDITOR
        EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
