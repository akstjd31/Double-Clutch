using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/// <summary>
/// �� ���� ��Ŭ���� ���� ������ �г��� �нú�0, 1, 2�� ���� ����
/// �� ������ PassiveExplainBox���� ����
/// </summary>
public class PassiveProfileBox : MonoBehaviour, IPointerUpHandler, IPointerDownHandler
{
    const string DEFAULT_TEXT = "비어 있음";
    Player_PassiveData _data;
    [SerializeField] TextMeshProUGUI _passiveText;

    public void Init(Player_PassiveData data)
    {
        _data = data;
        SetPassiveText();
    }

    public void Init()
    {
        _data = default;
        _passiveText.text = DEFAULT_TEXT;
    }

    public void SetPassiveText()
    {        
        _passiveText.text = StringManager.Instance.GetString(_data.skillName);
    }

    public void OnPointerUp(PointerEventData eventData)
    {        
        StudentUIManager.Instance.OnPassiveBoxMouseOverEnd();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        StudentUIManager.Instance.OnPassiveBoxMouseOverStart(_data);
    }
}
