using System.Collections;
using System.IO;
using System.IO.Compression;

using UnityEngine;

public class FileSettings : MonoBehaviour
{
    public PixelAnimator pixAnim;
    public Camera gifCamera;

    private GameManager gameManager = GameManager.Instance;
    private AnimationManager animManager = AnimationManager.Instance;

    public void Save()
    {
        animManager.SaveFile(gameManager.currentAnimation);
    }

    public void SaveAsReverse()
    {
        animManager.SaveFileReversed(gameManager.currentAnimation);
    }

    public void SaveAsImageListZip()
    {
        //Set GIF view to real view
        gifCamera.orthographicSize = Camera.main.orthographicSize;
        gifCamera.transform.position = Camera.main.transform.position;

        pixAnim.AnimToZipStart();

        StartCoroutine(CreateImageList());
    }

    private IEnumerator CreateImageList()
    {
        Animation anim = gameManager.currentAnimation;

        string animDir = gameManager.animationsPath + "/" + anim.animationName;

        if (!Directory.Exists(animDir))
        {
            Directory.CreateDirectory(animDir);
        }

        int multiplier = Mathf.CeilToInt(Camera.main.orthographicSize / 2.0f);

        float xRatio = anim.gridSizeX / anim.gridSizeY;
        float yRatio = anim.gridSizeY / anim.gridSizeX;

        if (xRatio > yRatio) { yRatio = 1; }
        if (yRatio > xRatio) { xRatio = 1; }

        int xSize = (int)(64 * xRatio * multiplier);
        int ySize = (int)(64 * yRatio * multiplier);

        for (int animFrames = 0; animFrames < gameManager.currentAnimation.maxFrameCount; animFrames++)
        {
            pixAnim.frameSelect.value = animFrames;
            yield return new WaitForEndOfFrame();

            RenderTexture screenTexture = new RenderTexture(xSize, ySize, 16);
            gifCamera.targetTexture = screenTexture;
            RenderTexture.active = screenTexture;
            gifCamera.Render();
            Texture2D renderedTexture = new Texture2D(xSize, ySize);
            renderedTexture.ReadPixels(new Rect(0, 0, xSize, ySize), 0, 0);
            RenderTexture.active = null;
            gifCamera.targetTexture = null;
            byte[] byteArray = renderedTexture.EncodeToPNG();
            File.WriteAllBytes(animDir + "/" + anim.animationName + animFrames + ".png", byteArray);
        }

        CreateZipFile(animDir);
        DeleteDirectory(animDir);

        pixAnim.AnimToZipEnd();
    }

    private void CreateZipFile(string _dir)
    {
        if (File.Exists(_dir + "GIF.zip"))
        {
            File.Delete(_dir + "GIF.zip");
        }

        ZipFile.CreateFromDirectory(_dir, _dir + "GIF.zip");
    }

    private void DeleteDirectory(string _dir)
    {
        string[] files = Directory.GetFiles(_dir);

        foreach (string file in files)
        {
            File.Delete(file);
        }

        Directory.Delete(_dir);
    }
}