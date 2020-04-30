using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mineable : MonoBehaviour {
    public Item[] drops;
    public float[] chances;
    
    public void Mined(GameObject miner) {
        for(int i = 0; i < drops.Length; i++) {
            float chance = Random.Range(0.0f, 1.0f);
            if(chance <= chances[i]) {
                //miner.GetComponent<StructureStatsManager>().ChangeItem(drops[i], 1);
            }
        }
    }
}
