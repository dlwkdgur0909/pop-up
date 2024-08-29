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

    [Header("Default Stat")]
    public float HP = 5f;
    public float maxHP = 5f;
    public float damage;
    public float range = 5f;

    private bool isDeath = false;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
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

    private void InitializeMonster()
    {
        HP = maxHP;  // HP�� �ʱ�ȭ
        isDeath = false;  // ��� ���� �ʱ�ȭ
        if (agent != null) agent.enabled = true; // NavMeshAgent Ȱ��ȭ
        if (anim != null)
        {
            anim.ResetTrigger("Death");  // Death Ʈ���� ����
            anim.ResetTrigger("GetDamage");  // GetDamage Ʈ���� ����
        }
    }

    private void Death()
    {
        if (anim == null) return;

        anim.SetTrigger("Death");
        isDeath = true;
        agent.isStopped = true;
    }

    // Death �ִϸ��̼��� ������ ���� ������Ʈ�� ��Ȱ��ȭ�ϴ� �Լ�
    private void DisableGameObject()
    {
        if (agent == null) return;

        agent.enabled = false;
        gameObject.SetActive(false);
    }

    public void TakeDamage(float dmamge)
    {
        HP -= dmamge;
    }

    public void Respawn()
    {
        gameObject.SetActive(true);
        InitializeMonster();
    }

    private void Ray()
    {
        RaycastHit hit;
        if(Physics.Raycast(rayPosition.position, transform.forward, out hit, range))
        {
            float playerHp = hit.transform.GetComponent<Player>().HP;
            anim.SetTrigger("Attack");
            playerHp -= damage;
        }
    }
}
