using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseRelic : MonoBehaviour
{
    public abstract int relicId { get; protected set; }

    public abstract void SetEffect(bool isEquip, RelicInfo[] infos);
}
