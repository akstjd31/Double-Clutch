using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// CharacterList ????????? ?????? ??? ???? ??? ????
/// </summary>
public class CharacterList : MonoBehaviour
{
    [SerializeField] PlayerCard _playerCardPrefab;
    [SerializeField] Transform _cardContainer; //????? ??? ???
    [SerializeField] DropPosition _C;
    [SerializeField] DropPosition _PG;
    [SerializeField] DropPosition _PF;
    [SerializeField] DropPosition _SF;
    [SerializeField] DropPosition _SG;


    GenericObjectPool<PlayerCard> _playerCardPool;

    private List<PlayerCard> _cardList = new List<PlayerCard>();
    public List<PlayerCard> CardList => _cardList;


    private List<PlayerCard> _droppedCardList = new List<PlayerCard>();
    public List<PlayerCard> DroppedCardList => _droppedCardList;

    private List<DropPosition> _dropPositionList = new List<DropPosition>();

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
        _dropPositionList.Add(_C);
        _dropPositionList.Add(_PG);
        _dropPositionList.Add(_PF);
        _dropPositionList.Add(_SF);
        _dropPositionList.Add(_SG);
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
        // CardList에 있는 카드 반납
        foreach (var card in _cardList)
        {
            if (card != null) _playerCardPool.Release(card);
        }
        _cardList.Clear();

        // 드롭된 위치에 있는 카드들도 풀로 반납
        foreach (var position in _dropPositionList)
        {
            if (position != null) _playerCardPool.Release(position.GetComponentInChildren<PlayerCard>());
        }
        _droppedCardList.Clear();
    }
}
