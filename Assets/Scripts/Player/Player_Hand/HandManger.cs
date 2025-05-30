using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandManager : MonoBehaviour
{
    [Header("-------- 카드 부모 오브젝트 -------------")]
    [SerializeField] Transform handArea;

    [Header("-------- 카드 프리펩 -------------")]
    [SerializeField] GameObject cardPrefab;

    [Header("-------- 덱 -------------")]                    
    [SerializeField] List<CardData> drawPile;             

    [SerializeField] int maxHandSize = 7;

    public void DrawCard()
    {
        if (handArea.childCount >= maxHandSize)
        {
            Debug.Log("손패가 가득 찼습니다!");
            return;
        }

        if (drawPile.Count == 0)
        {
            Debug.Log("덱이 비었습니다!");
            return;
        }

        // 무작위 카드 하나 선택
        int index = Random.Range(0, drawPile.Count);
        CardData cardData = drawPile[index];

        // 카드 생성
        GameObject cardGO = Instantiate(cardPrefab, handArea);
        CardDisplay display = cardGO.GetComponent<CardDisplay>();
        display.cardData = cardData;
        display.UpdateDisplay();

        // 덱에서 제거 (선택사항)
        drawPile.RemoveAt(index);
    }
}