using UnityEngine;
using System.Collections;

public class GameStartController : MonoBehaviour
{
    [Header("핸드 매니저 참조")]
    [Tooltip("인스펙터에서 연결할 HandManager")]
    [SerializeField] private HandManager handManager;

    [Header("시작 손패 설정")]
    [Tooltip("시작 시 랜덤으로 뽑을 카드 수")]
    [SerializeField] private int initialRandomDrawCount = 3;
    [Tooltip("한 장 드로우 후 대기할 시간(초)")]
    [SerializeField] private float drawInterval = 0.5f;

    [Header("다람쥐 카드")]
    [Tooltip("인스펙터에서 연결할 다람쥐 CardData")]
    [SerializeField] private CardData squirrelCardData;

    private void Start()
    {
        // 게임 시작 직후 초기 손패 세팅 코루틴 실행
        StartCoroutine(InitialHandSetup());
    }

    private IEnumerator InitialHandSetup()
    {
        // (원한다면) 게임 시작 직후 살짝 대기
        yield return new WaitForSeconds(0.5f);

        // 1) 랜덤 카드 드로우
        for (int i = 0; i < initialRandomDrawCount; i++)
        {
            handManager.DrawCard();
            yield return new WaitForSeconds(drawInterval);
        }

        // 2) 다람쥐 카드 드로우
        if (squirrelCardData != null)
        {
            // HandManager에 특정 카드 드로우 기능이 없다면,
            // 내부에서 구현해 두신 Drawing API로 교체하세요.
            handManager.DrawSpecificCard(squirrelCardData);
        }
        else
        {
            Debug.LogWarning("GameStartController: SquirrelCardData가 할당되지 않았습니다.", this);
        }
    }
}
