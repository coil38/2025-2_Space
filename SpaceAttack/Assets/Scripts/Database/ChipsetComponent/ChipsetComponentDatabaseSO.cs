using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ChipsetComponentDatabase", menuName = "ChipsetComponent/Database")]
public class ChipsetComponentDatabaseSO : ScriptableObject
{
    public List<ChipsetComponentSO> chipsetComponents = new List<ChipsetComponentSO>();

    //캐싱을 위한 사전
    private Dictionary<string, ChipsetComponentSO> chipsetComponentByKey;

    public void Initialize()
    {
        chipsetComponentByKey = new Dictionary<string, ChipsetComponentSO>();
        foreach (var component in chipsetComponents)
        {
            chipsetComponentByKey[component.chipsetComponentKey] = component;
        }
    }

    public ChipsetComponentSO GetChipsetComponent(string chipsetComponentKey)
    {
        if (chipsetComponentByKey == null)
            Initialize();
        if(chipsetComponentByKey.TryGetValue(chipsetComponentKey, out ChipsetComponentSO component))
            return component;

        return null;
    }
}
