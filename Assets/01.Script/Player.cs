using System.Collections;
using UnityEditor;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class Player : MonoBehaviour
{
    [SerializeField] private GameObject bulletPos;
    [SerializeField] private ObjectPool bulletPool; // 오브젝트 풀 추가
    [SerializeField] private Light flash;

    [Header("Default Stat")]
    public float HP;

    [Header("ShotGun")]
    [SerializeField] private int pellets = 10;  // 산탄총의 탄환 개수
    [SerializeField] private float spreadAngle = 10f;  // 탄 퍼짐 각도
    [SerializeField] private float range = 50f;  // 사거리
    [SerializeField] private float damage = 10f; // 데미지

    [Header("Rotate")]
    public float rotateSpeed;
    float yRotate;
    float xRotate;

    public float moveSpeed;
    [SerializeField] private Camera cam;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        Move();
        Shot();
    }

    private void LateUpdate()
    {
        Rotate();
    }

    private void Move()
    {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        Vector3 dir = new Vector3(h, 0, v).normalized;

        transform.Translate(moveSpeed * Time.deltaTime * dir);
    }

    private void Rotate()
    {
        float mouseX = Input.GetAxisRaw("Mouse X") * rotateSpeed;
        float mouseY = -Input.GetAxisRaw("Mouse Y") * rotateSpeed;

        yRotate += mouseX;
        xRotate += mouseY;

        xRotate = Mathf.Clamp(xRotate, -90f, 90f); //수직 회전 값을 -90도에서 90도 사이로 제한

        cam.transform.rotation = Quaternion.Euler(xRotate, yRotate, 0);
        transform.rotation = Quaternion.Euler(0, yRotate, 0);
    }


    private void Shot()
    {
        if (Input.GetMouseButtonDown(0))
        {
            StartCoroutine(C_Shot());
        }
    }

    private IEnumerator C_Shot()
    {
        flash.intensity = 1f;

        for (int i = 0; i < pellets; i++)
        {
            Vector3 shootDirection = cam.transform.forward + new Vector3(
                Random.Range(-spreadAngle, spreadAngle),
                Random.Range(-spreadAngle, spreadAngle),
                0);

            RaycastHit hit;
            if (Physics.Raycast(cam.transform.position, shootDirection, out hit, range))
            {
                Monster monster = hit.transform.GetComponent<Monster>();
                if (monster != null)
                {
                    monster.TakeDamage(damage);
                    Debug.Log("TakeDamage");
                }

                // 맞은 위치에 이펙트 생성 (예: 파티클)
                // Instantiate(impactEffect, hit.point, Quaternion.LookRotation(hit.normal));
            }
        }

        yield return new WaitForSeconds(0.3f);
        flash.intensity = 0;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        for (int i = 0; i < pellets; i++)
        {
            // 탄환의 발사 방향을 설정 (퍼짐 효과 포함)
            Vector3 shootDirection = cam.transform.forward + new Vector3(
                Random.Range(-spreadAngle, spreadAngle),
                Random.Range(-spreadAngle, spreadAngle),
                0);

            // Gizmo로 레이캐스트 경로를 시각적으로 표시
            Gizmos.DrawRay(cam.transform.position, shootDirection * range);
        }
    }
}
