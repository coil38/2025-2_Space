using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ChipsetComponentDatabase", menuName = "ChipsetComponent/Database")]
public class ChipsetComponentDatabaseSO : ScriptableObject
{
    public List<ChipsetComponentSO> chipsetComponents = new List<ChipsetComponentSO>();

    //캐싱을 위한 사전
    private Dictionary<int, ChipsetComponentSO> chipsetComponentByID;

    public void Initialize()
    {
        chipsetComponentByID = new Dictionary<int, ChipsetComponentSO>();
        foreach (var component in chipsetComponents)
        {
            chipsetComponentByID[component.chipsetCompID] = component;
        }
    }

    public ChipsetComponentSO GetChipsetComponentByID(int id)
    {
        if (chipsetComponentByID == null)
            Initialize();
        if(chipsetComponentByID.TryGetValue(id, out ChipsetComponentSO component))
            return component;

        return null;
    }
}
