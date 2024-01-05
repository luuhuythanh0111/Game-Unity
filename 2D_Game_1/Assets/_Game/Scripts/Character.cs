using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    [SerializeField] private Animator anim;
    [SerializeField] protected HealthBar healthBar;

    [SerializeField] protected CombatText combatTextPrefab;


    protected string currentAnimName;

    private float hp;

    public bool isDead => hp <= 0;


    private void Start()
    {
        OnInit();
    }

    public virtual void OnInit()
    {
        hp = 100;
        if(healthBar != null)
        healthBar.OnInit(100, transform);
    }

    public virtual void OnRespawn()
    {

    }
    protected virtual void OnDeath()
    {
        ChangeAnim("Die");
        Invoke(nameof(OnRespawn), 2f);
    }

    protected void ChangeAnim(string animName)
    {
        if (currentAnimName != animName)
        {
            //Debug.Log(currentAnimName + " -> " + animName);
            anim.ResetTrigger(animName);
            currentAnimName = animName;
            anim.SetTrigger(currentAnimName);
        }
    }


    public void OnHit(float damage)
    {
        if(!isDead)
        {
            hp -= damage;

            if(isDead)
            {
                hp = 0;
                OnDeath();
            }

            if(healthBar != null)
            healthBar.SetNewHp(hp);
            Instantiate(combatTextPrefab, transform.position + Vector3.up,Quaternion.identity).OnInit(damage);

            /// prefab, vị trí , góc xoay   
        }
    }

}
