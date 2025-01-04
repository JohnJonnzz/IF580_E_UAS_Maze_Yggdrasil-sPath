using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ScRunner : MonoBehaviour
{
    [Header("Health Settings")]
    public int maxHP = 100;
    private int currentHP;
    public Slider healthSlider;
    public GameObject gameOverUI;

    [Header("Movement Settings")]
    public float moveSpeed = 6f;
    public float sprintSpeed = 15f;
    public float jumpForce = 200f;
    public float jumpCooldown = 1f;

    [Header("Camera Settings")]
    public Transform cameraTransform;
    [Header("Coin Settings")]
    public Text coinText;
    private int coinCount = 0;
    private Animator anim;
    private bool isJumping = false;
    private float lastJumpTime = 0f;
    private bool isDead = false;
    public AudioClip CoinSound;
    private AudioSource mAudioSource;

    [Header("Fade Settings")]
    public CanvasGroup fadeCanvas; // Referensi untuk CanvasGroup fade
    public float fadeDuration = 1f; // Waktu untuk fade in

    private bool isFading = false; // Mencegah input saat transisi

    // Start is called before the first frame update
    void Start()
    {
        mAudioSource = GetComponent<AudioSource>();

        anim = this.GetComponent<Animator>();
        currentHP = maxHP; // Set HP awal
        healthSlider.maxValue = maxHP;
        healthSlider.value = currentHP;

        if (gameOverUI != null)
        {
            gameOverUI.SetActive(false);
        }

        if (fadeCanvas != null)
        {
            fadeCanvas.alpha = 0; // Pastikan fade dimulai dengan transparan
        }
    }

    void Update()
    {
        if (isDead || isFading) return;

        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        Vector3 direction = new Vector3(h, 0, v).normalized;

        if (direction.magnitude >= 0.1f)
        {
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cameraTransform.eulerAngles.y;
            float smoothAngle = Mathf.LerpAngle(transform.eulerAngles.y, targetAngle, Time.deltaTime * 10);
            transform.rotation = Quaternion.Euler(0, smoothAngle, 0);

            Vector3 moveDirection = Quaternion.Euler(0, targetAngle, 0) * Vector3.forward;
            float currentSpeed = moveSpeed;

            if (Input.GetKey(KeyCode.LeftShift))
            {
                anim.SetBool("isSprint", true);
                currentSpeed = sprintSpeed;
            }
            else
            {
                anim.SetBool("isSprint", false);
            }

            transform.Translate(moveDirection * currentSpeed * Time.deltaTime, Space.World);
            anim.SetBool("isRun", true);
            anim.SetFloat("speed", currentSpeed / sprintSpeed);
        }
        else
        {
            anim.SetBool("isRun", false);
        }

        // Lompat
        if (Input.GetKeyDown(KeyCode.Space) && !isJumping && Time.time - lastJumpTime >= jumpCooldown)
        {
            Jump();
        }
    }

    public void TakeDamage(int damage)
    {
        currentHP -= damage;
        healthSlider.value = currentHP;

        if (currentHP <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Debug.Log("Runner has died!");
        isDead = true;
        anim.SetTrigger("Die");

        if (gameOverUI != null)
        {
            StartCoroutine(HandleGameOver());
        }
    }

    private IEnumerator HandleGameOver()
    {
        yield return StartCoroutine(FadeToBlack());
        SceneManager.LoadScene("GameOver"); // Ganti ke scene GameOver
    }

    private IEnumerator FadeToBlack()
    {
        isFading = true;
        float timer = 0;

        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            if (fadeCanvas != null)
            {
                fadeCanvas.alpha = timer / fadeDuration;
            }
            yield return null;
        }

        if (fadeCanvas != null) fadeCanvas.alpha = 1;
    }

    void Jump()
    {
        anim.SetBool("isJump", true);
        isJumping = true;
        GetComponent<Rigidbody>().AddForce(Vector3.up * jumpForce);
        lastJumpTime = Time.time;
        Invoke("SelesaiJump", 1f);
    }

    void SelesaiJump()
    {
        anim.SetBool("isJump", false);
        isJumping = false;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag.Equals("Coin"))
        {
            if (mAudioSource != null && CoinSound != null)
            {
                Debug.Log("Memainkan suara koin.");
                mAudioSource.PlayOneShot(CoinSound);
            }

            // Tambahkan koin
            coinCount++;
            UpdateCoinUI();

            Destroy(other.gameObject);
        }
    }

    // Fungsi untuk mengupdate UI jumlah koin
    void UpdateCoinUI()
    {
        if (coinText != null)
        {
            coinText.text = "Score: " + coinCount;
        }
    }
}
