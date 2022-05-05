using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Hero : MonoBehaviour
{
    [SerializeField] private float speed = 3f;
    [SerializeField] private int health = 5;
    [SerializeField] private int lives;
    [SerializeField] private float jampForce = 0.4f;
    private bool isGrounded = false;

    [SerializeField] private Image[] hearts;

    [SerializeField] private Sprite Hearth;
    [SerializeField] private Sprite Ded_hearth;

    public bool isAttacking = false;
    public bool isRecharged = true;

    public Transform attackPos;
    public float attackRange;
    public LayerMask enemy;


    private Rigidbody2D rb;
    private Animator anim;
    private SpriteRenderer sprite;

    public static Hero Instance { get; set; }

    private void OnAttack()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(attackPos.position, attackRange, enemy);

        for (int i = 0; i < colliders.Length; i++)
        {
            colliders[i].GetComponent<Entity>().GetDamage();
        }
    }

    private States State
    {
        get { return (States)anim.GetInteger("state"); }
        set { anim.SetInteger("state", (int)value); }
    }

    private void Awake()
    {
        lives = 3;
        health = lives;
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        sprite = GetComponentInChildren<SpriteRenderer>();
        Instance = this;
        isRecharged = true;
    }
    private void FixedUpdate()
    {
        CheckGround();
    }

    public void GetDamage()
    {
        lives -= 1;
        Debug.Log(lives);
    }
    private void Update()
    {
        if (isGrounded && !isAttacking) State = States.Idle;
        if (!isAttacking && Input.GetButton("Horizontal"))
            Run();
        if (!isAttacking && Input.GetButton("Jump"))
            Jump();
        if (isGrounded && Input.GetButton("Jump"))
            Jump();
        if (Input.GetButtonDown("Fire1"))
            Attack();

        if (health > lives)
            health = lives;

        for (int i = 0; i < hearts.Length; i++)
        {
            if (i < health)
                hearts[i].sprite = Hearth;
            else
                hearts[i].sprite = Ded_hearth;
        }

    }


    private void Run()
    {
        if (isGrounded) State = States.Run;

        Vector3 dir = transform.right * Input.GetAxis("Horizontal");
        transform.position = Vector3.MoveTowards(transform.position, transform.position + dir, speed * Time.deltaTime);
        sprite.flipX = dir.x < 0.0f;
    }
    private void Jump()
    {
        rb.AddForce(transform.up * jampForce, ForceMode2D.Impulse);
    }
    private void CheckGround()
    {
        Collider2D[] collider = Physics2D.OverlapCircleAll(transform.position, 0.3f);
        isGrounded = collider.Length > 1;
        if (!isGrounded) State = States.Jump;
    }

    private void Attack()
    {
        if (isGrounded && isRecharged)
        {
            State = States.Attack;
            isAttacking = true;
            isRecharged = false;


            StartCoroutine(AttackAnimation());
            StartCoroutine(AttackCoolDown());

        }
    }


    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPos.position, attackRange);
    }

    private IEnumerator AttackAnimation()
    {
        yield return new WaitForSeconds(0.4f);
        isAttacking = false;

    }

    private IEnumerator AttackCoolDown()
    {
        yield return new WaitForSeconds(0.5f);
        isRecharged = true;

    }

}
public enum States
{
    Idle,
    Run,
    Jump,
    Attack
}
