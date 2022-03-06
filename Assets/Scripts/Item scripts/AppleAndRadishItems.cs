using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AppleAndRadishItems : MonoBehaviour
{
    public GameObject effect;
    private Transform player;

    // Start is called before the first frame update
    void Start()
    {
        //player = GameObject.FindGameObjectWithTag("Erik").transform;
    }

    public void Use()
    {
        if (transform.parent.tag.Equals("ErikItem"))
        {
            player = GameObject.FindGameObjectWithTag("Erik").transform;
            Instantiate(effect, player.position, Quaternion.identity);      // ca sa nu se spawneze cu rotatie
            ErikControls erik = FindObjectOfType<ErikControls>();
            erik.UpdateLives(1);
        }
        else if (transform.parent.tag.Equals("BaleogItem"))
        {
            player = GameObject.FindGameObjectWithTag("Baleog").transform;
            Instantiate(effect, player.position, Quaternion.identity);      // ca sa nu se spawneze cu rotatie
            BaleogControls baleog = FindObjectOfType<BaleogControls>();
            baleog.UpdateLives(1);
        }
        else if (transform.parent.tag.Equals("OlafItem"))
        {
            player = GameObject.FindGameObjectWithTag("Olaf").transform;
            Instantiate(effect, player.position, Quaternion.identity);      // ca sa nu se spawneze cu rotatie
            OlafControls olaf = FindObjectOfType<OlafControls>();
            olaf.UpdateLives(1);
        }
        Destroy(gameObject);
    }
}
