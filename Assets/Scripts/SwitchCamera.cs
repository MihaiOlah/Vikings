using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchCamera : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject camErik;
    public GameObject camBaleog;
    public GameObject camOlaf;
    public Camera []cam2;

    void Start()
    {
        
        camErik.SetActive(true);
        camBaleog.SetActive(false);
        camOlaf.SetActive(false);


        /*
        cam2 = FindObjectsOfType<Camera>();
        cam2[0].enabled = true;
        cam2[1].enabled = false;
        cam2[2].enabled = false;
        Debug.Log(cam2[0].);
        */
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.LeftControl))
        {
            if (camErik.activeSelf)
            {
                camErik.SetActive(false);
                camBaleog.SetActive(true);
                camOlaf.SetActive(false);
            }
            else if(camBaleog.activeSelf)
            {
                camErik.SetActive(false);
                camBaleog.SetActive(false);
                camOlaf.SetActive(true);
            }
            else
            {
                camErik.SetActive(true);
                camBaleog.SetActive(false);
                camOlaf.SetActive(false);
            }
            /*
            if (cam2[0].enabled)
            {
                cam2[0].enabled = false;
                cam2[1].enabled = true;
               // cam2[2].enabled = false;
            }
            else if (cam2[1].enabled)
            {
                cam2[0].enabled = false;
                cam2[1].enabled = false;
               // cam2[2].enabled = true;
            }
            else
            {
                cam2[0].enabled = true;
                cam2[1].enabled = false;
                cam2[2].enabled = false;
            }
            */
            /*
            if (cam2[0].enabled)
            {
                cam2[0].enabled = false;
                //cam2[1].enabled = true;
            }
            else if (cam2[1].enabled)
            {
                cam2[0].enabled = true;
                cam2[1].enabled = false;
            }
            */
        }
    }
}
