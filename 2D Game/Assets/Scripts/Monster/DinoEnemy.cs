using UnityEngine;

public class DinoEnemy : MonoBehaviour
{
    public float moveSpeed = 2f;
    public Transform leftPoint;
    public Transform rightPoint;
    public GameObject deathEffectPrefab; // ���� enemy-death prefab

    private Rigidbody2D rb;
    private SpriteRenderer sr;
    private bool movingRight = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();

        // ȷ�� Patrol �㲻���� Dino �ƶ�
        leftPoint.parent = null;
        rightPoint.parent = null;

        movingRight = false;
    }

    void FixedUpdate()
    {
        Vector2 velocity = rb.velocity;
        velocity.x = movingRight ? moveSpeed : -moveSpeed;
        rb.velocity = new Vector2(velocity.x, rb.velocity.y);

        // ��ֹ��΢ Y ����
        if (Mathf.Abs(rb.velocity.y) < 0.01f)
        {
            rb.velocity = new Vector2(rb.velocity.x, 0f);
        }

        sr.flipX = movingRight;

        // Ѳ�߽߱��ж�
        if (movingRight && transform.position.x >= rightPoint.position.x - 0.05f)
        {
            movingRight = false;
        }
        else if (!movingRight && transform.position.x <= leftPoint.position.x + 0.05f)
        {
            movingRight = true;
        }
    }

    //  ��Ҳ����ң�����ҵ��ã�
    public void OnStomped()
    {
        // ����������Ч
        if (deathEffectPrefab != null)
        {
            Instantiate(deathEffectPrefab, transform.position, Quaternion.identity);
        }

        // ֪ͨ����� DinoSpawner�����Ѿ�����
        DinoSpawner spawner = FindClosestSpawner();
        if (spawner != null)
        {
            spawner.Clear();
        }

        Destroy(gameObject);
    }

    //  ��������� DinoSpawner
    private DinoSpawner FindClosestSpawner()
    {
        DinoSpawner[] spawners = FindObjectsOfType<DinoSpawner>();
        DinoSpawner closest = null;
        float minDist = Mathf.Infinity;

        foreach (var s in spawners)
        {
            float d = Vector2.Distance(transform.position, s.transform.position);
            if (d < minDist)
            {
                minDist = d;
                closest = s;
            }
        }

        return closest;
    }
}
