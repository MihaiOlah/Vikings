using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// avem tag-urile green, yellow si white pentru legatura cheie-incuietoare
public class KeyLock : MonoBehaviour
{
    private Inventory inventoryErik;                        // comunica cu inventarul lui Erik
    private Inventory inventoryBaleog;                      // comunica cu inventarul lui Baleog
    private Inventory inventoryOlaf;                        // comunica cu inventarul lui Olaf

    private Transform keyOfLockType;                      // retine butonul care e de tipul incuietorii
    private bool []playerInRange = new bool[3];             // verifica daca jucatorul este langa incuietoare, pos: 0-Erik, 1-Baleog, 2-Olaf

    public bool unlocked;                                   // verifica daca incuietoarea a fost descuiata

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // cautam cu care jucator interactioneaza incuietoarea
        if (collision.tag.Equals("Erik"))
        {
            playerInRange[0] = true;
            GameObject[] tmp = GameObject.FindGameObjectsWithTag("ErikItem");       // facem un vector cu toate obiectele din inventarul jucatorului tinta
            for (int i = 0; i < tmp.Length; i++)                 // iteram prin copiii slotului, care sunt elementele, si le stergem, in cazul asta va rula doar o data foreach-ul
            {
                foreach(Transform child in tmp[i].transform)    // iteram prin elemente si verificam daca exista cheia corespunzatoare incuietorii
                {
                    if (child.tag.Equals(transform.tag))
                    {
                        keyOfLockType = child;
                        break;
                    }
                }
            }
        }
        else if (collision.tag.Equals("Baleog"))
        {
            playerInRange[1] = true;
            GameObject[] tmp = GameObject.FindGameObjectsWithTag("BaleogItem");       // facem un vector cu toate obiectele din inventarul jucatorului tinta
            for (int i = 0; i < tmp.Length; i++)            // iteram prin copiii slotului, care sunt elementele, si le stergem, in cazul asta va rula doar o data foreach-ul
            {
                foreach (Transform child in tmp[i].transform)    // iteram prin elemente si verificam daca exista cheia corespunzatoare incuietorii
                {
                    if (child.tag.Equals(transform.tag))
                    {
                        keyOfLockType = child;
                        break;
                    }
                }
            }
        }
        else if (collision.tag.Equals("Olaf"))
        {
            playerInRange[2] = true;
            GameObject[] tmp = GameObject.FindGameObjectsWithTag("OlafItem");       // facem un vector cu toate obiectele din inventarul jucatorului tinta
            for (int i = 0; i < tmp.Length; i++)       // iteram prin copiii slotului, care sunt elementele, si le stergem, in cazul asta va rula doar o data foreach-ul
            {
                foreach (Transform child in tmp[i].transform)    // iteram prin elemente si verificam daca exista cheia corespunzatoare incuietorii
                {
                    if (child.tag.Equals(transform.tag))
                    {
                        keyOfLockType = child;
                        break;
                    }
                }
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        
        if (collision.tag.Equals("Erik"))
        {
            playerInRange[0] = false;
        }
        else if (collision.tag.Equals("Baleog"))
        {
            playerInRange[1] = false;
        }
        else if (collision.tag.Equals("Olaf"))
        {
            playerInRange[2] = false;
        }
        
    }

    // Start is called before the first frame update
    void Start()
    {
        inventoryErik = GameObject.FindGameObjectWithTag("Erik").GetComponent<Inventory>();
        inventoryBaleog = GameObject.FindGameObjectWithTag("Baleog").GetComponent<Inventory>();
        inventoryOlaf = GameObject.FindGameObjectWithTag("Olaf").GetComponent<Inventory>();
    }

    // Update is called once per frame
    void Update()
    {
        // verificam daca caracterul cu cheia este langa incuietoare si are cheia si a apasat click sa o foloseasca
        if (keyOfLockType != null && keyOfLockType.gameObject.GetComponent<Key>().wasClicked)
        {
            if (keyOfLockType.parent.tag.Equals("ErikItem") && playerInRange[0] || keyOfLockType.parent.tag.Equals("BaleogItem") && playerInRange[1] 
                || keyOfLockType.parent.tag.Equals("OlafItem") && playerInRange[2])      // verificam daca Erik este langa incuietoare si are are cheia, analog pentru restul
            {
                unlocked = true;
                Destroy(keyOfLockType.gameObject);
            }
        }
    }
}
