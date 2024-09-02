using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterSpawner : MonoBehaviour
{
    public GameObject monsterPrefab;
    public float spawnTime = 0;
    public Vector3 spawnRange = new Vector3(10f, 0f, 10f);
    public int poolSize = 10;

    private List<GameObject> monsterPool;
    [SerializeField] private GameObject monsterParent;

    void Start()
    {
        monsterPool = new List<GameObject>();

        for (int i = 0; i < poolSize; i++)
        {
            GameObject monster = Instantiate(monsterPrefab);
            monster.SetActive(false);
            monsterPool.Add(monster);
            monster.transform.SetParent(monsterParent.transform, false);
        }

        InvokeRepeating("SpawnMonster", 0f, spawnTime);
    }

    void SpawnMonster()
    {
        // Ǯ���� ��Ȱ��ȭ�� ���� ã��
        GameObject monsterToSpawn = null;
        foreach (GameObject monster in monsterPool)
        {
            if (!monster.activeInHierarchy)
            {
                monsterToSpawn = monster;
                break;
            }
        }

        if (monsterToSpawn == null)
            return;

        // ���� ��ġ ���
        Vector3 randomPosition = new Vector3(
            Random.Range(-spawnRange.x, spawnRange.x),
            0f,  // y�� 0���� ������ ��
            Random.Range(-spawnRange.z, spawnRange.z)
        );

        // Raycast�� ������ ǥ�� y��ǥ�� ���
        RaycastHit hit;
        if (Physics.Raycast(randomPosition + Vector3.up * 50f, Vector3.down, out hit, 100f))
        {
            randomPosition.y = hit.point.y;
        }
        else
        {
            // ������ ã�� ���� ���, ������ �⺻ y ��ǥ ����
            randomPosition.y = 0f;
        }

        monsterToSpawn.transform.position = randomPosition;

        Monster monsterScript = monsterToSpawn.GetComponent<Monster>();
        if (monsterScript != null)
        {
            monsterScript.Respawn();
        }
        else
        {
            monsterToSpawn.SetActive(true);  // Respawn �޼��尡 ���� ��� �⺻ Ȱ��ȭ
        }
    }
}
