using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerChange : MonoBehaviour
{
    public enum ActivePlayer
    {
        Erik,
        Baleog, 
        Olaf
    }

    private ErikControls erik;
    private BaleogControls baleog;
    private OlafControls olaf;
    public ActivePlayer activePlayer;
       
// Start is called before the first frame update
void Start()
    {
        //gasim instantele fiecarui caracter
        erik = FindObjectOfType<ErikControls>();   
        baleog = FindObjectOfType<BaleogControls>();
        olaf = FindObjectOfType<OlafControls>();
        /*
        if (activePlayer == ActivePlayer.Erik)
        {
            eik.activePlayer = true;
            baleog.activePlayer = false;
            olaf.activePlayer = false;
        }
        else if (activePlayer == ActivePlayer.Baleog)
        {
            erik.activePlayer = false;
            baleog.activePlayer = true;
            olaf.activePlayer = false;
        }
        else
        {
            erik.activePlayer = false;
            baleog.activePlayer = false;
            olaf.activePlayer = true;
        }
        */
        activePlayer = ActivePlayer.Erik;   // jocul incepe cu Erik, deci el devine jucatorul activ
        erik.activePlayer = true;
        baleog.activePlayer = false;
        olaf.activePlayer = false;
    }

    public ActivePlayer GetActivePlayer()   // getter pentru a sti care e jucatorul activ, e folosita la controlul camerei
    {
        return activePlayer;
    }

    // Update is called once per frame
    void Update()
    {
        /*
         * Se verifica daca a fost apasata tasta control, iar daca da, atunci este schimbat jucatorul curent
         * in functie de cine era activ inainte.
         * Ordinea e Erik-Baleog-Olaf
         */
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            if (activePlayer == ActivePlayer.Erik)
            {
                activePlayer = ActivePlayer.Baleog;
                erik.activePlayer = false;
                baleog.activePlayer = true;
                olaf.activePlayer = false;
            }
            else if (activePlayer == ActivePlayer.Baleog)
            {
                activePlayer = ActivePlayer.Olaf;
                erik.activePlayer = false;
                baleog.activePlayer = false;
                olaf.activePlayer = true;
            }
            else
            {
                activePlayer = ActivePlayer.Erik;
                erik.activePlayer = true;
                baleog.activePlayer = false;
                olaf.activePlayer = false;
            }
        }
    }
}
