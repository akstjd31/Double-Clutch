using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class PassiveProfileBox : MonoBehaviour, IPointerUpHandler, IPointerDownHandler
{
    const string DEFAULT_TEXT = "비어 있음";
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
        _passiveText.text = DEFAULT_TEXT;
    }

    public void SetPassiveText()
    {
        if (_isEmpty) return;
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
