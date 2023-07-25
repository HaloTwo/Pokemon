using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapMove : MonoBehaviour
{
    [SerializeField] GameObject go_obj;
    [SerializeField] GameObject out_obj;
    [Header("Go ������Ʈ���� ������ ��")]
    [SerializeField] string Areaname;

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("�̵�����");
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
            if (name.Contains("go"))
            {
                SoundManager.instance.PlayBGM(Areaname);
            }
            else
            {
                SoundManager.instance.PlayBGM("City");
            }
            other.GetComponent<PlayerMovement>().apply_motion_wait(1f);
            other.transform.position = out_obj.transform.position + out_obj.transform.forward * 3f;
            other.transform.rotation = out_obj.transform.rotation;
        }
    }
}
