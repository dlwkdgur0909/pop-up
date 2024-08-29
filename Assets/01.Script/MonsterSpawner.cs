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
        // 풀에서 비활성화된 몬스터 찾기
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

        // 랜덤 위치 계산
        Vector3 randomPosition = new Vector3(
            Random.Range(-spawnRange.x, spawnRange.x),
            Random.Range(-spawnRange.y, spawnRange.y),
            Random.Range(-spawnRange.z, spawnRange.z)
        );

        monsterToSpawn.transform.position = randomPosition;

        Monster monsterScript = monsterToSpawn.GetComponent<Monster>();
        if (monsterScript != null)
        {
            monsterScript.Respawn();
        }
        else
        {
            monsterToSpawn.SetActive(true);  // Respawn 메서드가 없는 경우 기본 활성화
        }
    }
}
