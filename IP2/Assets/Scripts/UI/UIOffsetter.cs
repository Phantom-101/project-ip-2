using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIOffsetter : MonoBehaviour
{
    public Vector3 offset;
    public GameObject target;

    void Update()
    {
        Vector3 targetPos = target.GetComponent<RectTransform>().position;
        GetComponent<RectTransform>().position = new Vector3(targetPos.x + offset.x, targetPos.y + offset.y, targetPos.z + offset.z);
    }
}
