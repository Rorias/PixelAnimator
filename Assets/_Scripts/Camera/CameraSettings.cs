using System;
using System.Collections;
using System.Collections.Generic;

using TMPro;

using UnityEngine;
using UnityEngine.Events;

public class CameraSettings : MonoBehaviour
{
    [HideInInspector] public float maxCameraZoom = 15f;
    [HideInInspector] public float cameraSpeed = 0.5f;

    private TMP_InputField CameraZoomIF;
    private TMP_InputField CameraSpeedIF;

    public static event UnityAction CameraZoomed;
    public static void OnCameraZoomed() => CameraZoomed?.Invoke();

    private GameManager gameManager;

    private void Awake()
    {
        CameraZoomIF = GameObject.Find("CameraZoom").GetComponent<TMP_InputField>();
        CameraSpeedIF = GameObject.Find("CameraSpeed").GetComponent<TMP_InputField>();

        CameraZoomed += () => SetCameraIFValues();
    }

    private void Start()
    {
        gameManager = GameManager.Instance;
        SetCameraIFValues();
    }

    private void SetCameraIFValues()
    {
        CameraZoomIF.text = gameManager.ParseToString(Camera.main.orthographicSize);
        CameraSpeedIF.text = gameManager.ParseToString(cameraSpeed);
    }

    public void SetCameraZoom()
    {
        float zoomLevel = gameManager.ParseToSingle(CameraZoomIF.text);
        zoomLevel = Mathf.Min(Mathf.Max(zoomLevel, 1), maxCameraZoom);

        Camera.main.orthographicSize = zoomLevel;
        CameraZoomIF.text = gameManager.ParseToString(Camera.main.orthographicSize);
        Debug.Log("Succesfully changed camera zoom to " + Camera.main.orthographicSize.ToString() + ".");
    }

    public void SetCameraSpeed()
    {
        float camSpeed = gameManager.ParseToSingle(CameraSpeedIF.text);
        camSpeed = Mathf.Min(Mathf.Max(camSpeed, 0.1f), 5);

        cameraSpeed = camSpeed;
        CameraSpeedIF.text = gameManager.ParseToString(cameraSpeed);
        Debug.Log("Succesfully changed camera speed to " + cameraSpeed.ToString() + ".");
    }

    public void ResetCameraZoom()
    {
        Camera.main.orthographicSize = 5;
        CameraZoomIF.text = "5";
    }

    public void ResetCameraSpeed()
    {
        cameraSpeed = 0.5f;
        CameraSpeedIF.text = gameManager.ParseToString(cameraSpeed);
    }

    public void ResetCameraPosition()
    {
        Camera.main.transform.position = new Vector3(0, 0, -10);
    }
}