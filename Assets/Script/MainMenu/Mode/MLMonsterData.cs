using UnityEngine;
using Unity.MLAgents;
using Unity.Barracuda;

[CreateAssetMenu(fileName = "NewMLMonsterData", menuName = "Game/ML Monster Data")]
public class MLMonsterData : ScriptableObject
{
    [Header("Model Settings")]
    public NNModel easyModel;
    public NNModel mediumModel;
    public NNModel hardModel;

    [Header("Base Stats")]
    public float baseHealth = 100f;
    public float baseDamage = 10f;
    public float baseSpeed = 3.5f;

    [Header("Difficulty Multipliers")]
    public float easyHealthMultiplier = 0.8f;
    public float mediumHealthMultiplier = 1.0f;
    public float hardHealthMultiplier = 1.5f;

    public NNModel GetModel(DifficultyLevel difficulty)
    {
        return difficulty switch
        {
            DifficultyLevel.Easy => easyModel,
            DifficultyLevel.Medium => mediumModel,
            DifficultyLevel.Hard => hardModel,
            _ => mediumModel
        };
    }

    public float GetHealth(DifficultyLevel difficulty)
    {
        return difficulty switch
        {
            DifficultyLevel.Easy => baseHealth * easyHealthMultiplier,
            DifficultyLevel.Medium => baseHealth * mediumHealthMultiplier,
            DifficultyLevel.Hard => baseHealth * hardHealthMultiplier,
            _ => baseHealth
        };
    }
}