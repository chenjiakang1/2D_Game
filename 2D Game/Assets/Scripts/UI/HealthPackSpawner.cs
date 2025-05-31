using UnityEngine;

public class HealthPackSpawner : MonoBehaviour
{
    public GameObject healthPackPrefab;  // 🩸 要生成的血包预制体
    public Transform spawnPoint;         //  生成位置
    public float spawnInterval = 30f;    //  刷新时间

    private float timer = 0f;
    private GameObject currentHealthPack;

    void Update()
    {
        timer += Time.deltaTime;

        // 如果计时超过间隔，且当前没有血包，生成一个
        if (timer >= spawnInterval && currentHealthPack == null)
        {
            SpawnHealthPack();
            timer = 0f;
        }
    }

    void SpawnHealthPack()
    {
        if (healthPackPrefab != null && spawnPoint != null)
        {
            currentHealthPack = Instantiate(healthPackPrefab, spawnPoint.position, Quaternion.identity);
        }
    }
}
