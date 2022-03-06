using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Virus2 : MonoBehaviour
{
    private float speed = 3f;
    private bool movingRight = true;    // tine directia de miscare
    public Transform groundCheck;       // verifica daca avem pamant in fata

    private float lives = 3;

    private ErikControls erik;
    private BaleogControls baleog;
    private OlafControls olaf;

    private Animator virusAnimation;

    private bool wasHit;
    private bool playerHit;

    private void Start()
    {
        erik = FindObjectOfType<ErikControls>();
        baleog = FindObjectOfType<BaleogControls>();
        olaf = FindObjectOfType<OlafControls>();
        virusAnimation = GetComponent<Animator>();
        Physics2D.IgnoreLayerCollision(12, 12, true);
    }

    public IEnumerator HasDied()
    {
        transform.position = Vector2.MoveTowards(transform.position, transform.position, 0);
        virusAnimation.SetTrigger("IsDead");
        yield return new WaitForSeconds(0.4f);
        Destroy(this.gameObject);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.tag.Equals("Erik") || collision.collider.tag.Equals("Baleog") || collision.collider.tag.Equals("Olaf"))
        {
            playerHit = true;
        }
        /*
        if (!(collision.collider.tag.Equals("Erik") || collision.collider.tag.Equals("Baleog") || collision.collider.tag.Equals("Olaf")))
        {
            Debug.Log("fff");
            movingRight = !movingRight;
        }
        */
    }

    public void DamageUpdate(float damage)
    {
        lives -= damage;
        wasHit = true;
    }

    private void AnimationOrientation(GameObject closestPlayer)
    {
        if (transform.position.x < closestPlayer.transform.position.x)           // reorientam animatia viruslui in caz ca merge spre stanga, sau spre dreapta
        {
            transform.localScale = new Vector2(1, 1);
        }
        else
        {
            transform.localScale = new Vector2(-1, 1);
        }
    }

    private float DistanceToPlayer(float xV, float yV, float xP, float yP)      // distanta intre inamic si un personaj
    {
        return (Mathf.Sqrt((xV - xP) * (xV - xP) + (yV - yP) * (yV - yP)));
    }


    private string MinDistance()        // numen referinta pentru ca vrem sa salvam coord gasite
    {
        float xV, yV;       // retin pozitia curenta a virusului, pentru o scriere mai compacta
        float distErik, distBaleog, distOlaf;
        xV = this.transform.position.x;
        yV = this.transform.position.y;
        //calculam toate distantele dintre virus si vikingi
        distErik = DistanceToPlayer(xV, yV, erik.transform.position.x, erik.transform.position.y);

        distBaleog = DistanceToPlayer(xV, yV, baleog.transform.position.x, baleog.transform.position.y);
        distOlaf = DistanceToPlayer(xV, yV, olaf.transform.position.x, olaf.transform.position.y);
        if (distErik < distBaleog && distErik < distOlaf)   // cautam distanta minima si returnam numele vikingului, pentrua  indica ca el va fi urmarit de virus
        {
            return "Erik";
        }
        if (distBaleog < distErik && distBaleog < distOlaf)
        {
            return "Baleog";
        }
        if (distOlaf < distErik && distOlaf < distBaleog)
        {
            return "Olaf";
        }
        return "Error";
    }

    private void movement()
    {
        transform.Translate(Vector2.right * speed * Time.deltaTime);
        RaycastHit2D groundInfo = Physics2D.Raycast(groundCheck.position, Vector2.down, 0.4f);  // origine, directie, lungimea razei
        if (groundInfo.collider == false)       // daca nu se mai detecteaza teren in fata
        {
            if (movingRight == true)        // daca mergea spre dreapta, il rotim spre stanga, si invers
            {
                transform.eulerAngles = new Vector3(0, -180, 0);
                movingRight = false;
            }
            else
            {
                transform.eulerAngles = new Vector3(0, 0, 0);
                movingRight = true;
            }
        }
    }

    public IEnumerator Freeze()
    {
        yield return new WaitForSeconds(0.3f);
        wasHit = false;
    }

    public IEnumerator AttackCooldown()     // cand loveste un jucator,s e opreste putin
    {
        yield return new WaitForSeconds(0.3f);
        playerHit = false;
    }

    private bool sameLevelYAxis(float yV, float yP)     // verifica daca jucatorul si virusul sunt apropiati ca si inaltime
    {
        if (Mathf.Abs(yV - yP) > 3)
            return false;
        else
            return true;
    }


    private void Update()
    {
        //Debug.Log(movingRight);
        if (!wasHit)        // daca nu e lovit, isi va continua traseul
        {
            if (playerHit)      // daca a lovit jucatorul, atunci are 0.3 secunde de cooldown pana la urmatorul atac
            {
                transform.position = Vector2.MoveTowards(transform.position, transform.position, 0);
                StartCoroutine(AttackCooldown());
            }
            else
            {
                GameObject closestPlayer = GameObject.FindGameObjectWithTag(MinDistance());
                if (DistanceToPlayer(transform.position.x, transform.position.y, closestPlayer.transform.position.x, closestPlayer.transform.position.y) < 15
                    && sameLevelYAxis(transform.position.y, closestPlayer.transform.position.y))    // daca e in raza si la acelasi nivel
                {
                    RaycastHit2D groundInfo = Physics2D.Raycast(groundCheck.position, Vector2.down, 2f);  // oriigine, directie, lungimea razei
                    if (closestPlayer.transform.position.x < transform.position.x)      // orientam virusul inspre viking
                    {
                        movingRight = false;
                        transform.eulerAngles = new Vector3(0, -180, 0);
                    }
                    else
                    {
                        movingRight = true;
                        transform.eulerAngles = new Vector3(0, 0, 0);
                    }
                    if (groundInfo.collider)        // daca inca este detectat pamant in fata
                    {
                        transform.Translate(Vector2.right * speed * Time.deltaTime);        // continuam miscarea in directia in care se duce vikingul
                    }   // altfel va sta pe loc ca sa nu cada de pe margine
                    /*
                    else
                    {
                        GetComponent<Rigidbody2D>().velocity = new Vector2(0, 0);
                    }
                    */
                }
                else
                {
                    movement();
                }
            }
        }
        else if (wasHit)      // daca a fost lovit si are mai mult de o viata (daca ar avea 0, atunci facem anumatia de murit
        {
            Debug.Log(lives);
            if (lives <= 0) // daca e mort
            {
                StartCoroutine(HasDied());
            }
            else
            {
                if (baleog.transform.position.x < transform.position.x)     // daca e lovit din stanga
                    transform.position = Vector2.MoveTowards(transform.position, new Vector2(transform.position.x + 3f, transform.position.y), speed * Time.deltaTime);     // knockback cand e lovit
                else       // daca e lovit din dreapta
                    transform.position = Vector2.MoveTowards(transform.position, new Vector2(transform.position.x - 3f, transform.position.y), speed * Time.deltaTime);     // knockback cand e lovit
                StartCoroutine(Freeze());  // nu reluam traseul normal pentru 0.3 secunde 
            }
        }
    }
}
