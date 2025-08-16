using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AniType
{
    Trrigger,
    Bool,
    Int,
    Float
}

public struct PlayerAniInfo
{
    public AniType type;
    public string name;
    public float speed;

    public PlayerAniInfo(string _name, AniType _type, float _speed)
    {
        type = _type;
        name = _name;
        speed = _speed;
    }
}
