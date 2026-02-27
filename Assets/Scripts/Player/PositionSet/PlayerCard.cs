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
        _playerName.text = student.Name;
        _playerPosition.text = _playerPosition.ToString();        
        if (student.State == StudentState.OverWorked)
        {
            _playerState.text = "�Ƿ�";
            _isAvailable = false;
        }
        else if (student.State == StudentState.Injured)
        {
            _playerState.text = "�λ�";
            _isAvailable = false;
        }
        else
        {
            _isAvailable = true;
        }

        if (_outline != null)
            _outline.enabled = false;
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
