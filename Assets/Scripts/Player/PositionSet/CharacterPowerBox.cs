using TMPro;
using UnityEngine;
using UnityEngine.UI;
 /// <summary>
 /// 전력 비교창 UI 표시용(학생 한 명에 해당)
 /// </summary>
public class CharacterPowerBox : MonoBehaviour
{
    //[SerializeField] Image _characterImage;
    [SerializeField] TextMeshProUGUI _characterName;
    [SerializeField] TextMeshProUGUI _characterPosition;
    [SerializeField] TextMeshProUGUI _characterAttack;
    [SerializeField] TextMeshProUGUI _characterDefense;

    private Student _player;
    private int _attack;
    private int _defense;

    public Student Player => _player;
    public int Attack => _attack;
    public int Defense => _defense;

    public void Init(Student target)
    {
        _player = target; // 실제 매칭 시 정보 전달을 위한 플레이어 세팅
        //_characterImage = 나중에 받아오기
        _attack = _player.Attack;
        _defense = _player.Defense;

        SetUI();
    }

    public void SetUI()
    {
        _characterName.text = _player.Name;
        _characterPosition.text = _player.Position.ToString();
        _characterAttack.text = _player.Attack.ToString();
        _characterDefense.text = _player.Defense.ToString();
    }
}
