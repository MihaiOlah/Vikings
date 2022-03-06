using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shield : MonoBehaviour
{
    public Transform olaf;
    // Start is called before the first frame update
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.tag.Equals("Enemy"))
            Debug.Log("scut");
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag.Equals("Enemy"))
            Debug.Log("scut");
    }   

    void Start()
    {
        this.gameObject.transform.SetParent(olaf);
        Physics2D.IgnoreLayerCollision(13, 11, true);
        Physics2D.IgnoreLayerCollision(13, 10, true);
        Physics2D.IgnoreLayerCollision(13, 9, true);
    }

    // Update is called once per frame
    void Update()
    {
        //GameObject olaf = GameObject.Find("Olaf");
        // transform.position = new Vector2(olaf.transform.position.x + 0.836f, olaf.transform.position.y + 0.162f);
        
    }
}
