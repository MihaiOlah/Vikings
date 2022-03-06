using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//item-ele se vor adauga ca si copii ai slotului (se poate vedea in inspector cand rulam in scene)
public class Inventory : MonoBehaviour
{
    public bool[] isFull;   // verifica daca e plin inventarul, sau slotul e liber
    public GameObject[] slots;      // tine ordinea obiectelor din inventar si ne lasa sa le folosim
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
