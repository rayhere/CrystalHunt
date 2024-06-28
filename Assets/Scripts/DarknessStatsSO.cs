using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DarknessStats", menuName = "Character Stats/Darkness")]
public class DarknessStatsSO : PlayerStatsSO
{
    // Additional properties specific to the Darkness character
    public int armor;
    public int criticalChance;
    // Other specific stats or methods

    public bool IsDead()
    {
        return currentHP <= 0;
    }
    
}

