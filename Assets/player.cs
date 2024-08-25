using System.Collections;
using UnityEngine;

public class player : MonoBehaviour
{
    [SerializeField] private GameObject bulletPos;
    [SerializeField] private Light flash;
    [SerializeField] private ObjectPool bulletPool; // 오브젝트 풀 추가

    public float moveSpeed;

    [Header("Rotate")]
    public float rotateSpeed;
    float yRotate;
    float xRotate;
    [SerializeField] private Camera cam;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        Move();
        Rotate();
        Shot();
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
        float mouseX = Input.GetAxisRaw("Mouse X") * rotateSpeed * Time.deltaTime;
        float mouseY = -Input.GetAxisRaw("Mouse Y") * rotateSpeed * Time.deltaTime;

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
        flash.intensity = 0.3f;

        var bulletGo = ObjectPool.instance.Pool.Get();

        bulletGo.transform.position = this.bulletPos.transform.position;
        bulletGo.transform.rotation = this.cam.transform.rotation;

        yield return new WaitForSeconds(0.05f);
        flash.intensity = 0;
    }
}
