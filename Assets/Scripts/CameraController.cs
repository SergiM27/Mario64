using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour, GameManager.IRestartGameElement
{
    // Start is called before the first frame update

    [SerializeField] private Transform marioHeadTransform;

    private float yaw = 0;
    private float pitch = 0;
    
    [SerializeField] private float horizontalSensitivity;
    [SerializeField] private float verticalSensitivity;
    [SerializeField] private float smooth;
    private Vector3 currentRotation;
    private Vector3 rotationSpeed;
    [Space]
    [SerializeField] private float camDistance = 5;
    [SerializeField] private float minCamDistance = 2;
    [SerializeField] private float maxCamDistance = 5;
    [Space]
    [SerializeField] private LayerMask camCollisionLayerMask;
    [SerializeField] private float camOffset;
    [Space]

    public Vector3 startPosition;
    public Quaternion startRotation;
    public float timeSinceLastInput = 0;
    public float maxTimeSinceLastInput = 10;
    private bool restartCamera;
    private void Start()
    {
        restartCamera = false;
        startPosition = transform.position;
        startRotation = transform.rotation;
        GameObject.FindObjectOfType<GameManager>().AddRestartGameElement(this);
    }
    void Update()
    {
        if (restartCamera==false)
        {
            //calcular yaw
            Vector3 lookAtPosition = marioHeadTransform.position;
            Vector3 directionToPlayer = lookAtPosition - transform.position;


            directionToPlayer.y = 0;
            directionToPlayer.Normalize();
            yaw = Mathf.Atan2(directionToPlayer.z, directionToPlayer.x);

            //get input axis
            float horizontalAxis = Input.GetAxis("Mouse X");
            float verticalAxis = Input.GetAxis("Mouse Y");

            //apply input to pitch and yaw
            yaw += horizontalAxis * (horizontalSensitivity * Mathf.Deg2Rad) * Time.deltaTime;
            pitch += verticalAxis * (verticalSensitivity * Mathf.Deg2Rad) * Time.deltaTime;

            //clamp pitch
            pitch = Mathf.Clamp(pitch, -1.4f, 0.9f);

            //solve roatation
            Vector3 direction = new Vector3(Mathf.Cos(yaw) * Mathf.Cos(pitch), Mathf.Sin(pitch), Mathf.Sin(yaw) * Mathf.Cos(pitch));
            currentRotation = Vector3.SmoothDamp(currentRotation, direction, ref rotationSpeed, smooth);

            //solve position
            Vector3 desiredPosition = lookAtPosition - currentRotation * camDistance;

            //solve collision
            RaycastHit raycastHit;
            Ray ray = new Ray(marioHeadTransform.position, -currentRotation);
            if (Physics.Raycast(ray, out raycastHit, maxCamDistance, camCollisionLayerMask))
            {
                desiredPosition = raycastHit.point + currentRotation * camOffset;
            }

            //apply position
            transform.position = desiredPosition;
            transform.LookAt(marioHeadTransform.position);

            if (!Input.anyKey && (InputSystem.instance.CameraX == 0) && (InputSystem.instance.CameraY == 0) && InputSystem.instance.MovementX ==0
                && InputSystem.instance.MovementY == 0)
            {
                timeSinceLastInput += Time.deltaTime;
            }
            else
            {
                timeSinceLastInput = 0;
            }
            if (timeSinceLastInput > maxTimeSinceLastInput)
            {
                restartCamera = true;
                CameraReset();
            }
        }
    }

    public void CameraReset()
    {
        yaw = 0;

        Vector3 lookAt = marioHeadTransform.position;
        Vector3 directionToPlayer = marioHeadTransform.position - transform.position;
        float distanceToPlayer = directionToPlayer.magnitude;
        distanceToPlayer = Mathf.Clamp(distanceToPlayer, minCamDistance, maxCamDistance);
        directionToPlayer.y = 0;
        directionToPlayer.Normalize();
        yaw+=marioHeadTransform.rotation.eulerAngles.y * Mathf.Deg2Rad;
        Vector3 desiredPosition = marioHeadTransform.position - marioHeadTransform.forward * distanceToPlayer;
        Ray ray = new Ray(marioHeadTransform.position, -marioHeadTransform.forward);
        RaycastHit raycastHit;
        if(Physics.Raycast(ray, out raycastHit, distanceToPlayer, camCollisionLayerMask.value))
        {
            desiredPosition = raycastHit.point + marioHeadTransform.forward * camOffset;
        }
        transform.position = Vector3.Lerp(transform.position, desiredPosition, Time.deltaTime * 8);
        pitch = Mathf.Lerp(pitch, 0, Time.deltaTime * 8);
        transform.LookAt(lookAt);
        restartCamera = false;
    }

    public void RestartGame()
    {
        yaw = 0;
        yaw = (startRotation.eulerAngles.y)*Mathf.Deg2Rad;
        pitch = 0;
        transform.position = startPosition;
        transform.rotation = startRotation;
    }
}
