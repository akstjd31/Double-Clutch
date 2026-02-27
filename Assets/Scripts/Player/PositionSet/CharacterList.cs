using System.Collections.Generic;
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


    GenericObjectPool<PlayerCard> _playerCardPool;

    [SerializeField] private List<PlayerCard> _cardList = new List<PlayerCard>();
    public List<PlayerCard> CardList => _cardList;


    [SerializeField] private PlayerCard[] _positionCards;
    public PlayerCard[] PositionCards => _positionCards;

    [SerializeField] private List<DropPosition> _dropPositionList = new List<DropPosition>();

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

        if (_dropPositionList == null)
        {
            foreach (var dp in _positionTrf.GetComponentsInChildren<DropPosition>())
            {
                _dropPositionList.Add(dp);
            }
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

    public void AddOnPosition(PlayerCard card)
    {
        if (card == null) return;

        if (IsCardInPositionSlots(card) == -1)
        {
            // 빈자리 채우기
            for (int i = 0; i < MAX_BATCH_COUNT; i++)
            {
                if (_positionCards[i] == null)
                {
                    _positionCards[i] = card;
                    break;
                }
            }

            _cardList.Remove(card);
        }
    }

    public void RemoveOnPosition(PlayerCard card)
    {
        if (card == null) return;

        int idx = IsCardInPositionSlots(card);
        if (idx == -1) return;

        _positionCards[idx] = null;

        for (int i = idx; i < _positionCards.Length - 1; i++)
        {
            _positionCards[i] = _positionCards[i + 1];
            _positionCards[i + 1] = null;
        }

        _cardList.Add(card);
    }

    // 포지션에 배치된 카드 중 동일한 카드 존재 시 인덱스 반환
    private int IsCardInPositionSlots(PlayerCard card)
    {
        for (int i = 0; i < _positionCards.Length; i++)
        {
            if (_positionCards[i] == null) continue;
            if (_positionCards[i].Equals(card))
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
        if (_dropPositionList != null)
        {
            foreach (var position in _dropPositionList)
            {
                if (position != null) _playerCardPool.Release(position.GetComponentInChildren<PlayerCard>());
            }
        }

        for (int i = 0; i < _positionCards.Length; i++)
            _positionCards[i] = null;
    }
}
