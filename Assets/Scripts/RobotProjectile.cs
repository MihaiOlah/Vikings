using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobotProjectile : MonoBehaviour
{
    private float speed = 10f;      // viteza sagetii
    private float initialX;         // retine pozitia sagetii cand a aparut

    private ErikControls erik;
    private BaleogControls baleog;
    private OlafControls olaf;

    // Start is called before the first frame update
    void Start()
    {
        Physics2D.IgnoreLayerCollision(12, 12);
        initialX = transform.position.x;        // salvam pozitia initiala
    }

    private void OnTriggerEnter2D(Collider2D other)         // other este celalalt obiect cu care interactioneaza (virus, perete, orice altceva decat sageata)
    {
        if (other.tag == "Shield")
        {
            //other.GetComponent<ErikControls>().DecrementLives(-1);
            Destroy(this.gameObject);
        }
        else if (other.tag == "Erik" && other.GetComponent<ErikControls>() != null)
        {
            //other.GetComponent<ErikControls>().DecrementLives(-1);
            Destroy(this.gameObject);
        }
        else if (other.tag == "Baleog" && other.GetComponent<BaleogControls>() != null)
        {
            //other.GetComponent<BaleogControls>().DecrementLives(-1);
            Destroy(this.gameObject);
        }
        else if (other.tag == "Olaf" && other.GetComponent<OlafControls>() != null)
        {
            //other.GetComponent<OlafControls>().DecrementLives(-1);
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
        if (transform.eulerAngles.y == 0)        // daca merge spre dreapta adunam 35, fiindca e pe x pozitiv, altfel scadem, fiindca e pe x negativ
        {
            transform.position = Vector2.MoveTowards(new Vector2(transform.position.x, transform.position.y), new Vector2(initialX + 35f, transform.position.y), speed * Time.deltaTime); // sageata calatoreste 15 pozitii
        }
        else     // adunam la initialX fiindac altfel s-ar duce la infinit pentru ca ar fi acceeasi pozitie
        {
            transform.position = Vector2.MoveTowards(new Vector2(transform.position.x, transform.position.y), new Vector2(initialX - 35f, transform.position.y), speed * Time.deltaTime);
        }

        if (Mathf.Approximately(initialX, transform.position.x + 35f) || Mathf.Approximately(initialX, transform.position.x - 35f))   //cand ajunge la pozitia limita, sageata e distrusa
            Destroy(this.gameObject);
    }
}
