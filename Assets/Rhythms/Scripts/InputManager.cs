using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    public delegate void OnKeyEvent();

    private Dictionary<KeyCode, List<OnKeyEvent>> KeyDownEvent = new Dictionary<KeyCode, List<OnKeyEvent>>();
    private Dictionary<KeyCode, List<OnKeyEvent>> KeyUpEvent = new Dictionary<KeyCode, List<OnKeyEvent>>();

    public static InputManager Instance;

    private void Awake()
    {
        if (Instance)
            Destroy(this);

        Instance = this;
    }

    private void Update()
    {
        foreach (KeyValuePair<KeyCode, List<OnKeyEvent>> keyDown in KeyDownEvent)
        {
            if (Input.GetKeyDown(keyDown.Key))
            {
                foreach (OnKeyEvent keyEvent in keyDown.Value)
                {
                    keyEvent?.Invoke();
                }
            }
        }

        foreach (KeyValuePair<KeyCode, List<OnKeyEvent>> keyUp in KeyUpEvent)
        {
            if (Input.GetKeyUp(keyUp.Key))
            {
                foreach (OnKeyEvent keyEvent in keyUp.Value)
                {
                    keyEvent?.Invoke();
                }
            }
        }
    }

    public void ListenToKeyDown(KeyCode key, OnKeyEvent keyEvent)
    {
        if (!KeyDownEvent.ContainsKey(key))
        {
            KeyDownEvent.Add(key, new List<OnKeyEvent>());
        }

        KeyDownEvent[key].Add(keyEvent);
    }

    public void ListenToKeyUp(KeyCode key, OnKeyEvent keyEvent)
    {
        if (!KeyUpEvent.ContainsKey(key))
        {
            KeyUpEvent.Add(key, new List<OnKeyEvent>());
        }

        KeyUpEvent[key].Add(keyEvent);
    }

    public void RemoveKeyDown(KeyCode key, OnKeyEvent keyEvent)
    {
        if (KeyDownEvent.ContainsKey(key))
        {
            KeyDownEvent[key].Remove(keyEvent);
        }
    }

    public void RemoveKeyUp(KeyCode key, OnKeyEvent keyEvent)
    {
        if (KeyUpEvent.ContainsKey(key))
        {
            KeyUpEvent[key].Remove(keyEvent);
        }
    }
}
