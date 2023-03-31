using System.Collections.Generic;
using System.Linq;

using TMPro;

using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Tooltips : MonoBehaviour
{
    private List<RaycastResult> rayResults = new List<RaycastResult>();

    public Toggle extendedTooltips;

    public RectTransform rect;
    public Image bg;
    public TMP_Text tooltipText;

    private Vector2 refRes;

    private bool tooltipsOn = true;
    private bool extendedOn = false;

    #region Tooltip text
    #region Editor settings
    private const string SaveButton = "Saves the animation to a .xml and .cs file.\n";
    private const string SaveButtonExtended = "The .xml file will be used to be able to edit the animation again later.\n" +
                                              "The .cs file can be used for any personal purposes.";

    private const string GifButton = "Saves the animation to a zip with images of all the frames.\n" +
                                     "This zip is located in the same path as the animation itself.\n";
    private const string GifButtonExtended = "The GIF images use the size of grid and the camera zoom to determine what images to create.\n" +
                                             "It is suggested to use only multiples of 2 for the camera zoom when exporting a gif.";

    private const string ReverseButton = "Saves the animation with the frame order reversed to a seperate file.\n";

    private const string CameraZoom = "Type the zoom level of the camera.\n<b>You can also increase and decrease the zoom level using the scroll wheel.</b>";
    private const string CameraSpeed = "Type the speed you want the camera to move at.\n<b>You can move the camera using the wasd keys.</b>";

    private const string ExtendedTooltipsToggle = "Having this on will display useful/advanced extra information about many buttons, toggles and fields.\n";
    private const string ExtendedTooltipsExtended = "See? Like this. The more you know!";

    private const string GridLOD = "Sets the grid's Level Of Detail. The higher the value, the more pixels per grid square.";
    private const string GridX = "Sets the width of the grid. This won't be saved unless you save the animation.";
    private const string GridY = "Sets the height of the grid. This won't be saved unless you save the animation.";

    private const string ResetColorButton = "Resets the background color to the default color used on editor startup.";

    private const string Play = "Press this button to start playing your animation.\nCopy To Next will be automatically turned off when you press play.\n";
    private const string PlayExtended = "No functions can be used whilst the animation is playing to prevent accidental changes during the playback.";

    private const string PlaybackSpeed = "Type the speed you want your animation to play at.\nThe lower the value, the faster the animation will play.\n";
    private const string PlaybackSpeedExtended = "You cannot go lower than 0.001.";
    #endregion
    #region Frame settings
    private const string PreviousFrame = "Load the previous frame of the animation to edit.\n";
    private const string NextFrame = "Load the next frame of the animation to edit.\n";
    private const string NextPreviousFrameExtended = "Can be especially useful for animations with 50+ frames.";

    private const string ClearFrame = "Clears the current frame of all it's sprites.\n<b>You can press the delete key if you only want to delete the selected sprite.</b>\n";
    private const string ClearFrameExtended = "The positions of the sprites when they were set to none <b>don't change.</b>";

    private const string FrameSlider = "Select the frame of animation you want to edit.\n<b>You can also use '+' and '-' to change the current frame.</b>\n";
    private const string FrameSliderExtended = "Changing the frame will not save the entire animation, only the position of all the sprites when you switch.";

    private const string RemoveFrame = "Decrease the total frames of the animation by one each time you press it.\n";
    private const string RemoveFrameExtended = "It will always be the last frame that gets removed, not the selected frame.";

    private const string AddFrame = "Increase the total frames of the animation by one each time you press it.\n";
    private const string AddFrameExtended = "It will always be the last frame that gets added, not the one after the selected frame.";

    private const string CopyToNextToggle = "Having this on will save all the current sprites and their positions to the next frame.\n";
    private const string CopyToNextToggleExtended = "If there are already sprites set in the next frame, this will be ignored.\n" +
                                              "<b>Deleting a sprite but not selecting a new one with Copy To Next still on will set the deleted sprite again.</b>\n" +
                                              "This is automatically turned off when playing back the animation.";

    private const string GhostingToggle = "Having this on will show you a transparent version of the sprites from the last frame.\n";
    private const string GhostingToggleExtended = "This is automatically turned off when playing back the animation.";
    #endregion
    #region Part settings
    private const string XPos = "Type the new X position of the currently selected sprite.\n";
    private const string YPos = "Type the new Y position of the currently selected sprite.\n";
    private const string XYPosExtended = "<b>You can also use the arrow keys to move a part one pixel at a time.</b>";

    private const string FlipX = "Having this on will mirror the image horizontally for the currently selected sprite.\n";
    private const string FlipY = "Having this on will mirror the image vertically for the currently selected sprite.\n";
    private const string FlipXYExtended = "This will not be saved through multiple frames unless Copy To Next is on.";

    private const string FixX = "Fixes the X position of the sprite if it is not correctly centered on the grid.\n";
    private const string FixY = "Fixes the Y position of the sprite if it is not correctly centered on the grid.\n";
    private const string FixXYExtended = "This is neccesary due to the fact that when selecting certain sprites they are automatically off centered.\n" +
                                         "Moving them with the mouse is also a way of fixing this problem.";

    private const string PartSlider = "Select the sprite in the frame that you want to edit.\n<b>You can also use '<' and '>' to change which sprite you are editing.</b>\n";
    private const string PartSliderExtended = "It is possible to use your mouse to move around sprites in the editor as well.";

    private const string AddPart = "Increases the total sprites of the animation by one each time you press it.\n";
    private const string AddPartExtended = "It will always be the last sprite that gets added.";

    private const string RemovePart = "Decreases the total sprites of the animation by one each time you press it.\n";
    private const string RemovePartExtended = "It will always be the last sprite that gets removed, not the selected or first sprite.";

    private const string Priority = "Set the priority of the currently selected sprite throughout the entire animation.\n";
    private const string PriorityExtended = "<b>Changing the priority in e.g. frame 5, will also change the priority for all the other frames.</b>\n" +
                                            "A higher priority number means it gets drawn over sprites with a lower number.";
    #endregion
    #endregion

    private void Awake()
    {
        refRes = GameObject.Find("UI").GetComponent<CanvasScaler>().referenceResolution;
    }

    private void Update()
    {
        if (!tooltipsOn)
        {
            DisableTooltip();
            return;
        }

        EventSystem.current.RaycastAll(new PointerEventData(EventSystem.current)
        { position = Input.mousePosition, pointerId = -1 }, rayResults);

        if (rayResults.Count <= 0)
        {
            DisableTooltip();
            return;
        }

        bg.color = new Color(1, 1, 1, 0.7f);

        tooltipText.text = rayResults[0].gameObject.name switch
        {
            //Editor settings
            "SaveButton" => SaveButton + (extendedOn ? SaveButtonExtended : ""),
            "GifButton" => GifButton + (extendedOn ? GifButtonExtended : ""),
            "ReverseButton" => ReverseButton,
            "CameraZoom" or "ZoomText" => CameraZoom,
            "CameraSpeed" or "SpeedText" => CameraSpeed,
            "ExtendedTooltipToggleBackground" => ExtendedTooltipsToggle + (extendedOn ? ExtendedTooltipsExtended : ""),
            "GridLOD" or "GridLODInputText" => GridLOD,
            "GridX" or "GridXInputText" => GridX,
            "GridY" or "GridYInputText" => GridY,
            "ResetColorButton" => ResetColorButton,
            "Play" => Play + (extendedOn ? PlayExtended : ""),
            "PlaybackSpeed" or "PlaybackInputText" => PlaybackSpeed + (extendedOn ? PlaybackSpeedExtended : ""),
            //Frame settings
            "RemoveFrame" => RemoveFrame + (extendedOn ? RemoveFrameExtended : ""),
            "AddFrame" => AddFrame + (extendedOn ? AddFrameExtended : ""),
            "PreviousFrame" => PreviousFrame + (extendedOn ? NextPreviousFrameExtended : ""),
            "NextFrame" => NextFrame + (extendedOn ? NextPreviousFrameExtended : ""),
            "FrameSelectBG" or "FrameFill" => FrameSlider + (extendedOn ? FrameSliderExtended : ""),
            "CopyToggleBackground" => CopyToNextToggle + (extendedOn ? CopyToNextToggleExtended : ""),
            "GhostToggleBackground" => GhostingToggle + (extendedOn ? GhostingToggleExtended : ""),
            "ClearFrame" => ClearFrame + (extendedOn ? ClearFrameExtended : ""),
            //Part settings
            "RemovePart" => RemovePart + (extendedOn ? RemovePartExtended : ""),
            "AddPart" => AddPart + (extendedOn ? AddPartExtended : ""),
            "PartSelectBG" or "PartFill" => PartSlider + (extendedOn ? PartSliderExtended : ""),
            "Xpos" or "XposInputText" => XPos + (extendedOn ? XYPosExtended : ""),
            "Ypos" or "YposInputText" => YPos + (extendedOn ? XYPosExtended : ""),
            "FlipXBG" => FlipX + (extendedOn ? FlipXYExtended : ""),
            "FlipYBG" => FlipY + (extendedOn ? FlipXYExtended : ""),
            "FixX" => FixX + (extendedOn ? FixXYExtended : ""),
            "FixY" => FixY + (extendedOn ? FixXYExtended : ""),
            "Priority" or "PriorityInputText" => Priority + (extendedOn ? PriorityExtended : ""),
            _ => "",
        };

        if (tooltipText.text == "")
        {
            DisableTooltip();
            return;
        }

        int textLength = tooltipText.text.Split('.').Max(x => x.Length);

        rect.sizeDelta = new Vector2(textLength * 7f, 70);

        float tooltipsSizeX = (rect.sizeDelta.x / 2) * (Screen.width / refRes.x);
        float tooltipsSizeY = (rect.sizeDelta.y / 2) * (Screen.height / refRes.y);
        transform.position = new Vector3(Input.mousePosition.x - (Input.mousePosition.x > (Screen.width / 2) ? tooltipsSizeX : -tooltipsSizeX),
                                         Input.mousePosition.y - (Input.mousePosition.y > (Screen.height * 0.93f) ? tooltipsSizeY : -tooltipsSizeY),
                                         Input.mousePosition.z);
    }

    private void DisableTooltip()
    {
        bg.color = new Color(0, 0, 0, 0);
        tooltipText.text = "";
    }

    public void TooltipState()
    {
        tooltipsOn = !tooltipsOn;
        extendedTooltips.interactable = tooltipsOn;
    }

    public void ExtendedState()
    {
        extendedOn = !extendedOn;
    }
}