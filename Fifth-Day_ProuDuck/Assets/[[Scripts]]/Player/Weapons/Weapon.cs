using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public enum WeaponType
{
    PISTOL,
    AXE
}

public class Weapon : MonoBehaviour
{
    public ParticleSystem muzzleParticle = null;
    public WeaponType weaponType;



    public void setupParticle()
    {
        muzzleParticle = GameObject.Find("MuzzleParticleSystem").GetComponent<ParticleSystem>();
    }
    public void PlayParticleSystem()
    { 
        
       if(muzzleParticle!= null)
            muzzleParticle.Play();
       else
       {
           muzzleParticle = GameObject.Find("MuzzleParticleSystem").GetComponent<ParticleSystem>();
           muzzleParticle.Play();
       }
    }
    
}
