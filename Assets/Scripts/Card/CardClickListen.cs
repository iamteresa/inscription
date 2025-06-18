using UnityEngine;
using UnityEngine.EventSystems; 
using System; 

public class CardClickListen : MonoBehaviour, IPointerClickHandler
{
    // ī�尡 Ŭ���Ǿ��� �� �ܺ�(CardPlayManager)�� �˸��� �̺�Ʈ
    public static event Action<GameObject> OnCardClickedForPlay;

    // �� ī�尡 ���� Ŭ�� ������ �������� ���� (CardPlayManager�� ����)
    private bool isClickable = true;

    // �ܺο��� Ŭ�� ���� ���θ� ����
    public void SetClickable(bool enable)
    {
        isClickable = enable;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        // Ŭ�� �Ұ��� �����̰ų� ��Ŭ���� �ƴϸ� ����
        if (!isClickable || eventData.button != PointerEventData.InputButton.Left) return;

        // Ŭ�� �̺�Ʈ�� �߻����� CardPlayManager���� � ī�尡 Ŭ���Ǿ����� �˸�
        OnCardClickedForPlay?.Invoke(this.gameObject);

        // ī�尡 Ŭ���Ǹ�, �� ī��� �Ͻ������� Ŭ�� �Ұ����ϰ� ����ϴ�.
        // CardPlayManager�� �÷��� ���μ��� ������ ������ ���Դϴ�.
        SetClickable(false);
    }
}
