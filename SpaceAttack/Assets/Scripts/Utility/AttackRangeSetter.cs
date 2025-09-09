using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum RangeType
{
    FloorSquare,
    FloorCircle,
    Square
}

public class AttackRangeSetter : MonoBehaviour
{
    [SerializeField] private GameObject spriteObject;

    public void SetAttackRange(Vector3 scale, RangeType rangeType)
    {
        if (spriteObject == null)
        {
            Debug.Log("공격범위의 변경될 스프라이트 대상이 할당되지 않음");
            return;
        }

        switch(rangeType)
        {
            case RangeType.FloorSquare:
                spriteObject.transform.localScale = scale;
                break;

            case RangeType.Square:
                spriteObject.transform.localScale = scale;
                break;
        }
    }
}
