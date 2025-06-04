using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LedgeDetection : MonoBehaviour
{
    private Rigidbody2D rb2D;
    private float defaultGravity;

    private PlayerMovement playerMovement; // isGrounded ve isGrabbing erismek icin
    private Animator playerAnimator;

    private bool greenBox, redBox;

    [Header("Ground Check")]
    public LayerMask groundMask;

    [Header("Green Box")]
    public float greenX;
    public float greenY, greenWidth, greenHeight;

    [Header("Red Box")]
    public float redX;
    public float redY, redWidth, redHeight;

    private void Start()
    {
        rb2D = gameObject.GetComponent<Rigidbody2D>();
        playerMovement = gameObject.GetComponent<PlayerMovement>();
        playerAnimator = gameObject.GetComponent<Animator>();
        defaultGravity = rb2D.gravityScale;
    }

    private void Update()
    {
        greenBox = Physics2D.OverlapBox(new Vector2(transform.position.x + (greenX * transform.localScale.x), transform.position.y + greenY), 
            new Vector2(greenWidth, greenHeight), 0f, groundMask);
        redBox = Physics2D.OverlapBox(new Vector2(transform.position.x + (redX * transform.localScale.x), transform.position.y + redY),
            new Vector2(redWidth, redHeight), 0f, groundMask);


        if(greenBox && !redBox && !playerMovement.isGrabbing && !playerMovement.isGrounded)
        {
            playerMovement.isGrabbing = true;
        }

        if (playerMovement.isGrabbing)
        {
            Grab();
        }
    }

    private void Grab()
    {
        rb2D.velocity = new Vector2(0f, 0f);
        rb2D.gravityScale = 0f;
        playerAnimator.SetBool("isGrabbing", true);

        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
        {
            transform.position = new Vector2(transform.position.x + (0.75f * transform.localScale.x), transform.position.y + 2.25f);
            playerMovement.isGrabbing = false;
            rb2D.gravityScale = defaultGravity;
            playerAnimator.SetBool("isGrabbing", false);

            gameObject.GetComponent<PlayerMovement>().audioSource.PlayOneShot(gameObject.GetComponent<PlayerMovement>().grabSound);
        }

        if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
        {
            gameObject.GetComponent<PlayerMovement>().audioSource.PlayOneShot(gameObject.GetComponent<PlayerMovement>().grabSound);
            playerMovement.isGrabbing = false;
            rb2D.gravityScale = defaultGravity;
            playerAnimator.SetBool("isGrabbing", false);

            StartCoroutine(Fall());
        }
    }

    private IEnumerator Fall()
    {
        greenX -= 0.35f;
        redX -= 0.35f;
        greenY -= 1f;
        redY -= 1f;

        yield return new WaitForSeconds(0.5f);

        greenX += 0.35f;
        redX += 0.35f;
        greenY += 1f;
        redY += 1f;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(new Vector2(transform.position.x + (greenX * transform.localScale.x), transform.position.y + greenY), new Vector2(greenWidth, greenHeight));
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(new Vector2(transform.position.x + (redX * transform.localScale.x), transform.position.y + redY), new Vector2(redWidth, redHeight));
    }
}
