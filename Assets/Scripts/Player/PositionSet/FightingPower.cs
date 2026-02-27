using TMPro;
using UnityEngine;
/// <summary>
/// FightingPower에 부착할 스크립트
/// 배치된 선수 정보를 PositionSet에서 받아와서 공격력 합산 및 UI에 표기
/// </summary>
public class FightingPower : MonoBehaviour
{
    [SerializeField] MercenaryMaker _mercenaryMaker;
    [SerializeField] TextMeshProUGUI _schoolName;
    [SerializeField] TextMeshProUGUI _fightingPowerText;
    [SerializeField] CharacterPowerBox[] _fightingList = new CharacterPowerBox[5];
    [SerializeField] CharacterList _characterList;
    int _totalFightingPower = 0;    

    public void Init()
    {
        _totalFightingPower = 0;

        // CharacterList에서 현재 배치된 카드 배열을 가져옴
        PlayerCard[] placedCards = _characterList.PositionCards;

        for (int i = 0; i < _fightingList.Length; i++)
        {
            Student targetStudent = null;

            // 1. 해당 슬롯에 유저가 배치한 카드가 있는지 확인
            if (placedCards[i] != null && placedCards[i].Player != null)
            {
                targetStudent = placedCards[i].Player;
            }
            // 2. 카드가 없다면 용병 생성
            else
            {
                // 인덱스 i를 포지션으로 변환 (0:PG, 1:SG, 2:SF, 3:PF, 4:C 라고 가정)
                Position targetPos = (Position)i + 1;
                targetStudent = _mercenaryMaker.MakeMercenary(targetPos);
                targetStudent.OnStatChanged();
                // 용병은 Student.Init을 통해 스탯 기반 공격력/방어력 계산이 필요할 수 있음
                // 만약 Student 내부에서 자동으로 계산되지 않는다면 여기서 강제 호출
                // targetStudent.Init(...) 혹은 내부 계산 메서드 호출
            }

            // 3. CharacterPowerBox에 정보 주입 (용병 포함)
            if (targetStudent != null)
            {
                _fightingList[i].Init(targetStudent);
                _totalFightingPower += (_fightingList[i].Attack + _fightingList[i].Defense);
            }
        }

        SetText();
    }

    public void SetText()
    {
        _schoolName.text = GameManager.Instance.SaveData.schoolName;
        _fightingPowerText.text = _totalFightingPower.ToString();
    }
}
