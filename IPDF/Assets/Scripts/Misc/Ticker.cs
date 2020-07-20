﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ticker : MonoBehaviour {
    public float tickLength = 0.05f;
    public GameUIHandler gameUIHandler;
    public StructuresManager structuresManager;
    public List<Projectile> projectiles = new List<Projectile> ();
    public MusicManager musicManager;
    public float lastClampedTicked = 0;
    public float clampedCurTime = 0;
    public float clampedDeltaTime = 0;
    public float lastFastestTicked = 0;
    public float fastestCurTime = 0;
    public float fastestDeltaTime = 0;

    void Awake () {
        gameUIHandler = FindObjectOfType<GameUIHandler> ();
        structuresManager = FindObjectOfType<StructuresManager> ();
        musicManager = FindObjectOfType<MusicManager> ();
    }

    void Start () {
        StartCoroutine (ClampedTick ());
        StartCoroutine (FastestTick ());
    }

    IEnumerator ClampedTick () {
        clampedCurTime = Time.time;
        clampedDeltaTime = clampedCurTime - lastClampedTicked;
        structuresManager.TickStructures (clampedDeltaTime);
        musicManager.Tick (clampedDeltaTime);
        gameUIHandler.ClampedTickCanvas ();
        lastClampedTicked = clampedCurTime;
        yield return new WaitForSeconds (tickLength);
        StartCoroutine (ClampedTick ());
    }

    IEnumerator FastestTick () {
        fastestCurTime = Time.time;
        fastestDeltaTime = fastestCurTime - lastFastestTicked;
        foreach (Projectile projectile in projectiles) projectile.Process (fastestDeltaTime);
        gameUIHandler.FastestTickCanvas ();
        lastFastestTicked = fastestCurTime;
        yield return null;
        StartCoroutine (FastestTick ());
    }
}
