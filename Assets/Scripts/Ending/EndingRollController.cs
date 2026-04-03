using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EndingRollController : MonoBehaviour
{
    [SerializeField] EndingRollRow _rowPrefab;
    [SerializeField] RectTransform _background;
    [SerializeField] RectTransform _rowParent;
    

    
    private Tween _rollTween;


    private GenericObjectPool<EndingRollRow> _rowPool;
    private List<EndingRollRow> _activeRowList = new List<EndingRollRow>();

    private void Awake()
    {
        _rowPool = new GenericObjectPool<EndingRollRow>(_rowPrefab, _rowParent, 10, 20);        
    }    
    
    public void Init(Dictionary<Student, int> everyStudents, List<EndingRollData> staffs)
    {
        foreach (var row in _activeRowList)
        {
            _rowPool.Release(row);
        }
        _activeRowList.Clear();
        MakeSubTitleCredit("Students");
        MakeBlankCredit();
        for (int i = 0; i < everyStudents.Count; i++)
        {

            int classNum =  everyStudents[EndingManager.Instance.StudentRollIndex[i]];
            MakeEndingCredit(EndingManager.Instance.StudentRollIndex[i], classNum);
        }
        MakeBlankCredit();
        MakeBlankCredit();
        MakeSubTitleCredit("Team Double-Clutch");
        MakeBlankCredit();
        for (int i = 0;i < staffs.Count; i++)
        {
            MakeEndingCredit(staffs[i]);
        }
    }    

    private void MakeEndingCredit(Student student, int classNum)
    {
        EndingRollRow newRow = _rowPool.Get();
        newRow.Init(classNum, student.Position.ToString(), student.Name);        
        _activeRowList.Add(newRow);
    }
    private void MakeEndingCredit(EndingRollData data)
    {
        EndingRollRow newRow = _rowPool.Get();
        newRow.Init(data.typeKey, data.nameKey);
        _activeRowList.Add(newRow);
    }

    private void MakeSubTitleCredit(string subtitle) //나중에 스트링키로 변경
    {
        EndingRollRow newRow = _rowPool.Get();
        newRow.Init(subtitle);
        _activeRowList.Add(newRow);
    }
    
    private void MakeBlankCredit()
    {
        EndingRollRow newRow = _rowPool.Get();
        newRow.Init();
        _activeRowList.Add(newRow);
    }



    public void StartRoll(float duration)
    {
        _rollTween?.Kill();

        LayoutRebuilder.ForceRebuildLayoutImmediate(_rowParent);
        Canvas.ForceUpdateCanvases();

        RectTransform viewport = _rowParent.parent as RectTransform;
        float viewHeight = viewport.rect.height;

       
        float contentHeight = _rowParent.rect.height;
        
        float startY = -viewHeight;

      
        float targetY = contentHeight;

        _rowParent.anchoredPosition = new Vector2(0, startY);

        
        _rollTween = _rowParent.DOAnchorPosY(targetY, duration)
            .SetEase(Ease.Linear)
            .OnComplete(() =>
            {                
                DOVirtual.DelayedCall(EndingManager.Instance.BeforeAndYouTime, () => EndingManager.Instance.PlayAndYou());
            });
    }

}
