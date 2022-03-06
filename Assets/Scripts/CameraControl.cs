using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Camera va urmari incet jucatorul si va avea un deplasament care va face facilita vizibilitatea
 * in directia de mers. Tranzitia se va face lin
 */

public class CameraControl : MonoBehaviour
{
    public GameObject player;      //jucatorul pe care il urmareste
    private float offset = 5f;       // deplasamentul camerei
    private Vector3 cameraPosition; // pozitia camerei fata de jucator
    private float offsetTiming = 2f;     // timpul in care se va face tranzitia de deplasament la schimbarea directiei de mers

    public enum ActivePlayer
    {
        Erik,
        Baleog,
        Olaf
    }
    public ActivePlayer activePlayer;
    public PlayerChange playerChange;   // va fi instanta din clasa PlayerChange, pentru a putea apela getterul 

    // Start is called before the first frame update
    /*Dorim sa obtinem jucatorul curent pentru a-i da coordonatele ca si paramentrii pentru camera
     */
    void Start()
    {
        playerChange = GameObject.FindObjectOfType<PlayerChange>(); 
        activePlayer = (ActivePlayer)playerChange.GetActivePlayer();
    }

    // Update is called once per frame
    void Update()
    {
        activePlayer = (ActivePlayer)playerChange.GetActivePlayer();
        if (activePlayer == ActivePlayer.Erik)
            player = GameObject.Find("Erik");
        else if (activePlayer == ActivePlayer.Baleog)
            player = GameObject.Find("Baleog");
        else
            player = GameObject.Find("Olaf");
        cameraPosition = new Vector3(player.transform.position.x, player.transform.position.y, transform.position.z);
        // va lua coord de pe x a jucatorului si coord y, z a camerei
        if (player.transform.localScale.x > 0f)   //daca jucatorul e orientat spre dreapta
        {
            cameraPosition = new Vector3(cameraPosition.x + offset, player.transform.position.y, cameraPosition.z);
        }
        else 
        {
            cameraPosition = new Vector3(cameraPosition.x - offset, player.transform.position.y, cameraPosition.z);
        }
        transform.position = Vector3.Lerp(transform.position, cameraPosition, offsetTiming * Time.deltaTime);
        // deltaTime va face tranzitia lina, chiar daca calculatorul nu este performant
        // transform reprezinta coordonatele obiectului din joc, fata de sistemul de coordonate al jocului
        //obiectul in cazula cesta va fi camera
    }
}
