using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Robot : MonoBehaviour
{
    private float speed = 4;
    private float speedY = 4f;
    private float lives = 1;

    private Animator robotAnimation;

    private ErikControls erik;
    private BaleogControls baleog;
    private OlafControls olaf;

    private bool isDead = false;
    private bool isAttacking = false;

    private const double maxAttackDistanceX = 20;
    private const double maxAttackDistanceY = 3;

    private CapsuleCollider2D cc;       // collider-ul de pe caracter
    private Vector2 colliderSize;

    [SerializeField]
    private float slopeCheckDistance;
    public LayerMask groundLayer;           // lista de obiecte de pe care se poate sari

    private bool isOnSlope;
    private float slopeSideAngle;
    private Vector2 slopeNormalPerp;        // perpendiculara la panta
    private float slopeDownAngleOld;
    private float slopeDownAngle;       // raycast-ul care liveste in jos

    private bool onGroundZeroDist;
    public Transform onGroundZeroDistCheckPoint;      // verifica daca caracterul e pe pamant
    public float onGroundZeroDistCheckRadius;         // raza pana la care e considerat ca se poate sari

    Rigidbody2D rigidBody;

    private bool canFall;
    private bool startCorutine;

    private bool movingRight = true;    // tine directia de miscare
    public Transform groundCheck;       // verifica daca avem pamant in fata

    public GameObject projectilePrefab;

    void OnDrawGizmosSelected()
    {
        // Draw a yellow sphere at the transform's position
        Gizmos.color = Color.yellow;
        //Gizmos.DrawWireSphere(onGroundZeroDistCheckPoint2.position, onGroundZeroDistCheckRadius2);
        Gizmos.DrawWireSphere(onGroundZeroDistCheckPoint.position, onGroundZeroDistCheckRadius);


        //RaycastHit2D groundInfo = Physics2D.Raycast(groundCheck.position, Vector2.down, 4f);  // origine, directie, lungimea razei
        Gizmos.color = Color.red;
        //Debug.Log("Ground " + groundInfo.collider);
        Vector2 direction = groundCheck.TransformDirection(Vector2.down);
        Gizmos.DrawWireSphere(groundCheck.position, 4f);
        Gizmos.DrawRay(groundCheck.position, direction);

    }

    public IEnumerator CanFall()     // reseteaza booleanul care permite miscarea pe y dupa 0.1 S
    {
        yield return new WaitForSeconds(0.1f);
        canFall = true;
        startCorutine = false;
    }

    public void SetIsDead()
    {
        isDead = true;
    }

    private IEnumerator IsDead()
    {
        transform.position = Vector2.MoveTowards(transform.position, transform.position, 0);
        robotAnimation.SetBool("isDead", true);
        yield return new WaitForSeconds(1f);
        Destroy(this.gameObject);
    }

    private void EndAttackAnimation(string message)
    {
        robotAnimation.SetBool("isAttacking", false);
        isAttacking = false;
    }

    private void BeginAttackAnimation(string message)
    {
        GameObject projectile = Instantiate(projectilePrefab) as GameObject;
        projectile.transform.localScale = transform.localScale;
        projectile.transform.position = new Vector3(transform.position.x, transform.position.y + 0.573f, transform.position.z);
    }

    private void IsAttacking(string closestViking)
    {
        transform.position = Vector2.MoveTowards(transform.position, transform.position, 0);
        robotAnimation.SetBool("isAttacking", true);
        // Spawn projectile HERE
        //yield return new WaitForSeconds(0.4f);
        //Debug.Log(robotAnimation.GetCurrentAnimatorStateInfo(0).normalizedTime);
        //robotAnimation.GetCurrentAn
        /*
        if (robotAnimation.GetCurrentAnimatorStateInfo(0).normalizedTime > 1)
        {
            robotAnimation.SetBool("isAttacking", false);
            isAttacking = false;
        }
        */
    }

    private string MinDistance(List<string> validVikings)
    {
        double minDistance = -1;
        string minDistanceViking = "";
        double dist = -1;
        
        // Cautam distanta minima si salvam numele vikingului caruia ii corespunde
        for (int i = 0; i < validVikings.Count; i++)
        {
            if (validVikings[i] == "Erik")
            {
                dist = Mathf.Abs(erik.transform.position.x - this.transform.position.x);
            }
            else if (validVikings[i] == "Baleog")
            {
                dist = Mathf.Abs(baleog.transform.position.x - this.transform.position.x);
            }
            else if (validVikings[i] == "Olaf")
            {
                dist = Mathf.Abs(olaf.transform.position.x - this.transform.position.x);
            }

            if (minDistance == -1)
            {
                minDistance = dist;
                minDistanceViking = validVikings[i];
            }
            else if (minDistance > dist)
            {
                minDistance = dist;
                minDistanceViking = validVikings[i];
            }
        }

        return minDistanceViking;
    }

    private void moveToViking(string closestViking)
    {
        if (closestViking == "Erik")
        {
            if (erik.transform.position.x < transform.position.x && movingRight)
            {
                movingRight = false;
                transform.eulerAngles = new Vector3(0, -180, 0);
            }
            else if (erik.transform.position.x > transform.position.x && !movingRight)
            {
                movingRight = true;
                transform.eulerAngles = new Vector3(0, 0, 0);
            }

            if (Mathf.Abs(transform.position.y - erik.transform.position.y) < 0.1f)
            {
                isAttacking = true;
            }
        }
        else if (closestViking == "Baleog")
        {
            if (baleog.transform.position.x < transform.position.x && movingRight)
            {
                movingRight = false;
                transform.eulerAngles = new Vector3(0, -180, 0);
            }
            else if (baleog.transform.position.x > transform.position.x && !movingRight)
            {
                movingRight = true;
                transform.eulerAngles = new Vector3(0, 0, 0);
            }

            if (Mathf.Abs(transform.position.y - baleog.transform.position.y) < 0.1f)
            {
                isAttacking = true;
            }
        }
        else if (closestViking == "Olaf")
        {
            if (olaf.transform.position.x < transform.position.x && movingRight)
            {
                movingRight = false;
                transform.eulerAngles = new Vector3(0, -180, 0);
            }
            else if (erik.transform.position.x > transform.position.x && !movingRight)
            {
                movingRight = true;
                transform.eulerAngles = new Vector3(0, 0, 0);
            }

            if (Mathf.Abs(transform.position.y - olaf.transform.position.y) < 0.1f)
            {
                isAttacking = true;
            }
        }
    }

    private string CheckAttackConditions()
    {
        string closestViking;
        List<string> validVikings = new List<string>();

        // Verifica daca exista un viking care este cam la acelasi nivel ca inaltime cu robotul (pe axa Y) si daca e in raza de atac
        if (Mathf.Abs(this.transform.position.x - erik.transform.position.x) < maxAttackDistanceX)
        {
            validVikings.Add("Erik");
        }
        else if (validVikings.Contains("Erik"))
        {
            validVikings.Remove("Erik");
        }

        if (Mathf.Abs(this.transform.position.x - baleog.transform.position.x) < maxAttackDistanceX)
        {
            validVikings.Add("Baleog");
        }
        else if (validVikings.Contains("Baleog"))
        {
            validVikings.Remove("Baleog");
        }

        if (Mathf.Abs(this.transform.position.x - olaf.transform.position.x) < maxAttackDistanceX)
        {
            validVikings.Add("Olaf");
        }
        else if (validVikings.Contains("Olaf"))
        {
            validVikings.Remove("Olaf");
        }

        //Debug.Log(validVikings.Count);

        closestViking = MinDistance(validVikings); 

        if (closestViking != "")
        {
            //isAttacking = true;
            moveToViking(closestViking);        // Schimba orientarea si directia de mers a robotului in functie de vikingul cel mai apropiat
        }

        //Debug.Log(closestViking);

        // Gaseste cel mai apropiat viking care e la nivel cu robotul
        return closestViking;
    }

    private void SplopeCheck()
    {
        Vector2 checkPos = transform.position - new Vector3(0.0f, colliderSize.y / 2);
        SlopeCheckHorizontal(checkPos);
        SplopeCheckVerfical(checkPos);
    }

    private void SlopeCheckHorizontal(Vector2 checkPos)
    {
        RaycastHit2D slopeHitFront = Physics2D.Raycast(checkPos, transform.right, slopeCheckDistance, groundLayer);
        RaycastHit2D slopeHitBack = Physics2D.Raycast(checkPos, -transform.right, slopeCheckDistance, groundLayer);
        if (slopeHitFront)  // daca in fata urmeaza o panta
        {
            isOnSlope = true;
            slopeSideAngle = Vector2.Angle(slopeHitFront.normal, Vector2.up);
        }
        else if (slopeHitBack)
        {
            isOnSlope = true;
            slopeSideAngle = Vector2.Angle(slopeHitBack.normal, Vector2.up);
        }
        else
        {
            slopeSideAngle = 0.0f;
            isOnSlope = false;
        }

    }

    private void SplopeCheckVerfical(Vector2 checkPos)
    {
        RaycastHit2D hit = Physics2D.Raycast(checkPos, Vector2.down, slopeCheckDistance, groundLayer);       // verifica ce lovsete
        if (hit)    // daca loveste ceva, o desenam
        {
            slopeNormalPerp = Vector2.Perpendicular(hit.normal).normalized;     // ca sa pastram acceasi viteza
            slopeDownAngle = Vector2.Angle(hit.normal, Vector2.up);
            if (slopeDownAngle != slopeDownAngleOld)
            {
                isOnSlope = true;
            }
            slopeDownAngleOld = slopeDownAngle;
            Debug.DrawRay(hit.point, slopeNormalPerp, Color.red);
            Debug.DrawRay(hit.point, hit.normal, Color.green);
        }
    }

    private void Movement()
    {
        RaycastHit2D groundInfo = Physics2D.Raycast(groundCheck.position, Vector2.down, 4f);  // origine, directie, lungimea razei

        if (groundInfo.collider == null || groundInfo.collider.tag == "Electricity" || groundInfo.collider.tag == "Wall")       // daca nu se mai detecteaza teren in fata
        {
            
            if (movingRight == true)        // daca mergea spre dreapta, il rotim spre stanga, si invers
            {
                //transform.localScale = new Vector2(-1, 1);
                transform.eulerAngles = new Vector3(0, -180, 0);
                movingRight = false;
            }
            else
            {
                //transform.localScale = new Vector2(-1, 1);
                transform.eulerAngles = new Vector3(0, 0, 0);
                movingRight = true;
            }
        }
        
        if (onGroundZeroDist && !isOnSlope)
        {
            canFall = false;
            startCorutine = false;
            rigidBody.velocity = new Vector2((movingRight ?1 :-1) * speed, 0.0f);
            Debug.Log("1");
        }
        else if (onGroundZeroDist && isOnSlope)
        {
            canFall = false;
            startCorutine = false;
            rigidBody.velocity = new Vector2((movingRight ? 1 : -1) * speed * slopeNormalPerp.x, 5);     // punem - din cauza ca trebuie compensat de cum e determianta normala
            Debug.Log("2");
        }
        else if (!onGroundZeroDist)
        {
            startCorutine = true;
            if (!canFall)       // daca este in capatul pantei , mai e tinuta miscarea pe y 0 timp de 0.1 S, dupa care se poate misca in voie
                rigidBody.velocity = new Vector2((movingRight ? 1 : -1) * speed, 0f);
            else
                rigidBody.velocity = new Vector2((movingRight ? 1 : -1) * speed, rigidBody.velocity.y);
            Debug.Log("3");
        }
        Debug.Log(movingRight);
    }

    // Start is called before the first frame update
    void Start()
    {
        erik = FindObjectOfType<ErikControls>();
        baleog = FindObjectOfType<BaleogControls>();
        olaf = FindObjectOfType<OlafControls>();
        robotAnimation = GetComponent<Animator>();
        Physics2D.IgnoreLayerCollision(12, 12, true);
        rigidBody = GetComponent<Rigidbody2D>();

        cc = GetComponent<CapsuleCollider2D>();
        colliderSize = cc.size;
    }

    // Update is called once per frame
    void Update()
    {
        string closestViking;
        onGroundZeroDist = Physics2D.OverlapCircle(onGroundZeroDistCheckPoint.position, onGroundZeroDistCheckRadius, groundLayer); // Physics2D.OverlapCircle(onGroundZeroDistCheckPoint2.position, 

        if (startCorutine)    // daca nu sare si ajunge la capatul pantei, atunci putem sa initiem corutina
        {
            StartCoroutine(CanFall());
        }

        SplopeCheck();

        if (isDead)
        {
            StartCoroutine(IsDead());
        }
        else
        {
            closestViking = CheckAttackConditions();
            Debug.Log(isAttacking);

            if (isAttacking)
            {
                //Debug.Log("ssss");
                IsAttacking(closestViking);
                rigidBody.velocity = new Vector2(0, 0);
            }
            else
            {
                //Debug.Log("ssss");
                Movement();
            }
        }
    }
}
