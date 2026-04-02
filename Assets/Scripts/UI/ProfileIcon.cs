using UnityEngine;
using UnityEngine.UI;

public class ProfileIcon : MonoBehaviour
{
    [SerializeField] Image _image;
    [SerializeField] Image _frame;
    [SerializeField] Button _button;
    [SerializeField] Outline _outLine;
    [SerializeField] Sprite _lockIcon;
    ProfileData? _data;
    Sprite _iconSprite;

    public ProfileData? Data => _data;

    public void Init(ProfileData data)
    {
        this._data = data;
        _image.sprite = _lockIcon;
        _button.interactable = false;
        _iconSprite = SpriteManager.Instance.GetSprite(data.playerImage);
        _image.enabled = true; // 이미지가 꺼져있을 경우를 대비            
        _frame.enabled = true; // 이미지가 꺼져있을 경우를 대비            
        OnOffOutLine(false);
    }

    public void Unlock()
    {
        _image.sprite = _iconSprite;
        _button.interactable = true;
    }

    public Button GetButton()
    {
        return _button;
    }

    public void OnOffOutLine(bool isOn)
    {
        _outLine.enabled = isOn;
    }
}