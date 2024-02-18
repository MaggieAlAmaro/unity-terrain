using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ViewManager : MonoBehaviour
{

    public Transform lookAtTarget;

    [SerializeField] private float rotateSpeed = 1f;
    [SerializeField] private bool rotate;
    [SerializeField] private Vector2 targetSize;
    //[SerializeField] private float FromTarget;
    [SerializeField] private float AngleFromTargetZ;

    // Start is called before the first frame update
    void Start()
    {

        
        transform.LookAt(lookAtTarget);

    }

    // Update is called once per frame
    void Update()
    {
        if (rotate)
        {
            //transform = transform.Rotate(lookAtTarget.position, rotateSpeed);
            //transform.LookAt(lookAtTarget);
            transform.Rotate(0.0f, 2.0f, 0.0f, Space.Self);

            Debug.Log("New posiion:" + transform.position);
            /*var dis = transform.position - lookAtTarget.position;
            transform.position = new Vector3(Mathf.Cos(transform.position.x + rotateSpeed) * dis.x, transform.position.y, Mathf.Sin(transform.position.z + rotateSpeed) * dis.z);
            */
        }
    }
}
