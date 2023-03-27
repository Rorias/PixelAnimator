using TMPro;

using UnityEngine;

public class DebugHelper : MonoBehaviour
{
    private static TMP_Text debugText;

    private static float disappearTime = 5.0f;
    private static bool visible = false;

    private void Awake()
    {
        debugText = GetComponent<TMP_Text>();
    }

    private void FixedUpdate()
    {
        if (visible)
        {
            disappearTime -= Time.fixedDeltaTime;

            if (disappearTime <= 0)
            {
                debugText.text = "";
                visible = false;
            }
        }
    }

    public static void Log(string _text)
    {
        debugText.text = _text;
        disappearTime = 5;
        visible = true;
    }
}