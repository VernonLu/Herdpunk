using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiscoVFXcontroller : MonoBehaviour
{
    private float VFXradius;
    public ParticleSystem particleSys;
    private ParticleSystem.MainModule particleSysMain;
    private DiscoBallController discoBall;

    // Start is called before the first frame update
    void Start()
    {
        discoBall = GetComponentInParent<DiscoBallController>();
        VFXradius = discoBall.triggerRadius;
        particleSysMain = particleSys.main;
        particleSysMain.startSizeY = VFXradius;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
