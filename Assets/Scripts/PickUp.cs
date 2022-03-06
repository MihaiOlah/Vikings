using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUp : MonoBehaviour
{
    private Inventory inventoryErik;        // comunica cu inventarul lui Erik
    private Inventory inventoryBaleog;        // comunica cu inventarul lui Baleog
    private Inventory inventoryOlaf;        // comunica cu inventarul lui Olaf

    public GameObject itemButton;       // va face sa apara imaginea in inventar si sa fie utilizabil


    // Start is called before the first frame update
    void Start()
    {
        inventoryErik = GameObject.FindGameObjectWithTag("Erik").GetComponent<Inventory>();
        inventoryBaleog = GameObject.FindGameObjectWithTag("Baleog").GetComponent<Inventory>();
        inventoryOlaf = GameObject.FindGameObjectWithTag("Olaf").GetComponent<Inventory>();
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.CompareTag("Erik"))
        {
            for (int i = 0; i < inventoryErik.slots.Length; i++)        // verificam daca inventarul are loc
            {
                if (inventoryErik.isFull[i] == false)       // daca nu e plim, mai incape si il setatm ca e ocupat slotul
                {
                    inventoryErik.isFull[i] = true;     // setam ca locul e acum ocupat
                    Instantiate(itemButton, inventoryErik.slots[i].transform, false);      //va spawna imaginea la coordonatele parintelui
                    //itemButton.tag = "ErikItem";    // asignam fiecarui item tag-ul de cine apartine
                    //itemButton.transform.position = new Vector3(inventoryErik.slots[i].transform.position.x, inventoryErik.slots[i].transform.position.y, 4);
                    //itemButton.transform.localPosition = new Vector3(0, 0, 0);
                    Destroy(gameObject);
                    break;
                }
            }
        }
        else if (collider.CompareTag("Baleog"))
        {
            for (int i = 0; i < inventoryBaleog.slots.Length; i++)        // verificam daca inventarul are loc
            {
                if (inventoryBaleog.isFull[i] == false)       // daca nu e plin, mai incape si il setam ca e ocupat slotul
                {
                    inventoryBaleog.isFull[i] = true;     // setam ca locul e acum ocupat
                    Instantiate(itemButton, inventoryBaleog.slots[i].transform, false);      //va spawna imaginea la coordonatele parintelui
                    //itemButton.tag = "BaleogItem";      // asignam fiecarui item tag-ul de cine apartine
                    Destroy(gameObject);
                    break;
                }
            }
        }
        else if (collider.CompareTag("Olaf"))
        {
            for (int i = 0; i < inventoryOlaf.slots.Length; i++)        // verificam daca inventarul are loc
            {
                if (inventoryOlaf.isFull[i] == false)       // daca nu e plim, mai incape si il setatm ca e ocupat slotul
                {
                    inventoryOlaf.isFull[i] = true;     // setam ca locul e acum ocupat
                    Instantiate(itemButton, inventoryOlaf.slots[i].transform, false);      //va spawna imaginea la coordonatele parintelui
                    //itemButton.tag = "OlafItem";    // asignam fiecarui item tag-ul de cine apartine
                    Destroy(gameObject);
                    break;
                }
            }
        }
    }
}
