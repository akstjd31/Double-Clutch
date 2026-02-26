using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerCard : MonoBehaviour
{
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
    }

    public void SetImageColor(Color color) => this.GetComponent<Image>().color = color;
}
