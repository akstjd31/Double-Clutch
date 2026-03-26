using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// CharacterList 카드 배치 및 보유 카드 리스트 관리
/// </summary>
public class CharacterList : MonoBehaviour
{
    private const int MAX_BATCH_COUNT = 5;

    [SerializeField] private PlayerCard _playerCardPrefab;
    [SerializeField] private Transform _cardContainer;
    [SerializeField] private Transform _positionTrf;
    [SerializeField] private GameObject _matchStartPanelObj;
    [SerializeField] private FightingPower _fightingPower;
    [SerializeField] private GameObject _backButtonObj;

    [SerializeField] private DropPosition[] _dropPositions;
    [SerializeField] private MercenaryMaker _mercenaryMaker;

    [SerializeField] private Image[] _synergyIcons = new Image[10];

    private GenericObjectPool<PlayerCard> _playerCardPool;

    [SerializeField] private List<PlayerCard> _cardList = new List<PlayerCard>();
    public List<PlayerCard> CardList => _cardList;

    [SerializeField] private PlayerCard[] _positionCards;
    public PlayerCard[] PositionCards => _positionCards;

    private PlayerCard _selectedCard;
    private DropPosition _selectedPosition;

    private readonly HashSet<string> _tempTraitIds = new HashSet<string>();
    private readonly List<PlayerSynergyData> _activeSynergies = new List<PlayerSynergyData>();    

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
        EnsureArrays();
        //CheckBackButtonVisibility();

        var data = CheckSaveData();

        ClearAllCards();

        HashSet<int> placedIds = new HashSet<int>();
        if (data != null && data.studentList != null)
        {
            for (int i = 0; i < data.studentList.Count && i < _dropPositions.Length; i++)
            {
                if (data.studentList[i] != null)
                    placedIds.Add(data.studentList[i].StudentId);
            }
        }

        Dictionary<int, PlayerCard> cardMap = new Dictionary<int, PlayerCard>();

        if (StudentManager.Instance != null && StudentManager.Instance.MyStudents != null)
        {
            foreach (Student student in StudentManager.Instance.MyStudents)
            {
                if (student == null) continue;

                PlayerCard card = _playerCardPool.Get();
                card.Init(student);

                int id = student.StudentId;
                if (!cardMap.ContainsKey(id))
                    cardMap.Add(id, card);

                if (!placedIds.Contains(id))
                {
                    _cardList.Add(card);
                    card.transform.SetParent(_cardContainer, false);
                    card.transform.SetAsLastSibling();
                    ResetCardRect(card.transform as RectTransform);
                }
                else
                {
                    card.gameObject.SetActive(true);
                }
            }
        }

