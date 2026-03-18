using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LobbyProfileIcon : MonoBehaviour
{
    [SerializeField] Image _icon;
    [SerializeField] Sprite _defaultIcon;
    
    void Start()
    {
        if (GameManager.Instance.SaveData == null || GameManager.Instance.SaveData.currentProfileImage == null)
        {
            _icon.sprite = _defaultIcon;
        }
        else
        {
            
            _icon.sprite = SpriteManager.Instance.GetSprite(GameManager.Instance.SaveData.currentProfileImage);
        }            
    }

}
