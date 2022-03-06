using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// scriptul resawneaza obiectele dupa stergerea din inventar
public class ItemSpawn : MonoBehaviour
{
    public GameObject item;     // obiectul care se va respawna dupa stergere, il setam in inspector
    private Transform player;      // va retine jucatorul dupa coordonatele caruia vom respawna obiectul

    // instantiem obiectul in spatele jucatorului (in functie de orientare - playerScale) si ii dam si roatia originala (item.transform.rotation)
    public void SpawnRemovedItem(string playerTag, float playerScale)
    {
        player = GameObject.FindGameObjectWithTag(playerTag).transform;
        Instantiate(item, new Vector2(player.position.x - (playerScale) * 2, player.position.y), item.transform.rotation);     
    }
}
