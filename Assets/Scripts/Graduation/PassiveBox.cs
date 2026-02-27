using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PassiveBox : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _skillName;
    [SerializeField] private Image _skillImage;
    [SerializeField] private TextMeshProUGUI _skillDetail;


    private void Init(Student student)
    {
        _skillName.text = student.PassiveId[transform.GetSiblingIndex()];
    }
}
