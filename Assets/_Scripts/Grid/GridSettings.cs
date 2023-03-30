using System;

using TMPro;

using UnityEngine;

public class GridSettings : MonoBehaviour
{
    private GameManager gameManager;

    public GameObject grid;
    public Transform gridTransform;
    public Material gridMaterial;

    private TMP_InputField gridLODIF;
    private TMP_InputField gridXIF;
    private TMP_InputField gridYIF;

    private float gridDetail = 2.0f;

    private void Awake()
    {
        gridLODIF = GameObject.Find("GridLOD").GetComponent<TMP_InputField>();
        gridXIF = GameObject.Find("GridX").GetComponent<TMP_InputField>();
        gridYIF = GameObject.Find("GridY").GetComponent<TMP_InputField>();
    }

    private void Start()
    {
        gameManager = GameManager.Instance;

        SetGrid();
        SetGridIFValues();
    }

    private void SetGridIFValues()
    {
        gridXIF.text = gameManager.currentAnimation.gridSizeX.ToString();
        gridYIF.text = gameManager.currentAnimation.gridSizeY.ToString();
        gridLODIF.text = gameManager.ParseToString(gridDetail);
    }

    public void SetGrid()
    {
        gridTransform.localScale = new Vector2(gameManager.currentAnimation.gridSizeX / 16f, gameManager.currentAnimation.gridSizeY / 16f);
        gridMaterial.mainTextureScale = new Vector2(gameManager.currentAnimation.gridSizeX / gridDetail, gameManager.currentAnimation.gridSizeY / gridDetail);
    }

    public void SetGridXsize()
    {
        int gridSizeX = Convert.ToInt32(gridXIF.text);
        gridSizeX = Mathf.Min(Mathf.Max(gridSizeX, 1), 4095);

        gameManager.currentAnimation.gridSizeX = gridSizeX;
        gridXIF.text = gameManager.currentAnimation.gridSizeX.ToString();
        SetGrid();
        Debug.Log("Succesfully changed grid x size to " + gameManager.currentAnimation.gridSizeX.ToString() + ".");
    }

    public void SetGridYsize()
    {
        int gridSizeY = Convert.ToInt32(gridYIF.text);
        gridSizeY = Mathf.Min(Mathf.Max(gridSizeY, 1), 4095);

        gameManager.currentAnimation.gridSizeY = gridSizeY;
        gridYIF.text = gameManager.currentAnimation.gridSizeY.ToString();
        SetGrid();
        Debug.Log("Succesfully changed grid y size to " + gameManager.currentAnimation.gridSizeY.ToString() + ".");
    }

    public void GridLevelOfDetail()
    {
        float gridLOD = gameManager.ParseToSingle(gridLODIF.text);
        gridLOD = Mathf.Min(Mathf.Max(gridLOD, 1), 4);

        gridDetail = gridLOD;
        gridLODIF.text = gameManager.ParseToString(gridDetail);
        SetGrid();
        Debug.Log("Succesfully changed grid LOD to " + gridDetail.ToString() + ".");
    }

    public void GridState()
    {
        grid.SetActive(!grid.activeSelf);
    }
}