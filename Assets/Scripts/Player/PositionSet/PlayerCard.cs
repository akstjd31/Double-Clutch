using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class PlayerCard : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private Outline _outline;
    [SerializeField] Image _playerImage;
    [SerializeField] Image _playerPosition;
    [SerializeField] Image _playerTraitIcon;
    [SerializeField] TextMeshProUGUI _playerName;
    [SerializeField] TextMeshProUGUI _playerTraitText;
    [SerializeField] TextMeshProUGUI _playerAttackPoint;
    [SerializeField] TextMeshProUGUI _playerDefensePoint;
    [SerializeField] TextMeshProUGUI _playerState;

    private Student _player;
    private bool _isAvailable;
    public Student Player => _player;
    public bool IsAvailable => _isAvailable;

    private void OnEnable()
    {
        StringManager.OnLanguageChanged += Refresh;
    }

    private void OnDisable()
    {
        StringManager.OnLanguageChanged -= Refresh;
    }
    public void Init(Student student)
    {
        SpriteManager spriteManager = SpriteManager.Instance;
        _player = student;

        _playerImage.sprite = spriteManager.GetSprite(_player.VisualData.portraitResource);

        StringManager manager = StringManager.Instance;
        string name = manager.GetString(_player.Name[0]) + manager.GetString(_player.Name[1]) + manager.GetString(_player.Name[2]);

        _playerName.text = name;
        _playerPosition.sprite = spriteManager.GetPositionSprite(student.Position);
        _playerTraitIcon.sprite = spriteManager.GetSprite(student.TraitData.traitResource);

        _playerTraitText.text = manager.GetString(student.TraitData.traitName);
        _playerAttackPoint.text = student.Attack.ToString();
        _playerDefensePoint.text = student.Defense.ToString();

        manager.ApplyFont(_playerName);
        manager.ApplyFont(_playerTraitText);

        if (student.State == StudentState.OverWorked)
        {
            _playerState.text = manager.GetString("UI_Player_과로");
            _isAvailable = false;
        }
        else if (student.State == StudentState.Injured)
        {
            _playerState.text = manager.GetString("UI_Player_부상");
            _isAvailable = false;
        }
        else
        {
            _playerState.text = string.Empty;
            _isAvailable = true;
        }
        manager.ApplyFont(_playerState);


        this.enabled = _isAvailable;
        this.GetComponent<Draggable>().enabled = _isAvailable;

        if (_outline != null)
            _outline.enabled = false;

        Debug.Log($"{student.Name} 플레이어 카드 세팅 완료!");
    }

    public void SetSelected(bool on)
    {
        if (_outline != null) _outline.enabled = on;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        var list = GetComponentInParent<CharacterList>();
        if (list != null)
            list.OnClickCard(this);
    }

    public void SetImageColor(Color color) => this.GetComponent<Image>().color = color;

    private void Refresh()
    {
        StringManager manager = StringManager.Instance;
        string name = manager.GetString(_player.Name[0]) + manager.GetString(_player.Name[1]) + manager.GetString(_player.Name[2]);
        
        _playerName.text = name;
        _playerTraitText.text = manager.GetString(_player.TraitData.traitName);

        manager.ApplyFont(_playerName);
        manager.ApplyFont(_playerTraitText);

        if (_player.State == StudentState.OverWorked)
        {
            _playerState.text = manager.GetString("UI_Player_과로");
            _isAvailable = false;
        }
        else if (_player.State == StudentState.Injured)
        {
            _playerState.text = manager.GetString("UI_Player_부상");
            _isAvailable = false;
        }
        else
        {
            _playerState.text = string.Empty;
            _isAvailable = true;
        }
        manager.ApplyFont(_playerState);
    }
}
