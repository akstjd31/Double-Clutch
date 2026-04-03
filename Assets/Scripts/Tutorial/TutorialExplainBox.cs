using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class TutorialExplainBox : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI _gradetext;
    [SerializeField] TextMeshProUGUI _devtext;
    [SerializeField] TextMeshProUGUI _pertext;
    [SerializeField] TextMeshProUGUI _traittext;
    [SerializeField] TextMeshProUGUI _atktext;
    [SerializeField] TextMeshProUGUI _dfstext;

    
    private void OnEnable()
    {
        Refresh();
        StringManager.OnLanguageChanged += Refresh;
    }

    private void OnDisable()
    {
        StringManager.OnLanguageChanged -= Refresh;
    }
    private void Refresh()
    {
        StringManager smg = StringManager.Instance;
        _gradetext.text = smg.GetString("UI_Player_1학년");
        smg.ApplyFont(_gradetext);
        _devtext.text = smg.GetString("UI_Development_이름");
        smg.ApplyFont( _devtext);
        _pertext.text = smg.GetString("UI_Player_성격");
        smg.ApplyFont( _pertext);
        _traittext.text = smg.GetString("UI_Player_특성");
        smg.ApplyFont( _traittext);
        _atktext.text = smg.GetString("UI_Player_공격력");
        smg.ApplyFont(_atktext);
        _dfstext.text = smg.GetString("UI_Player_수비력");
        smg.ApplyFont(_dfstext);
    }

}
