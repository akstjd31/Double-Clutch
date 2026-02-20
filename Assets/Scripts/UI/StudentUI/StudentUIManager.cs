using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 로비 화면의 선수 UI 활성화 상태를 관리하는 비 싱글톤 매니저
/// 활성화와 동시에 Init 호출로 학생 정보 갱신 필요
/// </summary>
public class StudentUIManager : MonoBehaviour
{
    public static StudentUIManager Instance;
    [SerializeField] ProfileDetailsPanel _profileDetailsPanel;
    [SerializeField] PassiveExplainBox _passiveExplainBox;
    [SerializeField] Button _backBotton;


    private void Awake()
    {
        Instance = this;
    }

    public void OnCharacterBoxClick(Student student) //캐릭터 박스 버튼 온클릭에서 호출
    {
        _profileDetailsPanel.gameObject.SetActive(true);
        _profileDetailsPanel.Init(student);
    }

    public void OnPassiveBoxMouseOverStart(Player_PassiveData data) //패시브 프로필 박스의 OnPointerEnter에서 호출
    {        
        _passiveExplainBox.gameObject.SetActive(true);        
        _passiveExplainBox.Init(data);
    }

    public void OnPassiveBoxMouseOverEnd() //패시브 프로필 박스 OnPointerExit에서 호출
    {
        _passiveExplainBox.gameObject.SetActive(false);        
    }
}
