using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class PassiveProfileBox : MonoBehaviour, IPointerUpHandler, IPointerDownHandler
{
    const string DEFAULT_TEXT_KEY = "UI_Player_비어있음";
    Player_PassiveData _data;
    bool _isEmpty = true;
    [SerializeField] Image _passiveIcon;
    [SerializeField] Image _passiveFrame;
    [SerializeField] CanvasGroup _imageParent;
    [SerializeField] TextMeshProUGUI _passiveText;

    const string FRAME1 = "Image_Passive_Frame_001";
    const string FRAME2 = "Image_Passive_Frame_002";
    const string FRAME3 = "Image_Passive_Frame_003";
    const string FRAME4 = "Image_Passive_Frame_004";

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
        SetPassiveImage(data);
    }

    private void SetPassiveImage(Player_PassiveData data)
    {
        SpriteManager spriteManager = SpriteManager.Instance;
        _passiveIcon.sprite = spriteManager.GetSprite(data.passiveResource);

        Sprite frame = null;
        switch(data.grade)
        {
            case 1: 
                frame = spriteManager.GetSprite(FRAME1);
                break;
            case 2:
                frame = spriteManager.GetSprite(FRAME2);
                break;
            case 3:
                frame = spriteManager.GetSprite(FRAME3);
                break;
            case 4:
                frame = spriteManager.GetSprite(FRAME4);
                break;

        }
        _passiveFrame.sprite = frame;

        _imageParent.alpha = 1;
    }

    public void Init()
    {
        _data = default;
        _isEmpty = true;

        SetPassiveText();
        _passiveFrame.sprite = null;
        _passiveFrame.sprite = null;
        _imageParent.alpha = 0;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        StudentUIManager.Instance.OnPassiveBoxMouseOverEnd();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        StudentUIManager.Instance.OnPassiveBoxMouseOverStart(_data);
    }

    public void SetPassiveText()
    {
        if (_isEmpty)
        {
            StringManager.Instance.GetString(DEFAULT_TEXT_KEY, _passiveText);
            StringManager.Instance.ApplyFont(_passiveText);
            return;
        }
        StringManager.Instance.GetString(_data.skillName, _passiveText);
        StringManager.Instance.ApplyFont(_passiveText);
    }
}
