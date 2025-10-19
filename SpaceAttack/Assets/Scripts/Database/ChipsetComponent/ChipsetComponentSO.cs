using UnityEngine;

[CreateAssetMenu(fileName = "new ChipsetComponent", menuName = "ChipsetComponent/chipsetComponent")]
public class ChipsetComponentSO : ScriptableObject
{
    public int chipsetCompID;
    public string chipsetCpname;
    public ChipsetComponentType componentType;
    public Sprite iconSprite;
    public string description;

    public float[] damageRate;
    public float[] coolTime;
    public float[] addedCritRate;
    public float[] addedCritChanceRate;
    public float[] attackTime;
    public float[] attackRange;
}
