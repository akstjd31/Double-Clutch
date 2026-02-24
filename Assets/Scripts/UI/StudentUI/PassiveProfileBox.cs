using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/// <summary>
/// �� ���� ��Ŭ���� ���� ������ �г��� �нú�0, 1, 2�� ���� ����
/// �� ������ PassiveExplainBox���� ����
/// </summary>
public class PassiveProfileBox : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    Player_PassiveData _data;
    [SerializeField] TextMeshProUGUI _passiveText;

    public void Init(Player_PassiveData data)
    {
        _data = data;
        SetPassiveText();
    }

    public void SetPassiveText()
    {        
        _passiveText.text = StringManager.Instance.GetString(_data.skillName);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        StudentUIManager.Instance.OnPassiveBoxMouseOverStart(_data);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        StudentUIManager.Instance.OnPassiveBoxMouseOverEnd();
    }
    
}
