using UnityEngine;

[CreateAssetMenu(fileName = "new ChipsetComponent", menuName = "ChipsetComponent/chipsetComponent")]
public class ChipsetComponentSO : ScriptableObject
{
    public string chipsetComponentKey;
    public string name;
    public ChipsetComponentType componentType;
    public Sprite iconSprite;
    public string description;
}
