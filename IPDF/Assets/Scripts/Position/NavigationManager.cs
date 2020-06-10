using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class NavigationManager : MonoBehaviour {
    public List<Sector> sectors = new List<Sector> ();
    public List<SectorLink> sectorLinks = new List<SectorLink> ();

    public Route GetRoute (Vector3 from, Vector3 to) {
        sectors = FindObjectsOfType<Sector> ().ToList ();
        Sector fromSector = sectors[0], toSector = sectors[0];
        foreach (Sector sector in sectors) {
            float fromDis = (from - sector.transform.position).sqrMagnitude, toDis = (to - sector.transform.position).sqrMagnitude;
            if (fromDis < (from - fromSector.transform.position).sqrMagnitude) fromSector = sector;
            if (toDis < (to - toSector.transform.position).sqrMagnitude) toSector = sector;
        }
        List<SectorLink> jumps = GetJumps (fromSector, toSector, new List<Sector> (), new List<SectorLink> ());
        Sector current = fromSector;
        List<Vector3> waypoints = new List<Vector3> ();
        foreach (SectorLink link in jumps) {
            if (link.a == current) waypoints.Add (link.bPos);
            else waypoints.Add (link.aPos);
        }
        waypoints.Add (to);
        return new Route (waypoints);
    }

    public List<SectorLink> GetJumps (Sector current, Sector to, List<Sector> visited, List<SectorLink> chain) {
        visited.Add (current);
        if (current == to) return chain;
        List<List<SectorLink>> results = new List<List<SectorLink>> ();
        foreach (SectorLink sectorLink in sectorLinks) {
            if (sectorLink.a == current) {
                if (!visited.Contains (sectorLink.b)) {
                    chain.Add (sectorLink);
                    results.Add (GetJumps (sectorLink.b, to, visited, chain));
                    chain.Remove (sectorLink);
                }
            } else if (sectorLink.b == current) {
                if (!visited.Contains (sectorLink.a)) {
                    chain.Add (sectorLink);
                    results.Add (GetJumps (sectorLink.a, to, visited, chain));
                    chain.Remove (sectorLink);
                }
            }
        }
        int index = -1;
        int leastLength = int.MaxValue;
        for (int i = 0; i < results.Count; i++)
            if (results != null && results[i].Count < leastLength) {
                index = i;
                leastLength = results[i].Count;
            }
        if (index != -1) return results[index];
        return null;
    }
}

[Serializable]
public struct SectorLink {
    public Sector a;
    public Vector3 aPos;
    public Sector b;
    public Vector3 bPos;

    public SectorLink (Sector a, Vector3 aPos, Sector b, Vector3 bPos) {
        this.a = a;
        this.aPos = aPos;
        this.b = b;
        this.bPos = bPos;
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