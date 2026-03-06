using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using UnityEngine;


/// <summary>
/// CharacterList ????????? ?????? ??? ???? ??? ????
/// </summary>
public class CharacterList : MonoBehaviour
{
    private const int MAX_BATCH_COUNT = 5;
    [SerializeField] PlayerCard _playerCardPrefab;
    [SerializeField] Transform _cardContainer; //????? ??? ???
    [SerializeField] Transform _positionTrf;
    [SerializeField] GameObject _matchStartPanelObj;
    [SerializeField] FightingPower _fightingPower;


    GenericObjectPool<PlayerCard> _playerCardPool;

    [SerializeField] private List<PlayerCard> _cardList = new List<PlayerCard>();
    public List<PlayerCard> CardList => _cardList;


    [SerializeField] private PlayerCard[] _positionCards;
    public PlayerCard[] PositionCards => _positionCards;

    [SerializeField] private DropPosition[] _dropPositions;
    private PlayerCard _selectedCard;
    private DropPosition _selectedPosition;
    [SerializeField] MercenaryMaker _mercenaryMaker; // 용병 생성기


    private int _colorIndex;
    private readonly Color[] _colors =
    {
        Color.red,
        new Color(1f, 0.5f, 0f),
        Color.yellow,
        Color.green,
        Color.blue,
        new Color(0.3f, 0f, 0.5f),
        new Color(0.56f, 0f, 1f)
    };


    private void Awake()
    {
        _playerCardPool = new GenericObjectPool<PlayerCard>(_playerCardPrefab, _cardContainer, 5, 20);

        _positionCards = new PlayerCard[MAX_BATCH_COUNT];
    }

private void OnEnable()
{
    var data = CheckSaveData();

    ClearAllCards();

    // 각 StudentId 가져오기
    HashSet<int> placedIds = new HashSet<int>();
    if (data != null && data.studentList != null)
    {
        for (int i = 0; i < data.studentList.Count && i < _dropPositions.Length; i++)
        {
            placedIds.Add(data.studentList[i].StudentId);
        }
    }

    // 카드 생성하는데 있어 배치 선수(_positionCard), 보유 선수(CardList)를 구분하여 집어넣는다
    Dictionary<int, PlayerCard> cardMap = new Dictionary<int, PlayerCard>();

    foreach (Student student in StudentManager.Instance.MyStudents)
    {
        PlayerCard card = _playerCardPool.Get();
        card.Init(student);

        int id = student.StudentId;
        if (!cardMap.ContainsKey(id))
            cardMap.Add(id, card);

        if (!placedIds.Contains(id))
        {
            // 하단 리스트로
            CardList.Add(card);
            card.transform.SetParent(_cardContainer, false);
            card.transform.SetAsLastSibling();
        }
        else
        {
            card.gameObject.SetActive(true);
        }
    }

    // 3) 저장 데이터가 있으면 studentList 순서대로 배치
    if (data == null || data.studentList == null) return;

    for (int i = 0; i < data.studentList.Count; i++)
    {
        if (i >= _dropPositions.Length) break;

        var savedStudent = data.studentList[i];

        if (!cardMap.TryGetValue(savedStudent.StudentId, out PlayerCard card))
            continue;

        AddOnPosition(card, _dropPositions[i]);
    }
}

    private StudentSaveData CheckSaveData()
    {
        if (SaveLoadManager.Instance != null)
        {
            bool hasMyStdData = SaveLoadManager.Instance.TryLoad<StudentSaveData>(FilePath.MY_STUDENT_MATCHING_PATH, out var stdData);

            int idx = PlayerPrefs.GetInt(PrefKeys.MATCH_PREP_UI_INDEX);

            if (hasMyStdData && stdData.studentList.Count > 0)
            {

                if (idx == 1)
                {
                    // 뒤로 가기 버튼 비활성화까지 넣어놓기
                    _matchStartPanelObj.SetActive(true);
                }
                else
                {
                    _fightingPower.gameObject.SetActive(true);
                    _fightingPower.Init();
                    this.gameObject.SetActive(false);
                }

                return stdData;

            }
            else
            {
                _fightingPower.gameObject.SetActive(false);
                _matchStartPanelObj.SetActive(false);
                return null;
            }
        }

        return null;
    }

