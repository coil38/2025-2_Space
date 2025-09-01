using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "new Chipset", menuName = "Chipset/chipset")]
public class ChipsetSO : ScriptableObject
{
    public string chipsetKey;
    public string name;
    public string[] chipsetComponentKeys;
    public Sprite iconSprite;
    public string description;
}
