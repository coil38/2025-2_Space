using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum RelicType
{
    NormalRelic,
    PurifiedRelic,
    SourceRelic
}

public class BaseRelic : MonoBehaviour
{
    public int relicId {  get; private set; }
    public string relicName { get; private set; }
    public Sprite relicIcon { get; private set; }

    public void Initialize(int relicID, string relicName, Sprite relicIcon)
    {
        this.relicId = relicID;
        this.relicName = relicName;
        if(relicIcon != null) this.relicIcon = relicIcon;

        gameObject.name = relicName;

        SpriteRenderer renderer = GetComponent<SpriteRenderer>();
        if (renderer == null )
            renderer = gameObject.AddComponent<SpriteRenderer>();
        renderer.sprite = relicIcon;

        Collider collider = GetComponent<Collider>();
        if (collider == null)
            collider = gameObject.AddComponent<BoxCollider>();
        collider.isTrigger = true;

        RelicAnimationController relicAniCon = GetComponent<RelicAnimationController>();
        if (relicAniCon == null)
            relicAniCon = gameObject.AddComponent<RelicAnimationController>();

    }
}
