using UnityEngine;
using Unity.MLAgents;
using Unity.Barracuda;

public class MLDifficultyManager : MonoBehaviour
{
    public static MLDifficultyManager Instance { get; private set; }

    [SerializeField] private MLMonsterData[] monsterDatas; // สำหรับมอนสเตอร์แต่ละประเภท
    [SerializeField] private float easyBossHealth = 1000f;
    [SerializeField] private float mediumBossHealth = 2000f;
    [SerializeField] private float hardBossHealth = 3500f;

    public DifficultyLevel currentDifficulty { get; private set; } = DifficultyLevel.Medium;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SetDifficulty(DifficultyLevel difficulty)
    {
        currentDifficulty = difficulty;
        UpdateAllMLAgents();
        Debug.Log($"Difficulty set to: {difficulty}");
    }

    public NNModel GetModelForMonsterType(int monsterType)
    {
        if (monsterType < 1 || monsterType > monsterDatas.Length) return null;
        return monsterDatas[monsterType - 1].GetModel(currentDifficulty);
    }

    public float GetHealthForMonsterType(int monsterType)
    {
        if (monsterType < 1 || monsterType > monsterDatas.Length) return 100f;
        return monsterDatas[monsterType - 1].GetHealth(currentDifficulty);
    }

    public float GetBossHealth()
    {
        return currentDifficulty switch
        {
            DifficultyLevel.Easy => easyBossHealth,
            DifficultyLevel.Medium => mediumBossHealth,
            DifficultyLevel.Hard => hardBossHealth,
            _ => mediumBossHealth
        };
    }

    private void UpdateAllMLAgents()
    {
        MLMonster[] mlMonsters = FindObjectsOfType<MLMonster>();
        foreach (var monster in mlMonsters)
        {
            monster.UpdateDifficultySettings();
        }
    }
}

public enum DifficultyLevel
{
    Easy,
    Medium,
    Hard
}