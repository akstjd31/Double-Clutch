using TMPro;
using UnityEngine.UI;
using UnityEngine;

public class AlbumStudentProfileUI : MonoBehaviour
{
    private Button _profileButton;
    [SerializeField] private Image _profileImage;
    [SerializeField] private TextMeshProUGUI _profileName;
    private void Awake()
    {
        _profileButton = this.GetComponent<Button>();
    }

    public void SetSprite(Sprite sprite)
    {
        _profileImage.sprite = sprite;
    }

    public void SetName(string name)
    {
        _profileName.text = name;
        StringManager.Instance.ApplyFont(_profileName);
    }

    public Button GetButton() => _profileButton;
}
