using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GraduationProfileDetailPanel : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _name;
    [SerializeField] private Image _image;
    [SerializeField] private TextMeshProUGUI _attackPoint;
    [SerializeField] private TextMeshProUGUI _defensePoint;
    [SerializeField] private TextMeshProUGUI _personality;

    [SerializeField] private TextMeshProUGUI[] _passiveName = new TextMeshProUGUI[3];
    [SerializeField] private Player_PassiveData _passiveData;
    private Student _student;
    private void OnEnable()
    {
        StringManager.OnLanguageChanged += Refresh;
        for(int i = 0;  i < _passiveName.Length; i++)
        {
            _passiveName[i].text = "";
        }
    }

    private void OnDisable()
    {
        StringManager.OnLanguageChanged -= Refresh;
    }

    public void Profile(Student student)
    {
        _student = student;

        StringManager manager = StringManager.Instance;
        //프로필 패널 활성화
        string name = manager.GetString(student.Name[0]) + manager.GetString(student.Name[1]) + manager.GetString(student.Name[2]);

        gameObject.SetActive(true);
        _name.text = name;
        manager.ApplyFont(_name);
        _image.sprite = SpriteManager.Instance.GetSprite(student.VisualData.playerImageResource);
        _attackPoint.text = student.Attack.ToString();
        _defensePoint.text = student.Defense.ToString();
        _personality.text = StringManager.Instance.GetString(student.PersonalityData.personalityName);
        manager.ApplyFont(_personality);

        for (int i = 0; i < student.PassiveId.Count; i++)
        {
            string skillId = student.PassiveId[i];
            string skillName = "스킬 못찾았음";
            for(int j = 0; j < student.Passive.Count; j++)
            {
                if(skillId == student.Passive[j].skillId)
                {
                    skillName = StringManager.Instance.GetString(student.Passive[j].skillName);
                }
            }

            _passiveName[i].text = skillName;
            StringManager.Instance.ApplyFont(_passiveName[i]);
        }
    }
    
    private void Refresh()
    {
        Profile(_student);
    }
}
