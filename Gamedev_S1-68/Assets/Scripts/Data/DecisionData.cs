using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DecisionData : MonoBehaviour
{
    [SubHeader("Decision changes")]
    [SerializeField] internal int Current_money_ = 1000;
    [SerializeField] internal int Current_debt_ = 0;
    [SerializeField] internal int Current_moral_ = 50;
}
