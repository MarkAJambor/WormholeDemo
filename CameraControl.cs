using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour
{
    public Rigidbody selfRB;
    public GameObject[] blackholes = new GameObject[2];
    public int frameWormholeTraversed = 0;
    public bool justTraversedWormhole = false;
    public float translationForce;
    public float rotationForce;
    public float wormholeRadius = 2.5f;
    public float wormholeAdjustment;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (justTraversedWormhole && (this.transform.position - blackholes[0].transform.position).magnitude > wormholeRadius && (this.transform.position - blackholes[1].transform.position).magnitude > wormholeRadius)
        {
            justTraversedWormhole = false;
        }
        for (int i = 0; i < blackholes.Length; i++)
        {
            if (!justTraversedWormhole)
            {
                if ((this.transform.position - blackholes[i].transform.position).magnitude < wormholeRadius)
                {
                    //teleport to other side of wormhole
                    justTraversedWormhole = true;
                    Vector3 displacement = this.transform.position - blackholes[i].transform.position;
                    if (i == 0)
                    {
                        this.transform.position = blackholes[1].transform.position - wormholeAdjustment * displacement;

                    }
                    else
                    {
                        this.transform.position = blackholes[0].transform.position - wormholeAdjustment * displacement;
                    }
                    Vector3 tempRotation = Quaternion.AngleAxis(180, displacement).eulerAngles;
                    this.transform.Rotate(tempRotation, Space.World);
                }
                //Debug.Log((this.transform.position - hole.transform.position).magnitude);
            }
        }

        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            translationForce *= 10;
        }
        if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            translationForce /= 10;
        }
        if (Input.GetKey(KeyCode.W))
        {
            selfRB.AddRelativeTorque(-rotationForce, 0, 0);
        }
        if (Input.GetKey(KeyCode.S))
        {
            selfRB.AddRelativeTorque(rotationForce, 0, 0);
        }
        if (Input.GetKey(KeyCode.A))
        {
            selfRB.AddRelativeTorque(0, -rotationForce, 0);
        }
        if (Input.GetKey(KeyCode.D))
        {
            selfRB.AddRelativeTorque(0, rotationForce, 0);
        }
        if (Input.GetKey(KeyCode.Q))
        {
            selfRB.AddRelativeTorque(0, 0, rotationForce);
        }
        if (Input.GetKey(KeyCode.E))
        {
            selfRB.AddRelativeTorque(0, 0, -rotationForce);
        }

        if (Input.GetKey(KeyCode.I))
        {
            selfRB.AddRelativeForce(0, translationForce, 0);
        }
        if (Input.GetKey(KeyCode.K))
        {
            selfRB.AddRelativeForce(0, -translationForce, 0);
        }
        if (Input.GetKey(KeyCode.J))
        {
            selfRB.AddRelativeForce(-translationForce, 0, 0);
        }
        if (Input.GetKey(KeyCode.L))
        {
            selfRB.AddRelativeForce(translationForce, 0, 0);
        }
        if (Input.GetKey(KeyCode.H))
        {
            selfRB.AddRelativeForce(0, 0, translationForce);
        }
        if (Input.GetKey(KeyCode.N))
        {
            selfRB.AddRelativeForce(0, 0, -translationForce);
        }
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            selfRB.velocity = Vector3.zero;
        }
    }
}
