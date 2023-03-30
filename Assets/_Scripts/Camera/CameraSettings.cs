using System;

using TMPro;

using UnityEngine;
using UnityEngine.Events;

public class CameraSettings : MonoBehaviour
{
    [NonSerialized] public float maxCameraZoom = 16f;
    [NonSerialized] public float cameraSpeed = 0.2f;

    private TMP_InputField cameraZoomIF;
    private TMP_InputField cameraSpeedIF;

    public static event UnityAction CameraZoomed;
    public static void OnCameraZoomed() => CameraZoomed?.Invoke();

    private GameManager gameManager;

    private void Awake()
    {
        cameraZoomIF = GameObject.Find("CameraZoom").GetComponent<TMP_InputField>();
        cameraSpeedIF = GameObject.Find("CameraSpeed").GetComponent<TMP_InputField>();

        CameraZoomed += () => SetCameraIFValues();
    }

    private void Start()
    {
        gameManager = GameManager.Instance;
        SetCameraIFValues();
    }

    private void SetCameraIFValues()
    {
        cameraZoomIF.text = gameManager.ParseToString(Camera.main.orthographicSize);
        cameraSpeedIF.text = gameManager.ParseToString(cameraSpeed);
    }

    public void SetCameraZoom()
    {
        float zoomLevel = gameManager.ParseToSingle(cameraZoomIF.text);
        zoomLevel = Mathf.Min(Mathf.Max(zoomLevel, 1), maxCameraZoom);

        Camera.main.orthographicSize = zoomLevel;
        cameraZoomIF.text = gameManager.ParseToString(Camera.main.orthographicSize);
        Debug.Log("Succesfully changed camera zoom to " + Camera.main.orthographicSize.ToString() + ".");
    }

    public void SetCameraSpeed()
    {
        float camSpeed = gameManager.ParseToSingle(cameraSpeedIF.text);
        camSpeed = Mathf.Min(Mathf.Max(camSpeed, 0.025f), 5);

        cameraSpeed = camSpeed;
        cameraSpeedIF.text = gameManager.ParseToString(cameraSpeed);
        Debug.Log("Succesfully changed camera speed to " + cameraSpeed.ToString() + ".");
    }

    public void ResetCameraZoom()
    {
        Camera.main.orthographicSize = 4;
        cameraZoomIF.text = "4";
    }

    public void ResetCameraSpeed()
    {
        cameraSpeed = 0.2f;
        cameraSpeedIF.text = gameManager.ParseToString(cameraSpeed);
    }

    public void ResetCameraPosition()
    {
        Camera.main.transform.position = new Vector3(0, 0, -10);
    }
}