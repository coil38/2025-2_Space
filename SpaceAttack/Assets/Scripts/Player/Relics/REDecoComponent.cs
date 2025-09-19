using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class REDecoComponent : MonoBehaviour
{
    public abstract void Operater(bool isEquip);
    public abstract void SetID();
    public abstract void SetInfo(RelicInfo info);
}
