using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GuideCategoryDatabase", menuName = "GuideCategory/Database")]
public class GuideCatecorySO : ScriptableObject
{
    public int gcId;
    public string gcName;

    public int[] subIds;
    public string[] subNames;
}
