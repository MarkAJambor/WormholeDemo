using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Helper : MonoBehaviour
{
    public Material singularityMaterial;
    public Cubemap cube;
    public ReflectionProbe probe;

    // Start is called before the first frame update
    void Start()
    {
        
    }
    public void FixedUpdate()
    {

    }

    //public void FixedUpdate()
    //{
    //    if (oneFacePerFrame)
    //    {
    //        int facemask = 1 << (Time.frameCount % 6);
    //        this.updateCubemap(facemask);
    //    }
    //    else
    //    {
    //        this.updateCubemap(63);
    //    }
    //}

    //public void updateCubemap(int faceMask)
    //{
    //    cam.RenderToCubemap(cube, faceMask);
    //}
}
