using UnityEngine;
using UnityEngine.SceneManagement;

public class BackgroundController : MonoBehaviour
{
    private GameManager gameManager;

    private void Awake()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene _scene, LoadSceneMode _lsm)
    {
        gameManager = GameManager.Instance;
        Camera.main.backgroundColor = gameManager.bgColor;
    }
}