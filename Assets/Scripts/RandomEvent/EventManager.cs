using System.Collections.Generic;
using UnityEditor.PackageManager;
using UnityEngine;

[System.Serializable]
public class EventManager : MonoBehaviour
{
    [SerializeField] private Event_DataModelReader _Event_DataModelReader;
    [SerializeField] private StudentManager _studentManager;

    [SerializeField] private List<Student> _myStudents;

    //선수별 이벤트 후보 저장 딕셔너리 : 선수ID, 이벤트
    private Dictionary<int, List<RandomEvent>> _eventListDictionary = new();

    //선수별 발생 이벤트 선정 딕셔너리
    private Dictionary<int, RandomEvent> _studentEventList = new();

    int max;

    //어떤 선수의 이벤트인지 대상 선택.
    //매서드가 호출되면 이벤트가 실행되도록 함.

    //훈련시작 시에 호출
    public void EventCheck()
    {
        _myStudents = StudentManager.Instance.MyStudents;
        CharacterEvent();
        CooldownTurnCheck();
    }


    //학생별 가능한 이벤트 리스트를 딕셔너리에 저장
    //1. 요구 잠재력 조건 만족
    private void CharacterEvent()
    {
        var data = _Event_DataModelReader.DataList;
        _eventListDictionary.Clear();


        //학생들 전체 검사
        for (int i = 0; i < _myStudents.Count; i++)
        {
            var studentID = _myStudents[i].StudentId;

            //등록되지 않은 ID면 새로 딕셔너리에 등록
            if (_eventListDictionary.ContainsKey(studentID) == false)
            {
                _eventListDictionary.Add(studentID, new List<RandomEvent>());
            }

            //모든 이벤트 수만큼 검사
            for (int j = 0; j < data.Count; j++)
            {
                //스텟 검사
                if (_myStudents[i].GetCurrentStat(data[j].mainPotentialType) >= data[j].requiredPotentialValue)
                {
                    //이벤트 id랑 쿨타임 저장
                    _eventListDictionary[studentID].Add(new RandomEvent(data[j].eventId, data[j].cooldownTurn, data[j].eventPriority));
                }
            }
                Debug.Log($"------[1차 후보]------");
            foreach (var n in _eventListDictionary[studentID])
            {
                Debug.Log($"1차 후보 : {i} - {n.EventId}");
            }
                Debug.Log($"----------------------");
        }
    }

    //2. 쿨타임 체크 및 우선순위 비교
    private void CooldownTurnCheck()
    {
        List<RandomEvent> events = new List<RandomEvent>();
        

        //학생들 전체 검사
        for (int i = 0; i < _myStudents.Count; i++)
        {
            events.Clear();

            var studentID = _myStudents[i].StudentId;
            //딕셔너리에 등록되지 않은 학생은 스킵
            if (!_eventListDictionary.TryGetValue(studentID, out var list))
            {
                continue;
            }

            Debug.Log($"쿨타임/우선순위검사");
            //딕셔너리>학생>이벤트리스트 개수만큼 체크
            events = MaxPriorityNumber(list);
                Debug.Log($"------[2차 후보]------");
            foreach(var n in events)
            {
                Debug.Log($"{i} - {n.EventId}");
            }
                Debug.Log($"----------------------");

            //우선순위가 같은 이벤트가 2개 이상이라면 그 중에 하나만 뽑기
            //Q. 우선순위가 같은 세개의 이벤트 중에 하나를 뽑았는데 뽑힌 이벤트가 발생확률이 낮으면 아무 이벤트도 발생하지 않을 수 있나?
            if (events.Count > 1)
            {
                int random = Random.Range(0, events.Count);
                _studentEventList[studentID] = events[random];
                Debug.Log($"------[결과 ]------");
                Debug.Log($"{i} - {events[random].EventId}");
                Debug.Log($"----------------------");
            }
            else if(events.Count == 1)
            {
                //이벤트발생 리스트에 넣기.
                _studentEventList[studentID] = events[0];
                Debug.Log($"------[결과 ]------");
                Debug.Log($"{i} - {events[0].EventId}");
                Debug.Log($"----------------------");
            }
            
        }
    }


    //주차가 끝나면 모든 학생이 가지고 있는 이벤트
    //IsReady = false만 쿨타임 감소
    public void WeekendCooldown()
    {
        var data = _Event_DataModelReader.DataList;

        //학생들 전체 검사
        for (int i = 0; i < _myStudents.Count; i++)
        {
            var studentID = _myStudents[i].StudentId;
            //딕셔너리에 등록되지 않은 학생은 스킵
            if (_eventListDictionary.ContainsKey(studentID) == false)
            {
                continue;
            }

            //딕셔너리>학생>이벤트리스트 개수만큼 체크
            for (int j = 0; j < _eventListDictionary[studentID].Count; j++)
            {
                //쿨다운 시작된 이벤트만 감소
                if (_eventListDictionary[studentID][j].IsReady == false)
                {
                    continue;
                }
                //쿨다운 값 감소
                _eventListDictionary[studentID][j].Cooldown();
            }
        }
    }

    //우선순위 최댓값 찾기
    private List<RandomEvent> MaxPriorityNumber(List<RandomEvent> data)
    {
        max = int.MinValue;
        List<RandomEvent> resultList = new List<RandomEvent>();

        for (int i = 0; i < data.Count; i++)
        {
            int num = data[i].EventPriority;

            //쿨다운 타이머 true인 이벤트만 통과
            if (data[i].IsReady == false)
            {
                continue;
            }

            //현재 숫자가 최대숫자라면
            if (num > max)
            {
                max = num;
                //초기화 후 추가
                resultList.Clear();
                resultList.Add(data[i]);
            }
            //같은 숫자면 그냥 추가
            else if(num == max)
            {
                resultList.Add(data[i]);
            }
        }
        Debug.Log($"--------------------------------");

        return resultList;
    }

    //Context 로드
    //시스템 시작 시 캐릭터 테이블을 조회하여 캐릭터의 현재 스탯과 잠재력을 시스템 메모리에 캐싱
    //실시간으로 캐릭터 스탯에 가감연산을 수행함
    //student > Stat2pt / Stat3pt , 컨디션, 상태

    //EventManager
    //후보 필터링 : 요구 잠재력, 캐릭터의 현재 잠재력 , 쿨타임 체크 > 이벤트 선정
    //가중치 추첨 : 후보 목록 중에서 랜덤으로 이벤트 하나 선택
    //쿨타임관리 : 이벤트 종료 시 이벤트 쿨타임 기록

}
