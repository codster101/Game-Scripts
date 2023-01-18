using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    
    public Animator animator;
    public LayerMask groundLayer;
    public float move;
    public float jumpFloat;
    public float fallMult = 2.5f;
    public KeyCode spaceBar;
    public float fallThreshold;
    public int sceneNum;
    public float dash=1;
    public float maxFall = -20;
    public float landingDelay = 0f;

    private bool isFacingRight = true;
    private bool canDash = false;
    private bool isLasered = false;
    private GameObject currentRespawn;
    private Rigidbody2D rb;
    private BoxCollider2D bc;

    void Awake()
    {
        rb = transform.GetComponent<Rigidbody2D>();
        bc = transform.GetComponent<BoxCollider2D>();
        currentRespawn = GameObject.Find("Checkpoint");


    }

    void Update()
    {
        HandleMovement();
        CheckDeath();
    }

    void FixedUpdate()
    {
        if (canDash)
        {
            Dash();
        }
    } 

    void HandleMovement()
    {
        float x = Input.GetAxisRaw("Horizontal");
        rb.velocity = new Vector2(x * move, rb.velocity.y);
        animator.SetFloat("Speed", Mathf.Abs(rb.velocity.x));
        if (x > 0 && !isFacingRight)
        {
            Flip();
        }else if (x < 0 && isFacingRight)
        {
            Flip();
        }

        //Jump
        if (IsGrounded())
        {
            animator.SetBool("isFall", false);
            animator.SetBool("isJump", false);
            if (Input.GetKeyDown(KeyCode.W))
            {
                rb.velocity = new Vector2(rb.velocity.x, jumpFloat);
                animator.SetBool("isJump", true);
            }
        }
        
        //fall
        if (!IsGrounded() && rb.velocity.y < 0 && rb.velocity.y > maxFall)
        {
            animator.SetBool("isJump", false);
            animator.SetBool("isFall", true);
            rb.velocity += Vector2.up * Physics2D.gravity.y * (fallMult - 1) * Time.deltaTime;
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            canDash = true;
        }
        //Debug.Log(Vector2.up * Physics2D.gravity.y * (fallMult - 1) * Time.deltaTime);
    }
    bool IsGrounded()
    {
        RaycastHit2D raycast = Physics2D.BoxCast(bc.bounds.center, bc.bounds.size, 0f, Vector2.down, .1f, groundLayer); 
        return raycast.collider != null;
        
        //UnityEngine.Debug.Log(raycast.collider != null);
    }

    void Dash()
    {
        rb.AddForce(new Vector2((dash), 0f), ForceMode2D.Impulse);
        canDash = false;
    }

    void Flip()
    {
        isFacingRight = !isFacingRight;
        transform.Rotate(0f, 180f, 0f);
        dash = -dash;
    }

    void CheckDeath()
    {
        if (rb.position.y < -5)
        {
            Death();
        }

        if (isLasered)
        {
            Death();
        }
    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject.tag == "Laser")
        {
            isLasered = true;
        }
    }

    public void Death()
    {
        Debug.Log("dead");
        rb.transform.position = currentRespawn.transform.position;
    }

    public void SetRespawn(GameObject respawn)
    {
        currentRespawn = respawn;
    }
}
