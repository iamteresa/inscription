using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DrawButtonController : MonoBehaviour
{
    [Header("------- 테스트용 버튼 -------")]
    [SerializeField] private Button drawButton;

    [Header("------- 핸드 메니져 ----------")]
    [SerializeField] private HandManager handManager;

    [Header("---------- 다람쥐 카드 프리펩 -------------")]
    [SerializeField] private CardData squirrelCardData;

    [Header("---------- 게임 흐름 컨트롤러 -------------")]
    [SerializeField] private GameFlowController gameFlowController; // GameFlowController 참조 추가

    private void Start()
    {
        // 버튼 클릭 시 DrawSquirrelCardAndEndTurn 함수 실행
        drawButton.onClick.AddListener(DrawSquirrelCardAndEndTurn);
    }

    private void DrawSquirrelCardAndEndTurn()
    {
        Debug.Log("다람쥐 카드 드로우 버튼 클릭됨");

        // 1. 다람쥐 카드 뽑기
        if (handManager != null && squirrelCardData != null)
        {
            handManager.DrawSpecificCard(squirrelCardData);
        }
        else
        {
            Debug.LogError("HandManager 또는 SquirrelCardData가 연결되지 않았습니다.");
            return; // 오류 발생 시 턴 종료 로직 실행 방지
        }

        // 2. 턴 종료 및 추가 카드 드로우 방지 (Special 타입으로 턴 종료)
        if (gameFlowController != null)
        {
            // CardType.Special을 전달하여 턴 종료 시 플레이어 카드 드로우를 막습니다.
            gameFlowController.EndPlayerTurn(CardType.Special);
            Debug.Log("다람쥐 카드 사용으로 턴 종료, 추가 카드 드로우 안 함.");
        }
        else
        {
            Debug.LogError("GameFlowController가 연결되지 않았습니다. 인스펙터에서 연결해주세요.");
        }
    }
}