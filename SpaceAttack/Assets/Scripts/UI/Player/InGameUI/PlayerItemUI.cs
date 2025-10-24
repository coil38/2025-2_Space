using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

public class PlayerItemUI : MonoBehaviour
{
    private List<PlayerInventorySlot> slots;
    private List<int> relicInstanceIds = new List<int>();
    private int currentIndex;

    public GameObject slotPrf;       //슬롯 프리팹
    public int minSlotCount = 7;     //슬롯 최소 개수

    private void OnEnable()
    {
        if (slots == null)
        {
            PlayerInventorySlot[] temps = GetComponentsInChildren<PlayerInventorySlot>();
            slots = new List<PlayerInventorySlot>(temps);
        }
    }
    public void AddItem(RelicSO relicSO, int relicInstanceId)  //아이템 장착
    {
        relicInstanceIds.Add(relicInstanceId);

        if (currentIndex >= minSlotCount)
        {
            PlayerInventorySlot temp = Instantiate(slotPrf, this.gameObject.transform).GetComponent<PlayerInventorySlot>();
            temp.AddItemImage(relicSO, relicInstanceId);
            slots.Add(temp);
        }
        else
        {
            //LogUtil.Log($"아이템_{currentIndex} 획득");
            slots[currentIndex].AddItemImage(relicSO, relicInstanceId);
        }

        currentIndex++;
    }
    
    public void RemoveItem(RelicSO relicSO, int relicInstanceId)              //아이템 장착 해제
    {
        Sprite relicIcon = relicSO.iconSprite;
        int targetIndex = 0;

        targetIndex = relicInstanceIds.IndexOf(relicInstanceId);
        slots[targetIndex].RemoveItem();
        relicInstanceIds.RemoveAt(targetIndex);

        for (int i = targetIndex; i < currentIndex - 1; i++)
        {
            RelicSO relic = slots[i + 1].relic;
            slots[i].AddItemImage(relic, slots[i + 1].relicInstanceId);
            slots[i + 1].RemoveItem();
        }

        if(currentIndex >= minSlotCount)
        {
            Destroy(slots[currentIndex].gameObject);
            slots.RemoveAt(currentIndex);
        }
        currentIndex--;
    }

    public void RemoveAllItems()
    {
        Initialize();
        foreach (var slot in slots)
        {
            slot.RemoveItem();
        }
    }

    private void Initialize()
    {
        currentIndex = 0;
    }
}
