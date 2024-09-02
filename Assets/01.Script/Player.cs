using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    [SerializeField] private GameObject bulletPos;
    [SerializeField] private ObjectPool bulletPool;
    [SerializeField] private Light flash;

    [Header("Default Stat")]
    public float HP;
    public float maxHP;
    public float stamina = default;
    public float maxStamina = default;
    public float staminaRegenRate = default; //스테미너 회복 속도
    public float sprintStaminaCost = default; //초당 소모되는 스테미너

    [Header("ShotGun")]
    [SerializeField] private int pellets = 10; //산탄총의 탄환 개수
    [SerializeField] private float spreadAngle = 10f; //탄 퍼짐 각도
    [SerializeField] private float range = 50f; //사거리
    [SerializeField] private float damage = 10f;
    [SerializeField] private int curAmmo;
    [SerializeField] private int maxAmmo = 5;
    [SerializeField] private float reloadTime;
    private bool isReload = false;

    [Header("Rotate")]
    public float rotateSpeed;
    float yRotate;
    float xRotate;

    private float moveSpeed = 5f;
    private float sprintSpeedMultiplier = 1.5f; //달릴 시 속도 배율

    #region UI
    [SerializeField] private Camera cam;
    [SerializeField] private Slider staminaSlider;
    [SerializeField] private Image reloadCoolTimeImage;
    [SerializeField] private TMP_Text reloadCoolTimeText;
    [SerializeField] private TMP_Text HPText;
    #endregion

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        reloadCoolTimeImage.fillAmount = 0;
        curAmmo = maxAmmo;
        HP = maxHP;

        stamina = maxStamina;
        staminaSlider.maxValue = maxStamina;
        staminaSlider.value = stamina;
    }

    private void Update()
    {
        Move();
        Shot();
        Reload();
        RegenerateStamina();
        UpdateUI();
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

        bool isSprinting = Input.GetKey(KeyCode.LeftShift) && stamina > 0;
        float curSpeed = isSprinting ? moveSpeed * sprintSpeedMultiplier : moveSpeed;

        if (isSprinting && !isReload)
        {
            stamina -= sprintStaminaCost * Time.deltaTime;
            stamina = Mathf.Clamp(stamina, 0, maxStamina);
        }

        transform.Translate(curSpeed * Time.deltaTime * dir);
    }

    //스테미너 채워주는 함수
    private void RegenerateStamina()
    {
        if (stamina < maxStamina && !Input.GetKey(KeyCode.LeftShift))
        {
            stamina += staminaRegenRate * Time.deltaTime;
            stamina = Mathf.Clamp(stamina, 0, maxStamina);
        }
    }

    private void Rotate()
    {
        float mouseX = Input.GetAxisRaw("Mouse X") * rotateSpeed;
        float mouseY = -Input.GetAxisRaw("Mouse Y") * rotateSpeed;

        yRotate += mouseX;
        xRotate += mouseY;

        //수직 회전 값 제한
        xRotate = Mathf.Clamp(xRotate, -90f, 90f); 

        cam.transform.localRotation = Quaternion.Euler(xRotate, 0, 0);
        transform.rotation = Quaternion.Euler(0, yRotate, 0);
    }

    private void Shot()
    {
        if (Input.GetMouseButtonDown(0))
        {
            //재장전 중이거나 총알이 없으면 반환
            if (isReload || curAmmo <= 0) return;
            StartCoroutine(C_Shot());
        }
    }

    private IEnumerator C_Shot()
    {
        flash.intensity = 1f;

        for (int i = 0; i < pellets; i++)
        {
            Vector3 randomDirection = Quaternion.Euler(
                Random.Range(-spreadAngle, spreadAngle),
                Random.Range(-spreadAngle, spreadAngle),
                0) * cam.transform.forward;

            RaycastHit hit;
            if (Physics.Raycast(cam.transform.position, randomDirection, out hit, range))
            {
                Monster monster = hit.transform.GetComponent<Monster>();
                if (monster != null)
                {
                    monster.TakeDamage(damage);
                }
            }
        }
        curAmmo--;

        yield return new WaitForSeconds(0.1f);
        flash.intensity = 0;
    }

    private void Reload()
    {
        if ((Input.GetKeyDown(KeyCode.R) && curAmmo < maxAmmo) || curAmmo <= 0)
        {
            StartCoroutine(C_Reload());
        }
    }

    private IEnumerator C_Reload()
    {
        if (!isReload)
        {
            isReload = true;
            float reloadingTime = reloadTime;
            while (reloadingTime > 0)
            {
                reloadingTime -= Time.deltaTime;
                reloadCoolTimeImage.fillAmount = 1 - (reloadingTime / reloadTime);
                yield return null;
            }
            curAmmo = maxAmmo;
            isReload = false;
            //쿨타임 이미지 초기화
            reloadCoolTimeImage.fillAmount = 0;
        }
    }

    private void UpdateUI()
    {
        staminaSlider.value = stamina;
        reloadCoolTimeText.text = $"{curAmmo}/{maxAmmo}";
        HPText.text = "HP: " + HP.ToString();
    }

    public void TakeDamage(float damage)
    {
        HP -= damage;
        HP = Mathf.Clamp(HP, 0, maxHP);
    }
}
