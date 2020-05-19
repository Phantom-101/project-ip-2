using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Essentials;

public struct StringPair {
    string a;
    string b;

    public StringPair (string a, string b) {
        if (a.CompareTo (b) < 0.0f) {
            this.a = a;
            this.b = b;
        } else {
            this.b = a;
            this.a = b;
        }
    }
}

public class DiplomacyManager : MonoBehaviour {
    public Dictionary<StringPair, float> relations = new Dictionary<StringPair, float> ();

    public float GetRelations (string factionA, string factionB) {
        StringPair involved = new StringPair (factionA, factionB);
        if (!relations.ContainsKey (involved)) relations.Add(involved, 0.0f);
        return relations[involved];
    }

    public void SetRelations (string factionA, string factionB, float value) {
        StringPair involved = new StringPair (factionA, factionB);
        if (!relations.ContainsKey (involved)) relations.Add(involved, value);
        else relations[involved] = value;
    }

    public void RelationsChanged (string factionA, string factionB, float change) {
        if (factionA == factionB) return;
        StringPair involved = new StringPair (factionA, factionB);
        if (!relations.ContainsKey (involved)) relations.Add(involved, 0.0f);
        relations[involved] += change;
    }
}
