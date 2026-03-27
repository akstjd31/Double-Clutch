using UnityEngine;
using Game.Constants;

public class EventPanel : MonoBehaviour
{
    private void OnEnable()
    {
        // PlaySound(SoundName.BGM_RANDOM_EVENT);
    }

    private void OnDisable()
    {
        // PlaySound(SoundName.BGM_LOBBY_01);
    }

    private void PlaySound(string id)
    {
        if (AudioManager.Instance == null) return;
        AudioManager.Instance.PlaySound(id);
    }
}
