using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class OlafControls : MonoBehaviour
{

    [HideInInspector] public bool activePlayer;

    private float speed = 5f;
    private float speedY = 4f;
    Rigidbody2D rigidBody;
    public Transform groundCheckPoint;      // verifica daca caracterul e pe pamant
    public float groundCheckRadius;         // raza pana la care e considerat ca se poate sari
    public LayerMask groundLayer;           // lista de obiecte de pe care se poate sari
    private Animator playerAnimation;

    private bool isTouchingGround;
    private bool isTouchingLadder;
    private bool shieldUp;

    public GameObject sideShield;
    public GameObject upShield;

    private int lives = 3;      // are 3 vieti
    private float timeToRespawn = 0.7f;

    private bool wasHit = false;
    private bool shieldUpdate;  // este doar ca sa nu reactualizeze scuturile la fiecare frame, pentru performanta

    private CapsuleCollider2D cc;       // collider-ul de pe caracter
    private Vector2 colliderSize;
    [SerializeField]
    private float slopeCheckDistance;
    private float slopeDownAngle;       // raycast-ul care liveste in jos
    private Vector2 slopeNormalPerp;        // perpendiculara la panta
    private bool isOnSlope;
    private float slopeDownAngleOld;
    private float slopeSideAngle;
    [SerializeField]
    private PhysicsMaterial2D noFriction;
    [SerializeField]
    private PhysicsMaterial2D fullFriction;

    private bool onGroundZeroDist;
    public Transform onGroundZeroDistCheckPoint;      // verifica daca caracterul e pe pamant
    public float onGroundZeroDistCheckRadius;         // raza pana la care e considerat ca se poate sari

    private bool canFall;
    private bool startCorutine;
    private bool endOfTheLadder;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag.Equals("Ladder"))
            isTouchingLadder = true;
        if (collision.tag.Equals("Enemy"))
        {
            lives--;
            wasHit = true;
        }
        if (collision.tag.Equals("Electricity"))
            lives = 0;
        if (collision.tag.Equals("EndOfLadder"))
            endOfTheLadder = true;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag.Equals("Ladder"))
            isTouchingLadder = false;
        if (collision.tag.Equals("Enemy"))
        {
            wasHit = false;
        }
        if (collision.tag.Equals("EndOfLadder"))
            endOfTheLadder = false;
    }

    public void DecrementLives(int hitPoints)
    {
        lives -= hitPoints;
    }

    private void HitScenario()
    {
        if (PlayerPrefs.GetInt("hitRight") == 1)    // daca a fost lovin din dreapta, orientam animatia coresunzarot si ne miscam spre stanga
        {
            rigidBody.velocity = new Vector2(3, rigidBody.velocity.y);
        }
        else
        {
            rigidBody.velocity = new Vector2(-3, rigidBody.velocity.y);
        }
    }

    public void UpdateSpeed(float percentage)
    {
        speed += percentage * speed / 100;
        speedY += percentage * speedY / 100;
    }

    private void MovementAxisX()
    {

        float movement = Input.GetAxis("Horizontal");
        if (movement > 0)
        {
            transform.localScale = new Vector2(1, 1);
            rigidBody.velocity = new Vector2(movement * speed, rigidBody.velocity.y);
        }
        else if (movement < 0)
        {
            transform.localScale = new Vector2(-1, 1);
            rigidBody.velocity = new Vector2(movement * speed, rigidBody.velocity.y);
        }
        else            // daca stam pe loc
        {
            rigidBody.velocity = new Vector2(0, rigidBody.velocity.y);   // daca nu e activ si sta pe loc, il tinem pe loc, sa nu alunece de la fizica din Unity
        }
        if (movement != 0)
            playerAnimation.SetFloat("Speed", Mathf.Abs(rigidBody.velocity.x));     // actualizam animatia daca sta, sau daca merge
        else
            playerAnimation.SetFloat("Speed", -1);

        if (onGroundZeroDist && !isOnSlope && !endOfTheLadder)
        {
            canFall = false;
            startCorutine = false;
            rigidBody.velocity = new Vector2(movement * speed, 0.0f);
        }
        else if (onGroundZeroDist && isOnSlope)
        {
            canFall = false;
            startCorutine = false;
            rigidBody.velocity = new Vector2((-movement) * speed * slopeNormalPerp.x, speed * slopeNormalPerp.y * (-movement));     // punem - din cauza ca trebuie compensat de cum e determianta normala
        }
        else if (!onGroundZeroDist)
        {
            startCorutine = true;
            if (!canFall)       // daca este in capatul pantei , mai e tinuta miscarea pe y 0 timp de 0.1 S, dupa care se poate misca in voie
                rigidBody.velocity = new Vector2(movement * speed, 0f);
            else
                rigidBody.velocity = new Vector2(movement * speed, rigidBody.velocity.y);
        }
    }

    private void MovementAxisY()        // adica pe scara
    {
        float movement = Input.GetAxis("Vertical");
        this.GetComponent<Rigidbody2D>().gravityScale = 0.0f;       // pe scara ignoram gravitatia ca sa nu para ca aluneca in jos
        if (movement > 0)
        {
            playerAnimation.speed = 1;
            rigidBody.velocity = new Vector2(rigidBody.velocity.x, movement * speedY);
        }
        else if (movement < 0)
        {
            playerAnimation.speed = 1;
            rigidBody.velocity = new Vector2(rigidBody.velocity.x, movement * speedY);
        }
        else            // daca stam pe loc
        {
            rigidBody.velocity = new Vector2(rigidBody.velocity.x, 0);
            if (!isTouchingGround && isTouchingLadder)
                playerAnimation.speed = 0;
        }
    }

    private void MovementAndAnimationUpdate()
    {
        if (isTouchingGround && !isTouchingLadder)      // daca e doar pe sol, dar nu langa o scara
        {
            playerAnimation.SetBool("OnGround", true);
            playerAnimation.SetBool("Stairs", false);
            MovementAxisX();
            this.GetComponent<Rigidbody2D>().gravityScale = 1f;
            playerAnimation.speed = 1;
        }
        else if (isTouchingGround && isTouchingLadder)  //daca e la baza scarii
        {
            playerAnimation.SetBool("OnGround", true);
            playerAnimation.SetBool("Stairs", false);
            if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))  // ca sa nu poata sa fie animatia de coborat cand suntem pe pamant
            {
                MovementAxisX();
                this.GetComponent<Rigidbody2D>().gravityScale = 1f;
                playerAnimation.speed = 1;
            }
            else      // altfel punem merge si sus si jos
            {
                MovementAxisX();
                MovementAxisY();
            }
        }
        else if (!isTouchingGround && isTouchingLadder)     // daca e pe scara
        {
            MovementAxisX();
            MovementAxisY();
            playerAnimation.SetBool("Stairs", true);
            playerAnimation.SetBool("OnGround", false);      // putem onground false, ca sa nu se blocheze in animatia de sarit in aer si pe scara
        }
        else if (!isTouchingGround && !isTouchingLadder)         // daca nu e nici pe scara, nici pe pamant, atunci e in cadere
        {
            playerAnimation.SetBool("OnGround", false);
            playerAnimation.SetBool("Stairs", false);      
            MovementAxisX();
            if (shieldUp)               // daca are scutul sus, va plana verical, alfel facadea liber
            {
                rigidBody.velocity = new Vector2(rigidBody.velocity.x, -2.2f);      // -2.2f e viteza aleasa de mine pentru planat
            }
            else
            {
                rigidBody.velocity = new Vector2(rigidBody.velocity.x, rigidBody.velocity.y);
            }
            this.GetComponent<Rigidbody2D>().gravityScale = 1f;
            playerAnimation.speed = 1;
        }
    }

    public IEnumerator CanFall()     // reseteaza booleanul care permite miscarea pe y dupa 0.1 S
    {
        yield return new WaitForSeconds(0.1f);
        canFall = true;
        startCorutine = false;
    }

    void OnDrawGizmosSelected()
    {
        // Draw a yellow sphere at the transform's position
        Gizmos.color = Color.yellow;
        //Gizmos.DrawWireSphere(onGroundZeroDistCheckPoint2.position, onGroundZeroDistCheckRadius2);
        Gizmos.DrawWireSphere(onGroundZeroDistCheckPoint.position, onGroundZeroDistCheckRadius);
    }

    // Start is called before the first frame update
    void Start()
    {
        rigidBody = GetComponent<Rigidbody2D>();
        playerAnimation = GetComponent<Animator>();
        Physics2D.IgnoreLayerCollision(9, 9);
        shieldUp = false;
        sideShield.GetComponent<CapsuleCollider2D>().gameObject.SetActive(true);        // initial scutul e jos, deci activam scutul doar pe lateral
        upShield.GetComponent<CapsuleCollider2D>().gameObject.SetActive(false);

        cc = GetComponent<CapsuleCollider2D>();
        colliderSize = cc.size;
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
        /////////////

        //Debug.Log(Input.GetAxis("Horizontal"));
        if (activePlayer)
        {
            if (Mathf.Approximately(Input.GetAxis("Horizontal"), 0f) && PlayerPrefs.GetInt("OlafgHit") == 0) /*&& isOnSlope)*/      // daca stam pe loc, nu vrem sa alunece
            {
                rigidBody.sharedMaterial = fullFriction;
                cc.sharedMaterial = fullFriction;
                //rigidBody.sharedMaterial.friction = 10000;
                //cc.sharedMaterial.friction = 10000;
            }
            else
            {
                rigidBody.sharedMaterial = noFriction;
                cc.sharedMaterial = noFriction;
                //rigidBody.sharedMaterial.friction = 0;
            }
        }
        else
        {
            if (PlayerPrefs.GetInt("OlafHit") == 0) /*&& isOnSlope)*/      // daca stam pe loc, nu vrem sa alunece
            {
                rigidBody.sharedMaterial = fullFriction;
                cc.sharedMaterial = fullFriction;
                //rigidBody.sharedMaterial.friction = 10000;
                //cc.sharedMaterial.friction = 10000;
            }
            else
            {
                rigidBody.sharedMaterial = noFriction;
                cc.sharedMaterial = noFriction;
                //rigidBody.sharedMaterial.friction = 0;
            }
        }

    }


    public void UpdateLives(int extraLives)
    {
        lives += extraLives;    // actualizeaza numarul de vieti
        if (lives > 3)          // daca e mai mult de 3, este adus la maximul de 3
            lives = 3;

        LifeCount newLives = GetComponent<LifeCount>();     // se actualizeaza si imaginile cu inimioare
        newLives.RestoreHearts(extraLives);
    }

    //Update is called once per frame
    void Update()
    {
        isTouchingGround = Physics2D.OverlapCircle(groundCheckPoint.position, groundCheckRadius, groundLayer);
        onGroundZeroDist = Physics2D.OverlapCircle(onGroundZeroDistCheckPoint.position, onGroundZeroDistCheckRadius, groundLayer); // Physics2D.OverlapCircle(onGroundZeroDistCheckPoint2.position, onGroundZeroDistCheckRadius2, groundLayer) || 
        //onGroundZeroDist = Physics2D.OverlapCircle(onGroundZeroDistCheckPoint.position, onGroundZeroDistCheckRadius, groundLayer);

        if (startCorutine)    // daca nu sare si ajunge la capatul pantei, atunci putem sa initiem corutina
        {
            StartCoroutine(CanFall());
        }

        SplopeCheck();
        if (Input.GetKeyDown(KeyCode.Space) && activePlayer)    // tasta de coborat si urcat scutul, iar caracterul trebuie sa fie activ
        {
            shieldUp = !shieldUp;
            playerAnimation.SetBool("ShieldUp", shieldUp);
            if (shieldUp)       // daca scutul e sus, dezactivez scutul pe margine si il activez sus
            {
                sideShield.GetComponent<CapsuleCollider2D>().gameObject.SetActive(false);
                upShield.GetComponent<CapsuleCollider2D>().gameObject.SetActive(true);
            }
            else
            {
                sideShield.GetComponent<CapsuleCollider2D>().gameObject.SetActive(true);
                upShield.GetComponent<CapsuleCollider2D>().gameObject.SetActive(false);
            }
        }

        if (!isTouchingGround && isTouchingLadder)      // daca e pe scara, nu e protejat, pentru canu are cu sa tina scutul
        {
            sideShield.GetComponent<CapsuleCollider2D>().gameObject.SetActive(false);
            upShield.GetComponent<CapsuleCollider2D>().gameObject.SetActive(false);
            shieldUpdate = true;        // daca am deazctivat scuturile, e true, iar daca e true cand nu mai e pe scara, se reactiveaza
        }
        else if (shieldUpdate)
        {
            if (shieldUp)       // daca scutul e sus, dezactivez scutul pe margine si il activez sus
            {
                sideShield.GetComponent<CapsuleCollider2D>().gameObject.SetActive(false);
                upShield.GetComponent<CapsuleCollider2D>().gameObject.SetActive(true);
            }
            else
            {
                sideShield.GetComponent<CapsuleCollider2D>().gameObject.SetActive(true);
                upShield.GetComponent<CapsuleCollider2D>().gameObject.SetActive(false);
            }
            shieldUpdate = false;
        }

        if (PlayerPrefs.GetInt("OlafHit") == 0 && activePlayer && lives > 0)     // daca nu este lovit de inamic si este jucator activ si traieste
        {
            MovementAndAnimationUpdate();
        }
        else if (PlayerPrefs.GetInt("OlafHit") == 1 /*wasHit*/ && lives > 0) // daca e lovit de inamic, dar nu e mort
        {
            HitScenario();
        }
        else if (lives <= 0)        // daca e mort
        {
            playerAnimation.SetTrigger("IsDeadOlaf");
            rigidBody.velocity = new Vector2(0, rigidBody.velocity.y);       // daca nu e activ si sta pe loc, il tinem pe loc, sa nu alunece de la fizica din Unity
            if (timeToRespawn < 0)
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            else
                timeToRespawn -= Time.deltaTime;
        }
        else if (!activePlayer) // daca e viu, dar nu este activ
        {
            playerAnimation.SetFloat("Speed", -1);       // daca jucatorul e inactiv, atunci clar viteza va fi 0
            // luam fiecare caz pe rand si actulizam animatiile corespunzatoare
            if (isTouchingGround && isTouchingLadder)
            {
                playerAnimation.SetBool("OnGround", true);
                playerAnimation.SetBool("Stairs", false);
                this.GetComponent<Rigidbody2D>().gravityScale = 1f;
                rigidBody.velocity = new Vector2(0, rigidBody.velocity.y);   // daca nu e activ si sta pe loc, il tinem pe loc, sa nu alunece de la fizica din Unity
                playerAnimation.speed = 1;
            }
            else if (isTouchingGround && !isTouchingLadder)
            {
                playerAnimation.SetBool("OnGround", true);
                playerAnimation.SetBool("Stairs", false);
                this.GetComponent<Rigidbody2D>().gravityScale = 1f;
                rigidBody.velocity = new Vector2(0, rigidBody.velocity.y);   // daca nu e activ si sta pe loc, il tinem pe loc, sa nu alunece de la fizica din Unity
                playerAnimation.speed = 1;
            }
            else if (!isTouchingGround && isTouchingLadder)
            {
                playerAnimation.SetBool("OnGround", true);      // facem tranzitia de la sarit la stat pe scara
                playerAnimation.SetBool("Stairs", true);
                this.GetComponent<Rigidbody2D>().gravityScale = 0f;
                rigidBody.velocity = new Vector2(0, 0);             // 0 si la Y ca sa nu urce singur, sau sa coboare
                playerAnimation.speed = 0;                          // oprim animatia cand nu activ
            }
            else if (!isTouchingGround && !isTouchingLadder)
            {
                playerAnimation.SetBool("OnGround", false);
                playerAnimation.SetBool("Stairs", false);
                this.GetComponent<Rigidbody2D>().gravityScale = 1f;
                if (shieldUp)   // daca are scutul sus, va plana verical, alfel facadea liber
                    rigidBody.velocity = new Vector2(0, -2.2f);   
                else
                    rigidBody.velocity = new Vector2(0, rigidBody.velocity.y);

                playerAnimation.speed = 1;
            }
        }
    }
}
