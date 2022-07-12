using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraPos : MonoBehaviour
{
    [SerializeField] private Transform marioHead;
    [SerializeField] private float smoothDamp;
    private Vector3 speed;

    void Update()
    {
        transform.position = Vector3.SmoothDamp(transform.position, marioHead.position, ref speed, smoothDamp);
    }
}
