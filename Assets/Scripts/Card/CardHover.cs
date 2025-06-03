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

    [Header("-------- ī�� �ִϸ��̼� ----------")]
    [SerializeField] float hoverHeight = 50f;
    [SerializeField] float moveSpeed = 10f;

    [field: SerializeField] public float SideShiftAmount { get; private set; } = 80f;

    void Awake()
    {
        handManager = FindObjectOfType<HandManager>();
        if (handManager == null)
        {
            Debug.LogError("HandManager�� ã�� �� �����ϴ�. ���� HandManager�� �ִ��� Ȯ���ϼ���.");
        }
    }

    // HandManager�� ������ ī���� �ʱ� ���� ��ġ�� �����մϴ�.
    public void SetOriginalPosition(Vector3 pos)
    {
        originalArrangedPosition = pos;
        currentBasePosition = pos; // �ʱ⿡�� ���� ��ġ�� �⺻ ��ġ�� �˴ϴ�.
        //transform.localPosition = pos; // ��� ��ġ�� �����ϴ� ��� (���� ����)
    }

    void Update()
    {
        // ȣ�� ���� ���: currentBasePosition (���� �Ǵ� ���ѳ� ��ġ) + ���� hoverHeight
        // ȣ�� ���� �ƴ� ���: currentBasePosition (���� �Ǵ� ���ѳ� ��ġ)
        Vector3 targetPosForHover = isHovered
            ? currentBasePosition + Vector3.up * hoverHeight
            : currentBasePosition;

        // ���� localPosition���� targetPosForHover�� �ε巴�� �̵�
        transform.localPosition = Vector3.Lerp(transform.localPosition, targetPosForHover, Time.deltaTime * moveSpeed);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (isHovered) return;

        isHovered = true;
        if (handManager != null)
        {
            handManager.OnCardHovered(this, true);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!isHovered) return;

        isHovered = false;
        if (handManager != null)
        {
            handManager.OnCardHovered(this, false);
        }
    }

    // �� ī���� '�⺻ ��ġ'�� ������ �̵���ŵ�ϴ�.
    // �� �޼��尡 ȣ��� ��, Update()�� ������ currentBasePosition�� �����մϴ�.
    public void ShiftPosition(float shiftAmount)
    {
        // ���� ���� ��ġ(originalArrangedPosition)���� shiftAmount��ŭ �̵��� ���� ���ο� �⺻ ��ġ�� �˴ϴ�.
        currentBasePosition = originalArrangedPosition + new Vector3(shiftAmount, 0, 0);
       
    }

    // �� ī���� '�⺻ ��ġ'�� ���� ���� ��ġ�� �ǵ����ϴ�.
    public void ResetShift()
    {
        // ���� ���� ��ġ�� ���ο� �⺻ ��ġ�� �˴ϴ�.
        currentBasePosition = originalArrangedPosition;
        
    }
}