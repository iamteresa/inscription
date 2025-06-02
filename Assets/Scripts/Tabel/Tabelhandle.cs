using UnityEngine;
using UnityEngine.EventSystems;
using System; // Action ���

public class TabelHandle : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private int areaIndex; // �� ������ �ĺ��ϴ� ���� �ε��� (1, 2, 3, 4 ��)

    // �ܺο��� �̺�Ʈ�� ������ �� �ֵ��� Action ����
    public static event Action<int> OnTableAreaClicked;

    private bool isListening = false; // Ŭ�� ���� Ȱ��ȭ/��Ȱ��ȭ �÷���

    public void SetListening(bool enable)
    {
        isListening = enable;
        // ���� ����: Ŭ�� ������ �� �ð��� �ǵ�� ���� (��: ���̶���Ʈ)
        // GetComponent<UnityEngine.UI.Image>()?.CrossFadeAlpha(enable ? 0.5f : 1f, 0.2f, true); 
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        // Ŭ�� ���� ��Ȱ��ȭ �����̰ų� ��Ŭ���� �ƴϸ� ����
        if (!isListening || eventData.button != PointerEventData.InputButton.Left) return;

        // �̺�Ʈ�� �߻����� CardPlayManager���� � ������ Ŭ���Ǿ����� �˸�
        OnTableAreaClicked?.Invoke(areaIndex);

        // Ŭ�� �� ��� �������� ��Ȱ��ȭ (CardPlayManager�� �ٸ� ������ ��Ȱ��ȭ�� ����)
        SetListening(false);
    }
}