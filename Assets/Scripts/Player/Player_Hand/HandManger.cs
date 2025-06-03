using System.Collections; // 코루틴 사용을 위해 포함
using System.Collections.Generic; // List<T> 사용을 위해 포함
using UnityEngine; // Unity 엔진의 기본 기능 사용을 위해 포함

public class HandManager : MonoBehaviour
{
    // --- 인스펙터에서 설정 가능한 변수들 ---

    [Header("-------- 카드 부모 오브젝트 -------------")]
    [SerializeField] Transform handArea;
    [SerializeField] RectTransform mainCanvasRectTransform;

    [Header("-------- 카드 프리팹 -------------")]
    [SerializeField] GameObject cardPrefab; 

    [Header("-------- 덱 -------------")]
    [SerializeField] List<CardData> drawPile;       // 현재 뽑을 수 있는 카드 데이터들의 리스트

    [SerializeField] int maxHandSize = 7;           // 플레이어 손패의 최대 카드 수
    [SerializeField] float cardSpacing = 120f;      // 손패 내 카드들 사이의 간격


    private List<GameObject> handCards = new List<GameObject>();    // 현재 손패에 있는 카드 GameObject들의 리스트
    private CardHover currentlyHoveredCard = null;                  // 현재 마우스가 올라가 있는 (호버된) CardHover 컴포넌트 참조


    /// <summary>
    /// 덱에서 카드를 한 장 뽑아 손패에 추가합니다.
    /// </summary>
    public void DrawCard()
    {
        // 손패가 최대치에 도달했는지 확인
        if (handCards.Count >= maxHandSize)
        {
            Debug.Log("손패가 가득 찼습니다!");
            return; // 카드 뽑기 중단
        }

        // 덱이 비었는지 확인
        if (drawPile.Count == 0)
        {
            Debug.Log("덱이 비었습니다!");
            return; // 카드 뽑기 중단
        }

        // 덱에서 무작위 카드 선택
        int index = Random.Range(0, drawPile.Count);
        CardData cardData = drawPile[index];

        // 카드 프리팹을 handArea 아래에 생성
        GameObject cardGO = Instantiate(cardPrefab, handArea);

        // 생성된 카드에 CardDisplay 컴포넌트가 있다면 카드 데이터로 UI 업데이트
        CardDisplay display = cardGO.GetComponent<CardDisplay>();
        if (display != null)
        {
            display.cardData = cardData;
            display.UpdateDisplay();
        }
        

        // 손패 리스트에 새로 뽑은 카드 추가
        handCards.Add(cardGO);
        // 덱에서 뽑은 카드 데이터 제거
        drawPile.RemoveAt(index);

        // 손패의 카드들을 재정렬합니다.
        ArrangeHandCards();
    }

    /// <summary>
    /// 현재 손패에 있는 카드들을 정해진 간격에 맞춰 정렬합니다.
    /// </summary>
    private void ArrangeHandCards()
    {
        int count = handCards.Count;        // 현재 손패에 있는 카드 수
        if (count == 0) return;             // 손패가 없을시 return

        // 카드들이 중앙에 정렬되도록 첫 카드의 X 위치 계산
        // (총 너비 - 카드 하나 너비) / 2 가 아닌, 전체 카드들의 중심이 0이 되도록 계산
        float startX = -(cardSpacing * (count - 1)) / 2f;

        // 각 카드를 순회하며 위치 설정
        for (int i = 0; i < count; i++)
        {
            GameObject card = handCards[i];
            // 현재 카드의 목표 X 위치 계산
            Vector3 targetPos = new Vector3(startX + cardSpacing * i, 0, 0);

            // 카드 GameObject의 로컬 위치를 즉시 목표 위치로 설정
            card.transform.localPosition = targetPos;

            // 카드에 CardHover 컴포넌트가 있다면, 해당 카드의 '원래 정렬 위치'를 설정
            // 이 위치는 CardHover가 호버/비켜주기 애니메이션의 기준점으로 사용합니다.
            CardHover hover = card.GetComponent<CardHover>();
            if (hover != null)
            {
                hover.SetOriginalPosition(targetPos);
            }
        }
    }

    /// <summary>
    /// CardHover 스크립트로부터 마우스 호버 알림을 받습니다.
    /// 호버된 카드와 다른 카드들의 위치를 조정하는 역할을 합니다.
    /// </summary>
    /// <param name="card">현재 호버되거나 호버가 해제된 CardHover 컴포넌트</param>
    /// <param name="isHovering">호버 중이면 true, 호버 해제면 false</param>
    public void OnCardHovered(CardHover card, bool isHovering)
    {
        if (isHovering)
        {
            currentlyHoveredCard = card;    // 현재 호버된 카드를 저장
            ApplySideShift();               // 주변 카드들을 비켜주도록 적용
        }
        else
        {
            currentlyHoveredCard = null;    // 호버된 카드 없음으로 설정
            ResetSideShift();               // 비켜줬던 카드들을 원래 위치로 되돌림
        }
    }

    /// <summary>
    /// 현재 호버된 카드를 기준으로 주변 카드들을 옆으로 비켜주도록 합니다.
    /// </summary>
    private void ApplySideShift()
    {
        // 호버된 카드가 없으면 아무것도 하지 않음
        if (currentlyHoveredCard == null) return;

        int hoveredIndex = -1;      // 호버된 카드의 손패 리스트 내 인덱스
        // 손패 리스트를 순회하며 호버된 카드의 인덱스 찾기
        for (int i = 0; i < handCards.Count; i++)
        {
            // 현재 카드가 호버된 카드와 동일한지 확인
            if (handCards[i].GetComponent<CardHover>() == currentlyHoveredCard)
            {
                hoveredIndex = i;           // 인덱스 저장
                break;                      // 찾았으니 반복 중단
            }
        }

        // 호버된 카드를 찾지 못했으면 아무것도 하지 않음 (오류)
        if (hoveredIndex == -1) return;

        // 호버된 카드의 SideShiftAmount 값을 가져옴 (CardHover 스크립트에서 정의)
        float shiftAmount = currentlyHoveredCard.SideShiftAmount;

        // 모든 손패 카드들을 순회하며 위치 조정
        for (int i = 0; i < handCards.Count; i++)
        {
            // 호버된 카드보다 왼쪽에 있는 카드들 (인덱스가 작은 경우)
            if (i < hoveredIndex)
            {
                // 해당 카드의 CardHover 컴포넌트의 ShiftPosition을 호출하여 왼쪽으로 이동
                handCards[i].GetComponent<CardHover>()?.ShiftPosition(-shiftAmount);
            }
            // 호버된 카드보다 오른쪽에 있는 카드들 (인덱스가 큰 경우)
            else if (i > hoveredIndex)
            {
                // 해당 카드의 CardHover 컴포넌트의 ShiftPosition을 호출하여 오른쪽으로 이동
                handCards[i].GetComponent<CardHover>()?.ShiftPosition(shiftAmount);
            }
            // 호버된 카드 자신은 위치 조정이 필요 없으므로 이 블록에 포함되지 않음
        }
    }

    /// <summary>
    /// 모든 비켜줬던 카드들을 원래의 정렬 위치로 되돌립니다.
    /// (마우스 호버가 해제되었을 때 호출됨)
    /// </summary>
    private void ResetSideShift()
    {
        // 모든 손패 카드 GameObject들을 순회하며 ResetShift 호출
        foreach (GameObject cardGO in handCards)
        {
            cardGO.GetComponent<CardHover>()?.ResetShift();
        }
    }
}