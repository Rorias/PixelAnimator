using System;

using UnityEngine;

public class PartHandle : MonoBehaviour
{
    private PixelAnimator pixelAnim;
    private Camera MC;

    private float xDifference;
    private float yDifference;

    private void Awake()
    {
        pixelAnim = GameObject.Find("SceneManager").GetComponent<PixelAnimator>();
        MC = Camera.main;
    }

    private void OnMouseDown()
    {
        pixelAnim.partSelect.value = Convert.ToInt32(gameObject.name.Substring(10));
        pixelAnim.SetSelectedPart();

        xDifference = MC.ScreenToWorldPoint(Input.mousePosition).x - transform.position.x;
        yDifference = MC.ScreenToWorldPoint(Input.mousePosition).y - transform.position.y;
    }

    private void OnMouseDrag()
    {
        transform.position = new Vector3(Mathf.Round((MC.ScreenToWorldPoint(Input.mousePosition).x - xDifference) * 32.0f) / 32.0f, Mathf.Round((MC.ScreenToWorldPoint(Input.mousePosition).y - yDifference) * 32.0f) / 32.0f, 0);
        pixelAnim.UpdatePos();
    }
}