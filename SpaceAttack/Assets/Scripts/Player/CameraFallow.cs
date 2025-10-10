using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFallow : MonoBehaviour
{
    public float FollowSpeed = 2.0f;
    public Transform Target
    {
        get { return target; }
        set 
        {
            target = value;
            //Debug.Log($"대상이 활당됨!!, 대상이름: {value.gameObject.name}");
        }
    }
    private Transform target;

    public Vector3 cameraDir;       //방향 백터(카메라)
    public Vector3 cameraRot;
    public float cameraDis = 10;

    public float shakeDuration = 0;
    public float shakeAmount = 0.1f;
    public float decreaseFactor = 1.0f;

    private bool isInitialing;  //초기 중인지 여부 판단

    void OnEnable()
    {
        StartCoroutine(FindTargetAndInitialize());
    }

    void Update()
    {
        if (target == null)
        {
            //LogUtil.Log("대상이 존재하지 않다");
            return;
        }
        else
        {
            //LogUtil.Log("대상이 존재한다");
        }

        if (isInitialing)
        {
            LogUtil.Log("카메라 초기설정 중...");
            return;
        }

        Vector3 newPosition = target.position + cameraDir.normalized * cameraDis;
        transform.position = Vector3.Slerp(transform.position, newPosition, FollowSpeed * Time.deltaTime);

        if (shakeDuration > 0)
        {
            transform.position = transform.position + Random.insideUnitSphere * shakeAmount;
            shakeDuration -= Time.deltaTime * decreaseFactor;
        }
    }

    public void CameraShack()
    {
        shakeDuration = 0.2f;
    }

    private IEnumerator FindTargetAndInitialize()
    {
        isInitialing = true;  //초기 설정 중 활성화

        yield return new WaitUntil(() => PlayerStatus.Instance != null);
        Target = PlayerStatus.Instance.transform;

        transform.rotation = Quaternion.Euler(cameraRot);
        transform.position = target.position + cameraDir.normalized * cameraDis;

        isInitialing = false;  //초기 설정 중 비활성화
    }
}
