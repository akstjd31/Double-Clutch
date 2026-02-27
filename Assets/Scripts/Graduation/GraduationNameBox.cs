using UnityEngine;

public class GraduationNameBox : MonoBehaviour
{
    public Student clickData;
    public GameObject graduationUI;

    //각 이름 박스 클릭 시에 프로필 디테일 패널 열림
    public void OnNameClick()
    {
        graduationUI.GetComponent<GraduationProfileDetailPanel>().Profile(clickData);
    }
}
