using System.Collections.Generic;
using System.Linq;

using UnityEngine;

public class MainMenu : MonoBehaviour
{
    public MainMenuItem mainMenu;
    private List<MainMenuItem> menus = new List<MainMenuItem>();

    private MainMenuItem currentMenu;

    private void Start()
    {
        menus = FindObjectsOfType<MainMenuItem>().ToList();
        menus.Remove(mainMenu);
        ResetMenu();
    }

    private void ResetMenu()
    {
        for (int i = 0; i < menus.Count; i++)
        {
            menus[i].gameObject.SetActive(false);
        }

        currentMenu = mainMenu;
        mainMenu.gameObject.SetActive(true);
    }

    public void ActivateNextMenu(MainMenuItem nextMenu)
    {
        nextMenu.previousItem = currentMenu;
        currentMenu.gameObject.SetActive(false);
        currentMenu = nextMenu;
        currentMenu.gameObject.SetActive(true);
    }

    public void Back()
    {
        currentMenu.gameObject.SetActive(false);
        currentMenu = currentMenu.previousItem;
        currentMenu.gameObject.SetActive(true);
    }

    public void ExitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
                Application.Quit();
#endif
    }
}
