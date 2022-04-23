using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : MonoBehaviour
{
    private float speed = 10f;      // viteza sagetii
    private float initialX;         // retine pozitia sagetii cand a aparut
    // Start is called before the first frame update
    void Start()
    {
        Physics2D.IgnoreLayerCollision(15, 16);
        initialX = transform.position.x;        // salvam pozitia initiala
    }

    private void OnTriggerEnter2D(Collider2D other)         // other este celalalt obiect cu care interactioneaza (virus, perete, orice altceva decat sageata)
    {
        if (other.tag == "Enemy")
        {
            if (other.GetComponent<Virus2>() != null)
            {
                other.GetComponent<Virus2>().DamageUpdate(0.5f);
            }
            else if (other.GetComponent<Virus1>() != null)
            {
                other.GetComponent<Virus1>().DamageUpdate(0.5f);
            }
            else if (other.GetComponent<Robot>() != null)
            {
                Debug.Log("Arrow hit");
                other.GetComponent<Robot>().DamageUpdate(1f);
            }

            Destroy(this.gameObject);
        }
        else if (other.tag == "Wall" || other.tag == "Ground")
        {
            Destroy(this.gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.localScale.x == 1)        // daca merge spre dreapta adunam 35, fiindca e pe x pozitiv, altfel scadem, fiindca e pe x negativ
            transform.position = Vector2.MoveTowards(new Vector2(transform.position.x, transform.position.y), new Vector2(initialX + 35f, transform.position.y), speed * Time.deltaTime); // sageata calatoreste 15 pozitii
        else     // adunam la initialX fiindac altfel s-ar duce la infinit pentru ca ar fi acceeasi pozitie
            transform.position = Vector2.MoveTowards(new Vector2(transform.position.x, transform.position.y), new Vector2(initialX - 35f, transform.position.y), speed * Time.deltaTime);
        if (Mathf.Approximately(initialX, transform.position.x + 35f) || Mathf.Approximately(initialX, transform.position.x - 35f))   //cand ajunge la pozitia limita, sageata e distrusa
            Destroy(this.gameObject);

    }
}
