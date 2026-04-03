using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class CharacterPassiveProfileRow : MonoBehaviour, IPointerUpHandler, IPointerDownHandler
{
    const string DEFAULT_KEY = "UI_Player_비어있음";
    Player_PassiveData _data;
    bool _isEmpty = true;
    [SerializeField] TextMeshProUGUI _passiveText;

    private void OnEnable()
    {
        StringManager.OnLanguageChanged += SetPassiveText;
    }

    private void OnDisable()
    {
        StringManager.OnLanguageChanged -= SetPassiveText;
    }

    public void Init(Player_PassiveData data)
    {
        _data = data;
        _isEmpty = false;
        SetPassiveText();
    }

    public void Init()
    {
        _data = default;
        _isEmpty = true;
        SetPassiveText();
    }

    public void SetPassiveText()
    {
        if (_isEmpty)
        {
            StringManager.Instance.GetString(DEFAULT_KEY, _passiveText);
            StringManager.Instance.ApplyFont(_passiveText);
            return;
        }
        StringManager.Instance.GetString(_data.skillName, _passiveText);
        StringManager.Instance.ApplyFont(_passiveText);
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
