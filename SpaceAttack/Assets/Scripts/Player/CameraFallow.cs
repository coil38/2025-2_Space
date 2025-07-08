using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFallow : MonoBehaviour
{
    public float FollowSpeed = 2.0f;
    public Transform Target;
    public Transform camTransform;

    public Vector3 cameraDir;       //방향 백터(카메라)
    public Vector3 cameraRot;
    public float cameraDis = 10;

    public float shakeDuration = 0.1f;
    public float shakeAmount = 0.1f;
    public float decreaseFactor = 1.0f;


    void OnEnable()
    {
        camTransform.rotation = Quaternion.Euler(cameraRot);
        camTransform.position = Target.position + cameraDir.normalized * cameraDis;
    }

    void Update()
    {
        Vector3 newPosition = Target.position + cameraDir.normalized * cameraDis;
        transform.position = Vector3.Slerp(transform.position, newPosition, FollowSpeed * Time.deltaTime);
    }
}
