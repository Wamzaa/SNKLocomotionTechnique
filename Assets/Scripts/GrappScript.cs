using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrappScript : MonoBehaviour
{
    ConfigurableJoint joint;

    void Start(){
        joint = this.GetComponent<ConfigurableJoint>();
    }

    public float GetLimit(){
        SoftJointLimit linLim = joint.linearLimit;
        return(linLim.limit);
    }

    public void MoveGrapp(Vector3 newPos){
        transform.position = newPos;
    }

    public void ChangeLimit(float lim){
        SoftJointLimit linLim = joint.linearLimit;
        linLim.limit = lim;
        joint.linearLimit = linLim;
    }

    public void ReduceLimit(float lim){
        SoftJointLimit linLim = joint.linearLimit;
        if(linLim.limit > lim){
            linLim.limit = lim;
            joint.linearLimit = linLim;
        }
    }

    public void Resize(float s){
        transform.localScale = new Vector3(s, s, s);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
