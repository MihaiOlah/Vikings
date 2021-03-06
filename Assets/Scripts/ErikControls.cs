using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class ErikControls : MonoBehaviour
{

    [HideInInspector] public bool activePlayer;

    private float speed = 7f;
    private float speedY = 4f;
    private float jumpSpeed = 8f;
    private Rigidbody2D rigidBody;
    public Transform groundCheckPoint;      // verifica daca caracterul e pe pamant
    public float groundCheckRadius;         // raza pana la care e considerat ca se poate sari
    public LayerMask groundLayer;           // lista de obiecte de pe care se poate sari
    bool isTouchingGround;
    private Animator playerAnimation;

    private float timeToRespawn = 0.7f;
    private int lives = 3;      // are 3 vieti
    private bool isTouchingLadder;

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
    private bool isJumping;

    private bool onGroundZeroDist;
    public Transform onGroundZeroDistCheckPoint;      // verifica daca caracterul e pe pamant
    public float onGroundZeroDistCheckRadius;         // raza pana la care e considerat ca se poate sari
    public Transform onGroundZeroDistCheckPoint2;      // verifica daca caracterul e pe pamant
    public float onGroundZeroDistCheckRadius2;         // raza pana la care e considerat ca se poate sari

    private bool canFall;
    private bool startCorutine;

    private bool endOfTheLadder;

    // Start is called before the first frame update
    void Start()
    {
        Physics2D.IgnoreLayerCollision(9, 10, true);
        rigidBody = GetComponent<Rigidbody2D>();
        playerAnimation = GetComponent<Animator>();
        Physics2D.IgnoreLayerCollision(9, 9, true);
        Physics2D.IgnoreLayerCollision(9, 11, true);

        cc = GetComponent<CapsuleCollider2D>();
        colliderSize = cc.size;
    }
    public void UpdateSpeed(float percentage)
    {
        speed += percentage * speed / 100;
        speedY += percentage * speedY / 100;
    }

    public void DecrementLives(int hitPoints)
    {
        lives -= hitPoints;
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
            if (Mathf.Approximately(Input.GetAxis("Horizontal"), 0f) && PlayerPrefs.GetInt("ErikHit") == 0) /*&& isOnSlope)*/      // daca stam pe loc, nu vrem sa alunece
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
            if (PlayerPrefs.GetInt("ErikHit") == 0) /*&& isOnSlope)*/      // daca stam pe loc, nu vrem sa alunece
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

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag.Equals("Ladder"))
            isTouchingLadder = true;
        if (collision.tag.Equals("Enemy"))
            lives--;
        if (collision.tag.Equals("Electricity"))
            lives = 0;
        if (collision.tag.Equals("EndOfLadder"))
            endOfTheLadder = true;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag.Equals("Ladder"))
            isTouchingLadder = false;
        if (collision.tag.Equals("EndOfLadder"))
            endOfTheLadder = false;
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
        playerAnimation.SetFloat("Speed", Mathf.Abs(rigidBody.velocity.x));     // actualizam animatia daca sta, sau daca merge

        /*
        if (isTouchingGround && !isOnSlope && !isJumping)
        {
            Debug.Log("da");
            rigidBody.velocity = new Vector2(movement * speed, 0.0f);
        }
        else if (isTouchingGround && isOnSlope && !isJumping)
        {
            rigidBody.velocity = new Vector2((-movement) * speed * slopeNormalPerp.x, speed * slopeNormalPerp.y * (-movement));     // punem - din cauza ca trebuie compensat de cum e determianta normala
        }
        else if (!isTouchingGround)
        {
            rigidBody.velocity = new Vector2(movement * speed, rigidBody.velocity.y);
        }
        */

        
        if (onGroundZeroDist && !isOnSlope && !isJumping && !endOfTheLadder)
        {
            canFall = false;
            startCorutine = false;
            rigidBody.velocity = new Vector2(movement * speed, 0.0f);
        }
        else if (onGroundZeroDist && isOnSlope && !isJumping)
        {
            canFall = false;
            startCorutine = false;
            rigidBody.velocity = new Vector2((-movement) * speed * slopeNormalPerp.x, speed * slopeNormalPerp.y * (-movement));     // punem - din cauza ca trebuie compensat de cum e determianta normala
        }
        else if (!onGroundZeroDist)
        {
            startCorutine = true;
            if (isJumping)  // daca sare, atunci se poate misca sus-jos, altfel se va verifica daca este in capatul unei pante
            {
                rigidBody.velocity = new Vector2(movement * speed, rigidBody.velocity.y);
            }
            else
            {
                if (!canFall)       // daca este in capatul pantei , mai e tinuta miscarea pe y 0 timp de 0.1 S, dupa care se poate misca in voie
                    rigidBody.velocity = new Vector2(movement * speed, 0f);
                else
                    rigidBody.velocity = new Vector2(movement * speed, rigidBody.velocity.y);
            }
        }
    }

    public IEnumerator CanFall()     // reseteaza booleanul care permite miscarea pe y dupa 0.1 S
    {
        yield return new WaitForSeconds(0.1f);
        canFall = true;
        startCorutine = false;
    }

    private void Jump()
    {
        if (!isTouchingLadder && Input.GetButtonDown("Jump"))
        {
            rigidBody.velocity = new Vector2(rigidBody.velocity.x, jumpSpeed);
            isJumping = true;
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
            Jump();
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
                Jump();
                MovementAxisY();
            }
        }
        else if (!isTouchingGround && isTouchingLadder)     // daca e pe scara
        {
            MovementAxisX();
            MovementAxisY();
            playerAnimation.SetBool("Stairs", true);
            playerAnimation.SetBool("OnGround", true);      // putem onground false, ca sa nu se blocheze in animatia de sarit in aer si pe scara
        }
        else if (!isTouchingGround && !isTouchingLadder)         // daca nu e nici pe scara, nici pe pamant, atunci e in cadere
        {
            playerAnimation.SetBool("OnGround", false);
            MovementAxisX();
            this.GetComponent<Rigidbody2D>().gravityScale = 1f;
            playerAnimation.speed = 1;
        }
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

    public void UpdateLives(int extraLives)
    {
        lives += extraLives;    // actualizeaza numarul de vieti
        if (lives > 3)          // daca e mai mult de 3, este adus la maximul de 3
            lives = 3;

        LifeCount newLives = GetComponent<LifeCount>();     // se actualizeaza si imaginile cu inimioare
        newLives.RestoreHearts(extraLives);
    }

    void OnDrawGizmosSelected()
    {
        // Draw a yellow sphere at the transform's position
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(onGroundZeroDistCheckPoint2.position, onGroundZeroDistCheckRadius2);
        Gizmos.DrawWireSphere(onGroundZeroDistCheckPoint.position, onGroundZeroDistCheckRadius);
    }

    //Update is called once per frame
    private void Update()
    {
        isTouchingGround = Physics2D.OverlapCircle(groundCheckPoint.position, groundCheckRadius, groundLayer);
        onGroundZeroDist = Physics2D.OverlapCircle(onGroundZeroDistCheckPoint2.position, onGroundZeroDistCheckRadius2, groundLayer) || Physics2D.OverlapCircle(onGroundZeroDistCheckPoint.position, onGroundZeroDistCheckRadius, groundLayer); 
        //onGroundZeroDist = Physics2D.OverlapCircle(onGroundZeroDistCheckPoint.position, onGroundZeroDistCheckRadius, groundLayer);
        
        if (startCorutine && !isJumping)    // daca nu sare si ajunge la capatul pantei, atunci putem sa initiem corutina
        {
            StartCoroutine(CanFall());
        }
        
        SplopeCheck();
        if (rigidBody.velocity.y <= 0.0f)
            isJumping = false;
        //Debug.Log(Physics2D.OverlapCircle(groundCheckPoint.position, groundCheckRadius, groundLayer));
        if (PlayerPrefs.GetInt("ErikHit") == 0 && activePlayer && lives > 0)     // daca nu este lovit de inamic si este jucator activ si traieste
        {
            MovementAndAnimationUpdate();
        }
        else if (PlayerPrefs.GetInt("ErikHit") == 1 && lives > 0) // daca e lovit de inamic, dar nu e mort
        {
            playerAnimation.SetBool("IsHit", true);
            HitScenario();
        }
        else if (lives <= 0)        // daca e mort
        {
            playerAnimation.SetTrigger("IsDeadErik");
            rigidBody.velocity = new Vector2(0, rigidBody.velocity.y);       // daca nu e activ si sta pe loc, il tinem pe loc, sa nu alunece de la fizica din Unity
            if (timeToRespawn < 0)
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            else
                timeToRespawn -= Time.deltaTime;
        }
        else if (!activePlayer) // daca e viu, dar nu este activ
        {
            playerAnimation.SetFloat("Speed", 0);       // daca jucatorul e inactiv, atunci clar viteza va fi 0
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
                playerAnimation.SetBool("Stairs", true);            ///
                this.GetComponent<Rigidbody2D>().gravityScale = 0f;
                rigidBody.velocity = new Vector2(0, 0);             // 0 si la Y ca sa nu urce singur, sau sa coboare
                playerAnimation.speed = 0;                          // oprim animatia cand nu activ
            }
            else if (!isTouchingGround && !isTouchingLadder)
            {
                playerAnimation.SetBool("OnGround", false);
                playerAnimation.SetBool("Stairs", false);
                this.GetComponent<Rigidbody2D>().gravityScale = 1f;
                rigidBody.velocity = new Vector2(0, rigidBody.velocity.y);   // daca nu e activ si sta pe loc, il tinem pe loc, sa nu alunece de la fizica din Unity
                playerAnimation.speed = 1;
            }
            //rigidBody.velocity = new Vector2(0, rigidBody.velocity.y);   // daca nu e activ si sta pe loc, il tinem pe loc, sa nu alunece de la fizica din Unity
        }
    }
}
