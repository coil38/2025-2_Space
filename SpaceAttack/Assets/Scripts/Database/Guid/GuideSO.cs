using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "new Guid", menuName = "Guid/guid")]
public class GuideSO : ScriptableObject
{
    public int mainId;
    public string mainTitle;

    public int subId;
    public string subTitle;

    public int pageId;
    public string pageTitle;
    public Sprite pageSprite;
    public string description;
}
