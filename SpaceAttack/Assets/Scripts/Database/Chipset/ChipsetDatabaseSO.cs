using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ChipsetDatabase", menuName = "Chipset/Database")]
public class ChipsetDatabaseSO : ScriptableObject
{
    public List<ChipsetSO> chipsets = new List<ChipsetSO>();

    public ChipsetComponentDatabaseSO chipsetComponentDatabase;

    private Dictionary<string, ChipsetSO> chipsetByKey;

    public void Initialize()
    {
        chipsetByKey = new Dictionary<string, ChipsetSO>();
        foreach (var chipset in chipsets)
        {
            chipsetByKey[chipset.chipsetKey] = chipset;
        }
    }

    public ChipsetSO GetChipset(string chipsetKey)
    {
        if (chipsetByKey == null)
            Initialize();
        if (chipsetByKey.TryGetValue(chipsetKey, out ChipsetSO chipset))
            return chipset;

        return null;
    }

    public ChipsetComponentSO GetChipsetComponent(string chipsetKey, ChipsetComponentType type)  //특정 칩셋 컴포넌트 찾기
    {
        if (chipsetByKey == null)
            Initialize();
        if (chipsetByKey.TryGetValue(chipsetKey, out ChipsetSO chipset))
        {
            foreach (var key in chipset.chipsetComponentKeys)
            {
                ChipsetComponentSO temp = chipsetComponentDatabase.GetChipsetComponent(key);
                if (temp.componentType == type)
                {
                    return temp;
                }
            }
        }

        return null;
    }
}
