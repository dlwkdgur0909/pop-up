using System.Collections.Generic;
using UnityEngine;

public class ParticlePool : MonoBehaviour
{
    [SerializeField] private GameObject particlePrefab;
    [SerializeField] private GameObject particleParent;
    [SerializeField] private int poolSize = 10;

    private Queue<GameObject> poolQueue;

    void Start()
    {
        poolQueue = new Queue<GameObject>();

        for (int i = 0; i < poolSize; i++)
        {
            GameObject particle = Instantiate(particlePrefab);
            particle.SetActive(false);
            poolQueue.Enqueue(particle);
            particle.transform.SetParent(particleParent.transform, false);
        }
    }

    public GameObject GetParticle()
    {
        if (poolQueue.Count > 0)
        {
            GameObject particle = poolQueue.Dequeue();
            particle.SetActive(true);
            return particle;
        }
        else
        {
            GameObject particle = Instantiate(particlePrefab);
            return particle;
        }
    }

    public void ReturnParticle(GameObject particle)
    {
        particle.SetActive(false);
        poolQueue.Enqueue(particle);
    }
}
