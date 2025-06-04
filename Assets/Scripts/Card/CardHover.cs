using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CardHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private Vector3 originalArrangedPosition; // HandManager�� ������ �ʱ� ���� ��ġ
    private Vector3 currentBasePosition;      // ī�尡 ���� �־�� �� �⺻ ��ġ (������ ���ѳ��� ���� ����)
    private bool isHovered = false;
    private HandManager handManager;
    private CardSelector cardSelector; // CardSelector ���� �߰�

    [Header("-------- ī�� �ִϸ��̼� ----------")]
    [SerializeField] float _hoverHeight = 50f;
    [SerializeField] float _moveSpeed = 10f;
    [SerializeField] float _selectedHeight = 100f; // ���õǾ��� �� �߰��� �ö� ����

    [field: SerializeField] public float SideShiftAmount { get; private set; } = 80f;

    void Awake()
    {
        handManager = FindObjectOfType<HandManager>();
        if (handManager == null)
        {
            Debug.LogError("HandManager�� ã�� �� �����ϴ�.");
        }
        cardSelector = GetComponent<CardSelector>(); // CardSelector ������Ʈ ��������
        if (cardSelector == null)
        {
            Debug.LogError("CardHover: CardSelector ������Ʈ�� ã�� �� �����ϴ�. " +
                "CardHover�� CardSelector�� ���� ���� ������Ʈ�� �־�� �մϴ�.", this);
        }
    }

    // HandManager�� ������ ī���� �ʱ� ���� ��ġ�� �����մϴ�.
    public void SetOriginalPosition(Vector3 pos)
    {
        originalArrangedPosition = pos;
        currentBasePosition = pos;      // �ʱ⿡�� ���� ��ġ�� �⺻ ��ġ�� �˴ϴ�.
    }

    void Update()
    {
        Vector3 targetPosForHover;

        // CardSelector�� �� ī�带 �����ߴٰ� �˷��ָ� ���õ� ���̸� ����
        if (cardSelector != null && cardSelector.IsSelected())
        {
            targetPosForHover = currentBasePosition + Vector3.up * _selectedHeight;
        }

        // ���õ��� �ʾҰ� ȣ�� ���� ���
        else if (isHovered)
        {
            targetPosForHover = currentBasePosition + Vector3.up * _hoverHeight;
        }

        // �� ���� ��� (���õ����� ȣ�� �ߵ� �ƴ� ���)
        else
        {
            targetPosForHover = currentBasePosition;
        }
        

        transform.localPosition = Vector3.Lerp(transform.localPosition, targetPosForHover, Time.deltaTime * _moveSpeed);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        // ī�尡 �̹� ���õ� ���¶�� ȣ�� ȿ���� �������� �ʽ��ϴ�. (���� ���°� �켱)
        if (cardSelector != null && cardSelector.IsSelected()) return;

        if (isHovered) return;

        isHovered = true;
        if (handManager != null)
        {
            handManager.OnCardHovered(this, true);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        // ī�尡 �̹� ���õ� ���¶�� ȣ�� ���� ȿ���� �������� �ʽ��ϴ�.
        if (cardSelector != null && cardSelector.IsSelected()) return;

        if (!isHovered) return;

        isHovered = false;
        if (handManager != null)
        {
            handManager.OnCardHovered(this, false);
        }
    }

    /// <summary>
    /// ī���� �⺻ ��ġ�� shiftAmount��ŭ �̵���Ų��.
    /// </summary>
    /// <param name="shiftAmount"></param>
    public void ShiftPosition(float shiftAmount)
    {
        currentBasePosition = originalArrangedPosition + new Vector3(shiftAmount, 0, 0);
    }
    
    /// <summary>
    /// ī�带 �⺻ ��ġ�� �ǵ�����.
    /// </summary>
    public void ResetShift()
    {
        currentBasePosition = originalArrangedPosition;
    }
    
}