using UnityEngine;
using TMPro;

public class GraduationProfileDetailPanel : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _name;
    [SerializeField] private TextMeshProUGUI _attackPoint;
    [SerializeField] private TextMeshProUGUI _defensePoint;
    [SerializeField] private TextMeshProUGUI _personality;

    [SerializeField] private TextMeshProUGUI _passiveName0;
    [SerializeField] private TextMeshProUGUI _passiveName1;
    [SerializeField] private TextMeshProUGUI _passiveName2;

    public void Profile(Student student)
    {
        //프로필 패널 활성화
        gameObject.SetActive(true);
        _name.text = student.Name;
    }
}
