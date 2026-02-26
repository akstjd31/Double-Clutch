using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// CharacterList 오브젝트에 할당해서 하단 선수 카드 생성
/// </summary>
public class CharacterList : MonoBehaviour
{
    [SerializeField] PlayerCard _playerCardPrefab;
    [SerializeField] Transform _cardContainer; //아마도 자기 자신

    GenericObjectPool<PlayerCard> _playerCardPool;
    public List<PlayerCard> CardList = new List<PlayerCard>();


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


        foreach (Student student in StudentManager.Instance.MyStudents)
        {
            PlayerCard newCard = _playerCardPool.Get();
            CardList.Add(newCard);
            newCard.Init(student);
        }
    }
}
