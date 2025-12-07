using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GuidDatabase", menuName = "Guid/Database")]
public class GuideDatabaseSO : ScriptableObject
{
    public List<GuideSO> guideList = new List<GuideSO>();
    public List<GuideCatecorySO> gcList = new List<GuideCatecorySO>();

    //캐싱을 위한 사전
    private Dictionary<int, List<GuideSO>> guideBySubId;

    void Initialize()
    {
        guideBySubId = new Dictionary<int, List<GuideSO>> ();

        foreach (var guide in guideList)
        {
            if (guideBySubId.ContainsKey(guide.subId))
            {
                guideBySubId[guide.subId].Add(guide);
            }
            else
            {
                guideBySubId[guide.subId] = new List<GuideSO>();
                guideBySubId[guide.subId].Add(guide);
            }
        }
    }

    public GuideSO[] GetGuids()
    {
        return guideList.ToArray();
    }

    public GuideCatecorySO[] GetGuideCats()
    {
        return gcList.ToArray();
    }

    public GuideSO[] GetGuidesBySubId(int subId)
    {
        if (guideBySubId == null)
            Initialize();

        return guideBySubId[subId].ToArray();
    }
}