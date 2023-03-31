using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

using TMPro;

using UnityEngine;

public class SettingsMenu : MonoBehaviour
{
    public static readonly List<string> imageTypes = new List<string> { ".jpg", ".jpeg", ".png" };

    private GameManager gameManager;

    //Settings UI elements
    //Personal
    private TMP_InputField spritesetPathIF;
    private TMP_Dropdown currentSpritesetDD;

    private TMP_InputField animationsPathIF;
    //Editor
    public TMP_Dropdown resDD;

    private void Awake()
    {
        spritesetPathIF = GameObject.Find("SpritesetPath").GetComponent<TMP_InputField>();
        spritesetPathIF.onValueChanged.AddListener(delegate { SetSpritesetPathViaBrowse(); });
        spritesetPathIF.onEndEdit.AddListener(delegate { SetSpritesetPath(); });

        currentSpritesetDD = GameObject.Find("SpritesetDD").GetComponent<TMP_Dropdown>();
        currentSpritesetDD.onValueChanged.AddListener(delegate { SetCurrentSpriteset(); });

        animationsPathIF = GameObject.Find("AnimationsPath").GetComponent<TMP_InputField>();
        animationsPathIF.onValueChanged.AddListener(delegate { SetAnimationsPathViaBrowse(); });
        animationsPathIF.onEndEdit.AddListener(delegate { SetAnimationsPath(); });
    }

    private void Start()
    {
        gameManager = GameManager.Instance;
    }

    public void InitializeSettingsMenu()
    {
        LoadSpritesetSettings();
        LoadAnimationsSettings();
        LoadResSettings();
    }

    private void LoadSpritesetSettings()
    {
        if (!string.IsNullOrWhiteSpace(gameManager.spritesetsPath))
        {
            if (Directory.Exists(gameManager.spritesetsPath))
            {
                spritesetPathIF.text = gameManager.spritesetsPath;
                currentSpritesetDD.value = currentSpritesetDD.options.FindIndex(x => x.text == gameManager.currentSpriteset);
            }
            else
            {
                UnityEngine.Debug.Log("Spriteset path doesn't exist or has been changed.");
                DebugHelper.Log("Spriteset path doesn't exist or has been changed.");
            }
        }
    }

    private void LoadAnimationsSettings()
    {
        if (!string.IsNullOrWhiteSpace(gameManager.animationsPath))
        {
            if (Directory.Exists(gameManager.animationsPath))
            {
                animationsPathIF.text = gameManager.animationsPath;
            }
            else
            {
                UnityEngine.Debug.Log("Animations path doesn't exist or has been changed.");
                DebugHelper.Log("Animations path doesn't exist or has been changed.");
            }
        }
    }

    public void LoadResSettings()
    {
        resDD.ClearOptions();

        Resolution[] resolutions = Screen.resolutions;

        List<string> resOptions = new List<string>();

        foreach (Resolution res in resolutions)
        {
            if (res.height % 9 == 0 && res.width % 16 == 0 && Mathf.Approximately((float)res.width / (float)res.height, 1.777778f))
            {
                resOptions.Add(res.width + "x" + res.height + " : " + res.refreshRate);
            }
        }

        resDD.AddOptions(resOptions);

        for (int i = 0; i < resOptions.Count; i++)
        {
            resDD.options[i].text = resOptions[i];
        }

        resDD.value = gameManager.resNumber;

        gameManager.SaveGameSettings();
        gameManager.SetGameSettings();
    }

    public void SetResolution()
    {
        gameManager.resNumber = resDD.value;

        gameManager.SaveGameSettings();
        gameManager.SetGameSettings();
    }

    public void OpenSettingsFile()
    {
        ProcessStartInfo startInfo = new ProcessStartInfo
        {
            Arguments = Application.persistentDataPath + "/pixelSettings.ini",
            FileName = "notepad.exe",
        };

        Process.Start(startInfo);
    }

    #region Spriteset settings
    public void SetSpritesetPathViaBrowse()
    {
        string spritesetPath = spritesetPathIF.text;

        if (!string.IsNullOrWhiteSpace(spritesetPath))
        {
            if (Directory.Exists(spritesetPath))
            {
                gameManager.spritesetsPath = spritesetPath;
                ReloadSpritesetOptions(gameManager.spritesetsPath);
                SetCurrentSpriteset();
                gameManager.SaveGameSettings();
            }
            else
            {
                currentSpritesetDD.ClearOptions();
                gameManager.currentSpriteset = "";
                gameManager.SaveGameSettings();
            }
        }
    }

    public void SetSpritesetPath()
    {
        string spritesetPath = spritesetPathIF.text;

        if (!string.IsNullOrWhiteSpace(spritesetPath))
        {
            if (Directory.Exists(spritesetPath))
            {
                gameManager.spritesetsPath = spritesetPath;
                ReloadSpritesetOptions(gameManager.spritesetsPath);
                SetCurrentSpriteset();
                gameManager.SaveGameSettings();
            }
            else
            {
                UnityEngine.Debug.Log("Path cannot be found. Check if you spelled it correctly or use the browse button instead.");
                DebugHelper.Log("Path cannot be found. Check if you spelled it correctly or use the browse button instead.");
                currentSpritesetDD.ClearOptions();
            }
        }
    }

