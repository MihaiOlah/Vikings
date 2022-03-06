using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// scriptul ne permite sa aruncam itemele din inventarul lui Erik

public class SlotErik : MonoBehaviour
{
    private Inventory inventory;
    public int slotIndex;      // index-ul slotului din care eliberam item-ul si il initializam din inspector

    // Start is called before the first frame update
    void Start()
    {
        inventory = GameObject.FindGameObjectWithTag("Erik").GetComponent<Inventory>();
    }

    public void DropObject()
    {
        foreach(Transform child in transform)       // iteram prin copiii slotului, care sunt elementele, si le stergem, in cazul asta va rula doar o data foreach-ul
        {
            child.GetComponent<ItemSpawn>().SpawnRemovedItem("Erik", GameObject.FindGameObjectWithTag("Erik").transform.localScale.x);       // apelam metoda de drop cu tag-ul Erik pentru itemul sters
            GameObject.Destroy(child.gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.childCount <= 0)      // daca nu are niciun copil, inseamna ca slotul e gol si marcam asta in inventarul global
        {
            inventory.isFull[slotIndex] = false;
        }
    }
}
