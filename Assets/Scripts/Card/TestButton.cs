using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DrawButtonController : MonoBehaviour
{
    [Header("------- �׽�Ʈ�� ��ư -------")]
    [SerializeField] private Button drawButton;

    [Header("------- �ڵ� �޴��� ----------")]
    [SerializeField] private HandManager handManager;

    private void Start()
    {
        // ��ư Ŭ�� �� DrawCard() ����
        drawButton.onClick.AddListener(() => {
            Debug.Log("ī�� ��ο� ��ư Ŭ����");
            handManager.DrawCard();
        });
    }
}