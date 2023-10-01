using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float Speed;
    public static PlayerController Instance;

    protected Vector2 movement;
    protected Rigidbody2D rb;
    protected SpriteRenderer spriteRen;
    protected Animator animator;

    void Awake() => Instance = this;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRen = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");

        rb.velocity = movement.normalized * Speed;

        spriteRen.flipX = movement.x == 0 ? spriteRen.flipX : movement.x < 0;

        animator.SetBool("moving", movement != Vector2.zero);
    }
}
