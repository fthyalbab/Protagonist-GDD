﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/**
 * Simple base class that contains some basic functionality for most letter-throwing puzzles.
 */
public abstract class StandardPuzzle : PuzzleBase, SpellInteractionTarget
{
    // whether we've finished the transition or not
    protected bool active = false;

    string scene;
    public override void Initialize(string scene)
    {
        this.scene = scene;
    }

    protected virtual void Start()
    {
        // should create transition to the target scene in child
        DontDestroyOnLoad(gameObject);
        SceneTransitions.Transition("Spin", new TransitionTime(1f, 0.3f, 1f), scene);
        SceneManager.sceneLoaded += StartScene;
    }

    void StartScene(Scene scene, LoadSceneMode mode)
    {
        // activate destroy on load again
        SceneManager.MoveGameObjectToScene(gameObject, SceneManager.GetActiveScene());
        // finished transitioning
        active = true;
        SceneManager.sceneLoaded -= StartScene;
        // create player targeting this puzzle
        Puzzles.CreatePlayer(this);
        SceneStart();
    }
    protected abstract void SceneStart();

    public virtual void PlacedLetter(string letter) { }
    public virtual void SpellEnd(string spell) { }
    public virtual void SpellHit(string spell, Vector2 pos) { }
    public virtual void SpellStart(string spell) { }
}
