using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;

using UnityEngine;

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

    public enum Styles { Light, Dark };


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

    [NonSerialized] public Styles editorStyle = Styles.Dark;
    [NonSerialized] public Color bgColor;
    #endregion

    #region const strings
    public const string SfullScreen = "fullScreen";
    public const string SresNumber = "resNumber";

    public const string SspritesetsPath = "spritesetsPath";
    public const string ScurrentSpriteset = "currentSpriteset";

    public const string SanimationsPath = "animationsPath";

    public const string SlastPlaybackSpeed = "lastPlaybackSpeed";

    public const string SeditorStyle = "editorStyle";
    public const string SbgColor = "bgColor";
    #endregion

    private void LoadGameSettings()
    {
        fullScreen = Convert.ToBoolean(ini.Read(SfullScreen, "False"));
        resNumber = Convert.ToInt32(ini.Read(SresNumber, "1"));

        spritesetsPath = ini.Read(SspritesetsPath, "");
        currentSpriteset = ini.Read(ScurrentSpriteset, "");

        animationsPath = ini.Read(SanimationsPath, Application.dataPath + "/StreamingAssets/");

        lastPlaybackSpeed = ParseToSingle(ini.Read(SlastPlaybackSpeed, "0"));

        editorStyle = (Styles)Enum.Parse(typeof(Styles), ini.Read(SeditorStyle, "Dark"));
        bgColor = ColorFromString(ini.Read(SbgColor, "RGBA(0.196,0.294,0.627,1.000)"));
    }

    public void SetGameSettings()
    {
        Screen.fullScreenMode = fullScreen ? FullScreenMode.FullScreenWindow : FullScreenMode.Windowed;

        Resolution[] resolutions = Screen.resolutions;
        List<int> acceptedResNumbers = new List<int>();

        for (int i = 0; i < resolutions.Length; i++)
        {
            Debug.Log((float)resolutions[i].width / (float)resolutions[i].height);

            if (resolutions[i].height % 9 == 0 && resolutions[i].width % 16 == 0 && Mathf.Approximately((float)resolutions[i].width / (float)resolutions[i].height, 1.777778f))
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

        ini.Write(SeditorStyle, editorStyle.ToString());
        ini.Write(SbgColor, bgColor.ToString());
    }

    private Color ColorFromString(string color)
    {
        if (color.Length <= 0) { return Color.black; }

        color = color.Substring(5, 23);
        color = Regex.Replace(color, @"\s", "");
        string[] colors = color.Split(',');
        float[] colorValues = new float[4];

        for (int i = 0; i < colors.Length; i++)
        {
            colorValues[i] = ParseToSingle(colors[i]);
        }

        return new Color(colorValues[0], colorValues[1], colorValues[2], colorValues[3]);
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