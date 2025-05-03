using UnityEngine;

public class DinoSpawner : MonoBehaviour
{
    public GameObject dinoPrefab;     // �� Inspector ���� Dino �� prefab
    private GameObject currentDino;   // ��ǰʵ���������� Dino

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
