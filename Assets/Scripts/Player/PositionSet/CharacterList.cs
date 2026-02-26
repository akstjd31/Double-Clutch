using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// CharacterList ������Ʈ�� �Ҵ��ؼ� �ϴ� ���� ī�� ����
/// </summary>
public class CharacterList : MonoBehaviour
{
    [SerializeField] PlayerCard _playerCardPrefab;
    [SerializeField] Transform _cardContainer; //�Ƹ��� �ڱ� �ڽ�

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
            newCard.Init(student, CardList.Count);
            CardList.Add(newCard);
        }
    }

    // public void ReFresh()
    // {
        
    // }
}
