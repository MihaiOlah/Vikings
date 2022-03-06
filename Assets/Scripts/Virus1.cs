using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Virus1 : MonoBehaviour
{
    private Animator virusAnimation;
    private float speed = 1.5f;       //viteza de miscare a inamicului
    private ErikControls erik;
    private BaleogControls baleog;
    private OlafControls olaf;

    private bool hit = false;   // flag care retine daca a lovit tinta, sau nu
    private float timer = 0.1f;
    private float deathTimer = 0.5f;    // timpul pana cand se distruge de cand are 0 vieti
    private bool destroyFlag = false;   // arata daca are 0 vieti ca sa fie distrus
    private bool wasHit = false; // retine daca virusula fost lovit de jucator
    private float wasHitTimer = 0f;     // timer pentru timpul cat a fost lovit virusul

    private float hitPoints = 3;      // numarul de vieti ale virusului

    private float DistanceToPlayer(float xV, float yV, float xP, float yP)      // distanta intre inamic si un personaj
    {
        return (Mathf.Sqrt((xV - xP) * (xV - xP) + (yV - yP) * (yV - yP)));
    }

    // Start is called before the first frame update
    void Start()
    {
        erik = FindObjectOfType<ErikControls>();
        baleog = FindObjectOfType<BaleogControls>();
        olaf = FindObjectOfType<OlafControls>();
        virusAnimation = GetComponent<Animator>();
        Physics2D.IgnoreLayerCollision(14, 8, true);
        Physics2D.IgnoreLayerCollision(14, 17, true);
    }

    // Cautam distanta minina dintre jucator si virus
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
        if (distErik < 15 || distBaleog < 15 || distOlaf < 15)     // urmareste doar in apropiere, ca sa nu fie enervant ca apare de nicaieri
        {
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
        else
        {
            return "Too far";
        }
    }

    private void MovementUpdate(float x, float y)
    {
        float step = speed * Time.deltaTime;    // practic viteza in intervalul de timp
        this.transform.position = Vector2.MoveTowards(transform.position, new Vector2(x, y), step);
    }

    private void AnimationOrientation(string targetName)
    {
        GameObject targetPlayer = GameObject.FindGameObjectWithTag(targetName);     // cautam vikingul care fost gasit ca este cel mai aproape
        if (transform.position.x < targetPlayer.transform.position.x)           // reorientam animatia viruslui in caz ca merge spre stanga, sau spre dreapta
        {
            transform.localScale = new Vector2(1, 1);
        }
        else
        {
            transform.localScale = new Vector2(-1, 1);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag.Equals("Erik") || collision.tag.Equals("Baleog") || collision.tag.Equals("Olaf"))     // daca a lovit un viking atunci hit devine true
        {
            hit = true;
        }
        /*
        if (collision.tag.Equals("Weapon"))     // daca este lovit de o arma controlata de jucator
        {
            HitPointsUpdate();
        }
        */
    }

    public void DamageUpdate(float damage)        // daca e lovit de jucator, se scade din viata
    {
        hitPoints -= damage;
        wasHit = true;
        wasHitTimer = 0.5f;
        //if (hitPoints == 0)
        if(Mathf.Approximately(hitPoints, 0))
        {
            virusAnimation.SetBool("IsDead", true);
            destroyFlag = true;
        }
    }
 
    // Update is called once per frame
    void Update()
    {
        if (!hit)       // daca nu a lovit niciun viking, virusul continua sa urmareasca jucatorul
        {
            if (wasHit || wasHitTimer > 0) // daca virusul a fost lovit, atunci are knock back
            {
                wasHit = false;
                wasHitTimer -= Time.deltaTime;
                if (baleog.transform.position.x < transform.position.x)     // Baleog e singurul care il poate ataca, deci ne uitam unde e pozitia lui fata de virus, ca sa stim in ce directe ia knock back
                {
                    transform.position = Vector2.MoveTowards(transform.position, new Vector2(transform.position.x + 3, transform.position.y), (speed - 1) * Time.deltaTime);
                }
                else
                {
                    transform.position = Vector2.MoveTowards(transform.position, new Vector2(transform.position.x - 3, transform.position.y), (speed - 1) * Time.deltaTime);
                }
            }
            else
            {
                string minDistanceName = MinDistance();     // se preia numele vikingului cel mai apropiat
                if (minDistanceName.Equals("Erik"))
                {
                    MovementUpdate(erik.transform.position.x, erik.transform.position.y);     // se dau coord. vikingului cel mai apropit pentru a porni urmarirea spre pozitia lui
                    AnimationOrientation("Erik");       // cauta sa se orienteze in functie de pozitia vikingului urmarit
                }
                else if (minDistanceName.Equals("Baleog"))
                {
                    MovementUpdate(baleog.transform.position.x, baleog.transform.position.y);
                    AnimationOrientation("Baleog");
                }
                else if (minDistanceName.Equals("Olaf"))
                {
                    MovementUpdate(olaf.transform.position.x, olaf.transform.position.y);
                    AnimationOrientation("Olaf");
                }
                else if (minDistanceName.Equals("Too far"))
                {
                    MovementUpdate(transform.position.x, transform.position.y);
                }
                else
                {
                    Debug.Log("Eroare, nu se gaseste distanta minima");
                }
            }
        }
        else      // in caz ca a fost lovit un viking, virusul se opreste o fractiune de secunda (cooldown time) intre atacuri
        {
            // avem un cronometru de o fractiune de secunda, iar pozitia virusul nu se modifica in acest timp
            transform.position = new Vector2(transform.position.x, transform.position.y);
            timer -= Time.deltaTime;
            if (timer < 0f)
            {
                hit = false;
                timer = 0.5f;
            }
        }
        if (destroyFlag)
        {
            deathTimer -= Time.deltaTime;
            if (deathTimer < 0)
            {
                Destroy(this.gameObject);
            }
        }
    }
}
