using System.Collections;
using UnityEngine;
using UnityEngine.Pool;

public class Bullet : MonoBehaviour
{
    public IObjectPool<GameObject> Pool { get; set; }
    public float speed = 5f;
    public float lifeTime;

    private void OnEnable()
    {
        StartCoroutine(Return(lifeTime));
    }

    void Update()
    {
        transform.Translate(Vector3.forward * speed * Time.deltaTime);
    }

    private IEnumerator Return(float lifeTime)
    {
        yield return new WaitForSeconds(lifeTime);
        // 오브젝트 풀에 반환
        Pool.Release(gameObject);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.CompareTag("Monster"))
        {
            // 오브젝트 풀에 반환
            Pool.Release(gameObject);
        }
    }
}
