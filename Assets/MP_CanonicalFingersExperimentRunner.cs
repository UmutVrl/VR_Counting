using System;
using bmlTUX.Scripts.ExperimentParts;
using TMPro;
// ReSharper disable once RedundantUsingDirective
using UnityEngine;
using UnityEngine.Serialization;
using System.Collections;

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
    public GameObject[] modelHandLeftCounting;
    public GameObject cloneHandPosition;
    
    public float reactionTimer;
    public bool isTiming;
    public static int TrialFrameConstant = 7;  //Static keyword makes this variable a Member of the class, not of any particular instance.
    
    public string folder = "ScreenshotFolder";
    public int id;
    
    public static class WaitFor
    {
        public static IEnumerator Frames(int frameCount)
        {
            if (frameCount <= 0)
            {
                throw new ArgumentOutOfRangeException("frameCount", "Cannot wait for less that 1 frame");
            }

            while (frameCount > 0)
            {
                frameCount--;
                yield return null;
            }
        }
    }
    
    // Instantiate Hand Models 
    /// <param name="a"></param>
    /// a is the digit to be shown
    /// <param name="b"></param>
    /// b is the starting position of the finger counting
    /// LeftHandFirst or RightHandFirst  
    //
    public void CallModelPrefab(int a, string b)
    {
        
        var position = cloneHandPosition.transform.position;
        var rotation = cloneHandPosition.transform.rotation;

        if (b == "LeftHandFirst")
            Instantiate(modelHandLeftCounting[a-1], position, rotation);
        else
            Instantiate(modelHand[a-1], position, rotation);
        
    }

    public static void DestroyModelPrefab()
    {
        Destroy(GameObject.FindWithTag("HandClone"));
        Destroy(GameObject.FindWithTag("HandCloneLeftStart"));
    }
 
    private void Update()
    {
        if (isTiming){reactionTimer += Time.deltaTime;}
    }
    
}


 
    

