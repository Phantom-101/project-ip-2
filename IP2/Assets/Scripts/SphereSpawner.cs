using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SphereSpawner : MonoBehaviour
{
    public GameObject sphere;

    void Update()
    {
        Instantiate(sphere, transform.position, Quaternion.identity);
    }
}
