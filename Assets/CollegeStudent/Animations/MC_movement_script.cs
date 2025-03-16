using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MC_movement_script : MonoBehaviour
{
    // Movement variables
    public float moveSpeed = 5f;
    public float jumpForce = 10f;
    private float moveInput;
    private bool isFacingRight = true;

    // Ground check
    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;
    public LayerMask groundLayer;
    private bool isGrounded;

    // Health system
    public int maxHealth = 100;
    private int currentHealth;

    // Attack variables
    public float attackCooldown = 0.5f;
    private bool isAttacking = false;

    // Components
    private Rigidbody2D rb;
    private Animator anim;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        currentHealth = maxHealth;
    }

    void Update()
    {
        MovePlayer();
        Jump();
        Attack();
        UpdateAnimations();
    }

    void MovePlayer()
    {
        moveInput = Input.GetAxisRaw("Horizontal");
        rb.velocity = new Vector2(moveInput * moveSpeed, rb.velocity.y);

        // Flip character when changing direction
        if (moveInput > 0 && !isFacingRight)
            Flip();
        else if (moveInput < 0 && isFacingRight)
            Flip();
    }

    void Jump()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
        
        if (isGrounded && Input.GetKeyDown(KeyCode.Space))
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            anim.SetTrigger("Jump");
        }
    }

    void Attack()
    {
        if (Input.GetKeyDown(KeyCode.F) && !isAttacking) // Attack key (F)
        {
            isAttacking = true;
            anim.SetTrigger("Attack");
            StartCoroutine(ResetAttack());
        }
    }

    IEnumerator ResetAttack()
    {
        yield return new WaitForSeconds(attackCooldown);
        isAttacking = false;
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        anim.SetTrigger("Hurt");

        if (currentHealth <= 0)
            Die();
    }

    void Die()
    {
        anim.SetTrigger("Die");
        rb.velocity = Vector2.zero;
        this.enabled = false; // Disable script
    }

    void Flip()
    {
        isFacingRight = !isFacingRight;
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }

    void UpdateAnimations()
    {
        anim.SetBool("Run", moveInput != 0);
        anim.SetBool("isGrounded", isGrounded);
    }
}
