using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ViewManager : MonoBehaviour
{

    public Transform lookAtTarget;
    [SerializeField] private float cameraDistance;


    [SerializeField] private float constRotationSpeed;
    private float currentAngle;
    private bool constRotation;
    private float rotateRadius;

    void Start()
    {

        currentAngle = 0;
        constRotation = false;
        transform.position = new Vector3(cameraDistance, transform.position.y, 0);
        transform.LookAt(lookAtTarget);
        rotateRadius = Vector3.Distance(transform.position, lookAtTarget.position);

    }

    public void SetDistanceFromTarget(float distance)
    {

        rotateRadius = Vector3.Distance(transform.position, lookAtTarget.position);
        var thing = Mathf.Abs(rotateRadius - distance);
        if (distance > rotateRadius)
        {
            var ratio = rotateRadius / distance;
            transform.position = new Vector3(transform.position.x * ratio, transform.position.y, transform.position.z * ratio);

        }
        RotateAndLookAtTarget();
    }

    public void ChangeTarget(Transform target)
    {
        lookAtTarget = target;
        RotateAndLookAtTarget();
    }

    public void SetRotationAngle(float angleRadians)
    {
        constRotation = false;
        currentAngle = angleRadians;
        RotateAndLookAtTarget();
    }
    public void StopRotation()
    {
        constRotation = false;
    }

    public void StartRotation()
    {
        constRotation = true;
    }

    public void ToggleRotation()
    {
        constRotation = !constRotation;
    }

    private void RotateAndLookAtTarget()
    {
        transform.position = new Vector3(lookAtTarget.position.x + Mathf.Cos(currentAngle) * rotateRadius, transform.position.y, lookAtTarget.position.z - Mathf.Sin(currentAngle) * rotateRadius);
        Vector3 lookDirection = lookAtTarget.position - transform.position;
        transform.rotation = Quaternion.LookRotation(lookDirection);

    }


    void Update()
    {
        if (constRotation)
        {
            currentAngle += constRotationSpeed;
            currentAngle %= (Mathf.PI * 2);
            RotateAndLookAtTarget();

        }
    }
}
