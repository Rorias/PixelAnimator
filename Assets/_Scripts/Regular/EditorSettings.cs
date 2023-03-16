using UnityEngine;

public static class EditorSettings
{
    public const float maxCameraZoom = 15f;
    public static float cameraSpeed { get; private set; } = 0.5f;
    public static float lastPlaybackspeed { get; private set; }

    public static bool isNew = false;

    public static bool SetLastPlayspeed(float value)
    {
        if (value < 0.001f)
        {
            Debug.Log("Playback speed cannot be lower than 0.001. Auto-set to 0.001.");
            value = 0.001f;
        }

        lastPlaybackspeed = value;
        return true;
    }

    public static float SetCameraSpeed(float value)
    {
        cameraSpeed = Mathf.Min(Mathf.Max(value, 0.1f), 3);
        return cameraSpeed;
    }
}