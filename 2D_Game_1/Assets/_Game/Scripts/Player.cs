using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Character
{
    [SerializeField] private Rigidbody2D rb;
    
    [SerializeField] private LayerMask groundLayer; /// để như này dễ sửa thông số của ground
    [SerializeField] private float speed = 5;
    [SerializeField] private float jumpForce = 350;

    [SerializeField] private Kunai kunaiPrefab;
    [SerializeField] private Transform throwPoint;
    [SerializeField] private GameObject attackArea;

    private bool isGrounded = false;
    private bool isJumping = false;
    private bool isAttack = false;

    private float horizontal;

    private int coin = 0;

    private Vector3 savePoint;

    private void Awake()
    {
        coin = PlayerPrefs.GetInt("coin",0);
    }
    void Update()           /// đổi hàm từ FixedUpdate sang Update tránh delay GetKey, code mẫu bị delay
    {
        //Debug.Log("isGrounded : "    + isGrounded + " isJumping : " +   isJumping);
        if (isDead)
        {
            return;
        }
        
       
        isGrounded = CheckGrounded();

        //horizontal = Input.GetAxisRaw("Horizontal");

        if(isAttack)
        {
            rb.velocity = Vector2.zero;
            return;
        }

        if(isGrounded)
        {
            

            /// jump
            if (Input.GetKeyDown(KeyCode.Space) && isGrounded && isJumping==false)
            {
                Jump();
            }

            if (Mathf.Abs(horizontal) > 0.01f && isJumping == false)
            {
                ChangeAnim("Run");
            }

            /// attack
            if (Input.GetKeyDown(KeyCode.C) && isGrounded && isJumping == false)
            {
                Attack();
            }

            /// throw
            if (Input.GetKeyDown(KeyCode.V) && isGrounded && isJumping == false)
            {
                Throw();
            }
        }
        
        if (!isGrounded && rb.velocity.y <= 0)
        {
            Fall();
        }

        if (Mathf.Abs(horizontal) > 0.01f)
        {
            rb.velocity = new Vector2(horizontal * speed, rb.velocity.y);

            transform.rotation = Quaternion.Euler(new Vector3(0, horizontal > 0 ? 0 : 180, 0));
        }
        else if (isGrounded && isJumping==false)
        {
            ChangeAnim("Idle");
            rb.velocity = Vector2.zero;
        }

    }

    public override void OnInit()
    {
        base.OnInit();

        isAttack = false;
        currentAnimName = "Die";
        transform.position = savePoint;
        ChangeAnim("Idle");
        DeActiveAttack();
        SavePoint();
        UIManager.instance.SetCoin(coin);
    }

    public override void OnRespawn()
    {
        base.OnRespawn();
        OnInit();
    }

    protected override void OnDeath()
    {
        ChangeAnim("Die");
        Invoke(nameof(OnInit), 1f);
    }

    private bool CheckGrounded()
    {
        Debug.DrawLine(transform.position , transform.position + Vector3.down * 1.05f, Color.green);
        RaycastHit2D hit = Physics2D.Raycast(transform.position , Vector2.down, 1.05f, groundLayer); /// bắn 1 tia xem có va chạm với Player

        return (hit.collider != null); 
    }

    public void Jump()
    {
        if (isJumping || isGrounded==false)
        {//// thêm code tránh lỗi nhảy vô hạn lúc bấm button
            return;
        }
        isJumping = true;
        ChangeAnim("Jump");
        rb.AddForce(jumpForce * Vector2.up);
    }

    private void Fall()
    {
        isJumping = false;
        ChangeAnim("Fall");
    }

    public void Attack()
    {
        if (isAttack)
        {//// thêm code tránh lỗi nhảy vô hạn lúc bấm button
            return;
        }
        ChangeAnim("Attack");
        isAttack = true;
        Invoke(nameof(ResetAttack), 1f);
        ActiveAttack();
        Invoke(nameof(DeActiveAttack), 1f);
    }

    public void Throw()
    {
        if (isAttack)
        {//// thêm code tránh lỗi nhảy vô hạn lúc bấm button
            return;
        }
        ChangeAnim("Throw");
        isAttack = true;
        Invoke(nameof(ResetAttack), 1f);

        Instantiate(kunaiPrefab,throwPoint.position,throwPoint.rotation);
    }

    private void ResetAttack()
    {
        isAttack = false;
        /// Bug này do sau khi hàm invoke ngủ thì currentAninName = Idle
        /// fixed
        currentAnimName = "Attack";
        ChangeAnim("Idle");
    }

    internal void SavePoint()
    {
        savePoint = transform.position;
    }

    private void ActiveAttack()
    {
        attackArea.SetActive(true);
    }

    private void DeActiveAttack()
    {
        attackArea.SetActive(false);
    }

    public void SetMove(float horizontal)
    {
        this.horizontal = horizontal;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Coin")
        {
            coin++;
            PlayerPrefs.SetInt("coin", coin);
            UIManager.instance.SetCoin(coin);
            Destroy(collision.gameObject);
        }

        if(collision.tag == "DeathZone")
        {
            ChangeAnim("Die");

            Invoke(nameof(OnInit), 1f);
        }
    }

}