    public void OpenSpritesetsFolder()
    {
        if (!Directory.Exists(spritesetPathIF.text))
        {
            DebugHelper.Log("Current path is invalid. Check if you spelled it correctly or if the folder still exists.");
            return;

        }

        ProcessStartInfo startInfo = new ProcessStartInfo
        {
            Arguments = spritesetPathIF.text,
            FileName = "explorer.exe",
        };

        Process.Start(startInfo);
    }

    public void SetCurrentSpriteset()
    {
        UnityEngine.Debug.Log("setting spriteset");
        gameManager.currentSpriteset = currentSpritesetDD.options[currentSpritesetDD.value].text;
        LoadSpriteset();
        gameManager.SaveGameSettings();
    }

    private void ReloadSpritesetOptions(string _path)
    {
        List<string> spritesetNames = new List<string>();
        string[] spritesets = Directory.GetDirectories(_path);

        foreach (string s in spritesets)
        {
            string[] files;

            try
            {
                files = Directory.GetFiles(s);
            }
            catch
            {
                continue;
            }

            if (files.Length <= 0) { continue; }

            bool unacceptedTypes = false;

            foreach (string f in files)
            {
                if (f.Contains("Thumbs.db"))
                {
                    continue;
                }

                if (!imageTypes.Contains(Path.GetExtension(f).ToLowerInvariant()))
                {
                    unacceptedTypes = true;
                    break;
                }
            }

            if (unacceptedTypes)
            {
                continue;
            }

            string spritesetName = s.Substring(s.LastIndexOf('\\') + 1);
            spritesetNames.Add(spritesetName);
        }

        if (currentSpritesetDD.options.Count > 0) { currentSpritesetDD.ClearOptions(); }

        currentSpritesetDD.AddOptions(spritesetNames);

        for (int i = 0; i < spritesetNames.Count; i++)
        {
            currentSpritesetDD.options[i].text = spritesetNames[i];
        }
    }

    public void LoadSpriteset()
    {
        if (!string.IsNullOrWhiteSpace(gameManager.spritesetsPath))
        {
            if (Directory.Exists(gameManager.spritesetsPath))
            {
                string[] spritesetFolders = Directory.GetDirectories(gameManager.spritesetsPath);

                if (spritesetFolders.Length > 0)
                {
                    foreach (string folder in spritesetFolders)
                    {
                        if (folder.Substring(folder.LastIndexOf('\\') + 1) == gameManager.currentSpriteset)
                        {
                            string[] files = Directory.GetFiles(folder, "*.*", SearchOption.TopDirectoryOnly)
                                                      .Where(s => s.EndsWith(imageTypes[0]) || s.EndsWith(imageTypes[1]) || s.EndsWith(imageTypes[2])).ToArray();

                            ImportImagesAsSprites(files);
                            break;
                        }
                    }
                }
            }
        }
    }

    private void ImportImagesAsSprites(string[] _images)
    {
        gameManager.spritesetImages.Clear();

        for (int i = 0; i < _images.Length; i++)
        {
            byte[] byteArray = File.ReadAllBytes(_images[i]);
            Texture2D sampleTexture = new Texture2D(2, 2);
            sampleTexture.LoadImage(byteArray);
            sampleTexture.filterMode = FilterMode.Point;
            Sprite newSprite = Sprite.Create(sampleTexture, new Rect(0, 0, sampleTexture.width, sampleTexture.height), new Vector2(0.5f, 0.5f), 16, 0, SpriteMeshType.FullRect);

            string spriteName = _images[i];

            if (spriteName.Contains(imageTypes[0]) || spriteName.Contains(imageTypes[1]) || spriteName.Contains(imageTypes[2]))
            {
                spriteName = _images[i].Remove(_images[i].Length - 4);
            }

            newSprite.name = spriteName.Substring(spriteName.LastIndexOf('\\') + 1);

            gameManager.spritesetImages.Add(i, newSprite);
        }
    }
    #endregion

    #region Animations settings
    public void SetAnimationsPathViaBrowse()
    {
        string animationsPath = animationsPathIF.text;

        if (!string.IsNullOrWhiteSpace(animationsPath))
        {
            if (Directory.Exists(animationsPath))
            {
                gameManager.animationsPath = animationsPath;
                gameManager.SaveGameSettings();
            }
        }
    }

    public void SetAnimationsPath()
    {
        string animationsPath = animationsPathIF.text;

        if (!string.IsNullOrWhiteSpace(animationsPath))
        {
            if (Directory.Exists(animationsPath))
            {
                gameManager.animationsPath = animationsPath;
                gameManager.SaveGameSettings();
            }
            else
            {
                UnityEngine.Debug.Log("Path cannot be found. Check if you spelled it correctly or use the browse button instead.");
                DebugHelper.Log("Path cannot be found. Check if you spelled it correctly or use the browse button instead.");
            }
        }
    }

    public void OpenAnimationsFolder()
    {
        if (!Directory.Exists(animationsPathIF.text))
        {
            DebugHelper.Log("Current path is invalid. Check if you spelled it correctly or if the folder still exists.");
            return;
        }

        ProcessStartInfo startInfo = new ProcessStartInfo
        {
            Arguments = animationsPathIF.text,
            FileName = "explorer.exe",
        };

        Process.Start(startInfo);
    }
    #endregion
}