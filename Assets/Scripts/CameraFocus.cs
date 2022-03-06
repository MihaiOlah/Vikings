using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFocus : MonoBehaviour
{
    public Transform targetToSet;
    public float smoothing = 0.5f;
    public float offsetHeight = 0;
    public float zoom = 10;
    public float zoomSmoothing = 0.5f;

    public float startDelay = 1;
    public float focusTime = 2;
    public float endDelay = 1;
    public bool oneTime = true;

    private CameraFollow cameraFollow;
    private ErikControls erik;
    private BaleogControls baleog;
    private OlafControls olaf;

    // Start is called before the first frame update
    void Start()
    {
        cameraFollow = FindObjectOfType<CameraFollow>();
        erik = FindObjectOfType<ErikControls>();
        baleog = FindObjectOfType<BaleogControls>();
        olaf = FindObjectOfType<OlafControls>();
    }

    

    IEnumerator TargetChange()
    {
        if (erik)
        {
            erik.enabled = false;
        }
        if (baleog)
        {
            baleog.enabled = false;
        }
        if (olaf)
        {
            olaf.enabled = false;
        }
        yield return new WaitForSeconds(startDelay);
        cameraFollow.target = targetToSet;
        cameraFollow.tSmoothing = smoothing;
        cameraFollow.tOffsetHeight = offsetHeight;
        cameraFollow.tZoom = zoom;
        cameraFollow.tZoomSmoothing = zoomSmoothing;
        yield return new WaitForSeconds(focusTime);
        cameraFollow.target = null;
        yield return new WaitForSeconds(endDelay);
        if (erik)
            erik.enabled = true;
        if (baleog)
            baleog.enabled = true;
        if (olaf)
            olaf.enabled = true;
        if (oneTime)
            Destroy(gameObject);
    }
}
