using System.Collections.Generic;
using System.IO;

using TMPro;

using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadMenu : MonoBehaviour
{
    private static string editorSceneName = "Editor";

    private GameManager gameManager;
    private AnimationManager animManager;

    private List<string> loadableAnims = new List<string>();

    private GameObject editAnimMenu;
    private GameObject renameAnimMenu;
    private GameObject deleteAnimMenu;

    private TMP_InputField renameIF;
    private TMP_Dropdown loadAnimsDD;

    private string previousAnimName;

    private void Awake()
    {
        editAnimMenu = GameObject.Find("EditMenu");
        renameAnimMenu = GameObject.Find("RenameMenu");
        deleteAnimMenu = GameObject.Find("DeleteConfirmationMenu");

        renameIF = GameObject.Find("Rename").GetComponent<TMP_InputField>();
        loadAnimsDD = GameObject.Find("LoadableAnimsDD").GetComponent<TMP_Dropdown>();
    }

    private void Start()
    {
        gameManager = GameManager.Instance;
        animManager = AnimationManager.Instance;
    }

    public void InitializeLoadAnimationMenu()
    {
        editAnimMenu.SetActive(false);
        renameAnimMenu.SetActive(false);
        deleteAnimMenu.SetActive(false);

        GetLoadableAnimations();

        //ERROR: No animations to load, but user selected load anyway.
        if (loadableAnims.Count <= 0)
        {
            Debug.Log("No loadable animations were found. Please create a new one instead.");
            DebugHelper.Log("No loadable animations were found. Please create a new one instead.");
            return;
        }

        InitializeDropdown();

        gameManager.currentAnimation = new Animation();
        gameManager.currentAnimation.animationName = loadAnimsDD.captionText.text;
    }

    private void GetLoadableAnimations()
    {
        string[] fileNames = Directory.GetFiles(gameManager.animationsPath, "*.xml");

        if (loadableAnims.Count > 0) { loadableAnims.Clear(); }

        foreach (string s in fileNames)
        {
            string animName = s.Substring(s.LastIndexOf('/') + 1);

            //Check in case / = \\
            if (animName.Length == s.Length)
            {
                animName = s.Substring(s.LastIndexOf('\\') + 1);
            }

            animName = animName.Split('.')[0];
            loadableAnims.Add(animName);
        }
    }

    private void InitializeDropdown()
    {
        if (loadAnimsDD.options.Count > 0) { loadAnimsDD.ClearOptions(); }

        loadAnimsDD.AddOptions(loadableAnims);

        for (int i = 0; i < loadableAnims.Count; i++)
        {
            loadAnimsDD.options[i].text = loadableAnims[i];
        }
    }

    public void GetAnimationName()
    {
        gameManager.currentAnimation.animationName = loadAnimsDD.captionText.text;
    }

    public void LoadAnimation()
    {
        if (loadableAnims.Count <= 0)
        {
            Debug.Log("No loadable animations were found. Please create a new one instead.");
            DebugHelper.Log("No loadable animations were found. Please create a new one instead.");
            return;
        }

        if (!animManager.LoadAnimation(gameManager.currentAnimation))
        {
            return;
        }

        SceneManager.LoadScene(editorSceneName);
    }

    public void EditMenuState()
    {
        if (loadableAnims.Count <= 0)
        {
            return;
        }

        editAnimMenu.SetActive(!editAnimMenu.activeSelf);

        if (!editAnimMenu.activeSelf)
        {
            renameAnimMenu.SetActive(false);
            loadAnimsDD.interactable = true;
        }

        if (!editAnimMenu.activeSelf)
        {
            deleteAnimMenu.SetActive(false);
        }
    }

    public void RenameMenuState()
    {
        renameAnimMenu.SetActive(!renameAnimMenu.activeSelf);
        loadAnimsDD.interactable = !renameAnimMenu.activeSelf;

        if (renameAnimMenu.activeSelf)
        {
            renameIF.text = gameManager.currentAnimation.animationName;
            previousAnimName = renameIF.text;
        }
    }

    public void RenameAnimation()
    {
        string newName = renameIF.text;

        if (string.IsNullOrWhiteSpace(newName))
        {
            Debug.Log("Please give the animation a name.");
            DebugHelper.Log("Please the your animation a name.");
            return;
        }

        if (newName != previousAnimName && File.Exists(gameManager.animationsPath + "\\" + newName + ".xml"))
        { 
            Debug.Log("There is already an animation with this name.");
            DebugHelper.Log("There is already an animation with this name.");
            return;
        }

        if (!animManager.LoadAnimation(gameManager.currentAnimation))
        {
            Debug.Log("Failed to load the current animation. Try selecting a different one.");
            DebugHelper.Log("Failed to load the current animation. Try selecting a different one.");
            return;
        }

        gameManager.currentAnimation.animationName = newName;
        animManager.SaveFile(gameManager.currentAnimation);

        if (newName != previousAnimName)
        {
            //remove old file to pretend like we renamed it
            DeleteAnimation(previousAnimName);
        }
        else
        {
            GetLoadableAnimations();
            InitializeDropdown();
            EditMenuState();
        }
    }

    public void CopyAnimation()
    {
        string animName = gameManager.currentAnimation.animationName;

        if (!animManager.LoadAnimation(gameManager.currentAnimation))
        {
            return;
        }

        gameManager.currentAnimation.animationName = animName + "Copy";
        animManager.SaveFile(gameManager.currentAnimation);

        GetLoadableAnimations();
        InitializeDropdown();
        EditMenuState();
    }

    public void DeleteConfirmationPopup()
    {
        deleteAnimMenu.SetActive(!deleteAnimMenu.activeSelf);
    }

    public void DeleteAnimation()
    {
        string animName = gameManager.currentAnimation.animationName;
        DeleteAnimation(animName);
    }

    private void DeleteAnimation(string _name)
    {
        File.Delete(gameManager.animationsPath + "\\" + _name + ".xml");
        File.Delete(gameManager.animationsPath + "\\" + _name + ".cs");

        if (File.Exists(gameManager.animationsPath + "\\" + _name + ".xml.meta") &&
            File.Exists(gameManager.animationsPath + "\\" + _name + ".cs.meta"))
        {
            File.Delete(gameManager.animationsPath + "\\" + _name + ".xml.meta");
            File.Delete(gameManager.animationsPath + "\\" + _name + ".cs.meta");
        }

        GetLoadableAnimations();
        InitializeDropdown();
        EditMenuState();
    }
}