﻿using System.Collections.Generic;
using UnityEngine;

public interface DialogUserInput
{
    // Forces dialog to wait by preventing user input. Used for certain cutscenes
    void Freeze(object key);
    void Unfreeze(object key);
}

public class StandardDialogUserInput : MonoBehaviour, DialogUserInput
{
    float fastForwardSpd = 30f;

    // when we want to prevent user input
    Dictionary<object, bool> freezes = new Dictionary<object, bool>();

    void Update()
    {
        // do nothing if user input is frozen
        if (freezes.Count > 0)
        {
            return;
        }
        // handle user input
        if (Input.GetKeyDown(KeyCode.Z))
        {
            Dialog.RunDialog("testcase.protd");
        }
        if ((Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space)) && Dialog.GetInstance().Enabled)
        {
            if (Dialog.GetDisplay().TextFinished())
            {
                Dialog.Advance();
            }
            else
            {
                Dialog.GetDisplay().Advance(10f);
            }
        }
        if (Input.GetKey(KeyCode.LeftControl) && Dialog.GetInstance().Enabled)
        {
            if (Dialog.GetDisplay().TextFinished())
            {
                Dialog.Advance();
            }
            Dialog.GetDisplay().Advance(fastForwardSpd * UITime.deltaTime);
        }
    }

    public void Freeze(object key)
    {
        freezes[key] = true;
    }

    public void Unfreeze(object key)
    {
        freezes.Remove(key);
    }
}