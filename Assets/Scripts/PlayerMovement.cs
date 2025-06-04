using System.Collections;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private Rigidbody2D rb2D;

    private Animator playerAnimator;

    private BoxCollider2D boxCollider2D;
    private float offsetX, offsetY, sizeX, sizeY;

    [Header("Movement")]
    public float movementSpeed;
    public float jumpForce;

    private bool facingRight;

    [HideInInspector]
    public bool isGrounded;

    [Header("Ground Check")]
    public Transform groundPosition;
    public float groundRadius;
    public LayerMask groundLayer;

    [Header("Dash")]
    public float dashSpeed;
    public float dashTime, dashCooldown;
    public GameObject dashEffect;
    private bool canDash = true;
    [HideInInspector]
    public bool isDashing;
    

    [HideInInspector]
    public bool isGrabbing;

    private bool canJumpDown;

    [Header("Attack")]
    public Transform attackPoint;
    public float attackRange;
    public LayerMask enemyLayer;

    private bool isAttacking;
    public int attackDamage;
    public float attackRate;
    private float nextAttackTime;

    [HideInInspector]
    public float currentHealth, maxHealth = 100;

    [Header("UI")]
    public GameObject canvas;

    [Header("Audio")]
    public AudioClip hitSound;
    public AudioClip missSound;
    public AudioClip dashSound;
    public AudioClip jumpSound;
    public AudioClip walkSound;
    public AudioClip grabSound;
    public AudioClip damageSound;
    [HideInInspector]
    public AudioSource audioSource;

    private float horizontal;

    private void Start()
    {
        rb2D = gameObject.GetComponent<Rigidbody2D>();
        playerAnimator = gameObject.GetComponent<Animator>();
        boxCollider2D = gameObject.GetComponent<BoxCollider2D>();
        audioSource = gameObject.GetComponent<AudioSource>();
        facingRight = true;

        offsetX = boxCollider2D.offset.x;
        offsetY = boxCollider2D.offset.y;
        sizeX = boxCollider2D.size.x;
        sizeY = boxCollider2D.size.y;

        currentHealth = maxHealth;
    }


    private void Update()
    {
        GroundCheck();

        Jump();


        if (isDashing) // Dash atilirken FixedUpdate geri kalkani calismaz
        {
            return;
        }

        if (isGrabbing) // Tutunurken FixedUpdate geri kalkani calismaz
        {
            return;
        }

        if (isAttacking) // Saldirirken FixedUpdate geri kalkani calismaz
        {
            return;
        }


        horizontal = Input.GetAxis("Horizontal");
        

        if (rb2D.velocity.x < 0 && facingRight)
        {
            FlipFace();
        }
        else if (rb2D.velocity.x > 0 && facingRight == false)
        {
            FlipFace();
        }


        if (Input.GetKeyDown(KeyCode.LeftShift) && canDash && isGrounded)
        {
            StartCoroutine(Dash());
        }


        if ((Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S)) && canJumpDown && isGrounded)
        {
            audioSource.PlayOneShot(jumpSound);
            StartCoroutine(JumpDown());
        }


        if (Input.GetMouseButtonDown(0))
        {
            if (nextAttackTime <= Time.timeSinceLevelLoad && isGrounded && !isAttacking)
            {
                nextAttackTime = Time.timeSinceLevelLoad + attackRate;
                StartCoroutine(Attack());
            }
        }
    }

    private void FixedUpdate()
    {
        if (isDashing || isGrabbing || isAttacking)
        {
            return;
        }

        rb2D.velocity = new Vector2(horizontal * movementSpeed, rb2D.velocity.y);
        playerAnimator.SetFloat("playerSpeed", Mathf.Abs(rb2D.velocity.x));

        if (Mathf.Abs(rb2D.velocity.x) > 0 && isGrounded)
        {
            if (!audioSource.isPlaying)
            {
                audioSource.PlayOneShot(walkSound);
            }
        }
    }


    private void GroundCheck()
    {
        isGrounded = Physics2D.OverlapCircle(groundPosition.position, groundRadius, groundLayer);
        playerAnimator.SetBool("isGrounded", isGrounded);
    }


    private void Jump()
    {
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            rb2D.velocity = new Vector2(rb2D.velocity.x, jumpForce);
            audioSource.PlayOneShot(jumpSound);
        }

        if (Input.GetButtonUp("Jump") && rb2D.velocity.y > 0)
        {
            rb2D.velocity = new Vector2(rb2D.velocity.x, 0f);
        }
    }
    

    private IEnumerator Dash()
    {
        Physics2D.IgnoreLayerCollision(6, 7, true);
        canDash = false;
        isDashing = true;
        playerAnimator.SetTrigger("Dashing");
        rb2D.velocity = new Vector2(transform.localScale.x * dashSpeed, 0f);
        Instantiate(dashEffect, gameObject.transform);
        boxCollider2D.offset = new Vector2(offsetX, 0.4f);
        boxCollider2D.size = new Vector2(sizeX, 0.7f);
        audioSource.PlayOneShot(dashSound, 0.04f);

        yield return new WaitForSeconds(dashTime);

        isDashing = false;
        boxCollider2D.offset = new Vector2(offsetX, offsetY);
        boxCollider2D.size = new Vector2(sizeX, sizeY);
        Physics2D.IgnoreLayerCollision(6, 7, false);

        yield return new WaitForSeconds(dashCooldown);

        canDash = true;
    }


    private IEnumerator Attack()
    {
        isAttacking = true;
        rb2D.velocity = new Vector2(0f, 0f);
        playerAnimator.SetTrigger("Attack");

        yield return new WaitForSeconds(0.15f);

        // Alan icinde bulunan dusmanlarý tespit eder ve bu dusmanlarýn Collider2D bilesenlerini bir dizi olarak doner.
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayer);

        if (hitEnemies.Length > 0)
        {
            audioSource.PlayOneShot(hitSound, 0.2f);
            foreach (Collider2D enemy in hitEnemies)
            {
                enemy.GetComponent<Enemy>().TakeDamage(attackDamage);
            }
        }
        else
        {
            audioSource.PlayOneShot(missSound, 0.2f);
        }

        yield return new WaitForSeconds(0.4f);

        isAttacking = false;
    }


    public void TakeDamage(float damage)
    {
        currentHealth -= damage;

        playerAnimator.SetTrigger("Hurt");
        audioSource.PlayOneShot(damageSound, 0.5f);

        if (currentHealth <= 0)
        {
            Die();
        }
    }


    private void Die()
    {
        playerAnimator.SetBool("isDead", true);
        rb2D.velocity = new Vector2(0f, 0f);
        rb2D.gravityScale = 0f;
        boxCollider2D.enabled = false;
        this.enabled = false;
        canvas.GetComponent<UIManager>().StartCoroutine("ShowLoseScreen");
    }


    private IEnumerator JumpDown()
    {
        boxCollider2D.enabled = false;
        gameObject.GetComponent<LedgeDetection>().enabled = false;

        yield return new WaitForSeconds(0.2f);

        boxCollider2D.enabled = true;

        yield return new WaitForSeconds(0.3f);

        gameObject.GetComponent<LedgeDetection>().enabled = true;
        
    }


    private void FlipFace()
    {
        facingRight = !facingRight;
        gameObject.transform.localScale = new Vector3(gameObject.transform.localScale.x * -1, gameObject.transform.localScale.y, gameObject.transform.localScale.z);
    }


    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Death"))
        {
            audioSource.PlayOneShot(damageSound, 0.5f);
            TakeDamage(100);
        }


    }


    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Dropdown"))
        {
            canJumpDown = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        canJumpDown = false;
    }


    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(groundPosition.position, groundRadius);

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
}