using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponType : MonoBehaviour, IAttack, ICheckAttack
{
    //임시로 플레이어에 할당
    public Animator attackAnimator { get; set; }

    public virtual void CheckAttack(Vector2 currentPos)
    {

    }

    public virtual void Attack(GameObject target)
    {

    }
}