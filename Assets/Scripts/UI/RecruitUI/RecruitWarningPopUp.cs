using TMPro;
using UnityEngine;
using Game.Constants;

public class RecruitWarningPopUp : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI _warningText;

    private void OnEnable()
    {
        if (AudioManager.Instance != null)
            AudioManager.Instance.PlaySoundOneShot(SoundName.UI_WARNING_01);
    }

    public void Init(int number)
    {
        _warningText.text = $"선수를 {number} 명 더 영입하셔야 합니다.";
    }
}
