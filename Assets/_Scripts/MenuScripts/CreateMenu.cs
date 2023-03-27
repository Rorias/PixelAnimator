using System;
using System.IO;

using TMPro;

using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CreateMenu : MonoBehaviour
{
    private static string editorSceneName = "Editor";

    private GameManager gameManager;

    private TMP_InputField IFName;
    private TMP_InputField IFFrames;
    private TMP_InputField IFParts;
    private TMP_InputField IFXsize;
    private TMP_InputField IFYsize;

    private void Awake()
    {
        IFName = GameObject.Find("NameField").GetComponent<TMP_InputField>();
        IFFrames = GameObject.Find("FramesField").GetComponent<TMP_InputField>();
        IFParts = GameObject.Find("PartsField").GetComponent<TMP_InputField>();
        IFXsize = GameObject.Find("XsizeField").GetComponent<TMP_InputField>();
        IFYsize = GameObject.Find("YsizeField").GetComponent<TMP_InputField>();
    }

    private void Start()
    {
        gameManager = GameManager.Instance;
    }

    public void InitializeCreateAnimationMenu()
    {
        gameManager.currentAnimation = new Animation();
        IFName.text = string.Empty;
        IFFrames.text = "1";
        IFParts.text = "7";
        IFXsize.text = "1";
        IFYsize.text = "1";
    }

    public void SetAnimationName()
    {
        CheckAnimationName();
    }

    public bool CheckAnimationName()
    {
        if (string.IsNullOrWhiteSpace(IFName.text))
        {
            Debug.Log("Please give your animation a name.");
            DebugHelper.Log("Please give your animation a name.");
            return false;
        }

        gameManager.currentAnimation.animationName = IFName.text;
        return true;
    }

    public void SetMaxFrameCount()
    {
        gameManager.currentAnimation.maxFrameCount = Mathf.Min(Mathf.Max(Convert.ToInt32(IFFrames.text), 1), 999);
        IFFrames.text = gameManager.currentAnimation.maxFrameCount.ToString();
    }

    public void SetMaxPartCount()
    {
        gameManager.currentAnimation.maxPartCount = Mathf.Min(Mathf.Max(Convert.ToInt32(IFParts.text), 1), 99);
        IFParts.text = gameManager.currentAnimation.maxPartCount.ToString();
    }

    public void SetGridSizeX()
    {
        gameManager.currentAnimation.gridSizeX = Mathf.Min(Mathf.Max(Convert.ToInt32(IFXsize.text), 1), 2047);
        IFXsize.text = gameManager.currentAnimation.gridSizeX.ToString();
    }

    public void SetGridSizeY()
    {
        gameManager.currentAnimation.gridSizeY = Mathf.Min(Mathf.Max(Convert.ToInt32(IFYsize.text), 1), 2047);
        IFYsize.text = gameManager.currentAnimation.gridSizeY.ToString();
    }

    private void InitializeNewAnimation()
    {
        gameManager.currentAnimation.usedSpriteset = gameManager.currentSpriteset;

        for (int i = 0; i < gameManager.currentAnimation.maxFrameCount; i++)
        {
            gameManager.currentAnimation.frames.Add(new Frame() { frameID = i });
        }

        for (int i = 0; i < gameManager.currentAnimation.maxFrameCount; i++)
        {
            for (int part = 0; part < gameManager.currentAnimation.maxPartCount; part++)
            {
                gameManager.currentAnimation.frames[i].frameParts.Add(new Part() { partID = part, partIndex = -1 });
            }
        }
    }

    public void CreateNewAnimation()
    {
        if (File.Exists(Application.dataPath + "/StreamingAssets/" + gameManager.currentAnimation.animationName + ".xml"))
        {
            Debug.Log("There is already an animation with this name.");
            DebugHelper.Log("There is already an animation with this name.");
            return;
        }

        if (!CheckAnimationName()) { return; };

        SetMaxFrameCount();
        SetMaxPartCount();
        SetGridSizeX();
        SetGridSizeY();

        InitializeNewAnimation();

        SceneManager.LoadScene(editorSceneName);
    }
}