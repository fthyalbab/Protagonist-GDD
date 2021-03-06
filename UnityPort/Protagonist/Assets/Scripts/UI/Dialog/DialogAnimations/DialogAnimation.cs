﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * Base class for dialog animations (those are the things where a character image fades in/out)
 * These are automatically created by the dialog system.
 */
public abstract class DialogAnimation : MonoBehaviour
{
    // return true if done with the animation
    public abstract bool Finished();
}

/**
 * Same as above, but a bit less abstract. Timer from [0, 1] is included.
 */
public abstract class TimedDialogAnimation : DialogAnimation
{
    // returns a normalized version of this transition's progress on [0, 1]
    protected virtual float timer {
        get
        {
            // if incrementing up, lerp on [0, duration]
            return count >= 0 ? Mathf.InverseLerp(0, duration, timerSeconds) :
                // if incrementing down, lerp on [0, -duration]
                Mathf.InverseLerp(-duration, 0, timerSeconds);
        }
    }

    // total duration and progress so far
    float duration;
    float timerSeconds = 0;
    // counts up: 1, counts down: -1
    int count = 1;
    // finished or not
    bool finished = false;

    protected void Initialize(float duration, int count = 1)
    {
        this.duration = duration;
        this.count = count;
        timerSeconds = 0;
    }

    protected virtual void Update()
    {
        if (finished)
        {
            return;
        }
        // increment timerSeconds
        timerSeconds += UITime.deltaTime * count;
        // if absolute value is greater than duration, we're done
        if (Mathf.Abs(timerSeconds) > duration)
        {
            finished = true;
            Finish();
        }
        timerSeconds = Mathf.Clamp(timerSeconds, -duration, duration);
    }

    // called when timer is finished.
    protected virtual void Finish()
    {
        
    }

    // checks whether or not this animation is finished.
    public override bool Finished()
    {
        return finished;
    }
}
