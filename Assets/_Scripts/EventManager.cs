using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public static class EventManager
{
    public static Stack<UnityEvent> events = new Stack<UnityEvent>();

    public static event UnityAction CutsceneStarted;
    public static void OnCutsceneStarted() => CutsceneStarted?.Invoke();
}
