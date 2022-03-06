using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices.ComTypes;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartGame : MonoBehaviour
{
    public void GameStart(bool isClickedStart)
    {
        if (isClickedStart)
        {
            if (!File.Exists(Application.dataPath + "/save.txt"))
            {
                PlayerPrefs.SetInt("levelAt", 2);   // daca jucatorul e nou, atunci va incepe de la primul nivel
            }
            else
            {
                string fileContent = File.ReadAllText(Application.dataPath + "/save.txt");
                if (fileContent != "")
                {
                    PlayerPrefs.SetInt("levelAt", int.Parse(fileContent));  // daca jucatorul a mai jucat jocul, va incepe de unde ramas
                }
                else
                {
                    PlayerPrefs.SetInt("levelAt", 2);   // daca jucatorul e nou, atunci va incepe de la primul nivel, dar fisierul exista si e gol
                }
            }
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }
    }

    public void GameQuit(bool isClickedQuit)
    {
        if (isClickedQuit)
        {
            if (!File.Exists(Application.dataPath + "/save.txt"))
                File.WriteAllText(Application.dataPath + "/save.txt", PlayerPrefs.GetInt("levelAt").ToString());
            else
            {
                int i = 2;
                File.WriteAllText(Application.dataPath + "/save.txt", i.ToString());
            }
            QuitGame();
        }
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
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
