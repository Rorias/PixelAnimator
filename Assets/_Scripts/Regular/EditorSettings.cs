using UnityEngine;

public static class EditorSettings
{
    public static float lastPlaybackspeed { get; private set; }

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
}