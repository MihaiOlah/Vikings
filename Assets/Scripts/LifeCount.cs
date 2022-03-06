using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LifeCount : MonoBehaviour
{
    public Image[] lives;
    private int livesRemaining;
    private Animator playerAnimation;
    private float timer;
    [HideInInspector] public bool wasHit;

    private ErikControls erik;
    private BaleogControls baleog;
    private OlafControls olaf;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //hitRight tine minte daca jucatorul a atins inamicul din partea dreapta (dreapta inamicului), iar left similar
        PlayerPrefs.SetInt("hitRight", 0);  //punem 0 la fieare coliziune pentru a nu ramane 1
        PlayerPrefs.SetInt("hitLeft", 0);
        if (collision.tag == "Enemy")
        {
            // calculeaza in funtie de x, care obiect este in fata caruia
            if ((this.transform.position.x - collision.transform.position.x) < 0)   
            {
                PlayerPrefs.SetInt("hitLeft", 1);
            }
            else if ((this.transform.position.x - collision.transform.position.x) > 0)
            {
                PlayerPrefs.SetInt("hitRight", 1);
            }
        }
        if (collision.tag.Equals("Electricity"))
        {
            livesRemaining = 0;
            //PlayerPrefs.SetInt("HasDied", 1);
            for (int i = 0; i < lives.Length; i++)
            {
                lives[i].enabled = false;
            }
        }
        if (collision.tag == "Enemy" && livesRemaining != 0)   // daca loveste un obiect cu tag-ul "Enemy", atunci pierde o viata si vietile diferite de 0 ca sa nu iasa din vectorul lives[] cu indici negativi
        {
            lives[--livesRemaining].enabled = false;
            playerAnimation.SetBool("IsHit", true);
            timer = 1f;
            wasHit = true;
            if (tag == "Erik")
            {
                PlayerPrefs.SetInt("ErikHit", 1);
            }
            if (tag == "Baleog")
            {
                PlayerPrefs.SetInt("BaleogHit", 1);
            }
            if (tag == "Olaf")
            {
                PlayerPrefs.SetInt("OlafHit", 1);
            }
        }
        //de completat daca e mancare
    }

    public void RestoreHearts(int hearts)       // actualizam imaginile cu inimioare
    {
        while (hearts != 0 && livesRemaining < 3)       // atata timp cat nu se depasesc 3 vieti si numarul de vieti care trebuie adaugate inca nu s-a adaugat, repunem o inima
        {
            lives[livesRemaining++].enabled = true;
            hearts--;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        livesRemaining = 3;
        playerAnimation = GetComponent<Animator>();
        wasHit = false;
        erik = FindObjectOfType<ErikControls>();
        baleog = FindObjectOfType<BaleogControls>();
        olaf = FindObjectOfType<OlafControls>();
        PlayerPrefs.SetInt("ErikHit", 0);
        PlayerPrefs.SetInt("BaleogHit", 0);
        PlayerPrefs.SetInt("OlafHit", 0);
        PlayerPrefs.SetInt("HasDied", 0);
        //Physics2D.IgnoreLayerCollision(9, 12, false);         de testat fara
    }

    // Update is called once per frame
    void Update()
    {
        if(livesRemaining == 0)
        {
            PlayerPrefs.SetInt("HasDied", 1);
            //Physics2D.IgnoreLayerCollision(9, 12, true);
            //SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
        if (wasHit)
        {
            timer -= Time.deltaTime;
            if (timer < 0)
            {
                playerAnimation.SetBool("IsHit", false);
                wasHit = false;
                if (PlayerPrefs.GetInt("ErikHit") == 1)
                    PlayerPrefs.SetInt("ErikHit", 0);
                if (PlayerPrefs.GetInt("BaleogHit") == 1)
                    PlayerPrefs.SetInt("BaleogHit", 0);
                if (PlayerPrefs.GetInt("OlafHit") == 1)
                    PlayerPrefs.SetInt("OlafHit", 0);
            }
        }
    }
}
