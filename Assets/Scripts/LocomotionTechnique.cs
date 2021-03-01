using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocomotionTechnique : MonoBehaviour
{
    // Please implement your locomotion technique in this script.
    public OVRInput.Controller leftController;
    public OVRInput.Controller rightController;

    public GameObject hmd;
    public GameObject leftGear;
    public GameObject rightGear;
    public GrappScript leftScriptBall;
    public GrappScript rightScriptBall;
    public float gasPower;
    public float grappSpeed;

    private Vector3 rightGrappPoint;
    private Vector3 leftGrappPoint;
    private bool rightSelected;
    private bool leftSelected;

    private LineRenderer rightLr;
    private LineRenderer leftLr;
    private Rigidbody rb;
    private float leftTriggerValue;
    private float rightTriggerValue;
    private float leftTriggerHandValue;
    private float rightTriggerHandValue;


    /////////////////////////////////////////////////////////
    // These are for the game mechanism.
    public ParkourCounter parkourCounter;
    public string stage;

    void Start()
    {
        rightGrappPoint = new Vector3(0.0f,0.0f,0.0f);
        leftGrappPoint = new Vector3(0.0f,0.0f,0.0f);
        rightSelected = false;
        leftSelected = false;

        rightLr = rightGear.GetComponent<LineRenderer>();
        leftLr = leftGear.GetComponent<LineRenderer>();
        rb = GetComponent<Rigidbody>();
        leftTriggerValue = 0.0f;
        rightTriggerValue = 0.0f;
    }

    void FixedUpdate(){

        leftTriggerHandValue = OVRInput.Get(OVRInput.Axis1D.PrimaryHandTrigger, leftController);
        rightTriggerHandValue = OVRInput.Get(OVRInput.Axis1D.PrimaryHandTrigger, rightController);

        if(rightSelected && rightTriggerHandValue > 0.25f){
            float currentD = rightScriptBall.GetLimit() + leftScriptBall.GetLimit();
            float minD = (rightGrappPoint - leftGrappPoint).magnitude;
            if(currentD > minD + Time.deltaTime * rightTriggerHandValue * grappSpeed){
                rb.AddForce(Vector3.Normalize(rightGrappPoint - rb.transform.position) * rightTriggerHandValue * grappSpeed);
            }
        }

        if(leftSelected && leftTriggerHandValue > 0.25f){
            float currentD = rightScriptBall.GetLimit() + leftScriptBall.GetLimit();
            float minD = (rightGrappPoint - leftGrappPoint).magnitude;
            if(currentD > minD + Time.deltaTime * leftTriggerHandValue * grappSpeed){
                rb.AddForce(Vector3.Normalize(leftGrappPoint - rb.transform.position) * leftTriggerHandValue * grappSpeed);
            }
        }

        if(OVRInput.Get(OVRInput.Button.One)){
            Vector3 gasDir = Vector3.Normalize(new Vector3(0.0f, 1.0f, 0.0f));
            rb.AddForce(gasDir * gasPower);
        }

        if(OVRInput.Get(OVRInput.Button.Three)){
           Vector3 gasDir = Vector3.Normalize(hmd.transform.forward);
           rb.AddForce(gasDir * gasPower);
        }


    }

    void Update()
    {
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        // Please implement your LOCOMOTION TECHNIQUE in this script :D.
        float tempL = leftTriggerValue;
        float tempR = rightTriggerValue;
        leftTriggerValue = OVRInput.Get(OVRInput.Axis1D.PrimaryIndexTrigger, leftController);
        rightTriggerValue = OVRInput.Get(OVRInput.Axis1D.PrimaryIndexTrigger, rightController);

        if (rightTriggerValue > 0.95f && tempR <=0.95){
            RaycastHit hit;
            int layer = (1<<8) | (1<<9);
            layer = ~layer;
            if(Physics.Raycast(rightGear.transform.position, rightGear.transform.forward, out hit, 300, layer)){
                rightGrappPoint = hit.point;
                rightScriptBall.MoveGrapp(rightGrappPoint);
                rightSelected = true;
                rightScriptBall.ChangeLimit((rb.transform.position - rightGrappPoint).magnitude);
                rightScriptBall.Resize(0.2f);
            }
        }else if(rightTriggerValue <= 0.95f){
            rightScriptBall.MoveGrapp(rb.transform.position);
            rightSelected = false;
            rightScriptBall.ChangeLimit(100.0f);
            rightScriptBall.Resize(0.0f);
        }

        if (leftTriggerValue > 0.95f && tempL <=0.95){
            RaycastHit hit;
            int layer = (1<<8) | (1<<9);
            layer = ~layer;
            if(Physics.Raycast(leftGear.transform.position, leftGear.transform.forward, out hit, 300, layer)){
                leftGrappPoint = hit.point;
                leftScriptBall.MoveGrapp(leftGrappPoint);
                leftSelected = true;
                leftScriptBall.ChangeLimit((rb.transform.position - leftGrappPoint).magnitude);
                leftScriptBall.Resize(0.2f);
            }
        }else if(leftTriggerValue <= 0.95f){
            leftScriptBall.MoveGrapp(rb.transform.position);
            leftSelected = false;
            leftScriptBall.ChangeLimit(100.0f);
            leftScriptBall.Resize(0.0f);
        }

        ////////////////////////////////////////////////////////////////////////////////
        // These are for the game mechanism.
        if (OVRInput.Get(OVRInput.Button.Two) || OVRInput.Get(OVRInput.Button.Four))
        {
            if (parkourCounter.parkourStart)
            {
                this.transform.position = parkourCounter.currentRespawnPos;
            }
        }
    }

    void LateUpdate(){
        rightLr.enabled = rightSelected;
        leftLr.enabled = leftSelected;

        if(rightSelected && rightTriggerHandValue > 0.25f){
            rightScriptBall.ReduceLimit((rightGrappPoint - rb.transform.position).magnitude);
        }

        if(leftSelected && leftTriggerHandValue > 0.25f){
            leftScriptBall.ReduceLimit((leftGrappPoint - rb.transform.position).magnitude);
        }

        if(!rightSelected){
            rightScriptBall.MoveGrapp(rb.transform.position);
        }
        if(!leftSelected){
            leftScriptBall.MoveGrapp(rb.transform.position);
        }

        if(rightGrappPoint != null){
            rightLr.SetPosition(0, rightGear.transform.position);
            rightLr.SetPosition(1, rightGrappPoint);
        }
        if(leftGrappPoint != null){
            leftLr.SetPosition(0, leftGear.transform.position);
            leftLr.SetPosition(1, leftGrappPoint);
        }
    }

    void OnTriggerEnter(Collider other)
    {

        // These are for the game mechanism.
        if (other.CompareTag("banner"))
        {
            stage = other.gameObject.name;
            parkourCounter.isStageChange = true;
        }
        else if (other.CompareTag("coin"))
        {
            parkourCounter.coinCount += 1;
            this.GetComponent<AudioSource>().Play();
            other.gameObject.SetActive(false);
        }
        // These are for the game mechanism.

    }
}