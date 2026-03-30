using UnityEngine;
using UnityEngine.UI;

public class GoToEndingTestButton : MonoBehaviour
{    
    void Start()
    {
        Button button = GetComponent<Button>();
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(GameManager.Instance.GoToEnding);
    }

    private void OnDestroy()
    {
        Button button = GetComponent<Button>();
        button.onClick.RemoveAllListeners();
    }
}
