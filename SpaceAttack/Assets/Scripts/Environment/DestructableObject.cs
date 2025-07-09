using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestructableObject : MonoBehaviour
{
    public float life = 3f;

    private float shakeDuration = 0f;   //흔들림 지속시간
    private float shakeMagnitude = 0.25f;  //흔들림 크기
    private float dampingSpeed  = 1f;    //지속시간 감소 속도

    private Vector3 initialPosition;
    void Start()
    {
        initialPosition = transform.position;   //초기 위치 할당
    }

    void Update()
    {
        if (life <= 0)
        {
            Destroy(gameObject);
        }
        else if (shakeDuration > 0)
        {
            transform.localPosition = initialPosition + Random.insideUnitSphere * shakeMagnitude;

            shakeDuration -= Time.deltaTime * dampingSpeed;
        }
        else
        {
            shakeDuration = 0f;
            transform.localPosition = initialPosition;
        }
    }

    public void ApplyDamage(AttackInfo attackInfo)
    {
        life--;
        shakeDuration = 0.2f;
    }
}