    public void OnClickPosition(DropPosition dPos)
    {
        if (dPos == null) return;

        if (_selectedPosition == dPos)
        {
            ClearSelectedCards();
            TryPlaceSelected();
            return;
        }

        ClearSelectedPosition();

        _selectedPosition = dPos;
        _selectedPosition.SetSelected(true);

        TryPlaceSelected();
    }

    public void OnClickCard(PlayerCard card)
    {
        if (card == null) return;

        if (_selectedCard == card)
        {
            ClearSelectedCards();
            TryPlaceSelected();
            return;
        }

        ClearSelectedCards();

        _selectedCard = card;
        _selectedCard.SetSelected(true);

        TryPlaceSelected();
    }

    private void TryPlaceSelected()
    {
        if (_selectedCard == null || _selectedPosition == null) return;

        // 선택된 카드가 이미 다른 포지션에 배치된 상태여도
        // AddOnPosition 내부에서 already 처리 + 교체 처리함
        bool placed = AddOnPosition(_selectedCard, _selectedPosition);

        // 배치가 성공하면 선택 해제
        if (placed)
        {
            ClearSelectedCards();
            ClearSelectedPosition();
        }
        // 실패 경우
        else
        {
            ClearSelectedCards();
        }
    }

    // 선택된 포지션 null 처리
    private void ClearSelectedPosition()
    {
        if (_selectedPosition != null)
            _selectedPosition.SetSelected(false);

        _selectedPosition = null;
    }

    // 선택된 카드 null 처리
    private void ClearSelectedCards()
    {
        if (_selectedCard != null)
            _selectedCard.SetSelected(false);

        _selectedCard = null;
    }

    public Color GetNextColor()
    {
        Color c = _colors[_colorIndex];
        _colorIndex = (_colorIndex + 1) % _colors.Length;
        return c;
    }

    public void ReFresh()
    {
        for (int i = 0; i < CardList.Count; i++)
        {
            CardList[i].transform.SetSiblingIndex(i);
        }
    }

    public bool CheckMaxPositionBatch()
    {
        // 남은 카드가 없다?
        if (_cardList == null || _cardList.Count == 0)
            return true;

        // 배치 가능한 카드가 하나도 없으면(경기 참가 불가능 플레이어 존재) 더 배치할 수 없음
        bool hasAvailableCard = false;
        for (int i = 0; i < _cardList.Count; i++)
        {
            var card = _cardList[i];
            if (card != null && card.IsAvailable)
            {
                hasAvailableCard = true;
                break;
            }
        }
        if (!hasAvailableCard)
            return true;

        // 포지션 슬롯이 없거나 길이가 0이면 꽉 찬 것으로
        if (_positionCards == null || _positionCards.Length == 0)
            return true;

        // 슬롯이 하나라도 비어있으면 아직 최대 아님
        int limit = Mathf.Min(MAX_BATCH_COUNT, _positionCards.Length);
        for (int i = 0; i < limit; i++)
        {

            if (_positionCards[i] == null)
                return false;
        }

        // 여기까지 왔으면 limit 범위 내 슬롯이 다 참
        return true;
    }

    public void OnMatchStartButtonClick()
    {
        PlayerPrefs.SetInt(PrefKeys.MATCH_PREP_UI_INDEX, 2);
        _fightingPower.gameObject.SetActive(true);
        _fightingPower.Init();
        _fightingPower.SaveRivalMachingStudentData();
        gameObject.SetActive(false);
    }

