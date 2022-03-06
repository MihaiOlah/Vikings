using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Key : MonoBehaviour
{
    public bool wasClicked;         // verifica daca cheia a fost apasata
    //public string keyColor;         // retine culoarea cheii

    private IEnumerator NotInLockerRange()     // va seta cheia din nou pe fals daca nu se descuie cheia in 0.1 secunde, altfel s-ar deschide automat cand apasam click, chiar daca suntem departe
    {
        yield return new WaitForSeconds(0.1f);
        wasClicked = false;
    }

    public void Use()
    {
        wasClicked = true;
        StartCoroutine(NotInLockerRange());
    }
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
