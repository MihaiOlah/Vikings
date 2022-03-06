using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public float smoothing;
    public float offsetHeight;
    public float zoom;
    public float zoomSmoothing;

    [HideInInspector] public Transform player;
    [HideInInspector] public Transform target;
    [HideInInspector] public float tSmoothing;
    [HideInInspector] public float tOffsetHeight;
    [HideInInspector] public float tZoom;
    [HideInInspector] public float tZoomSmoothing;

    private Vector3 movePos;
    private Vector3 camVelocity;
    private float zoomVelocity;
    private Camera mCam;

    // Start is called before the first frame update
    void Start()
    {
       // if (!GetComponent<PlayerChange>())      // cauta jucatorul activ, iar daca nu il gaseste, atunci
        //{
            if (GameObject.Find("Erik"))        // il cauta pe Erik si devine principal, altfel ii ia pre restul
            {
                player = GameObject.Find("Erik").transform;
            }
           // else if (GameObject.Find("Baleog"))
           // {
               // player = GameObject.Find("Baleog").transform;
            //}
            //else
           // {
                //player = GameObject.Find("Olaf").transform;
            //}
       // }
        mCam = GetComponent<Camera>();      //ia camera 
        mCam.orthographicSize = zoom;       // seteaza zoom-ul
    }

    // Update is called once per frame
    void Update()
    {
        if (target == null)
        {
            movePos = Vector3.SmoothDamp(transform.position, new Vector3(player.position.x, player.position.y + offsetHeight, -10), ref camVelocity, smoothing, Mathf.Infinity, Time.deltaTime);
            mCam.orthographicSize = Mathf.SmoothDamp(mCam.orthographicSize, tZoom, ref zoomVelocity, tZoomSmoothing, Mathf.Infinity, Time.deltaTime);
        }
        else
        {
            movePos = Vector3.SmoothDamp(transform.position, new Vector3(target.position.x, target.position.y + offsetHeight, -10), ref camVelocity, smoothing, Mathf.Infinity, Time.deltaTime);
            mCam.orthographicSize = Mathf.SmoothDamp(mCam.orthographicSize, tZoom, ref zoomVelocity, tZoomSmoothing, Mathf.Infinity, Time.deltaTime);
        }
        transform.position = movePos;
    }
}
