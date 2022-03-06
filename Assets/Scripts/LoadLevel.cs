using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadLevel : MonoBehaviour
{
    public Button[] levelButtons;
    public int i;

    // Start is called before the first frame update
    void Start()
    {
        int levelAt = PlayerPrefs.GetInt("levelAt", 2); // e 2 fiindca scena de selectie a nivelului e a doua
        for (int i = 0; i < levelButtons.Length; i++)
            if (i + 2 > levelAt)    // restul de butoane (adica scenele de la care incep nivelele) 
                levelButtons[i].interactable = false;
    }

}
