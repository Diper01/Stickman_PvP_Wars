using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour {
    public static CameraFollow Instance;

    public Transform UpperBorder;
    public Transform LeftBorder;
    public Transform RightBorder;
    public Transform BottomBorder;

    [SerializeField]
    private Transform target;
    [SerializeField]
    private float moveSpeed = 3; 

    private Transform mainCameraTransform;
    private float upperLimit;
    private float leftLimit;
    private float rightLimit;
    private float bottomLimit;
    private float halfCameraHeight;
    private float halfCameraWidth;
    private float teleportDistance = 14f;

    private bool isDirectionRight;

    private void Awake()
    {
        if (Instance == null)
        {
            Init();
            Instance = this;
        }
        else {
            Destroy(this.gameObject);
        }
      
    }

    private void Init()
    {
        mainCameraTransform = Camera.main.transform;
        upperLimit = UpperBorder.position.y;
        leftLimit = LeftBorder.position.x;
        rightLimit = RightBorder.position.x;
        bottomLimit = BottomBorder.position.y;
        halfCameraHeight = Camera.main.orthographicSize;
        halfCameraWidth = halfCameraHeight * Camera.main.aspect;
    }

    private void Update()
    {       
        if (target != null)
        {
            FollowTarget();
        }      

    }

    public void SetTarget(Transform target) {
        this.target = target;
    }

    private  void FollowTarget()
    {
        if (((Vector2)mainCameraTransform.position - (Vector2)target.position).magnitude > teleportDistance)
        {
            mainCameraTransform.position = new Vector3(target.position.x, target.position.y, mainCameraTransform.position.z);
        }
        else
        {
            Vector2 newPosition = Vector2.Lerp(mainCameraTransform.position, target.position, Time.deltaTime * moveSpeed);
            mainCameraTransform.position = new Vector3(newPosition.x, newPosition.y, mainCameraTransform.position.z);

        }
        CheckBorderLimits();
    }

    private void CheckBorderLimits() {                  
        if (mainCameraTransform.position.y + halfCameraHeight > upperLimit) {
            mainCameraTransform.position = new Vector3(mainCameraTransform.position.x, upperLimit - halfCameraHeight, mainCameraTransform.position.z);
        }
        else if (mainCameraTransform.position.y - halfCameraHeight < bottomLimit) {
            mainCameraTransform.position = new Vector3(mainCameraTransform.position.x, bottomLimit + halfCameraHeight, mainCameraTransform.position.z);
        }

        if (mainCameraTransform.position.x - halfCameraWidth < leftLimit) {
            mainCameraTransform.position = new Vector3(leftLimit + halfCameraWidth, mainCameraTransform.position.y, mainCameraTransform.position.z);
        }
        else if (mainCameraTransform.position.x + halfCameraWidth > rightLimit) {
            mainCameraTransform.position = new Vector3(rightLimit - halfCameraWidth, mainCameraTransform.position.y, mainCameraTransform.position.z);
        }
        
    }
}
