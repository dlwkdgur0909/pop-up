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
            0f,  // y는 0으로 설정해 둠
            Random.Range(-spawnRange.z, spawnRange.z)
        );

        // Raycast로 지형의 표면 y좌표를 계산
        RaycastHit hit;
        if (Physics.Raycast(randomPosition + Vector3.up * 50f, Vector3.down, out hit, 100f))
        {
            randomPosition.y = hit.point.y;
        }
        else
        {
            // 지형을 찾지 못한 경우, 임의의 기본 y 좌표 설정
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
            monsterToSpawn.SetActive(true);  // Respawn 메서드가 없는 경우 기본 활성화
        }
    }
}
