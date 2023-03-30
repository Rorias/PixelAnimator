using System;

using UnityEngine;

public class PartHandle : MonoBehaviour
{
    private PixelAnimator pixelAnim;
    private Camera mainCam;

    private float xDifference;
    private float yDifference;

    private void Awake()
    {
        pixelAnim = GameObject.Find("SceneManager").GetComponent<PixelAnimator>();
        mainCam = Camera.main;
    }

    private void OnMouseDown()
    {
        pixelAnim.partSelect.value = Convert.ToInt32(gameObject.name.Substring(10));
        pixelAnim.SetSelectedPart();

        xDifference = mainCam.ScreenToWorldPoint(Input.mousePosition).x - transform.position.x;
        yDifference = mainCam.ScreenToWorldPoint(Input.mousePosition).y - transform.position.y;
    }

    private void OnMouseDrag()
    {
        transform.position = new Vector3(Mathf.Round((mainCam.ScreenToWorldPoint(Input.mousePosition).x - xDifference) * 32.0f) / 32.0f, Mathf.Round((mainCam.ScreenToWorldPoint(Input.mousePosition).y - yDifference) * 32.0f) / 32.0f, 0);
        pixelAnim.UpdatePos();
    }
}