using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class REDecoComponent : MonoBehaviour
{
    public abstract void Operater(bool isEquip, RelicInfo info);
    public abstract void SetID();
}
