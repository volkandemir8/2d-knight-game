using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float maxHealth = 100;
    public float speed;

    [HideInInspector]
    public float currentHealth;

    private Animator enemyAnimator;
    private Rigidbody2D rb2D;

    public Transform attackPoint;
    public float attackRange;
    public LayerMask playerLayer;

    private bool isAttacking;
    public int attackDamage;
    public float attackRate;
    private float nextAttackTime;

    private GameObject player;
    private float distanceToPlayer;

    private bool facingRight = false;

    [Header("Audio")]
    public AudioClip hitSound;
    public AudioClip walkSound;
    public AudioClip damageSound;
    private AudioSource audioSource;

    private void Start()
    {
        currentHealth = maxHealth;
        enemyAnimator = gameObject.GetComponent<Animator>();
        rb2D = gameObject.GetComponent<Rigidbody2D>();
        player = GameObject.FindGameObjectWithTag("Player");
        audioSource = gameObject.GetComponent<AudioSource>();
    }

    private void Update()
    {
        distanceToPlayer = player.transform.position.x - gameObject.transform.position.x;

        if (Mathf.Abs(distanceToPlayer) <= 3f)
        {
            if (nextAttackTime <= Time.timeSinceLevelLoad && !isAttacking)
            {
                if ((distanceToPlayer > 0 && !facingRight) || (distanceToPlayer < 0 && facingRight))
                {
                    FlipFace();
                }

                nextAttackTime = Time.timeSinceLevelLoad + attackRate;
                StartCoroutine(Attack());
            }
        }
    }

    private void FixedUpdate()
    {
        if (isAttacking)
        {
            return;
        }

        if (Mathf.Abs(distanceToPlayer) >= 2.5f && Mathf.Abs(distanceToPlayer) < 10f)
        {
            MoveTowardsPlayer();
        }

        if (Mathf.Abs(rb2D.velocity.x) > 0)
        {
            if (!audioSource.isPlaying)
            {
                audioSource.PlayOneShot(walkSound, 0.2f);
            }
        }

        enemyAnimator.SetFloat("Speed", Mathf.Abs(rb2D.velocity.x));
    }

    private void MoveTowardsPlayer()
    {
        float moveDirection;

        if (distanceToPlayer > 0)
            moveDirection = 1;
        else
            moveDirection = -1;

        rb2D.velocity = new Vector2(moveDirection * speed, rb2D.velocity.y);

        if ((distanceToPlayer > 0 && !facingRight) || (distanceToPlayer < 0 && facingRight))
        {
            FlipFace();
        }
    }

    private IEnumerator Attack()
    {
        isAttacking = true;
        rb2D.velocity = new Vector2(0f, 0f);
        enemyAnimator.SetTrigger("Attack");

        yield return new WaitForSeconds(0.15f);

        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, playerLayer);

        if(hitEnemies.Length > 0)
        {
            foreach (Collider2D player in hitEnemies)
            {
                if (player.GetComponent<PlayerMovement>().isDashing == false)
                {
                    player.GetComponent<PlayerMovement>().TakeDamage(attackDamage);
                }
            }
        }
        audioSource.PlayOneShot(hitSound, 0.2f);

        yield return new WaitForSeconds(0.4f);

        isAttacking = false;
    }
    
    public void TakeDamage(float damage)
    {
        currentHealth -= damage;

        enemyAnimator.SetTrigger("Hurt");
        audioSource.PlayOneShot(damageSound, 0.2f);

        if (currentHealth <= 0)
        {
            StartCoroutine(Die());
        }
    }

    public IEnumerator Die()
    {
        rb2D.velocity = new Vector2(0f, 0f);
        enemyAnimator.SetBool("isDead", true);
        this.enabled = false;
        gameObject.GetComponent<BoxCollider2D>().enabled = false;
        rb2D.gravityScale = 0f;

        yield return new WaitForSeconds(2f);

        gameObject.SetActive(false);
    }

    private void FlipFace()
    {
        facingRight = !facingRight;
        gameObject.transform.localScale = new Vector3(gameObject.transform.localScale.x * -1, gameObject.transform.localScale.y, gameObject.transform.localScale.z);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
}