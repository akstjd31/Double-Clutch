using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// CharacterList ????????? ?????? ??? ???? ??? ????
/// </summary>
public class CharacterList : MonoBehaviour
{
    [SerializeField] PlayerCard _playerCardPrefab;
    [SerializeField] Transform _cardContainer; //????? ??? ???

    GenericObjectPool<PlayerCard> _playerCardPool;
    public List<PlayerCard> CardList = new List<PlayerCard>();

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
        foreach (var card in CardList)
        {
            _playerCardPool.Release(card);
        }
        CardList.Clear();
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
}
