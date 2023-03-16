using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using TMPro;

using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PixelAnimator : MonoBehaviour
{
    private GameManager gameManager = GameManager.Instance;
    private AnimationManager animManager = AnimationManager.Instance;

    //current frame and current part
    private Frame currentFrame;
    public Part currentPart;
    //Scene stuff
    public GameObject spritePrefab;

    public Transform gridTransform;
    public Material gridMaterial;

    private List<GameObject> GameParts = new List<GameObject>();
    private List<SpriteRenderer> GamePartsSRs = new List<SpriteRenderer>();

    private GameObject currentGamePart;
    private SpriteRenderer currentGamePartSR;

    private Dictionary<int, Sprite> possibleSprites = new Dictionary<int, Sprite>();

    //Scenedata
    private TMP_InputField XPosIF;
    private TMP_InputField YPosIF;

    [HideInInspector] public Slider partSelect;
    private TMP_Text partSelectText;
    private Slider frameSelect;
    private TMP_Text frameSelectText;
    private TMP_Dropdown ddSprites;
    private GameObject BackConfirmation;

    private WaitForSeconds playbackSpeedWFS = new WaitForSeconds(0.02f);

    private const float standardDelayTime = 0.3f;
    private const float hotkeyDelayTime = 0.05f;

    private float leftArrowTimer;
    private float rightArrowTimer;
    private float upArrowTimer;
    private float downArrowTimer;
    private float periodTimer;
    private float commaTimer;
    private float equalsTimer;
    private float minusTimer;

    private float lastOpenedPositionDD = 0f;
    private float playbackSpeed = 0.02f;
    private bool playingAnimation = false;
    private bool dropdownActive = false;

    private void Awake()
    {
        BackConfirmation = GameObject.Find("BackConfirmation");

        XPosIF = GameObject.Find("XPos").GetComponent<TMP_InputField>();
        YPosIF = GameObject.Find("YPos").GetComponent<TMP_InputField>();

        ddSprites = GameObject.Find("Sprites").GetComponent<TMP_Dropdown>();

        partSelect = GameObject.Find("PartSelect").GetComponent<Slider>();
        partSelectText = GameObject.Find("CurrentPart").GetComponent<TMP_Text>();

        frameSelect = GameObject.Find("FrameSelect").GetComponent<Slider>();
        frameSelectText = GameObject.Find("CurrentFrame").GetComponent<TMP_Text>();
    }

    private void Start()
    {
        if (gameManager.spritesetImages.Count > 0)
        {
            foreach (KeyValuePair<int, Sprite> mapping in gameManager.spritesetImages)
            {
                possibleSprites.Add(mapping.Key, mapping.Value);
            }
        }
        else
        {
            SceneManager.LoadScene("MainMenu");
            return;
        }

        if (possibleSprites.Count > 0)
        {
            ddSprites.AddOptions(possibleSprites.Select(x => x.Value).ToList());
        }
        else
        {
            Debug.Log("Possible sprites list is empty.");
        }

        foreach (KeyValuePair<int, Sprite> mapping in possibleSprites)
        {
            ddSprites.options[mapping.Key].text = possibleSprites[mapping.Key].name;
            ddSprites.options[mapping.Key].image = possibleSprites[mapping.Key];
        }

        UpdatePartSelectText();
        UpdateFrameSelectText();

        for (int frame = 0; frame < gameManager.currentAnimation.frames.Count; frame++)
        {
            for (int part = 0; part < gameManager.currentAnimation.frames[frame].frameParts.Count; part++)
            {
                Part thisPart = gameManager.currentAnimation.frames[frame].frameParts[part];

                if (thisPart.partIndex != -1)
                {
                    thisPart.part = GameObject.Find("Sprites").GetComponent<TMP_Dropdown>().options[thisPart.partIndex].image;
                }
            }
        }

        for (int part = 0; part < gameManager.currentAnimation.maxPartCount; part++)
        {
            CreateSprite(part);
        }

        SetValues();
        SetGrid();

        currentFrame = gameManager.currentAnimation.frames[0];
        currentPart = currentFrame.frameParts[0];
        currentGamePart = GameParts[0];
        currentGamePartSR = currentGamePart.GetComponent<SpriteRenderer>();

        if (gameManager.currentSpriteset != gameManager.currentAnimation.usedSpriteset)
        {
            Debug.Log("A different spriteset was used when making this animation. Using this one can cause the animation to look different than intended.");
        }

        if (BackConfirmation.activeSelf)
        {
            BackConfirmation.SetActive(false);
        }

        playbackSpeed = EditorSettings.lastPlaybackspeed;
        playbackSpeedWFS = new WaitForSeconds(playbackSpeed);
        GameObject.Find("PlaybackSpeed").GetComponent<TMP_InputField>().text = gameManager.ParseToString(playbackSpeed);
    }

    private void Update()
    {
        if (!dropdownActive && null != GameObject.Find("Dropdown List"))
        {
            dropdownActive = true;
            GameObject spriteContent = GameObject.Find("SpritesContent");
            RectTransform contentRect = spriteContent.GetComponent<RectTransform>();
            contentRect.position = new Vector3(contentRect.position.x, lastOpenedPositionDD, contentRect.position.z);

            for (int i = 1; i < spriteContent.transform.childCount; i++)
            {
                Image currentImage = spriteContent.transform.GetChild(i).Find("Item Background").GetComponent<Image>();
                currentImage.sprite = ddSprites.options[i - 1].image;
            }
        }
        else if (dropdownActive && null == GameObject.Find("Dropdown List"))
        {
            dropdownActive = false;
        }

        if (!dropdownActive && !playingAnimation)
        {
            if (Input.GetKeyDown(KeyCode.Equals))
            {
                if (Convert.ToInt32(frameSelect.value) < gameManager.currentAnimation.maxFrameCount)
                {
                    frameSelect.value++;
                    equalsTimer = Time.time + standardDelayTime;
                }
            }
            else if (Input.GetKeyDown(KeyCode.Minus))
            {
                if (Convert.ToInt32(frameSelect.value) > 0)
                {
                    frameSelect.value--;
                    minusTimer = Time.time + standardDelayTime;
                }
            }

            if (Input.GetKey(KeyCode.Equals) && Time.time > equalsTimer)
            {
                if (Convert.ToInt32(frameSelect.value) < gameManager.currentAnimation.maxFrameCount)
                {
                    frameSelect.value++;
                    equalsTimer = Time.time + hotkeyDelayTime;
                }
            }
            if (Input.GetKey(KeyCode.Minus) && Time.time > minusTimer)
            {
                if (Convert.ToInt32(frameSelect.value) > 0)
                {
                    frameSelect.value--;
                    minusTimer = Time.time + hotkeyDelayTime;
                }
            }

            if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                MovePartLeft();
                leftArrowTimer = Time.time + standardDelayTime;
            }
            if (Input.GetKey(KeyCode.LeftArrow) && Time.time > leftArrowTimer)
            {
                MovePartLeft();
                leftArrowTimer = Time.time + hotkeyDelayTime;
            }

            if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                MovePartRight();
                rightArrowTimer = Time.time + standardDelayTime;
            }
            if (Input.GetKey(KeyCode.RightArrow) && Time.time > rightArrowTimer)
            {
                MovePartRight();
                rightArrowTimer = Time.time + hotkeyDelayTime;
            }

            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                MovePartUp();
                upArrowTimer = Time.time + standardDelayTime;
            }
            if (Input.GetKey(KeyCode.UpArrow) && Time.time > upArrowTimer)
            {
                MovePartUp();
                upArrowTimer = Time.time + hotkeyDelayTime;
            }

            if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                MovePartDown();
                downArrowTimer = Time.time + standardDelayTime;
            }
            if (Input.GetKey(KeyCode.DownArrow) && Time.time > downArrowTimer)
            {
                MovePartDown();
                downArrowTimer = Time.time + hotkeyDelayTime;
            }

            if (Input.GetKeyDown(KeyCode.Period))
            {
                if (Convert.ToInt32(partSelect.value) < gameManager.currentAnimation.maxPartCount)
                {
                    partSelect.value++;
                    SetSelectedPart();
                    UpdatePos();
                    periodTimer = Time.time + standardDelayTime;
                }
            }
            else if (Input.GetKeyDown(KeyCode.Comma))
            {
                if (Convert.ToInt32(partSelect.value) > 0)
                {
                    partSelect.value--;
                    SetSelectedPart();
                    UpdatePos();
                    commaTimer = Time.time + standardDelayTime;
                }
            }

            if (Input.GetKey(KeyCode.Period) && Time.time > periodTimer)
            {
                if (Convert.ToInt32(partSelect.value) < gameManager.currentAnimation.maxPartCount)
                {
                    partSelect.value++;
                    SetSelectedPart();
                    periodTimer = Time.time + hotkeyDelayTime;
                }
            }
            if (Input.GetKey(KeyCode.Comma) && Time.time > commaTimer)
            {
                if (Convert.ToInt32(partSelect.value) > 0)
                {
                    partSelect.value--;
                    SetSelectedPart();
                    commaTimer = Time.time + hotkeyDelayTime;
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.Delete))
        {
            currentGamePartSR.sprite = null;
            currentPart.part = null;
        }
    }

    #region Initialization
    private void UpdatePartSelectText()
    {
        partSelectText.text = "Sprite " + (Convert.ToInt32(partSelect.value) + 1) + "/" + gameManager.currentAnimation.maxPartCount;
    }

    private void UpdateFrameSelectText()
    {
        frameSelectText.text = "Frame " + (Convert.ToInt32(frameSelect.value) + 1) + "/" + gameManager.currentAnimation.maxFrameCount;
    }

    private void CreateSprite(int _part)
    {
        GameObject gamePart = null;

        if (null != spritePrefab)
        {
            gamePart = Instantiate(spritePrefab);
        }
        else
        {
            Debug.Log("spriteGameObject is not set.");
            SceneManager.LoadScene("MainMenu");
        }

        gamePart.name = "AnimSprite" + _part;
        GameParts.Add(gamePart);

        SpriteRenderer gamePartSpriteRdr = gamePart.GetComponent<SpriteRenderer>();
        gamePartSpriteRdr.sortingOrder = _part + 1;
        gamePartSpriteRdr.sprite = gameManager.currentAnimation.frames[0].frameParts[_part].part;
        gamePartSpriteRdr.transform.position = new Vector3(gameManager.currentAnimation.frames[0].frameParts[_part].xPos, gameManager.currentAnimation.frames[0].frameParts[_part].yPos, -(_part / 100.0f));
        gamePartSpriteRdr.flipX = gameManager.currentAnimation.frames[0].frameParts[_part].flipX;
        gamePartSpriteRdr.flipY = gameManager.currentAnimation.frames[0].frameParts[_part].flipY;
        GamePartsSRs.Add(gamePartSpriteRdr);

        if (gamePartSpriteRdr.sprite != null)
        {
            gamePart.AddComponent<PolygonCollider2D>();
        }
    }

    private void SetValues()
    {
        partSelect.maxValue = gameManager.currentAnimation.maxPartCount - 1;
        frameSelect.maxValue = gameManager.currentAnimation.maxFrameCount - 1;
    }

    private void SetGrid()
    {
        gridTransform.localScale = new Vector2(gameManager.currentAnimation.gridSizeX / 16f, gameManager.currentAnimation.gridSizeY / 16f);
        gridMaterial.mainTextureScale = new Vector2(gameManager.currentAnimation.gridSizeX / 2, gameManager.currentAnimation.gridSizeY / 2);
    }
    #endregion

    #region AnimationFunctions
    public void PlayAnimation()
    {
        if (!playingAnimation)
        {
            playingAnimation = true;

            for (int i = 0; i < GameParts.Count; i++)
            {
                if (GameParts[i].GetComponent<PolygonCollider2D>() != null)
                {
                    GameParts[i].GetComponent<PolygonCollider2D>().enabled = false;
                }
            }

            GameObject.Find("Play").GetComponentInChildren<TMP_Text>().text = "Stop";

            ddSprites.interactable = false;

            StartCoroutine(StartAnimation());
        }
        else
        {
            playingAnimation = false;

            for (int i = 0; i < GameParts.Count; i++)
            {
                if (GameParts[i].GetComponent<PolygonCollider2D>() != null)
                {
                    GameParts[i].GetComponent<PolygonCollider2D>().enabled = true;
                }
            }

            GameObject.Find("Play").GetComponentInChildren<TMP_Text>().text = "Play";

            ddSprites.interactable = true;

            StopAllCoroutines();
        }
    }

    private IEnumerator StartAnimation()
    {
        while (playingAnimation)
        {
            for (int animFrames = 0; animFrames < gameManager.currentAnimation.maxFrameCount; animFrames++)
            {
                frameSelect.value = animFrames;
                yield return playbackSpeedWFS;
            }
        }
    }
    #endregion

    #region FrameFunctions
    public void NextFrame()
    {
        frameSelect.value++;
    }

    public void PrevFrame()
    {
        frameSelect.value--;
    }

    public void ChangeSelectedFrame()
    {
        for (int i = 0; i < gameManager.currentAnimation.maxPartCount; i++)
        {
            currentFrame.frameParts[i].part = GamePartsSRs[i].sprite;
            currentFrame.frameParts[i].xPos = GameParts[i].transform.position.x;
            currentFrame.frameParts[i].yPos = GameParts[i].transform.position.y;
            currentFrame.frameParts[i].flipX = GamePartsSRs[i].flipX;
            currentFrame.frameParts[i].flipY = GamePartsSRs[i].flipY;

            GameParts[i].transform.position = new Vector3(gameManager.currentAnimation.frames[Convert.ToInt32(frameSelect.value)].frameParts[i].xPos, gameManager.currentAnimation.frames[Convert.ToInt32(frameSelect.value)].frameParts[i].yPos, -(i / 100.0f));
            GamePartsSRs[i].sprite = gameManager.currentAnimation.frames[Convert.ToInt32(frameSelect.value)].frameParts[i].part;
            GamePartsSRs[i].flipX = gameManager.currentAnimation.frames[Convert.ToInt32(frameSelect.value)].frameParts[i].flipX;
            GamePartsSRs[i].flipY = gameManager.currentAnimation.frames[Convert.ToInt32(frameSelect.value)].frameParts[i].flipY;

            if (!playingAnimation)
            {
                if (GameParts[i].GetComponent<PolygonCollider2D>() && GameParts[i].GetComponent<PolygonCollider2D>().enabled)
                {
                    Destroy(GameParts[i].GetComponent<PolygonCollider2D>());
                    GameParts[i].AddComponent<PolygonCollider2D>();
                }
            }
        }

        currentFrame = gameManager.currentAnimation.frames[Convert.ToInt32(frameSelect.value)];
        UpdateFrameSelectText();

        if (!playingAnimation)
        {
            SetSelectedPart();
            LoadPartData();
        }
    }

    private void LoadPartData()
    {
        currentPart.part = currentGamePartSR.sprite;
        currentPart.flipX = currentGamePartSR.flipX;
        currentPart.flipY = currentGamePartSR.flipY;

        UpdatePos();
    }

    public void SetPlaybackSpeed()
    {
        playbackSpeed = gameManager.ParseToSingle(GameObject.Find("PlaybackSpeed").GetComponent<TMP_InputField>().text);

        if (playbackSpeed < 0.001f)
        {
            Debug.Log("Playback speed cannot be lower than 0.001. Auto-set to 0.001.");
            playbackSpeed = 0.001f;
        }

        EditorSettings.SetLastPlayspeed(playbackSpeed);
        playbackSpeedWFS = new WaitForSeconds(playbackSpeed);
    }
    #endregion

    #region PartFunctions
    public void SetPartSpriteForFrame()
    {
        currentPart.part = ddSprites.options[ddSprites.value].image;
        currentPart.partIndex = ddSprites.value;
        currentGamePartSR.sprite = currentPart.part;
        //When setting a sprite for the gamePart, turn on the box collider
        if (currentGamePart.GetComponent<PolygonCollider2D>() != null)
        {
            Destroy(currentGamePart.GetComponent<PolygonCollider2D>());
        }
        currentGamePart.AddComponent<PolygonCollider2D>();

        lastOpenedPositionDD = GameObject.Find("SpritesContent").GetComponent<RectTransform>().position.y;
    }

    public void SetSelectedPart()
    {
        Debug.Log(partSelect + " partSelect");

        if (Convert.ToInt32(partSelect.value) < gameManager.currentAnimation.maxPartCount)
        {
            currentGamePart = GameParts[Convert.ToInt32(partSelect.value)];
            currentGamePartSR = currentGamePart.GetComponent<SpriteRenderer>();

            currentPart = currentFrame.frameParts[Convert.ToInt32(partSelect.value)];
            UpdatePartSelectText();
            UpdatePriorityText();

            if (currentGamePart.GetComponent<PolygonCollider2D>() != null)
            {
                //if the part from the new frame has no sprite
                if (currentGamePartSR.sprite == null)
                {
                    //turn off the box collider
                    currentGamePart.GetComponent<PolygonCollider2D>().enabled = false;
                }
                else//otherwise
                {
                    //turn on the box collider
                    currentGamePart.GetComponent<PolygonCollider2D>().enabled = true;
                }
            }
        }
    }

    public void FlipX()
    {
        currentGamePartSR.flipX = !currentGamePartSR.flipX;
        currentPart.flipX = currentGamePartSR.flipX;
    }

    public void FlipY()
    {
        currentGamePartSR.flipY = !currentGamePartSR.flipY;
        currentPart.flipY = currentGamePartSR.flipY;
    }

    public void SetPriority()
    {
        int priority = Convert.ToInt32(GameObject.Find("Priority").GetComponent<TMP_InputField>().text);

        if (priority < 1)
        {
            Debug.Log("Priority cannot be lower than 1. Auto-set to 1.");
            priority = 1;
        }

        currentGamePartSR.sortingOrder = priority;
        currentGamePart.transform.position = new Vector3(currentGamePart.transform.position.x, currentGamePart.transform.position.y, -(priority / 100.0f));
        UpdatePriorityText();
    }

    public void UpdatePriorityText()
    {
        GameObject.Find("Priority").GetComponent<TMP_InputField>().text = currentGamePartSR.sortingOrder.ToString();
    }

    public void SetXPos()
    {
        currentGamePart.transform.position = new Vector3((Convert.ToSingle(XPosIF.text) + (gameManager.currentAnimation.gridSizeX / 2)) / 16f, currentGamePart.transform.position.y, currentGamePart.transform.position.z);
        UpdatePos();
    }

    public void SetYPos()
    {
        currentGamePart.transform.position = new Vector3(currentGamePart.transform.position.x, (Convert.ToSingle(YPosIF.text) - (gameManager.currentAnimation.gridSizeY / 2)) / 16f, currentGamePart.transform.position.z);
        UpdatePos();
    }

    public void MovePartUp()
    {
        if (null == currentGamePartSR.sprite)
        {
            Debug.Log("Select an image for the sprite first.");
            return;
        }

        currentGamePart.transform.position += new Vector3(0, 1.0f / 16.0f, 0);
        UpdatePos();
    }

    public void MovePartDown()
    {
        if (null == currentGamePartSR.sprite)
        {
            Debug.Log("Select an image for the sprite first.");
            return;
        }

        currentGamePart.transform.position -= new Vector3(0, 1.0f / 16.0f, 0);
        UpdatePos();
    }

    public void MovePartRight()
    {
        if (null == currentGamePartSR.sprite)
        {
            Debug.Log("Select an image for the sprite first.");
            return;
        }

        currentGamePart.transform.position += new Vector3(1.0f / 16.0f, 0, 0);
        UpdatePos();
    }

    public void MovePartLeft()
    {
        if (null == currentGamePartSR.sprite)
        {
            Debug.Log("Select an image for the sprite first.");
            return;
        }

        currentGamePart.transform.position -= new Vector3(1.0f / 16.0f, 0, 0);
        UpdatePos();
    }

    public void FixPartX()
    {
        currentGamePart.transform.position += new Vector3(1.0f / 32.0f, 0, 0);
        UpdatePos();
    }

    public void FixPartY()
    {
        currentGamePart.transform.position += new Vector3(0, 1.0f / 32.0f, 0);
        UpdatePos();
    }

    public void UpdatePos()
    {
        currentPart.xPos = currentGamePart.transform.position.x;
        currentPart.yPos = currentGamePart.transform.position.y;

        YPosIF.text = gameManager.ParseToString((currentGamePart.transform.position.y * 16f) + (gameManager.currentAnimation.gridSizeY / 2));

        XPosIF.text = gameManager.ParseToString((currentGamePart.transform.position.x * 16f) - (gameManager.currentAnimation.gridSizeX / 2));
    }
    #endregion

    public void BackConfirmationPopup()
    {
        BackConfirmation.SetActive(!BackConfirmation.activeSelf);
    }

    public void Save()
    {
        animManager.SaveFile(gameManager.currentAnimation);
    }

    public void Quit()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void SaveAndQuit()
    {
        Save();
        Quit();
    }
}