using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackRangeSetter : MonoBehaviour
{
    [SerializeField] private GameObject spriteObject;

    public void SetAttackRange(Vector3 scale)
    {
        if (spriteObject == null)
        {
            Debug.Log("공격범위의 변경될 스프라이트 대상이 할당되지 않음");
            return;
        }

        spriteObject.transform.localScale = scale;
        Vector3 currentPos = spriteObject.transform.position;
        spriteObject.transform.position = new Vector3(currentPos.x, currentPos.y, - scale.z / 2f);
    }
}
