using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GameFlowController : MonoBehaviour
{
    [Header("참조할 컴포넌트")]
    [SerializeField] private FieldCardAttack cardAttack;
    [SerializeField] private EnemyCardManager enemyCardManager;
    [SerializeField] private HandManager handManager;       // 플레이어 카드 뽑기용

    [Header("소환/딜레이 설정")]
    [SerializeField] private int spawnPerTurn = 1;   // 한 턴에 몇 장 소환할지 (난이도)
    [SerializeField] private float spawnDelay = 0.3f; // 카드 하나 뽑고 다음 뽑기까지 대기

    [Header("UI 버튼")]
    [SerializeField] private Button endTurnButton;     // 인스펙터에서 연결

    void Awake()
    {
        endTurnButton.onClick.AddListener(OnEndTurnButton);
    }

    void OnDestroy()
    {
        endTurnButton.onClick.RemoveListener(OnEndTurnButton);
    }

    private void OnEndTurnButton()
    {
        StartCoroutine(EndTurnFlow());
    }

    private IEnumerator EndTurnFlow()
    {
        // 1) 플레이어 공격
        //   기존 PlayerAttackRoutine → AttackSequence 하나로 대체
        yield return StartCoroutine(cardAttack.AttackSequence());

        // 2) 적 카드 소환
        for (int i = 0; i < spawnPerTurn; i++)
        {
            if (!enemyCardManager.DrawAndSpawnEnemyCard())
                break; // 더 이상 덱/슬롯 없으면 중단
            yield return new WaitForSeconds(spawnDelay);
        }

        // 3) (EnemyAttackRoutine 호출은 제거)
        //    AttackSequence() 내부에 이미 적 턴 공격이 포함되어 있습니다.

        // 4) 플레이어 카드 1장 드로우
        handManager.DrawCard();
    }
}
