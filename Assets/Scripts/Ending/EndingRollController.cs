using System.Collections.Generic;
using UnityEngine;

public class EndingRollController : MonoBehaviour
{
    [SerializeField] EndingRollRow _rowPrefab;
    [SerializeField] Transform _rowParent;
    [Header("엔딩 롤 속도")]
    [SerializeField] float _rollSpeed;



    private GenericObjectPool<EndingRollRow> _rowPool;
    private List<EndingRollRow> _activeRowList = new List<EndingRollRow>();

    private void Awake()
    {
        _rowPool = new GenericObjectPool<EndingRollRow>(_rowPrefab, _rowParent, 10, 20);        
    }    
    
    public void Init(List<Student> everyStudents)
    {

    }
    
}