        if (data != null && data.studentList != null)
        {
            for (int i = 0; i < data.studentList.Count; i++)
            {
                if (i >= _dropPositions.Length) break;

                var savedStudent = data.studentList[i];
                if (savedStudent == null) continue;

                if (!cardMap.TryGetValue(savedStudent.StudentId, out PlayerCard card))
                    continue;

                AddOnPosition(card, _dropPositions[i]);
            }
        }
    }

    private StudentSaveData CheckSaveData()
    {
        if (SaveLoadManager.Instance == null)
            return null;

        SaveLoadManager.Instance.TryLoad<StudentSaveData>(FilePath.MY_STUDENT_MATCHING_PATH, out var stdData);

        int idx = PlayerPrefs.GetInt(PrefKeys.MATCH_PREP_UI_INDEX, 0);
        Debug.Log($"현재 인덱스: {idx}");

        // 기본 상태 먼저 초기화
        if (_matchStartPanelObj != null)
            _matchStartPanelObj.SetActive(false);

        if (_fightingPower != null)
            _fightingPower.gameObject.SetActive(false);

        this.gameObject.SetActive(true);

        // 저장 데이터가 없으면 그대로 종료
        if (stdData == null || stdData.studentList == null || stdData.studentList.Count <= 0)
            return null;

        if (StudentManager.Instance != null)
            StudentManager.Instance.SetCurrentTeam(stdData.studentList);

        switch (idx)
        {
            case 1:
                // 배치 완료, 시작 전 상태
                if (_matchStartPanelObj != null)
                    _matchStartPanelObj.SetActive(true);
                break;

            case 2:
                // 매치 시작 눌러서 전투력 화면으로 넘어간 상태
                if (_fightingPower != null)
                {
                    _fightingPower.gameObject.SetActive(true);
                    _fightingPower.Init();
                }

                this.gameObject.SetActive(false);
                break;

            default:
                // idx == 0 또는 예상 밖 값
                // 아직 배치 전 상태이므로 기본 초기화 상태 유지
                break;
        }

        return stdData;
    }

    public void OnClickPosition(DropPosition dPos)
    {
        if (dPos == null) return;

        if (_selectedPosition == dPos)
        {
            ClearSelectedPosition();
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
        if (_selectedCard == null || _selectedPosition == null)
            return;

        bool placed = AddOnPosition(_selectedCard, _selectedPosition);

        if (placed)
        {
            ClearSelectedCards();
            ClearSelectedPosition();
        }
        else
        {
            ClearSelectedCards();
        }
    }

    private void ClearSelectedPosition()
    {
        if (_selectedPosition != null)
            _selectedPosition.SetSelected(false);

        _selectedPosition = null;
    }

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
        EnsureArrays();

        _cardList.RemoveAll(card => card == null);
        _cardList = _cardList.Distinct().ToList();

        for (int i = 0; i < _positionCards.Length; i++)
        {
            var placedCard = _positionCards[i];
            if (placedCard != null)
                _cardList.Remove(placedCard);
        }

        for (int i = 0; i < _positionCards.Length && i < _dropPositions.Length; i++)
        {
            var card = _positionCards[i];
            if (card == null) continue;

            if (card.transform.parent != _dropPositions[i].transform)
                card.transform.SetParent(_dropPositions[i].transform, false);

            card.transform.SetAsLastSibling();
            ResetCardRect(card.transform as RectTransform);
            card.gameObject.SetActive(true);
        }

        for (int i = 0; i < _cardList.Count; i++)
        {
            var card = _cardList[i];
            if (card == null) continue;
            if (IndexOfCard(card) >= 0) continue;

            if (card.transform.parent != _cardContainer)
                card.transform.SetParent(_cardContainer, false);

            card.transform.SetSiblingIndex(i);
            ResetCardRect(card.transform as RectTransform);
            card.gameObject.SetActive(true);
        }

        if (_dropPositions != null)
        {
            for (int i = 0; i < _dropPositions.Length; i++)
            {
                var pos = _dropPositions[i];
                if (pos == null) continue;

                var childCards = pos.GetComponentsInChildren<PlayerCard>(true);
                foreach (var childCard in childCards)
                {
                    if (childCard == null) continue;

                    bool isCorrectCard = (i < _positionCards.Length && _positionCards[i] == childCard);
                    if (!isCorrectCard)
                    {
                        MoveToCardList(childCard);
                    }
                }
            }
        }

        UpdateMatchStartUI();
    }

    public bool CheckMaxPositionBatch()
    {
        if (_cardList == null || _cardList.Count == 0)
            return true;

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

        if (_positionCards == null || _positionCards.Length == 0)
            return true;

        int limit = Mathf.Min(MAX_BATCH_COUNT, _positionCards.Length);
        for (int i = 0; i < limit; i++)
        {
            if (_positionCards[i] == null)
                return false;
        }

        return true;
    }

    public void OnMatchStartButtonClick()
    {
        PlayerPrefs.SetInt(PrefKeys.MATCH_PREP_UI_INDEX, 2);

        if (_fightingPower != null)
        {
            _fightingPower.gameObject.SetActive(true);
            _fightingPower.Init();
        }

        _matchStartPanelObj.SetActive(false);
        gameObject.SetActive(false);
    }

    public bool AddOnPosition(PlayerCard card, DropPosition dPos)
    {
        if (card == null || dPos == null) return false;

        EnsureArrays();

        int idx = GetSlotIndex(dPos);
        if (idx < 0) return false;

        int alreadyIdx = IndexOfCard(card);
        if (alreadyIdx >= 0 && alreadyIdx != idx)
        {
            _positionCards[alreadyIdx] = null;
        }

        PlayerCard prevCard = _positionCards[idx];
        if (prevCard != null && prevCard != card)
        {
            _positionCards[idx] = null;
            MoveToCardList(prevCard);
        }

        _cardList.Remove(card);

        _positionCards[idx] = card;
        card.transform.SetParent(dPos.transform, false);
        card.transform.SetAsLastSibling();
        ResetCardRect(card.transform as RectTransform);

        UpdateMatchStartUI();
        return true;
    }

    private void ResetCardRect(RectTransform rect)
    {
        if (rect == null) return;

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

        if (card.transform.parent != _cardContainer)
            card.transform.SetParent(_cardContainer, false);

        card.transform.SetAsLastSibling();
        ResetCardRect(card.transform as RectTransform);

        UpdateMatchStartUI();
        return true;
    }

    private void UpdateMatchStartUI()
    {
        bool canStart = CheckMaxPositionBatch();
        CheckSynergy();
        if (_matchStartPanelObj != null)
            _matchStartPanelObj.SetActive(canStart);

        if (_backButtonObj != null)
            _backButtonObj.SetActive(canStart);

        if (canStart)
        {
            SaveBatchStudentData();
            PlayerPrefs.SetInt(PrefKeys.MATCH_PREP_UI_INDEX, 1);
        }
    }

    private void CheckSynergy()
    {        
        if (StudentManager.Instance == null || StudentManager.Instance.GetFactory() == null) return;
        
        _tempTraitIds.Clear();
        _activeSynergies.Clear();

        
        for (int i = 0; i < _positionCards.Length; i++)
        {
            var card = _positionCards[i];
            if (card != null && card.Player != null && card.Player.TraitId != string.Empty)
            {
                _tempTraitIds.Add(card.Player.TraitData.traitId);
            }
        }
        
        if (_tempTraitIds.Count < 2)
        {
            ClearSynergyUI();
            return;
        }        

        var allSynergyData = StudentManager.Instance.GetFactory().GetSynergyDataList();
        if (allSynergyData == null)
        {
            Debug.LogError("SynergyDataList가 Null입니다! 데이터 로드 상태를 확인하세요.");
            ClearSynergyUI();
            return;
        }

        int totalCount = allSynergyData.Count;

        for (int i = 0; i < totalCount; i++)
        {
            var synergy = allSynergyData[i];
            if (_tempTraitIds.Contains(synergy.traitId1) && _tempTraitIds.Contains(synergy.traitId2))
            {
                _activeSynergies.Add(synergy);                
                if (_activeSynergies.Count >= _synergyIcons.Length) break;
            }
        }
        
        UpdateSynergyUI();
    }

    private void UpdateSynergyUI()
    {
        for (int i = 0; i < _synergyIcons.Length; i++)
        {
            if (i < _activeSynergies.Count)
            {
                _synergyIcons[i].gameObject.SetActive(true);                
                _synergyIcons[i].sprite = SpriteManager.Instance.GetSprite(_activeSynergies[i].synergyResource);
            }
            else
            {
                _synergyIcons[i].gameObject.SetActive(false);
            }
        }
    }

    private void ClearSynergyUI()
    {
        for (int i = 0; i < _synergyIcons.Length; i++)
        {
            if (_synergyIcons[i].gameObject.activeSelf)
                _synergyIcons[i].gameObject.SetActive(false);
        }
    }

    public void ClearData()
    {
        EnsureArrays();

        if (PlayerPrefs.GetInt(PrefKeys.MATCH_PREP_UI_INDEX) == 0)
        {
            ClearAllCards();
            gameObject.SetActive(false);
            ReFresh();
            return;
        }

        for (int i = 0; i < MAX_BATCH_COUNT; i++)
        {
            var card = _positionCards[i];
            if (card == null) continue;

            MoveToCardList(card);
        }

        var data = new StudentSaveData();
        if (SaveLoadManager.Instance != null)
            SaveLoadManager.Instance.Save(FilePath.MY_STUDENT_MATCHING_PATH, data);

        PlayerPrefs.SetInt(PrefKeys.MATCH_PREP_UI_INDEX, 0);

        ReFresh();
    }

    public void SaveBatchStudentData()
    {
        if (_positionCards == null || _positionCards.Length < 1) return;
        if (StudentManager.Instance == null) return;
        if (SaveLoadManager.Instance == null) return;

        var sList = new List<Student>();

        for (int i = 0; i < _positionCards.Length; i++)
        {
            if (_positionCards[i] == null)
            {
                Position targetPos = (Position)i + 1;
                if (_mercenaryMaker != null)
                {
                    var mercenary = _mercenaryMaker.MakeMercenary(targetPos);
                    if (mercenary != null)
                    {
                        StudentManager.Instance.InitStudent(mercenary);
                        mercenary.OnStatChanged();
                        sList.Add(mercenary);
                        continue;
                    }
                }

                continue;
            }

            if (_positionCards[i].Player != null)
                sList.Add(_positionCards[i].Player);
        }

        StudentManager.Instance.SetCurrentTeam(sList);

        var batchData = new StudentSaveData(MAX_BATCH_COUNT, sList, StudentManager.Instance.CurrentTeam);
        SaveLoadManager.Instance.Save(FilePath.MY_STUDENT_MATCHING_PATH, batchData);
    }

    private void EnsureArrays()
    {
        if (_positionCards == null || _positionCards.Length != MAX_BATCH_COUNT)
            _positionCards = new PlayerCard[MAX_BATCH_COUNT];

        if (_cardList == null)
            _cardList = new List<PlayerCard>();
    }

    private int GetSlotIndex(DropPosition dPos)
    {
        return IsCardInPositionSlots(dPos);
    }

    private int IndexOfCard(PlayerCard card)
    {
        if (_positionCards == null) return -1;

        for (int i = 0; i < _positionCards.Length; i++)
        {
            if (_positionCards[i] == card)
                return i;
        }

        return -1;
    }

    private int IsCardInPositionSlots(DropPosition dPos)
    {
        if (_dropPositions == null) return -1;

        int limit = Mathf.Min(_dropPositions.Length, _positionCards.Length);
        for (int i = 0; i < limit; i++)
        {
            if (_dropPositions[i] == dPos)
                return i;
        }

        return -1;
    }

    private void ClearAllCards()
    {
        EnsureArrays();

        foreach (var card in _cardList)
        {
            if (card != null)
                _playerCardPool.Release(card);
        }
        _cardList.Clear();

        if (_dropPositions != null)
        {
            foreach (var position in _dropPositions)
            {
                if (position == null) continue;

                var cards = position.GetComponentsInChildren<PlayerCard>(true);
                foreach (var card in cards)
                {
                    if (card != null)
                        _playerCardPool.Release(card);
                }
            }
        }

        for (int i = 0; i < _positionCards.Length; i++)
            _positionCards[i] = null;
    }
}