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

    //current frame and current part
    private Frame currentFrame;
    public Part currentPart;
    //Scene stuff
    public GameObject spritePrefab;
    public GameObject ghostPrefab;

    private List<GameObject> GameParts = new List<GameObject>();
    private List<SpriteRenderer> GamePartsSRs = new List<SpriteRenderer>();
    private List<GameObject> GhostParts = new List<GameObject>();
    private List<SpriteRenderer> GhostPartsSRs = new List<SpriteRenderer>();

    private GameObject currentGamePart;
    private SpriteRenderer currentGamePartSR;
    private Animator currentGamePartAnimator;

    private Dictionary<int, Sprite> possibleSprites = new Dictionary<int, Sprite>();

    //Scenedata
    private TMP_InputField xPosIF;
    private TMP_InputField yPosIF;

    private TMP_InputField[] allInputfields;
    private Button[] allButtons;
    private Toggle[] allToggles;

    [NonSerialized] public Slider partSelect;
    private TMP_Text partSelectText;
    public Slider frameSelect { get; private set; }
    private TMP_Text frameSelectText;
    private TMP_Dropdown ddSprites;
    private GameObject backConfirmation;

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
    private bool copyToNextFrame = true;
    private bool copyToNextFrameWasOn = false;
    private bool ghosting = true;
    private bool ghostingWasOn = false;
    private bool focussed = false;

    private void Awake()
    {
        allInputfields = FindObjectsOfType<TMP_InputField>();
        allButtons = FindObjectsOfType<Button>();
        allToggles = FindObjectsOfType<Toggle>();

        xPosIF = GameObject.Find("XPos").GetComponent<TMP_InputField>();
        yPosIF = GameObject.Find("YPos").GetComponent<TMP_InputField>();

        partSelect = GameObject.Find("PartSelect").GetComponent<Slider>();
        partSelectText = GameObject.Find("CurrentPart").GetComponent<TMP_Text>();

        frameSelect = GameObject.Find("FrameSelect").GetComponent<Slider>();
        frameSelectText = GameObject.Find("CurrentFrame").GetComponent<TMP_Text>();

        ddSprites = GameObject.Find("Sprites").GetComponent<TMP_Dropdown>();

        backConfirmation = GameObject.Find("BackConfirmation");
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
            CreateGhostSprite(part);
        }

        SetValues();

        currentFrame = gameManager.currentAnimation.frames[0];
        currentPart = currentFrame.frameParts[0];
        currentGamePart = GameParts[0];
        currentGamePartSR = currentGamePart.GetComponent<SpriteRenderer>();
        currentGamePartAnimator = currentGamePart.GetComponent<Animator>();

        if (gameManager.currentSpriteset != gameManager.currentAnimation.usedSpriteset)
        {
            Debug.Log("A different spriteset was used when making this animation. Using this one can cause the animation to look different than intended.");
        }

        if (backConfirmation.activeSelf)
        {
            backConfirmation.SetActive(false);
        }

        GameObject.Find("PlaybackSpeed").GetComponent<TMP_InputField>().text = gameManager.ParseToString(gameManager.lastPlaybackSpeed);
        SetPlaybackSpeed();
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

        foreach (TMP_InputField IF in allInputfields)
        {
            if (IF.isFocused)
            {
                focussed = true;
                break;
            }
        }

        focussed = false;

        if (!focussed && !dropdownActive && !playingAnimation)
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
            Debug.Log("spritePrefab is not set.");
            SceneManager.LoadScene("MainMenu");
        }

        gamePart.name = "AnimSprite" + _part;
        GameParts.Add(gamePart);

        SpriteRenderer gamePartSpriteRdr = gamePart.GetComponent<SpriteRenderer>();
        gamePartSpriteRdr.sortingOrder = _part + 1;
        gamePartSpriteRdr.sprite = gameManager.currentAnimation.frames[0].frameParts[_part].part;
        gamePartSpriteRdr.transform.position = new Vector3(gameManager.currentAnimation.frames[0].frameParts[_part].xPos, gameManager.currentAnimation.frames[0].frameParts[_part].yPos, 0);
        gamePartSpriteRdr.flipX = gameManager.currentAnimation.frames[0].frameParts[_part].flipX;
        gamePartSpriteRdr.flipY = gameManager.currentAnimation.frames[0].frameParts[_part].flipY;
        GamePartsSRs.Add(gamePartSpriteRdr);

        if (gamePartSpriteRdr.sprite != null)
        {
            gamePart.AddComponent<PolygonCollider2D>();
        }
    }

    private void CreateGhostSprite(int _part)
    {
        GameObject ghostPart = null;

        if (null != ghostPrefab)
        {
            ghostPart = Instantiate(ghostPrefab);
        }
        else
        {
            Debug.Log("ghostPrefab is not set.");
            SceneManager.LoadScene("MainMenu");
        }

        ghostPart.name = "GhostSprite" + _part;
        GhostParts.Add(ghostPart);

        SpriteRenderer ghostPartSpriteRdr = ghostPart.GetComponent<SpriteRenderer>();
        ghostPartSpriteRdr.color = new Color(0, 1, 1, 0.25f);
        GhostPartsSRs.Add(ghostPartSpriteRdr);
    }

    private void SetValues()
    {
        partSelect.maxValue = gameManager.currentAnimation.maxPartCount - 1;
        frameSelect.maxValue = gameManager.currentAnimation.maxFrameCount - 1;
    }
    #endregion

    #region AnimationFunctions
    public void PlayAnimation()
    {
        if (!playingAnimation)
        {
            playingAnimation = true;

            currentGamePartAnimator.SetBool("WasSelected", false);

            if (copyToNextFrame) { copyToNextFrameWasOn = true; copyToNextFrame = false; }
            else { copyToNextFrameWasOn = false; }

            if (ghosting) { ghostingWasOn = true; ghosting = false; }
            else { ghostingWasOn = false; }

            for (int i = 0; i < GameParts.Count; i++)
            {
                if (GameParts[i].GetComponent<PolygonCollider2D>() != null)
                {
                    GameParts[i].GetComponent<PolygonCollider2D>().enabled = false;
                }
            }

            GameObject.Find("Play").GetComponentInChildren<TMP_Text>().text = "Stop";
            SetButtonToggleInputfieldState(false);

            ddSprites.interactable = false;

            StartCoroutine(StartAnimation());
        }
        else
        {
            playingAnimation = false;

            if (copyToNextFrameWasOn) { copyToNextFrame = true; }
            if (ghostingWasOn) { ghosting = true; }

            for (int i = 0; i < GameParts.Count; i++)
            {
                if (GameParts[i].GetComponent<PolygonCollider2D>() != null)
                {
                    GameParts[i].GetComponent<PolygonCollider2D>().enabled = true;
                }
            }

            GameObject.Find("Play").GetComponentInChildren<TMP_Text>().text = "Play";
            SetButtonToggleInputfieldState(true);

            ddSprites.interactable = true;

            StopAllCoroutines();
        }
    }

    private void SetButtonToggleInputfieldState(bool _state)
    {
        foreach (TMP_InputField inputField in allInputfields)
        {
            inputField.interactable = _state;
        }

        foreach (Button button in allButtons)
        {
            if (button.name != "Play")
            {
                button.interactable = _state;
            }
        }

        foreach (Toggle toggle in allToggles)
        {
            toggle.interactable = _state;
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

    public void AnimToZipStart()
    {
        playingAnimation = true;
        currentGamePartAnimator.SetBool("WasSelected", false);
        playbackSpeed = 0.001f;
    }

    public void AnimToZipEnd()
    {
        playbackSpeed = gameManager.lastPlaybackSpeed;
        playingAnimation = false;
    }

    public void SetPlaybackSpeed()
    {
        playbackSpeed = gameManager.ParseToSingle(GameObject.Find("PlaybackSpeed").GetComponent<TMP_InputField>().text);

        if (playbackSpeed < 0.001f)
        {
            Debug.Log("Playback speed cannot be lower than 0.001. Auto-set to 0.1.");
            playbackSpeed = 0.1f;
            GameObject.Find("PlaybackSpeed").GetComponent<TMP_InputField>().text = gameManager.ParseToString(playbackSpeed);
        }

        playbackSpeedWFS = new WaitForSeconds(playbackSpeed);
        gameManager.lastPlaybackSpeed = playbackSpeed;
        gameManager.SaveGameSettings();
    }
    #endregion

    #region FrameFunctions
    public void AddFrame()
    {
        //For readability only
        Animation anim = gameManager.currentAnimation;

        int newMax = anim.maxFrameCount + 1;
        newMax = Mathf.Min(Mathf.Max(newMax, 1), 999);
        anim.maxFrameCount = newMax;

        UpdateFrameSelectText();

        anim.frames.Add(new Frame() { frameID = anim.maxFrameCount - 1 });

        for (int allParts = 0; allParts < anim.maxPartCount; allParts++)
        {
            anim.frames[anim.maxFrameCount - 1].frameParts.Add(new Part() { partID = allParts });
        }

        frameSelect.maxValue = anim.maxFrameCount - 1;
    }

    public void RemoveFrame()
    {
        //For readability only
        Animation anim = gameManager.currentAnimation;

        if (frameSelect.value == anim.maxFrameCount - 1)
        {
            frameSelect.value--;
        }

        int newMax = anim.maxFrameCount - 1;
        newMax = Mathf.Min(Mathf.Max(newMax, 1), 999);
        anim.maxFrameCount = newMax;

        UpdateFrameSelectText();

        anim.frames.RemoveAt(anim.maxFrameCount);

        frameSelect.maxValue = anim.maxFrameCount - 1;
    }

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

            if (ghosting && !playingAnimation && frameSelect.value > currentFrame.frameID)
            {
                GhostParts[i].transform.position = new Vector2(currentFrame.frameParts[i].xPos, currentFrame.frameParts[i].yPos);
                GhostPartsSRs[i].sprite = currentFrame.frameParts[i].part;
                GhostPartsSRs[i].flipX = currentFrame.frameParts[i].flipX;
                GhostPartsSRs[i].flipY = currentFrame.frameParts[i].flipY;
            }
            else
            {
                GhostPartsSRs[i].sprite = null;
            }

            if (copyToNextFrame && frameSelect.value > currentFrame.frameID &&
                gameManager.currentAnimation.frames[Convert.ToInt32(frameSelect.value)].frameParts[i].part == null)
            {
                //Set actual current part data to the last frame part data if copyToNextFrame is true
                gameManager.currentAnimation.frames[Convert.ToInt32(frameSelect.value)].frameParts[i].part = currentFrame.frameParts[i].part;
                gameManager.currentAnimation.frames[Convert.ToInt32(frameSelect.value)].frameParts[i].partIndex = currentFrame.frameParts[i].partIndex;
                gameManager.currentAnimation.frames[Convert.ToInt32(frameSelect.value)].frameParts[i].xPos = currentFrame.frameParts[i].xPos;
                gameManager.currentAnimation.frames[Convert.ToInt32(frameSelect.value)].frameParts[i].yPos = currentFrame.frameParts[i].yPos;
                gameManager.currentAnimation.frames[Convert.ToInt32(frameSelect.value)].frameParts[i].flipX = currentFrame.frameParts[i].flipX;
                gameManager.currentAnimation.frames[Convert.ToInt32(frameSelect.value)].frameParts[i].flipY = currentFrame.frameParts[i].flipY;
            }

            GameParts[i].transform.position = new Vector3(gameManager.currentAnimation.frames[Convert.ToInt32(frameSelect.value)].frameParts[i].xPos, gameManager.currentAnimation.frames[Convert.ToInt32(frameSelect.value)].frameParts[i].yPos, 0);
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

    public void ClearFrame()
    {
        for (int i = 0; i < gameManager.currentAnimation.maxPartCount; i++)
        {
            currentFrame.frameParts[i].part = null;
            GamePartsSRs[i].sprite = null;
        }
    }

    public void CopyToNext()
    {
        copyToNextFrame = !copyToNextFrame;
    }

    public void Ghosting()
    {
        ghosting = !ghosting;
    }

    private void LoadPartData()
    {
        currentPart.part = currentGamePartSR.sprite;
        currentPart.flipX = currentGamePartSR.flipX;
        currentPart.flipY = currentGamePartSR.flipY;

        UpdatePos();
    }
    #endregion

    #region PartFunctions
    public void AddPart()
    {
        //For readability only
        Animation anim = gameManager.currentAnimation;

        int newMax = anim.maxPartCount + 1;
        newMax = Mathf.Min(Mathf.Max(newMax, 1), 99);
        anim.maxPartCount = newMax;

        UpdatePartSelectText();

        for (int allFrames = 0; allFrames < anim.maxFrameCount; allFrames++)
        {
            anim.frames[allFrames].frameParts.Add(new Part() { partID = anim.maxPartCount - 1 });
        }

        GameObject newSprite = Instantiate(spritePrefab);
        newSprite.name = "AnimSprite" + (anim.maxPartCount - 1);
        GameParts.Add(newSprite);
        SpriteRenderer newSpriteRenderer = newSprite.GetComponent<SpriteRenderer>();
        newSpriteRenderer.sortingOrder = anim.maxPartCount;
        GamePartsSRs.Add(newSpriteRenderer);

        GameObject newGhost = Instantiate(ghostPrefab);
        newGhost.name = "GhostSprite" + (anim.maxPartCount - 1);
        GhostParts.Add(newGhost);
        SpriteRenderer newGhostRenderer = newGhost.GetComponent<SpriteRenderer>();
        newGhostRenderer.color = new Color(0, 1, 1, 0.25f);
        GhostPartsSRs.Add(newGhostRenderer);

        partSelect.maxValue = anim.maxPartCount - 1;
    }

    public void RemovePart()
    {
        //For readability only
        Animation anim = gameManager.currentAnimation;

        if (partSelect.value == anim.maxPartCount - 1)
        {
            partSelect.value--;
        }

        int newMax = anim.maxPartCount - 1;
        newMax = Mathf.Min(Mathf.Max(newMax, 1), 99);
        anim.maxPartCount = newMax;

        UpdatePartSelectText();

        for (int allFrames = 0; allFrames < anim.maxFrameCount; allFrames++)
        {
            anim.frames[allFrames].frameParts.RemoveAt(anim.maxPartCount);
        }

        GameParts.RemoveAt(anim.maxPartCount);
        GamePartsSRs.RemoveAt(anim.maxPartCount);

        GhostParts.RemoveAt(anim.maxPartCount);
        GhostPartsSRs.RemoveAt(anim.maxPartCount);

        partSelect.maxValue = anim.maxPartCount - 1;

        Destroy(GameObject.Find("AnimSprite" + anim.maxPartCount));
        Destroy(GameObject.Find("GhostSprite" + anim.maxPartCount));
    }

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
        currentGamePartAnimator.SetBool("WasSelected", true);

        lastOpenedPositionDD = GameObject.Find("SpritesContent").GetComponent<RectTransform>().position.y;
    }

    public void SetSelectedPart()
    {
        Debug.Log(partSelect + " partSelect");

        if (Convert.ToInt32(partSelect.value) < gameManager.currentAnimation.maxPartCount)
        {
            if (currentGamePartAnimator != null)
            {
                currentGamePartAnimator.SetBool("WasSelected", false);
            }

            currentGamePart = GameParts[Convert.ToInt32(partSelect.value)];
            currentGamePartSR = currentGamePart.GetComponent<SpriteRenderer>();
            currentGamePartAnimator = currentGamePart.GetComponent<Animator>();

            if (!playingAnimation)
            {
                currentGamePartAnimator.SetBool("WasSelected", true);
            }

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
        currentGamePart.transform.position = new Vector3(currentGamePart.transform.position.x, currentGamePart.transform.position.y, 0);
        UpdatePriorityText();
    }

    public void UpdatePriorityText()
    {
        GameObject.Find("Priority").GetComponent<TMP_InputField>().text = currentGamePartSR.sortingOrder.ToString();
    }

    public void SetXPos()
    {
        currentGamePart.transform.position = new Vector3((Convert.ToSingle(xPosIF.text) + (gameManager.currentAnimation.gridSizeX / 2)) / 16f, currentGamePart.transform.position.y, currentGamePart.transform.position.z);
        UpdatePos();
    }

    public void SetYPos()
    {
        currentGamePart.transform.position = new Vector3(currentGamePart.transform.position.x, (Convert.ToSingle(yPosIF.text) - (gameManager.currentAnimation.gridSizeY / 2)) / 16f, currentGamePart.transform.position.z);
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

        yPosIF.text = gameManager.ParseToString((currentGamePart.transform.position.y * 16f) + (gameManager.currentAnimation.gridSizeY / 2));

        xPosIF.text = gameManager.ParseToString((currentGamePart.transform.position.x * 16f) - (gameManager.currentAnimation.gridSizeX / 2));
    }
    #endregion

    public void BackConfirmationPopup()
    {
        backConfirmation.SetActive(!backConfirmation.activeSelf);
    }

    public void SaveAndQuit()
    {
        AnimationManager animManager = AnimationManager.Instance;
        animManager.SaveFile(gameManager.currentAnimation);
        Quit();
    }

    public void Quit()
    {
        gameManager.SaveGameSettings();
        SceneManager.LoadScene("MainMenu");
    }
}