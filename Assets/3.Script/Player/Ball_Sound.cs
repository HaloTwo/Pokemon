using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball_Sound : MonoBehaviour
{

    private void OnDisable()
    {
        TryGetComponent(out Rigidbody rb);
        rb.freezeRotation = false;
        rb.constraints = RigidbodyConstraints.FreezeRotationY;
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Object"))
        {
            SoundManager.instance.PlayEffect("BallGround");
        }
    }
}
