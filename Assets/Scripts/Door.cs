using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    public Transform []locks;               // retine incuietorile necesare pentru a deschide o usa (pot sa fie oricate)
    private int openedLocks;                // numara cate incuietori sunt deschise
    private float firstLockX;               // retine coordonata primei incuietori ca sa stim in ce directie deschidem usa

    private Animator doorAnimation;

    // Start is called before the first frame update
    void Start()
    {
        openedLocks = 0;
        doorAnimation = GetComponent<Animator>();
        firstLockX = locks[0].position.x;
    }

    // Update is called once per frame
    void Update()
    {
        if (openedLocks == locks.Length)    // daca am numarat ca incuietorile deschise sunt cat totalul de incuietori, inseamna ca toate au fost deschise
        {
            if (transform.position.x < firstLockX)      // daca incuietoarea e la dreapta usii, o deschidem spre stanga
            {
                doorAnimation.SetTrigger("ToLeft");
            }
            else
            {
                doorAnimation.SetTrigger("ToRight");
            }
            gameObject.GetComponent<BoxCollider2D>().enabled = false;
        }
        else
        {
            for (int i = 0; i < locks.Length; i++)  // cautam sa vedem daca a mai fost descuiata o incuietoare, incuietorile deschise sunt setate cu null, ca se nu le mai numaram la alte treceri
            {
                if (locks[i] != null && locks[i].gameObject.GetComponent<KeyLock>().unlocked)       // daca incuietoarea a fost descuiata
                {
                    openedLocks++;
                    Destroy(locks[i].gameObject);
                    locks[i] = null;
                }
            }
        }
    }
}