    public bool AddOnPosition(PlayerCard card, DropPosition dPos)
    {
        if (card == null || dPos == null) return false;

        EnsureArrays();

        int idx = GetSlotIndex(dPos);
        if (idx < 0) return false;

        // 같은 카드가 다른 슬롯에 이미 있으면 제거
        int already = IndexOfCard(card);
        if (already >= 0 && already != idx)
        {
            _positionCards[already] = null;
        }

        // 교체
        var prev = _positionCards[idx];
        if (prev != null && prev != card)
        {
            if (!_cardList.Contains(prev)) _cardList.Add(prev);
            prev.transform.SetParent(_cardContainer, false);
            prev.transform.SetAsLastSibling();
        }

        // 배치
        _positionCards[idx] = card;

        _cardList.Remove(card);

        card.transform.SetParent(_dropPositions[idx].transform, false);
        card.transform.SetAsLastSibling();

        card.transform.SetParent(dPos.transform, true);
        var rect = (RectTransform)card.transform;
        SetAnchor(rect);

        // 포지셔닝이 완료되었다면 버튼 활성화 (용병 테스트는 해당 액티브를 true로 하면 됨)
        if (CheckMaxPositionBatch())
        {
            // 배치된 선수 저장 및 새로운 UI 인덱스 갱신
            SaveBatchStudentData();
            PlayerPrefs.SetInt(PrefKeys.MATCH_PREP_UI_INDEX, 1);
            _matchStartPanelObj.SetActive(true);
        }

        return true;
    }

    // 배치한 학생 정보 저장
    public void SaveBatchStudentData()
    {
        if (_positionCards == null || _positionCards.Length < 1) return;

        var sList = new List<Student>();

        for (int i = 0; i < _positionCards.Length; i++)
        {
            // 용병 생성
            if (_positionCards[i] == null)
            {
                Position targetPos = (Position)i + 1;
                if (_mercenaryMaker != null)
                {
                    var mercenary = _mercenaryMaker.MakeMercenary(targetPos);
                    mercenary.OnStatChanged();
                    sList.Add(mercenary);
                    continue;
                }
            }

            sList.Add(_positionCards[i].Player);
        }

        var batchData = new StudentSaveData(MAX_BATCH_COUNT, sList);

        if (SaveLoadManager.Instance == null) return;
        SaveLoadManager.Instance.Save(FilePath.MY_STUDENT_MATCHING_PATH, batchData);
    }

    public void SetAnchor(RectTransform rect)
    {
        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = Vector2.zero; // 부모 기준 정확히 중앙
    }

    public PlayerCard RemoveOnPosition(PlayerCard card)
    {
        if (card == null) return null;

        EnsureArrays();

        for (int i = 0; i < _positionCards.Length; i++)
        {
            if (_positionCards[i] == card)
            {
                _positionCards[i] = null;
                break;
            }
        }

        // 리스트에 없으면 복귀
        if (!_cardList.Contains(card))
            _cardList.Add(card);

        card.transform.SetParent(_cardContainer, false);
        card.transform.SetAsLastSibling();

        return card;
    }

    private void EnsureArrays()
    {
        if (_positionCards == null || _positionCards.Length != MAX_BATCH_COUNT)
            _positionCards = new PlayerCard[MAX_BATCH_COUNT];

        if (_cardList == null)
            _cardList = new List<PlayerCard>();
    }

    private int GetSlotIndex(DropPosition dPos) => IsCardInPositionSlots(dPos);

    private int IndexOfCard(PlayerCard card)
    {
        if (_positionCards == null) return -1;

        for (int i = 0; i < _positionCards.Length; i++)
        {
            if (_positionCards[i] == card) return i;
        }
        return -1;
    }

    // 배치된 카드의 현재 인덱스 반환 (하단 보유 카드 -> 포지션 배치시에만 사용)
    private int IsCardInPositionSlots(DropPosition dPos)
    {
        if (_dropPositions == null) return -1;

        for (int i = 0; i < _positionCards.Length; i++)
        {
            if (_dropPositions[i].Equals(dPos))
                return i;
        }

        return -1;
    }

    private void ClearAllCards()
    {
        // CardList?? ??? ??? ???
        foreach (var card in _cardList)
        {
            if (card != null) _playerCardPool.Release(card);
        }
        _cardList.Clear();

        // ???? ????? ??? ???? ??? ???
        if (_dropPositions != null)
        {
            foreach (var position in _dropPositions)
            {
                if (position.transform.childCount > 1) _playerCardPool.Release(position.GetComponentInChildren<PlayerCard>());
            }

        }

        for (int i = 0; i < _positionCards.Length; i++)
            _positionCards[i] = null;
    }
}
