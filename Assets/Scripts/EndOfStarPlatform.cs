using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndOfStarPlatform : MonoBehaviour
{
    private PlatformEffector2D endStair;
    private ErikControls erik;
    private BaleogControls baleog;
    private OlafControls olaf;

    private bool erikOn, baleogOn, olafOn;      // retine care sunt pe platforma

    // Start is called before the first frame update
    void Start()
    {
        endStair = GetComponent<PlatformEffector2D>();
        erik = GameObject.FindGameObjectWithTag("Erik").GetComponent<ErikControls>();
        baleog = GameObject.FindGameObjectWithTag("Baleog").GetComponent<BaleogControls>();
        olaf = GameObject.FindGameObjectWithTag("Olaf").GetComponent<OlafControls>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.tag.Equals("Erik"))
            erikOn = true;
        else if (collision.collider.tag.Equals("Baleog"))
            baleogOn = true;
        else if (collision.collider.tag.Equals("Olaf"))
            olafOn = true;
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.collider.tag.Equals("Erik"))
            erikOn = false;
        else if (collision.collider.tag.Equals("Baleog"))
            baleogOn = false;
        else if (collision.collider.tag.Equals("Olaf"))
            olafOn = false;
    }

    public IEnumerator Reset()
    {
        yield return new WaitForSeconds(2f);
        this.GetComponent<BoxCollider2D>().isTrigger = false;
    }

    // Update is called once per frame
    void Update()
    {
        /* verificam daca vikingul de pe platforma e si cel activ si daca da, de abia atunci facem ce e mai jos*/
        if (erik.activePlayer && erikOn || baleog.activePlayer && baleogOn || olaf.activePlayer && olafOn)
        {
            if (Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S))
            {
                this.GetComponent<BoxCollider2D>().isTrigger = true;
                StartCoroutine(Reset());
                endStair.rotationalOffset = 180f;
            }
            if (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W))
            {
                endStair.rotationalOffset = 0;
            }
        }
    }
}
