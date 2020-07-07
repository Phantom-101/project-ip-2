using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ticker : MonoBehaviour {
    public float tickLength = 0.05f;
    public GameUIHandler gameUIHandler;
    public StructuresManager structuresManager;
    public List<Projectile> projectiles = new List<Projectile> ();
    public float lastTicked = 0;
    public float curTime = 0;
    public float deltaTime = 0;

    void Awake () {
        gameUIHandler = FindObjectOfType<GameUIHandler> ();
        structuresManager = FindObjectOfType<StructuresManager> ();
    }

    void Start () {
        StartCoroutine (Tick ());
    }

    IEnumerator Tick () {
        curTime = Time.time;
        deltaTime = curTime - lastTicked;
        gameUIHandler.TickCanvas ();
        structuresManager.TickStructures (deltaTime);
        foreach (Projectile projectile in projectiles) projectile.Process (deltaTime);
        lastTicked = curTime;
        yield return new WaitForSeconds (tickLength);
        StartCoroutine (Tick ());
    }
}
