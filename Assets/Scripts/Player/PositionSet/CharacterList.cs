using System.Collections.Generic;
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


    GenericObjectPool<PlayerCard> _playerCardPool;

    [SerializeField] private List<PlayerCard> _cardList = new List<PlayerCard>();
    public List<PlayerCard> CardList => _cardList;


    [SerializeField] private PlayerCard[] _positionCards;
    public PlayerCard[] PositionCards => _positionCards;

    [SerializeField] private DropPosition[] _dropPositions;


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
        ClearAllCards();

        _colorIndex = 0;

        foreach (Student student in StudentManager.Instance.MyStudents)
        {
            PlayerCard newCard = _playerCardPool.Get();
            // newCard.SetImageColor(GetNextColor());
            newCard.Init(student);
            CardList.Add(newCard);
        }
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
        for (int i = 0; i < MAX_BATCH_COUNT; i++)
        {
            if (_positionCards[i] == null) return false;
        }

        return true;
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
            _positionCards[already] = null;

        // 교체
        var prev = _positionCards[idx];
        if (prev != null && prev != card)
            _cardList.Add(prev);

        // 배치
        _positionCards[idx] = card;

        _cardList.Remove(card);

        // 포지셔닝이 완료되었다면 버튼 활성화
        _matchStartPanelObj.SetActive(CheckMaxPositionBatch());
        return true;
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
        // CardList�� �ִ� ī�� �ݳ�
        foreach (var card in _cardList)
        {
            if (card != null) _playerCardPool.Release(card);
        }
        _cardList.Clear();

        // ��ӵ� ��ġ�� �ִ� ī��鵵 Ǯ�� �ݳ�
        if (_dropPositions != null)
        {
            foreach (var position in _dropPositions)
            {
                if (position.transform.childCount > 0) _playerCardPool.Release(position.GetComponentInChildren<PlayerCard>());
            }
        }

        for (int i = 0; i < _positionCards.Length; i++)
            _positionCards[i] = null;
    }
}
