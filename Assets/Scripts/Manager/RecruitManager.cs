using UnityEngine;

public class RecruitManager : MonoBehaviour
{
    public void StartRecruiting()
    {
        StudentUIManager.Instance.OpenRecruitPanel();
    }
}
