using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;

using TMPro;

using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadMenu : MonoBehaviour
{
    private static string editorSceneName = "Editor";

    private GameManager gameManager;
    private AnimationManager animManager;
    private MainMenu mainMenu;

    private List<string> loadableAnims = new List<string>();

    private GameObject DeleteAnim;
    private TMP_Dropdown ddLoadAnims;

    private MainMenuItem createAnimMenu;

    private void Awake()
    {
        DeleteAnim = GameObject.Find("DeleteConfirmation");
        ddLoadAnims = GameObject.Find("LoadableAnimsDD").GetComponent<TMP_Dropdown>();

        createAnimMenu = GameObject.Find("CreateAnimMenu").GetComponent<MainMenuItem>();

        mainMenu = GetComponent<MainMenu>();
    }

    private void Start()
    {
        gameManager = GameManager.Instance;
        animManager = AnimationManager.Instance;
    }

    public void InitializeLoadAnimationMenu()
    {
        if (DeleteAnim.activeSelf)
        {
            DeleteAnim.SetActive(false);
        }

        GetLoadableAnimations();

        //ERROR: No animations to load, but user selected load anyway.
        if (loadableAnims.Count <= 0)
        {
            Debug.Log("No loadable animations were found. Please create a new one instead.");
            return;
        }

        InitializeDropdown();

        gameManager.currentAnimation = new Animation();
        gameManager.currentAnimation.animationName = ddLoadAnims.captionText.text;
    }

    private void GetLoadableAnimations()
    {
        string[] fileNames = Directory.GetFiles(Application.dataPath + "/StreamingAssets/", "*.xml");

        if (loadableAnims.Count > 0) { loadableAnims.Clear(); }

        foreach (string s in fileNames)
        {
            string animName = s.Substring(s.LastIndexOf('/') + 1);
            animName = animName.Split('.')[0];
            loadableAnims.Add(animName);
        }
    }

    private void InitializeDropdown()
    {
        if (ddLoadAnims.options.Count > 0) { ddLoadAnims.ClearOptions(); }

        ddLoadAnims.AddOptions(loadableAnims);

        for (int i = 0; i < loadableAnims.Count; i++)
        {
            ddLoadAnims.options[i].text = loadableAnims[i];
        }
    }

    public void GetAnimationName()
    {
        gameManager.currentAnimation.animationName = ddLoadAnims.captionText.text;
    }

    public void LoadAnimation()
    {
        animManager.LoadAnimation(gameManager.currentAnimation);

        SceneManager.LoadScene(editorSceneName);
    }

    public void DeleteConfirmationPopup()
    {
        DeleteAnim.SetActive(!DeleteAnim.activeSelf);
    }

    public void DeleteAnimation()
    {
        string animName = gameManager.currentAnimation.animationName;

        File.Delete(Application.dataPath + "/StreamingAssets/" + animName + ".xml");
        File.Delete(Application.dataPath + "/StreamingAssets/" + animName + ".cs");

        if (File.Exists(Application.dataPath + "/StreamingAssets/" + animName + ".xml.meta") &&
            File.Exists(Application.dataPath + "/StreamingAssets/" + animName + ".cs.meta"))
        {
            File.Delete(Application.dataPath + "/StreamingAssets/" + animName + ".xml.meta");
            File.Delete(Application.dataPath + "/StreamingAssets/" + animName + ".cs.meta");
        }

        GetLoadableAnimations();
        InitializeDropdown();
        DeleteConfirmationPopup();
    }
}