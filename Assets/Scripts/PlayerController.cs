using UnityEngine;
using TMPro;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public float accelerationRate = 0.2f;
    public float maxSpeed = 20f;
    public float jumpForce = 12f;

    public AudioSource runningSound;
    public AudioSource jumpingSound;
    public AudioSource landingSound;

    private int coinValue = 0;
    public TMP_Text coinText;
    public TMP_Text scoreText;

   public GameObject deathUI;
public GameObject PauseButton;
    public TMP_Text finalScoreText;
    public TMP_Text finalCoinText;

    private float positionx;

    [Header("Ground Check")]
    public Transform groundCheck;
    public LayerMask groundLayer;
    private bool isGrounded;
    private bool wasGrounded;

    private Rigidbody2D rb;
    private Animator anim;
    private SpriteRenderer sr;
public static bool isPaused = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>();

        if (runningSound == null || jumpingSound == null || landingSound == null)
        {
            Debug.LogError("Please assign all audio sources in the inspector.");
        }
    }

    void Update()
    {
if(isPaused)
{
StopSound(runningSound);
anim.SetFloat("Speed", 0);
return;

}

     
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, 0.1f, groundLayer);

        // Jumping
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            PlaySound(jumpingSound);
        }

        // Animation + Score
        anim.SetFloat("Speed", Mathf.Abs(rb.velocity.x));
        anim.SetBool("isGrounded", isGrounded);
        anim.SetFloat("YVelocity", rb.velocity.y);
        addScore();

        // Running sound
        if (Mathf.Abs(rb.velocity.x) > 0.1f && isGrounded)
        {
            if (!runningSound.isPlaying)
                PlaySound(runningSound);
        }
        else
        {
            StopSound(runningSound);
        }

        // Landing sound
        if (isGrounded && !wasGrounded)
        {
            PlaySound(landingSound);
        }

        wasGrounded = isGrounded;
    }

    void FixedUpdate()
    {
        // Gradually increase running speed up to maxSpeed
        if (moveSpeed < maxSpeed)
        {
            moveSpeed += accelerationRate * Time.fixedDeltaTime;
        }

        // Auto-run to the right
        rb.velocity = new Vector2(moveSpeed, rb.velocity.y);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
       

        if (other.CompareTag("Obstacle"))
        {
            anim.SetTrigger("Hit");
if(runningSound.isPlaying)
{
runningSound.Stop();

}
rb.velocity = Vector2.zero;
rb.isKinematic = true;
ShowDeathUI();


        }
    }

    void addScore()
    {
        positionx = transform.position.x;
        float xyz = positionx * 5;
        scoreText.text = xyz.ToString("0");

    }

public void AddCoin()
{
coinValue++;
coinText.text = coinValue.ToString();


}
    void PlaySound(AudioSource sound)
    {
        if (!sound.isPlaying)
            sound.Play();
    }

    public void StopSound(AudioSource sound)
    {
        if (sound.isPlaying)
            sound.Stop();
    }


void ShowDeathUI()
    {
        Time.timeScale = 0f;
        isPaused = true;

        if (deathUI != null)
        {
            deathUI.SetActive(true);
PauseButton.SetActive(false);

            finalScoreText.text = scoreText.text;
            finalCoinText.text = coinText.text;

        }
    }

    
}