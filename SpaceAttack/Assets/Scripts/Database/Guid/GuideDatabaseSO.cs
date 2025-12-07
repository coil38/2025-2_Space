using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GuidDatabase", menuName = "Guid/Database")]
public class GuideDatabaseSO : ScriptableObject
{
    public List<GuideSO> guidList = new List<GuideSO>();

    //캐싱을 위한 사전

    public GuideSO[] GetGuids()
    {
        return guidList.ToArray();
    }
}