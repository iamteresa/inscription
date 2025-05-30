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

    private void Start()
    {
        // 버튼 클릭 시 DrawCard() 실행
        drawButton.onClick.AddListener(() => {
            Debug.Log("카드 드로우 버튼 클릭됨");
            handManager.DrawCard();
        });
    }
}