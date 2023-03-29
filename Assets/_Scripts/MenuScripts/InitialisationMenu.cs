using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using TMPro;

using UnityEngine;

public class InitialisationMenu : MonoBehaviour
{
    public MainMenuItem startMenu;

    private TMP_InputField spritesetPathIF;
    private TMP_Dropdown currentSpritesetDD;

    private MainMenu mainMenu;
    private GameManager gameManager;

    private void Awake()
    {
        spritesetPathIF = GameObject.Find("InitSpritesetPath").GetComponent<TMP_InputField>();
        spritesetPathIF.onValueChanged.AddListener(delegate { SetSpritesetPathViaBrowse(); });
        spritesetPathIF.onEndEdit.AddListener(delegate { SetSpritesetPath(); });

        currentSpritesetDD = GameObject.Find("InitSpritesetDD").GetComponent<TMP_Dropdown>();
        currentSpritesetDD.onValueChanged.AddListener(delegate { SetCurrentSpriteset(); });
    }

    private void Start()
    {
        mainMenu = GetComponent<MainMenu>();
        gameManager = GameManager.Instance;
        spritesetPathIF.text = gameManager.spritesetsPath;
    }

    public void ApplySettings()
    {
        if (string.IsNullOrWhiteSpace(gameManager.spritesetsPath) || !Directory.Exists(gameManager.spritesetsPath))
        {
            Debug.Log("No spriteset path has been selected, or the selected path is not valid.");
            DebugHelper.Log("No spriteset path has been selected, or the selected path is not valid.");
            return;
        }

        if (gameManager.currentSpriteset == "")
        {
            Debug.Log("No spriteset has been selected from the list.");
            DebugHelper.Log("No spriteset has been selected from the list.");
            return;
        }

        mainMenu.ActivateNextMenu(startMenu);
    }

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
                Debug.Log("Path cannot be found. Check if you spelled it correctly or use the browse button instead.");
                DebugHelper.Log("Path cannot be found. Check if you spelled it correctly or use the browse button instead.");
                currentSpritesetDD.ClearOptions();
            }
        }
    }

    public void SetCurrentSpriteset()
    {
        Debug.Log("setting spriteset");
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

                if (!SettingsMenu.imageTypes.Contains(Path.GetExtension(f).ToLowerInvariant()))
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
                                                      .Where(s => s.EndsWith(SettingsMenu.imageTypes[0]) || s.EndsWith(SettingsMenu.imageTypes[1]) || s.EndsWith(SettingsMenu.imageTypes[2])).ToArray();

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

            if (spriteName.Contains(SettingsMenu.imageTypes[0]) || spriteName.Contains(SettingsMenu.imageTypes[1]) || spriteName.Contains(SettingsMenu.imageTypes[2]))
            {
                spriteName = _images[i].Remove(_images[i].Length - 4);
            }

            newSprite.name = spriteName.Substring(spriteName.LastIndexOf('\\') + 1);

            gameManager.spritesetImages.Add(i, newSprite);
        }
    }
}