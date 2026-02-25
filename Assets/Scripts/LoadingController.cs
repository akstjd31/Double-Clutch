using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class LoadingController : MonoBehaviour
{
    private void Start()
    {
        GameManager.Instance.LoadNextScene();
    }
}