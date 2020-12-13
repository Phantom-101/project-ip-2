using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class NavigationManager : MonoBehaviour {
    public static NavigationManager current;

    public List<Sector> sectors = new List<Sector> ();
    public Dictionary<Sector, List<SectorLink>> adjacentSectors = new Dictionary<Sector, List<SectorLink>> ();

    void Awake () {
        current = this;
    }

    public static NavigationManager GetInstance () {
        return current;
    }

    public Route GetRoute (Vector3 from, Vector3 to) {
        sectors = FindObjectsOfType<Sector> ().ToList ();
        Sector fromSector = sectors[0], toSector = sectors[0];
        foreach (Sector sector in sectors) {
            float fromDis = (from - sector.transform.position).magnitude, toDis = (to - sector.transform.position).magnitude;
            if (fromDis < (from - fromSector.transform.position).magnitude) fromSector = sector;
            if (toDis < (to - toSector.transform.position).magnitude) toSector = sector;
        }
        List<Sector> jumps = GetJumps (fromSector, toSector, new List<Sector> (), new List<Sector> ());
        if (jumps == null) return null;
        List<Vector3> waypoints = new List<Vector3> ();
        for (int i = 1; i < jumps.Count; i++)
            waypoints.Add (GetAdjacentPosition (jumps[i - 1], jumps[i]));
        waypoints.Add (to);
        return new Route (waypoints);
    }

    public List<Sector> GetJumps (Sector current, Sector to, List<Sector> visited, List<Sector> chain) {
        if (visited.Contains (current)) return null;
        visited.Add (current);
        chain.Add (current);
        if (current == to) return chain;
        List<List<Sector>> results = new List<List<Sector>> ();
        foreach (Sector adjacent in GetAdjacentSectors (current))
            results.Add (GetJumps (adjacent, to, visited, chain));
        List<Sector> jumps = null;
        int shortest = int.MaxValue;
        foreach (List<Sector> result in results)
            if (result != null && result.Count < shortest) {
                jumps = result;
                shortest = result.Count;
            }
        return jumps;
    }

    public List<Sector> GetAdjacentSectors (Sector sector) {
        if (!adjacentSectors.ContainsKey (sector)) adjacentSectors.Add (sector, new List<SectorLink> ());
        List<SectorLink> links = adjacentSectors[sector];
        List<Sector> adjacents = new List<Sector> ();
        foreach (SectorLink link in links) adjacents.Add (link.destination);
        return adjacents;
    }

    public Vector3 GetAdjacentPosition (Sector from, Sector to) {
        if (!adjacentSectors.ContainsKey (from)) adjacentSectors.Add (from, new List<SectorLink> ());
        List<SectorLink> links = adjacentSectors[from];
        foreach (SectorLink link in links)
            if (link.destination == to)
                return link.position;
        return Vector3.zero;
    }

    public void AddAdjacency (Sector from, Sector to, Vector3 position) {
        if (!adjacentSectors.ContainsKey (from)) adjacentSectors.Add (from, new List<SectorLink> ());
        adjacentSectors[from].Add (new SectorLink (to, position));
    }
}

[Serializable]
public struct SectorLink {
    public Sector destination;
    public Vector3 position;

    public SectorLink (Sector destination, Vector3 position) {
        this.destination = destination;
        this.position = position;
    }
}

[Serializable]
public class Route {
    public List<Vector3> waypoints = new List<Vector3> ();

    public Route (List<Vector3> waypoints) {
        this.waypoints = waypoints;
    }

    public void ReachedWaypoint () {
        waypoints.RemoveAt (0);
    }
}