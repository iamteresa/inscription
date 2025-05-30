using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandManager : MonoBehaviour
{
    [Header("-------- ī�� �θ� ������Ʈ -------------")]
    [SerializeField] Transform handArea;

    [Header("-------- ī�� ������ -------------")]
    [SerializeField] GameObject cardPrefab;

    [Header("-------- �� -------------")]                    
    [SerializeField] List<CardData> drawPile;             

    [SerializeField] int maxHandSize = 7;

    public void DrawCard()
    {
        if (handArea.childCount >= maxHandSize)
        {
            Debug.Log("���а� ���� á���ϴ�!");
            return;
        }

        if (drawPile.Count == 0)
        {
            Debug.Log("���� ������ϴ�!");
            return;
        }

        // ������ ī�� �ϳ� ����
        int index = Random.Range(0, drawPile.Count);
        CardData cardData = drawPile[index];

        // ī�� ����
        GameObject cardGO = Instantiate(cardPrefab, handArea);
        CardDisplay display = cardGO.GetComponent<CardDisplay>();
        display.cardData = cardData;
        display.UpdateDisplay();

        // ������ ���� (���û���)
        drawPile.RemoveAt(index);
    }
}