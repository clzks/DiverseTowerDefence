using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Reward : MonoBehaviour
{
    public enum ERewardType
    {
        Gem,
        Gold,
        Life,
        Ap,
        Etc,
    }

    public ERewardType type;
    public int value;

}
