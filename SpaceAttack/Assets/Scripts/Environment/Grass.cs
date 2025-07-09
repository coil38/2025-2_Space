using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Grass : MonoBehaviour
{
    public ParticleSystem particle;

    private Animator animator;
    void Start()
    {
        animator = GetComponent<Animator>();
    }
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.transform.position.x > transform.position.x)   //플레이어가 오른쪽에 있을때
        {
            animator.Play("MovingGrassR");
        }
        else   //플레이어가 왼쪽에 있을때
        {
            animator.Play("MovingGrassL");
        }
    }

    public void ApplyDamage(AttackInfo info)
    {
        Instantiate(particle, transform.position + Vector3.up * 0.2f, Quaternion.identity);

        Destroy(gameObject);
    }
}
