using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElectricityButton : MonoBehaviour
{
    public GameObject electricBarrier;      // retine curentul pe care il opreste
    private Animator leverAnimation;

    // retine care dintre personaje este in apropierea butonului
    private bool playerNearbyErik;          
    private bool playerNearbyBaleog;          
    private bool playerNearbyOlaf;          

    // retine instanta de personaje, pentru a verifica daca sunt active in momentul apasarii tastei R
    private ErikControls erik;
    private BaleogControls baleog;
    private OlafControls olaf;

    private void BarrierDestruction(float x)       // x e coordonata obiectului care interactioneaza cu manerul
    {
        // verifica din ce parte a fost lovit ca sa se deruleze animatia corespunzatoare
        if (transform.position.x < x)
        {
            leverAnimation.SetTrigger("ToLeft");
        }
        else
        {
            leverAnimation.SetTrigger("ToRight");
        }
        Destroy(electricBarrier.gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag.Equals("Arrow"))  // daca e lovit cu sageata, se da si coordonata sagetii pentru a indrepta manerul in directia de mers a sagetii
        {
            BarrierDestruction(collision.transform.position.x);
        }
        // verificare care personaj atinge manerul
        if (collision.tag.Equals("Erik"))
            playerNearbyErik = true;
        else if (collision.tag.Equals("Baleog"))
            playerNearbyBaleog = true;
        else if (collision.tag.Equals("Olaf"))
            playerNearbyOlaf = true;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        // daca pleaca de langa maner, e setata ca nu mai e in proximitetea acestuia
        if (collision.tag.Equals("Erik"))
            playerNearbyErik = false;
        else if (collision.tag.Equals("Baleog"))
            playerNearbyBaleog = false;
        else if (collision.tag.Equals("Olaf"))
            playerNearbyOlaf = false;
    }

    // Start is called before the first frame update
    void Start()
    {
        leverAnimation = GetComponent<Animator>();
        erik = FindObjectOfType<ErikControls>();
        baleog = FindObjectOfType<BaleogControls>();
        olaf = FindObjectOfType<OlafControls>();
    }

    // Update is called once per frame
    void Update()
    {
        // daca este apasata tasta R si jucatorul din apropiere e si cel activ, atunci se opreste curentul
        if (Input.GetKeyDown(KeyCode.R) && (erik.activePlayer && playerNearbyErik || baleog.activePlayer && playerNearbyBaleog || olaf.activePlayer && playerNearbyOlaf))
            BarrierDestruction(0);
    }
}
