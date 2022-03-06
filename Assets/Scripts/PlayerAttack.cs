using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    private float attackTime = 0f;      // timpul dintre atacuri, jucatorul nu poate ataca non-stop, initial e 0, fiindca nu am atacat inca
    public Transform attackPosition;
    public LayerMask enemyLayer;
    public float range;
    private Animator playerAnimation;

    // Start is called before the first frame update
    void Start()
    {
        playerAnimation = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (attackTime <= 0 && PlayerPrefs.GetInt("BaleogHit") == 0)    // daca a trecut timpul dintre atacuri, atnci se poate ataca din nou daca apasam spatiu
        {
            if (Input.GetKey(KeyCode.Space) && GetComponent<BaleogControls>().activePlayer && GetComponent<BaleogControls>().isTouchingGround)
            {
                PlayerPrefs.SetInt("BaleogIsAttacking", 1);     // daca atacam, notificam in controale ca atacam
                //playerAnimation.SetBool("IsAttacking", true);
                playerAnimation.SetTrigger("IsAttacking");      //pornim animatia de atacat
                /*
                Collider2D[] enemies = Physics2D.OverlapCircleAll(attackPosition.position, range, enemyLayer);  //luam toti dusmanii din raza setata
                Debug.Log(enemies.Length);
                for (int i = 0; i < enemies.Length; i++)    // cautam care sunt si ii lovim
                {
                    if (GetComponent<Virus1>() != null)
                        enemies[i].GetComponent<Virus1>().DamageUpdate(1);
                    else if (GetComponent<Virus2>() != null)
                    {
                        Debug.Log("v2");
                        enemies[i].GetComponent<Virus2>().DamageUpdate(1);
                    }
                    // de completat cu restul de virusuri

                }
                */
                Collider2D enemy = Physics2D.OverlapCircle(attackPosition.position, range, enemyLayer);
                bool enemyFound = Physics2D.OverlapCircle(attackPosition.position, range, enemyLayer);
                if (enemyFound)
                {
                    if (enemy.GetComponent<Virus2>() != null)
                    {
                        enemy.GetComponent<Virus2>().DamageUpdate(1);
                    }
                    else if (enemy.GetComponent<Virus1>() != null)
                    {
                        enemy.GetComponent<Virus1>().DamageUpdate(1);
                    }
                }

            }
            attackTime = 0.2f;
        }
        else
        {
            PlayerPrefs.SetInt("BaleogIsAttacking", 0);
            //playerAnimation.SetBool("IsAttacking", false);
            attackTime -= Time.deltaTime;
        }
    }
    /*
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPosition.position, range);
    }
    */
}
