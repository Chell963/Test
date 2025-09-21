using UnityEngine;

[CreateAssetMenu(fileName = "ClickerData", menuName = "ScriptableObjects/ClickerData")]
public class ClickerData : ScriptableObject
{
    [field: SerializeField] public int MaxEnergy { get; private set; } = 1000;
    [field: SerializeField] public int EnergyCost { get; private set; } = 1;
    
    [field: SerializeField] public int ValueAward { get; private set; } = 1;
    [field: SerializeField] public int ValueCooldown { get; private set; } = 3;
    
    [field: SerializeField] public int EnergyRegen { get; private set; } = 10;
    [field: SerializeField] public int EnergyCooldown { get; private set; } = 10;
}
