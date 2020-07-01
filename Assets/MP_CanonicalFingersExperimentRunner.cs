using System;
using bmlTUX.Scripts.ExperimentParts;
using TMPro;
// ReSharper disable once RedundantUsingDirective
using UnityEngine;
using UnityEngine.Serialization;

/// <summary>
/// LeapFingerCounting Project
/// [Umut Can Vural]
/// [umutcan.vural@gmail.com]
/// </summary>

/// <summary>
/// This class is the main communication between the toolkit and the Unity scene. Drag this script onto an empty gameObject in your Unity scene.
/// In the gameObject's inspector you need to drag in your design file and any custom scripts.
/// </summary>
public class MP_CanonicalFingersExperimentRunner : ExperimentRunner {

    //Here is where you make a list of objects in your unity scene that need to be referenced by your scripts.
    //public GameObject ReferenceToGameObject;

    //TODO: TextMEshPRO
    [FormerlySerializedAs("Board")] public TextMeshPro writingBoard;
    public GameObject[] modelHand;
    public GameObject cloneHandPosition;
    
    public float reactionTimer;
    public bool isTiming;
    
    public void CallModelPrefab(int a)
    {
        var position = cloneHandPosition.transform.position;
        var rotation = cloneHandPosition.transform.rotation;
        
        Instantiate(modelHand[a-1], position, rotation);
    }

    public void DestroyModelPrefab()
    {
        Destroy(GameObject.FindWithTag("HandClone"));
    }

    private void Update()
    {
        if (isTiming){reactionTimer += Time.deltaTime;}
    }
}


 
    

