using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static UnityEditor.AddressableAssets.Build.Layout.BuildLayout;



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
    
    public void Init(List<Student> everyStudents, List<EndingRollData> staffs)
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
            MakeEndingCredit(everyStudents[i]);
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

    private void MakeEndingCredit(Student student)
    {
        EndingRollRow newRow = _rowPool.Get();
        newRow.Init(string.Empty, student.Name);
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
        float contentHeight = _background.sizeDelta.y;

        
        float startY = -viewHeight;

        float targetY = contentHeight + viewHeight * 2f;

        _rowParent.anchoredPosition = new Vector2(0, startY);

        // 트윈 실행
        _rollTween = _rowParent.DOAnchorPosY(targetY, duration)
            .SetEase(Ease.Linear)
            .OnComplete(() => {                
                // 여운을 주기 위해 1초 뒤에 로비로 이동
                DOVirtual.DelayedCall(1.0f, () => EndingManager.Instance.PlayAndYou());
            });
    }

    // 배속 기능 예시 (화면 누르고 있을 때 호출)
    public void SetSpeed(float multiplier)
    {
        if (_rollTween != null) _rollTween.timeScale = multiplier;
    }
}
