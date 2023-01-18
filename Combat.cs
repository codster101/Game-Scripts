using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Combat : MonoBehaviour
{
    public GameObject self;

    [Header("Attack")]
    public Transform attackOrigin;
    public float attackRange;
    public LayerMask enemyLayers;
    public float attackDamage;
    public GameObject target;
    public bool isAuto;
    public float autoCooldown;
    private float currentTime;

    [Header("Knockback")]
    public float knockback;
    public float knockbackTime;
    private Rigidbody2D rb;

    [Header("Health")]
    public HealthBar healthBar;
    public float maxHealth;
    [SerializeField]
    private float currentHealth;

    [Header("Animation")]
    public Animator animator;

    private void Awake()
    {
        rb = this.GetComponent<Rigidbody2D>();
    }

    void Start()
    {
        currentHealth = maxHealth;
        healthBar.SetMaxHealth(maxHealth);
    }

    // Update is called once per frame
    void Update()
    {
        if (currentHealth <= 0)
        {
            Death();
        }


        //if auto-attack and cooldown timer is 0 and the target is inside attack range then attack
        if (isAuto && (currentTime <= 0) && (Vector3.Distance(target.transform.position, this.transform.position) <= attackRange))
        {
            Attack();
        }
        else
        {
            //update cooldown timer
            currentTime -= 1 * Time.deltaTime;
        }
    }

    public void Attack()
    {
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackOrigin.position, attackRange, enemyLayers);
        //Debug.Log("hit");

        foreach (Collider2D collider in hitEnemies)
        {
            //Debug.Log(collider.tag);
            if (collider.CompareTag("Fish"))
            {
                //Debug.Log("Spawn");
                collider.GetComponent<Fish>().CreateEnemy();
            }
            else
            {
                collider.GetComponent<Combat>().TakeDamage(attackDamage, this.transform.position);
            }
        }

        if (isAuto)
        {
            currentTime = autoCooldown;
        }
        /*Vector2 rayDir = attackOrigin.position - this.transform.position;
        RaycastHit2D enemy = Physics2D.Raycast(this.transform.position, rayDir, range, enemyLayers);
        if(enemy.collider != null)
        {
            enemy.collider.GetComponent<Enemy>().TakeDamage(attackDamage, this.transform.position);
        }*/
    }

    public void TakeDamage(float damage, Vector2 attackerPos)
    {
        currentHealth -= damage;
        healthBar.Sethealth(currentHealth);
        StartCoroutine(Knockback(attackerPos));
        StartCoroutine(GetComponent<Flash>().FlashEffect());
        /*rb.isKinematic = false;
        Vector2 forceDir = (this.transform.position - new Vector3(attackerPos.x, attackerPos.y, 0)).normalized;
        rb.AddForce(forceDir*knockback);
        rb.isKinematic = true;*/
        //rb.velocity = Vector2.zero;
        //Debug.Log("hit");
    }

    private IEnumerator Knockback(Vector2 attackerPos)
    {
        //Debug.Log("Knockback");
        Vector2 forceDir = (this.transform.position - new Vector3(attackerPos.x, attackerPos.y, 0)).normalized;
        rb.AddForce(forceDir * knockback);
        yield return new WaitForSeconds(knockbackTime);
        rb.velocity = Vector2.zero;
        //Debug.Log(rb + ": stop ");
    }

    public void Death()
    {
        //animator.SetBool("isDead", true);
        Destroy(self);
        if(this.tag == "Player")
        {
            SceneManager.LoadScene(0);
        }
        //healthBar.SetMaxHealth(0);
        //GetComponent<Collider2D>().enabled = false;
        //GetComponent<SpriteRenderer>().enabled = false;
    }
}
