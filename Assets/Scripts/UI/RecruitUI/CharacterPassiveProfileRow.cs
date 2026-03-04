using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class CharacterPassiveProfileRow : MonoBehaviour, IPointerUpHandler, IPointerDownHandler
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

    public void OnPointerUp(PointerEventData eventData)
    {
        StudentUIManager.Instance.OnPassiveProfilePopUpEnd();
    }

    public void OnPointerDown(PointerEventData eventData)
    {        
        StudentUIManager.Instance.OnPassiveProfilePopUpStart(_data);
    }
}
