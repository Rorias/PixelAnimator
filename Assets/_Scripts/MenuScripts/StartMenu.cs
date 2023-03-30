using System.Collections;

using UnityEngine;

public class StartMenu : MonoBehaviour
{
    public MainMenuItem initMenu;

    private GameManager gameManager;
    private MainMenu mainMenu;

    private void Awake()
    {
        mainMenu = GetComponent<MainMenu>();
    }

    private void Start()
    {
        gameManager = GameManager.Instance;

        StartCoroutine(DelayedStart());
    }

    private IEnumerator DelayedStart()
    {
        yield return new WaitForEndOfFrame();

        if (gameManager.currentSpriteset == "")
        {
            mainMenu.ActivateNextMenu(initMenu);
        }
    }
}