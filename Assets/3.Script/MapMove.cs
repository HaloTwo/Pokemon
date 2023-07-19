using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapMove : MonoBehaviour
{
    [SerializeField]GameObject go_obj;
    [SerializeField] GameObject out_obj;

    private void OnTriggerEnter(Collider other)
    {
        other.transform.position = out_obj.transform.position + out_obj.transform.forward * 3f;
    }
}
