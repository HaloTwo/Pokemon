using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapMove : MonoBehaviour
{
    [SerializeField] GameObject go_obj;
    [SerializeField] GameObject out_obj;

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("이동하자");
            if (other.transform.position != out_obj.transform.position + out_obj.transform.forward * 3f)
            {
                other.transform.position = out_obj.transform.position + out_obj.transform.forward * 3f;
                other.transform.rotation = out_obj.transform.rotation;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("들어옴");
            other.GetComponent<PlayerMovement>().apply_motion_wait(1f);
            other.transform.position = out_obj.transform.position + out_obj.transform.forward * 3f;
            other.transform.rotation = out_obj.transform.rotation;
        }
    }
}
