using System.Collections.Generic;
using System.Linq;
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
    // 뒤로가기 버튼 연결용 변수
    [SerializeField] private GameObject _backButtonObj;

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
        // UI가 켜질 때 리그 상태를 확인하여 뒤로가기 버튼을 제어합니다.
        CheckBackButtonVisibility();

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

        UpdateMatchStartUI();

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
            SaveLoadManager.Instance.TryLoad<StudentSaveData>(FilePath.MY_STUDENT_MATCHING_PATH, out var stdData);

            int idx = PlayerPrefs.GetInt(PrefKeys.MATCH_PREP_UI_INDEX);

            if (stdData != null && stdData.studentList.Count > 0)
            {
                StudentManager.Instance.SetCurrentTeam(stdData.studentList);
                if (idx == 1)
                {
                    // 뒤로 가기 버튼 비활성화까지 넣어놓기
                    _matchStartPanelObj.SetActive(true);
                    if (_fightingPower != null)
                    {
                        _fightingPower.gameObject.SetActive(false);
                    }
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
        if (!card.IsAvailable) return;

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
        for (int i = 0; i < _positionCards.Length && i < _dropPositions.Length; i++)
        {
            var card = _positionCards[i];
            if (card == null) continue;

            if (card.transform.parent != _dropPositions[i].transform)
            {
                card.transform.SetParent(_dropPositions[i].transform, false);
            }

            card.transform.SetAsLastSibling();

            RectTransform rect = card.transform as RectTransform;
            if (rect != null)
                ResetCardRect(rect);

            card.gameObject.SetActive(true);
        }

        // 하단 카드들의 부모/순서 다시 맞추기
        for (int i = 0; i < _cardList.Count; i++)
        {
            var card = _cardList[i];
            if (card == null) continue;

            // 혹시 배치 슬롯에 들어가 있는 카드면 제외
            if (IndexOfCard(card) >= 0)
                continue;

            if (card.transform.parent != _cardContainer)
            {
                card.transform.SetParent(_cardContainer, false);
            }

            card.transform.SetSiblingIndex(i);

            RectTransform rect = card.transform as RectTransform;
            if (rect != null)
                ResetCardRect(rect);

            card.gameObject.SetActive(true);
        }

        // 혹시 드롭 포지션 자식 중 _positionCards에 없는 카드가 있으면 하단으로 이동
        if (_dropPositions != null)
        {
            for (int i = 0; i < _dropPositions.Length; i++)
            {
                var pos = _dropPositions[i];
                if (pos == null) continue;

                var childCard = pos.GetComponentInChildren<PlayerCard>();
                if (childCard == null) continue;

                if (i >= _positionCards.Length || _positionCards[i] != childCard)
                {
                    MoveToCardList(childCard);
                }
            }
        }

        UpdateMatchStartUI();
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
        //_fightingPower.SaveRivalMachingStudentData();
        gameObject.SetActive(false);
    }

    public bool AddOnPosition(PlayerCard card, DropPosition dPos)
    {
        if (card == null || dPos == null) return false;

        EnsureArrays();

        int idx = GetSlotIndex(dPos);
        if (idx < 0) return false;

        // 이 카드가 이미 다른 슬롯에 있으면 제거
        int alreadyIdx = IndexOfCard(card);
        if (alreadyIdx >= 0 && alreadyIdx != idx)
        {
            _positionCards[alreadyIdx] = null;
        }

        // 현재 슬롯에 카드가 있으면 하단 리스트로 복귀
        PlayerCard prevCard = _positionCards[idx];
        if (prevCard != null && prevCard != card)
        {
            _positionCards[idx] = null;

            if (!_cardList.Contains(prevCard))
                _cardList.Add(prevCard);

            prevCard.transform.SetParent(_cardContainer, false);
            prevCard.transform.SetAsLastSibling();

            RectTransform prevRect = prevCard.transform as RectTransform;
            if (prevRect != null)
                ResetCardRect(prevRect);
        }

        // 하단 리스트에 있던 카드 제거
        _cardList.Remove(card);

        // 새 카드 배치
        _positionCards[idx] = card;
        card.transform.SetParent(dPos.transform, false);
        card.transform.SetAsLastSibling();

        RectTransform rect = card.transform as RectTransform;
        if (rect != null)
            ResetCardRect(rect);

        UpdateMatchStartUI();

        return true;
    }

    // 카드 배치 시 Rect 리셋 (부모에 레이아웃 그룹 유무에 따라 달라지기 떄문)
    private void ResetCardRect(RectTransform rect)
    {
        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = Vector2.zero;
        rect.localScale = Vector3.one;
        rect.localRotation = Quaternion.identity;
    }

    public bool MoveToCardList(PlayerCard card)
    {
        if (card == null) return false;

        EnsureArrays();

        for (int i = 0; i < _positionCards.Length; i++)
        {
            if (_positionCards[i] == card)
            {
                _positionCards[i] = null;
                break;
            }
        }

        if (!_cardList.Contains(card))
            _cardList.Add(card);

        card.transform.SetParent(_cardContainer, false);
        card.transform.SetAsLastSibling();

        RectTransform rect = card.transform as RectTransform;
        if (rect != null)
            ResetCardRect(rect);

        UpdateMatchStartUI();
        return true;
    }

    private void UpdateMatchStartUI()
    {
        bool canStart = CheckMaxPositionBatch();

        _matchStartPanelObj.SetActive(canStart);

        if (canStart)
        {
            _backButtonObj.SetActive(canStart);
            SaveBatchStudentData();
            PlayerPrefs.SetInt(PrefKeys.MATCH_PREP_UI_INDEX, 1);
        }
    }

    // 뒤로가기 버튼을 눌렀을 떄 배치 초기화(기존 배치 인원들 보유 선수로 이동) 겸 데이터 초기화
    public void ClearData()
    {
        // 준비 시작 버튼이 떠있지 않다면 (배치 인원이 5명이 충족이 안됨)
        if (!_matchStartPanelObj.activeSelf)
        {
            ClearAllCards();
            this.gameObject.SetActive(false);
            ReFresh();
            return;
        }

        // 기존 배치된 카드들을 보유 선수 리스트로 옮긴 후 해당 자리 비우기
        for (int i = 0; i < MAX_BATCH_COUNT; i++)
        {
            if (_cardList.Contains(_positionCards[i]))
            {
                Debug.LogError("이미 배치에 옮긴 선수인데, 이건 발생되면 뭔가 문제가 있는거임! (보유 선수에 이미 배치 선수의 정보가 있다??)");
                return;
            }
            _cardList.Add(_positionCards[i]);
            _positionCards[i] = null;
        }

        // 껍데기 저장
        var data = new StudentSaveData();
        SaveLoadManager.Instance.Save(FilePath.MY_STUDENT_MATCHING_PATH, data);

        PlayerPrefs.SetInt(PrefKeys.MATCH_PREP_UI_INDEX, 0);

        ReFresh();
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

        if (StudentManager.Instance != null) StudentManager.Instance.SetCurrentTeam(sList);
        var batchData = new StudentSaveData(MAX_BATCH_COUNT, sList, StudentManager.Instance.CurrentTeam);

        if (StudentManager.Instance == null) return;
        SaveLoadManager.Instance.Save(FilePath.MY_STUDENT_MATCHING_PATH, batchData);
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

    // 리그 진행 상태에 따라 뒤로가기 버튼 켜기/끄기
    private void CheckBackButtonVisibility()
    {
        if (_backButtonObj == null) return;

        bool isMidLeague = false;
        var currentLeague = LeagueManager.Instance.CurrentLeague;

        // 현재 진행 중인 리그가 있고, 아직 종료되지 않았다면
        if (currentLeague != null && !currentLeague.isFinished)
        {
            // 0라운드가 아니다 = 이미 1라운드(첫 경기)를 치르고 2라운드 이상 진행 중이다
            if (currentLeague.currentRoundIndex > 0)
            {
                isMidLeague = true;
            }
        }

        // 리그 중간(isMidLeague == true)이면 버튼을 숨기고, 아니면 켭니다.
        _backButtonObj.SetActive(!isMidLeague);
    }
}
