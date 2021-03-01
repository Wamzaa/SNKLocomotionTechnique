using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaycastVisual : MonoBehaviour
{
    public GameObject gear;
    private LineRenderer line;
    private Vector3 hitPoint;

    void Start(){
        line = this.GetComponent<LineRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        RaycastHit hit;
        int layer = (1<<8) | (1<<9);
        layer = ~layer;
        if(Physics.Raycast(gear.transform.position, gear.transform.forward, out hit, 500, layer)){
            hitPoint = hit.point;
        }else{
            hitPoint = gear.transform.position + 500 * gear.transform.forward;
        }
        line.SetPosition(0, gear.transform.position);
        line.SetPosition(1, hitPoint);

    }
}
