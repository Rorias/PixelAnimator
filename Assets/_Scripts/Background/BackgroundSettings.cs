using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

public class BackgroundSettings : MonoBehaviour
{
    private GameManager gameManager;

    private Image redHandle;
    private Image greenHandle;
    private Image blueHandle;
    private Slider redSlider;
    private Slider greenSlider;
    private Slider blueSlider;

    private void Awake()
    {
        redHandle = GameObject.Find("RedHandle").GetComponent<Image>();
        greenHandle = GameObject.Find("GreenHandle").GetComponent<Image>();
        blueHandle = GameObject.Find("BlueHandle").GetComponent<Image>();
        redSlider = GameObject.Find("RedSlider").GetComponent<Slider>();
        greenSlider = GameObject.Find("GreenSlider").GetComponent<Slider>();
        blueSlider = GameObject.Find("BlueSlider").GetComponent<Slider>();
    }

    private void Start()
    {
        gameManager = GameManager.Instance;

        redSlider.value = gameManager.bgColor.r * 255f;
        greenSlider.value = gameManager.bgColor.g * 255f;
        blueSlider.value = gameManager.bgColor.b * 255f;
    }

    public void BGColorRed()
    {
        Color bgRed = Camera.main.backgroundColor;
        bgRed.r = redSlider.value / 255.0f;
        Camera.main.backgroundColor = bgRed;

        redHandle.color = new Color(redHandle.color.r, 1 - bgRed.r, 1 - bgRed.r);
        gameManager.bgColor = Camera.main.backgroundColor;
        gameManager.SaveGameSettings();
    }

    public void BGColorGreen()
    {
        Color bgGreen = Camera.main.backgroundColor;
        bgGreen.g = greenSlider.value / 255.0f;
        Camera.main.backgroundColor = bgGreen;

        greenHandle.color = new Color(1 - bgGreen.g, greenHandle.color.g, 1 - bgGreen.g);
        gameManager.bgColor = Camera.main.backgroundColor;
        gameManager.SaveGameSettings();
    }

    public void BGColorBlue()
    {
        Color bgBlue = Camera.main.backgroundColor;
        bgBlue.b = blueSlider.value / 255.0f;
        Camera.main.backgroundColor = bgBlue;

        blueHandle.color = new Color(1 - bgBlue.b, 1 - bgBlue.b, blueHandle.color.b);
        gameManager.bgColor = Camera.main.backgroundColor;
        gameManager.SaveGameSettings();
    }

    public void ResetColor()
    {
        Camera.main.backgroundColor = new Color(0.196f, 0.294f, 0.627f, 1);
        gameManager.bgColor = Camera.main.backgroundColor;
        gameManager.SaveGameSettings();

        redSlider.value = gameManager.bgColor.r * 255f;
        greenSlider.value = gameManager.bgColor.g * 255f;
        blueSlider.value = gameManager.bgColor.b * 255f;
    }
}