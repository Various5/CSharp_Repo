using UnityEngine;

public class Billboard : MonoBehaviour
{
    public Camera cam;

    void LateUpdate()
    {
        transform.LookAt(transform.position + cam.transform.forward);
    }
}
