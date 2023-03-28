using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;

using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    #region singleton
    private static GameManager instance = null;
    public static GameManager Instance
    {
        get { return instance; }
    }

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        else
        {
            ini = new IniFile("pixelSettings");

            LoadGameSettings();
            SetGameSettings();

            instance = this;
        }

        DontDestroyOnLoad(gameObject);
    }
    #endregion

    private static readonly CultureInfo CultUS = new CultureInfo("en-US");

    [NonSerialized] public Dictionary<int, Sprite> spritesetImages = new Dictionary<int, Sprite>();
    [HideInInspector] public IniFile ini { get; private set; }

    [NonSerialized] public Animation currentAnimation = null;

    //Settings that affect the entire program
    #region GameSettings 
    [NonSerialized] public bool fullScreen = true;
    [NonSerialized] public int resNumber = 1;

    [NonSerialized] public string spritesetsPath = "";
    [NonSerialized] public string currentSpriteset = "";

    [NonSerialized] public string animationsPath = "";

    [NonSerialized] public float lastPlaybackSpeed = 0.0f;
    #endregion

    #region const strings
    public const string SfullScreen = "fullScreen";
    public const string SresNumber = "resNumber";

    public const string SspritesetsPath = "spritesetsPath";
    public const string ScurrentSpriteset = "currentSpriteset";

    public const string SanimationsPath = "animationsPath";

    public const string SlastPlaybackSpeed = "lastPlaybackSpeed";
    #endregion

    private void LoadGameSettings()
    {
        fullScreen = Convert.ToBoolean(ini.Read(SfullScreen, "False"));
        resNumber = Convert.ToInt32(ini.Read(SresNumber, "1"));

        spritesetsPath = ini.Read(SspritesetsPath, "");
        currentSpriteset = ini.Read(ScurrentSpriteset, "");

        animationsPath = ini.Read(SanimationsPath, Application.dataPath + "/StreamingAssets/");

        lastPlaybackSpeed = ParseToSingle(ini.Read(SlastPlaybackSpeed, "0"));
    }

    public void SetGameSettings()
    {
        Screen.fullScreenMode = fullScreen ? FullScreenMode.FullScreenWindow : FullScreenMode.Windowed;

        Resolution[] resolutions = Screen.resolutions;
        List<int> acceptedResNumbers = new List<int>();

        for (int i = 0; i < resolutions.Length; i++)
        {
            if ((resolutions[i].height % 9 == 0) && (resolutions[i].refreshRate == 59 || resolutions[i].refreshRate == 60 || resolutions[i].refreshRate == 75))
            {
                acceptedResNumbers.Add(i);
            }
        }

        for (int i = 0; i < acceptedResNumbers.Count; i++)
        {
            if (i == resNumber)
            {
                Screen.SetResolution(resolutions[acceptedResNumbers[i]].width, resolutions[acceptedResNumbers[i]].height, Screen.fullScreenMode, resolutions[acceptedResNumbers[i]].refreshRate);
                break;
            }
        }
    }

    public void SaveGameSettings()
    {
        ini.Write(SfullScreen, fullScreen.ToString());
        ini.Write(SresNumber, resNumber.ToString());

        ini.Write(SspritesetsPath, spritesetsPath);
        ini.Write(ScurrentSpriteset, currentSpriteset);

        ini.Write(SanimationsPath, animationsPath);

        ini.Write(SlastPlaybackSpeed, ParseToString(lastPlaybackSpeed));
    }

    public float ParseToSingle(string parseValue)
    {
        return float.Parse(parseValue, NumberStyles.Float, CultUS);
    }

    public string ParseToString(float parseValue)
    {
        return parseValue.ToString(CultUS);
    }
}