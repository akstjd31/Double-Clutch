using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// CharacterList ????????? ?????? ??? ???? ??? ????
/// </summary>
public class CharacterList : MonoBehaviour
{
    [SerializeField] PlayerCard _playerCardPrefab;
    [SerializeField] Transform _cardContainer; //????? ??? ???
    [SerializeField] Transform _positionTrf;


    GenericObjectPool<PlayerCard> _playerCardPool;

    [SerializeField] private List<PlayerCard> _cardList = new List<PlayerCard>();
    public List<PlayerCard> CardList => _cardList;


    [SerializeField] private List<PlayerCard> _droppedCardList = new List<PlayerCard>();
    public List<PlayerCard> DroppedCardList => _droppedCardList;

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
    }


    private void OnEnable()
    {
        ClearAllCards();

        _colorIndex = 0;

        foreach (Student student in StudentManager.Instance.MyStudents)
        {
            PlayerCard newCard = _playerCardPool.Get();
            newCard.SetImageColor(GetNextColor());
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
        if (!_droppedCardList.Contains(card))
        {
            _droppedCardList.Add(card);
            _cardList.Remove(card);
        }
    }

    public void RemoveOnPosition(PlayerCard card)
    {
        if (_droppedCardList.Contains(card))
        {
            _droppedCardList.Remove(card);
            _cardList.Add(card);
        }
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
        foreach (var position in _dropPositionList)
        {
            if (position != null) _playerCardPool.Release(position.GetComponentInChildren<PlayerCard>());
        }
        _droppedCardList.Clear();
    }
}
