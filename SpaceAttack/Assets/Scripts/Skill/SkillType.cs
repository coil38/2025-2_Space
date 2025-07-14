using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SkillType : MonoBehaviour, IAttack, ICheckAttack
{

    public abstract void CheckAttack(Vector3 currentPos);

    public abstract void Attack();

    public abstract void SetInfo();

    public abstract void UpdateInfo();
}
