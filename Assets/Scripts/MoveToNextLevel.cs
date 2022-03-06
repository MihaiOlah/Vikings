using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MoveToNextLevel : MonoBehaviour
{
    public int nextSceneLoad;   // retine indexul urmatorului nivel
    private bool erik;
    private bool baleog;
    private bool olaf;

    // Start is called before the first frame update
    void Start()
    {
        nextSceneLoad = SceneManager.GetActiveScene().buildIndex + 1;   //calculeaza indexul nivelului urmator
        erik = false;
        baleog = false;
        olaf = false;
    }


    public void NextLevelPrecondition(Collider2D collision)
    {
        if (collision.gameObject.tag == "Erik")
        {
            erik = true;
            Debug.Log("Eik");
        }
        if (collision.gameObject.tag == "Baleog")
        {
            baleog = true;
            Debug.Log("Baleog");
        }
        if (collision.gameObject.tag == "Olaf")
        {
            olaf = true ;
            Debug.Log("olaf");
        }
    }
    public void OnTriggerEnter2D(Collider2D collision)
    {
        NextLevelPrecondition(collision);
         //Debug.Log(SceneManager.GetActiveScene().buildIndex);
        if (erik && baleog && olaf)
        {
            if (SceneManager.GetActiveScene().buildIndex == 11)  // daca ajungem la ultima scena, comparam cu indicele din Build
            {
                // AI CASTIGAT - de completat
                //PlayerPrefs.SetInt("levelAt", 2);
                SceneManager.LoadScene(12);     // scena de final
                Debug.Log("Ura");
            }
            else
            {
                //trecem la nivelul urmator
                SceneManager.LoadScene(nextSceneLoad);

                //salvam nivelul la care urmeaza sa fim
                if (nextSceneLoad > PlayerPrefs.GetInt("levelAt"))
                {
                    PlayerPrefs.SetInt("levelAt", nextSceneLoad);
                }
            }
        }
    }

    public void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Erik")
            erik = false;
        if (collision.tag == "Baleog")
            baleog = false;
        if (collision.tag == "Olaf")
            olaf = false;
    }
    private void Update()
    {
        
    }
}
