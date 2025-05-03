using UnityEngine;

public class DinoSpawner : MonoBehaviour
{
    public GameObject dinoPrefab;     // 从 Inspector 拖入 Dino 的 prefab
    private GameObject currentDino;   // 当前实例化出来的 Dino

    void Start()
    {
        Spawn();
    }

    public void Spawn()
    {
        if (currentDino == null)
        {
            currentDino = Instantiate(dinoPrefab, transform.position, Quaternion.identity);
        }
    }

    public void Clear()
    {
        currentDino = null;
    }
}
