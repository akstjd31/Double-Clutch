using UnityEngine;
using TMPro;

public class GraduationProfileDetailPanel : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _name;
    [SerializeField] private TextMeshProUGUI _attackPoint;
    [SerializeField] private TextMeshProUGUI _defensePoint;
    [SerializeField] private TextMeshProUGUI _personality;

    [SerializeField] private TextMeshProUGUI[] _passiveName = new TextMeshProUGUI[3];

    private void OnEnable()
    {
        for(int i = 0;  i < _passiveName.Length; i++)
        {
            _passiveName[i].text = "";
        }
    }

    public void Profile(Student student)
    {
        StringManager manager = StringManager.Instance;
        //프로필 패널 활성화
        string name = manager.GetString(student.Name[0]) + manager.GetString(student.Name[1]) + manager.GetString(student.Name[2]);

        gameObject.SetActive(true);
        _name.text = name;
        _attackPoint.text = student.Attack.ToString();
        _defensePoint.text = student.Defense.ToString();
        _personality.text = student.PersonalityData.personalityName;

        for (int i = 0; i < student.PassiveId.Count; i++)
        {
            _passiveName[i].text = student.PassiveId[i];
        }
    }
}
