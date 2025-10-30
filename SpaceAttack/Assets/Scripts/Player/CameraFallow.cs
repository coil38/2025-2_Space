using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFallow : MonoBehaviour
{
    public static CameraFallow instance;

    public float FollowSpeed = 2.0f;

    public Vector3 cameraDir;       //방향 백터(카메라)
    public Vector3 cameraRot;
    public float cameraDis = 10;
    private bool isLocked = false;
    public float shakeDuration = 0;
    public float shakeAmount = 0.1f;
    public float decreaseFactor = 1.0f;

    private Transform target;
    private bool isInitialing;  //초기 중인지 여부 판단

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            StartCoroutine(FindTargetAndInitialize());
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Update()
    {
        if (target == null)
        {
            LogUtil.Log("대상이 존재하지 않다");
            return;
        }

        if (isInitialing)
        {
            LogUtil.Log("카메라 초기설정 중...");
            return;
        }

        if (isLocked || target == null || isInitialing) return;

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
        target = PlayerStatus.Instance.transform;

        transform.rotation = Quaternion.Euler(cameraRot);
        transform.position = target.position + cameraDir.normalized * cameraDis;

        if(target != null) isInitialing = false;  //초기 설정 중 비활성화
    }

    public void LockCamera(bool state)
    {
        isLocked = state;
    }

}
