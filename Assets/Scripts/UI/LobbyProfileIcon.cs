using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LobbyProfileIcon : MonoBehaviour
{
    [SerializeField] Image _icon;
    [SerializeField] Sprite _defaultIcon;
    
    void Start()
    {
        Refresh();
    }

    public void SetImage(Sprite sprite)
    {
        _icon.sprite = sprite;
    }

    public void Refresh()
    {
        var saveData = GameManager.Instance.SaveData;

        // 데이터가 없거나 이미지 키가 없으면 기본 아이콘
        if (saveData == null || string.IsNullOrEmpty(saveData.currentProfileImage))
        {
            _icon.sprite = _defaultIcon;
        }
        else
        {
            // SpriteManager를 통해 저장된 키값으로 이미지 로드
            var savedSprite = SpriteManager.Instance.GetSprite(saveData.currentProfileImage);
            _icon.sprite = savedSprite != null ? savedSprite : _defaultIcon;
        }
    }
}
