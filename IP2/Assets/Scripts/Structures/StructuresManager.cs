using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StructuresManager : MonoBehaviour
{
    private List<StructureStatsManager> sbs;
    [SerializeField] float tickLength = 0.1f;
    
    void Start()
    {
        sbs = new List<StructureStatsManager>(GameObject.FindObjectsOfType<StructureStatsManager>());
        StartCoroutine(ProcessTick());
    }

    IEnumerator ProcessTick()
    {
        yield return new WaitForSeconds(tickLength);
        StartCoroutine(ProcessTick());
    }

    public List<StructureStatsManager> GetStructures() {
        return sbs;
    }

    public void Destroyed(StructureStatsManager sb) {
        sbs.Remove(sb);
        Destroy(sb.gameObject);
    }
}
