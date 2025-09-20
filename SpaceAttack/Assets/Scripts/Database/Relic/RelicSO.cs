using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "new Relic", menuName = "Relic/relic")]
public class RelicSO : ScriptableObject
{
    public int relicID;
    public string relicName;
    public int darkMaterialCount;
    public int[] relicEffects;
    public string relicDivision;
    public string description;
    public RelicInfo[] relicInfos;
    public Sprite iconSprite;
}
