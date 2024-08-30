using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Monster : MonoBehaviour
{
    private Transform player;
    private NavMeshAgent agent;
    private Animator anim;
    [SerializeField] private Transform rayPosition;
    [SerializeField] private GameObject damageParticlePrefab;
    [SerializeField] private GameObject hitEffectPosition;

    [Header("Default Stat")]
    public float HP = 5f;
    public float maxHP = 5f;
    public float damage;
    public float range = 5f;

    private bool isDeath = false;
    private ParticlePool particlePool;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        particlePool = FindObjectOfType<ParticlePool>();
    }

    void OnEnable()
    {
        if (GameManager.Instance != null) player = GameManager.Instance.player;

        InitializeMonster();
    }

    private void Update()
    {
        if (!isDeath && player != null)
        {
            agent.SetDestination(player.transform.position);

            if (HP <= 0)
            {
                Death();
            }
        }
    }
    #region TakeDamage and Death
    private void InitializeMonster()
    {
        HP = maxHP;  // HP를 초기화
        isDeath = false;  // 사망 상태 초기화
        if (agent != null) agent.enabled = true; // NavMeshAgent 활성화
        if (anim != null)
        {
            anim.ResetTrigger("Death");  // Death 트리거 리셋
            anim.ResetTrigger("GetDamage");  // GetDamage 트리거 리셋
        }
    }

    private void Death()
    {
        if (anim == null) return;

        anim.SetTrigger("Death");
        isDeath = true;
        agent.isStopped = true;
    }

    // Death 애니메이션이 끝나고 게임 오브젝트를 비활성화하는 함수
    private void DisableGameObject()
    {
        if (agent == null) return;

        agent.enabled = false;
        gameObject.SetActive(false);
    }

    public void TakeDamage(float damage)
    {
        HP -= damage;
        if (isDeath) return;
        StartCoroutine(Particle());
    }

    public IEnumerator Particle()
    {
        GameObject particle = particlePool.GetParticle();
        particle.transform.position = hitEffectPosition.transform.position;
        particle.transform.rotation = Quaternion.identity;
        particle.SetActive(true);
        yield return new WaitForSeconds(0.5f);
        particlePool.ReturnParticle(particle);
    }

    public void Respawn()
    {
        gameObject.SetActive(true);
        InitializeMonster();
    }
    #endregion

    private void Attack()
    {
        RaycastHit hit;
        if(Physics.Raycast(rayPosition.position, transform.forward, out hit, range))
        {
            hit.transform.GetComponent<Player>().TakeDamage(damage);
            anim.SetTrigger("Attack");
        }
    }
}
