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

    private TMP_InputField nameIF;
    private TMP_InputField framesIF;
    private TMP_InputField partsIF;
    private TMP_InputField xSizeIF;
    private TMP_InputField ySizeIF;

    private void Awake()
    {
        nameIF = GameObject.Find("NameField").GetComponent<TMP_InputField>();
        framesIF = GameObject.Find("FramesField").GetComponent<TMP_InputField>();
        partsIF = GameObject.Find("PartsField").GetComponent<TMP_InputField>();
        xSizeIF = GameObject.Find("XsizeField").GetComponent<TMP_InputField>();
        ySizeIF = GameObject.Find("YsizeField").GetComponent<TMP_InputField>();
    }

    private void Start()
    {
        gameManager = GameManager.Instance;
    }

    public void InitializeCreateAnimationMenu()
    {
        gameManager.currentAnimation = new Animation();
        nameIF.text = string.Empty;
        framesIF.text = "1";
        partsIF.text = "7";
        xSizeIF.text = "1";
        ySizeIF.text = "1";
    }

    public void SetAnimationName()
    {
        CheckAnimationName();
    }

    public bool CheckAnimationName()
    {
        if (string.IsNullOrWhiteSpace(nameIF.text))
        {
            Debug.Log("Please give your animation a name.");
            DebugHelper.Log("Please give your animation a name.");
            return false;
        }

        gameManager.currentAnimation.animationName = nameIF.text;
        return true;
    }

    public void SetMaxFrameCount()
    {
        gameManager.currentAnimation.maxFrameCount = Mathf.Min(Mathf.Max(Convert.ToInt32(framesIF.text), 1), 999);
        framesIF.text = gameManager.currentAnimation.maxFrameCount.ToString();
    }

    public void SetMaxPartCount()
    {
        gameManager.currentAnimation.maxPartCount = Mathf.Min(Mathf.Max(Convert.ToInt32(partsIF.text), 1), 99);
        partsIF.text = gameManager.currentAnimation.maxPartCount.ToString();
    }

    public void SetGridSizeX()
    {
        gameManager.currentAnimation.gridSizeX = Mathf.Min(Mathf.Max(Convert.ToInt32(xSizeIF.text), 1), 4095);
        xSizeIF.text = gameManager.currentAnimation.gridSizeX.ToString();
    }

    public void SetGridSizeY()
    {
        gameManager.currentAnimation.gridSizeY = Mathf.Min(Mathf.Max(Convert.ToInt32(ySizeIF.text), 1), 4095);
        ySizeIF.text = gameManager.currentAnimation.gridSizeY.ToString();
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
        if (File.Exists(gameManager.animationsPath + "\\" + gameManager.currentAnimation.animationName + ".xml"))
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