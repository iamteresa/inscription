using UnityEngine;
using UnityEngine.UI;
using System.Collections;
// using System; // 카드를 받는 기능을 제어할 것이므로 Action은 필요 없습니다.
public enum CardType
{
    Basic,   // 기본 카드 (턴 종료 시 카드 드로우)
    Special  // 특수 카드 (턴 종료 시 카드 드로우 안 함)
}
public class GameFlowController : MonoBehaviour
{
    [Header("참조할 컴포넌트")]
    [SerializeField] private FieldCardAttack cardAttack;
    [SerializeField] private EnemyCardManager enemyCardManager;
    [SerializeField] private HandManager handManager;        // 플레이어 카드 뽑기용

    [Header("소환/딜레이 설정")]
    [SerializeField] private int spawnPerTurn = 1;    // 한 턴에 몇 장 소환할지 (난이도)
    [SerializeField] private float spawnDelay = 0.3f; // 카드 하나 뽑고 다음 뽑기까지 대기

    [Header("UI 버튼")]
    [SerializeField] private Button endTurnButton;      // 인스펙터에서 연결

    void Awake()
    {
        // UI의 턴 종료 버튼은 기본 카드 타입으로 턴을 종료하도록 연결 (예시)
        endTurnButton.onClick.AddListener(() => EndPlayerTurn(CardType.Basic));
    }

    void OnDestroy()
    {
        endTurnButton.onClick.RemoveListener(() => EndPlayerTurn(CardType.Basic));
    }

    /// <summary>
    /// 플레이어 턴을 종료하고 다음 턴으로 넘어가는 전체 흐름을 시작합니다.
    /// 이 함수는 외부에서 직접 호출하여 턴을 넘길 수 있으며, 플레이어가 낸 카드의 타입을 전달합니다.
    /// </summary>
    /// <param name="playedCardType">플레이어가 턴을 종료할 때 낸 카드의 타입</param>
    public void EndPlayerTurn(CardType playedCardType)
    {
        StartCoroutine(EndTurnSequence(playedCardType));
    }

    private IEnumerator EndTurnSequence(CardType currentCardType)
    {
        // 1) 플레이어 공격
        yield return StartCoroutine(cardAttack.AttackSequence());

        // 2) 적 카드 소환
        for (int i = 0; i < spawnPerTurn; i++)
        {
            if (!enemyCardManager.DrawAndSpawnEnemyCard())
                break; // 더 이상 덱/슬롯 없으면 중단
            yield return new WaitForSeconds(spawnDelay);
        }

        // 3) (EnemyAttackRoutine 호출은 제거)

        // 4) 플레이어가 낸 카드의 타입에 따라 카드 드로우 결정
        if (currentCardType == CardType.Basic)
        {
            Debug.Log("기본 카드이므로 플레이어 카드 1장을 드로우합니다.");
            handManager.DrawCard(); // 기본 카드일 경우에만 카드 드로우
        }
        else if (currentCardType == CardType.Special)
        {
            Debug.Log("특수 카드이므로 플레이어 카드를 드로우하지 않습니다.");
            // 특수 카드일 경우 아무것도 하지 않음 (또는 다른 특수 로직 추가)
        }
    }
}