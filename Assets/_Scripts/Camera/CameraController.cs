using System.Collections.Generic;

using TMPro;

using UnityEngine;
using UnityEngine.EventSystems;

public class CameraController : MonoBehaviour
{
    private CameraSettings camSettings;

    private TMP_InputField[] allInputfields;

    private List<RaycastResult> rayResults = new List<RaycastResult>();

    private bool isOnUI = false;
    private bool focussed = false;

    private void Awake()
    {
        allInputfields = FindObjectsOfType<TMP_InputField>();

        camSettings = GameObject.Find("CameraMenu").GetComponent<CameraSettings>();
    }

    private void Update()
    {
        foreach (TMP_InputField IF in allInputfields)
        {
            if (IF.isFocused)
            {
                focussed = true;
                break;
            }
        }

        focussed = false;

        EventSystem.current.RaycastAll(new PointerEventData(EventSystem.current)
        { position = Input.mousePosition, pointerId = -1 }, rayResults);

        if (rayResults.Count > 0)
        {
            isOnUI = true;
        }
        else
        {
            isOnUI = false;
        }

        if (!focussed)
        {
            if (!isOnUI)
            {
                if (Input.GetAxisRaw("Mouse ScrollWheel") != 0f)
                {
                    Camera.main.orthographicSize = Mathf.Clamp(Camera.main.orthographicSize + (Input.GetAxisRaw("Mouse ScrollWheel") * -2), 1, camSettings.maxCameraZoom);
                    CameraSettings.OnCameraZoomed();
                }
            }

            if (Input.GetKey(KeyCode.W) && Camera.main.transform.position.y < 20)
            {
                Camera.main.transform.position += (Vector3.up / (camSettings.maxCameraZoom / Camera.main.orthographicSize)) * camSettings.cameraSpeed;
            }
            else if (Input.GetKey(KeyCode.S) && Camera.main.transform.position.y > -20)
            {
                Camera.main.transform.position += (Vector3.down / (camSettings.maxCameraZoom / Camera.main.orthographicSize)) * camSettings.cameraSpeed;
            }

            if (Input.GetKey(KeyCode.A) && Camera.main.transform.position.x > -20)
            {
                Camera.main.transform.position += (Vector3.left / (camSettings.maxCameraZoom / Camera.main.orthographicSize)) * camSettings.cameraSpeed;
            }
            else if (Input.GetKey(KeyCode.D) && Camera.main.transform.position.x < 20)
            {
                Camera.main.transform.position += (Vector3.right / (camSettings.maxCameraZoom / Camera.main.orthographicSize)) * camSettings.cameraSpeed;
            }
        }
    }
}