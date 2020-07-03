using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ticker : MonoBehaviour {
    public float tickLength = 0.05f;
    public GameUIHandler gameUIHandler;
    public StructuresManager structuresManager;
    public List<Projectile> projectiles = new List<Projectile> ();
    public DateTime lastTicked;

    void Awake () {
        gameUIHandler = FindObjectOfType<GameUIHandler> ();
        structuresManager = FindObjectOfType<StructuresManager> ();
    }

    void Start () {
        lastTicked = DateTime.Now;
        StartCoroutine (Tick ());
    }

    IEnumerator Tick () {
        TimeSpan delta = DateTime.Now.Subtract (lastTicked);
        float deltaTime = (float) delta.TotalSeconds;
        gameUIHandler.TickCanvas ();
        structuresManager.TickStructures (deltaTime);
        foreach (Projectile projectile in projectiles) projectile.Tick (deltaTime);
        lastTicked = DateTime.Now;
        yield return new WaitForSeconds (tickLength);
        StartCoroutine (Tick ());
    }
}
