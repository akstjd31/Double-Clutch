using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/// <summary>
/// 상세 설명 온클릭을 위해 프로필 패널의 패시브0, 1, 2에 각각 부착
/// 상세 설명은 PassiveExplainBox에서 관리
/// </summary>
public class PassiveProfileBox : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    Player_PassiveData _data;
    [SerializeField] Text _passiveText;

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
