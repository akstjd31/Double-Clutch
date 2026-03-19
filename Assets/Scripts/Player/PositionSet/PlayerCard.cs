using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class PlayerCard : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private Outline _outline;
    [SerializeField] Image _playerImage;
    [SerializeField] TextMeshProUGUI _playerName;
    [SerializeField] TextMeshProUGUI _playerPosition;
    [SerializeField] TextMeshProUGUI _playerState;

    private Student _player;
    private bool _isAvailable;
    public Student Player => _player;
    public bool IsAvailable => _isAvailable;


    public void Init(Student student)
    {
        _player = student;

        _playerImage.sprite = SpriteManager.Instance.GetSprite(_player.VisualData.portraitResource);

        StringManager manager = StringManager.Instance;
        string name = manager.GetString(_player.Name[0]) + manager.GetString(_player.Name[1]) + manager.GetString(_player.Name[2]);

        _playerName.text = name;
        _playerPosition.text = student.Position.ToString();   
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
}
