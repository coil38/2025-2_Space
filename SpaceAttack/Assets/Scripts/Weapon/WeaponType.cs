using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponType : MonoBehaviour, IAttack, ICheckAttack
{
    //임시로 플레이어에 할당
    public Animator attackAnimator { get; set; }
    public Vector2 attackDirection { get; protected set; }

    protected Vector2 _currentPos;
    public bool isAttacking { get; protected set; }

    public Vector2 attackMovePos { get; protected set; }
    public bool isAttackMoving { get; protected set; }

    public virtual void CheckAttack(Vector2 currentPos) { }

    public virtual void Attack() { }

    //Start 대신으로 임시 지정
    public virtual void SetInfo() { }
    //Update 대신으로 임시 지정
    public virtual void UpdateInfo() { }

}