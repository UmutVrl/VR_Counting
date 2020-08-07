using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;
using bmlTUX.Scripts.ExperimentParts;
using Leap;
using System;
using System.CodeDom;
using TMPro;

/// <summary>
/// LeapFingerCounting Project
/// Priming_Part
/// [Umut Can Vural]
/// [umutcan.vural@gmail.com]
/// </summary>

/// <summary>
/// Classes that inherit from Trial define custom behaviour for your experiment's trials.
/// Most experiments will need to edit this file to describe what happens in a trial.
///
/// This template shows how to set up a custom trial script using the toolkit's built-in functions.
///
/// You can delete any unused methods and unwanted comments. The only required parts are the constructor and the MainCoroutine.
/// </summary>
public class MP_CanonicalFingersTrialScriptPriming : Trial {


    // // You usually want to store a reference to your experiment runner
    // YourCustomExperimentRunner myRunner;

    private readonly MP_CanonicalFingersExperimentRunner experimentRunner;

    private readonly Controller controller = new Controller();
    private Frame current;
    private Hand handRight;
	private Hand handLeft;
    
	//private List<float> fingerRt = new List<float>(); //TODO:CURRENTLY DISABLED
	
	
    // Required Constructor. Good place to set up references to objects in the unity scene
    public MP_CanonicalFingersTrialScriptPriming(ExperimentRunner runner, DataRow data) : base(runner, data)
    {
	    // myRunner = (YourCustomExperimentRunner)runner;  //cast the generic runner to your custom type.
        // GameObject myGameObject = myRunner.MyGameObject;  // get reference to gameObject stored in your custom runner
        
        experimentRunner = (MP_CanonicalFingersExperimentRunner)runner;
        
    }
    



    // Optional Pre-Trial code. Useful for setting unity scene for trials. Executes in one frame at the start of each trial
    protected override void PreMethod() {
	    // float thisTrialsDistanceValue = (float)Data["MyDistanceFloatVariableName"]; // Read values of independent variables
        // myGameObject.transform.position = new Vector3(thisTrialsDistanceValue, 0, 0); // set up scene based on value
        
	}


    // Optional Pre-Trial code. Useful for waiting for the participant to
    // do something before each trial (multiple frames). Also might be useful for fixation points etc.
    protected override IEnumerator PreCoroutine()
    {
	    var fixationCrossDuration = (float)GetRandomNumber(1, 1.5);
	    var thisCharacter = (string) Data["Character"];
	    var thisFrameCount = (int) Data["FrameCount"];
	    
	    Data["FixCrossDuration"] = fixationCrossDuration;
	    
	    experimentRunner.writingBoard.color = Color.white;
	    experimentRunner.writingBoard.alignment = TextAlignmentOptions.Midline;
	    experimentRunner.writingBoard.text = ".";         
	    yield return new WaitForSeconds(fixationCrossDuration);
	    experimentRunner.writingBoard.text = "";     
	    yield return new WaitForSeconds(0.05f);
	    experimentRunner.writingBoard.text = thisCharacter;
	    yield return MP_CanonicalFingersExperimentRunner.WaitFor.Frames(thisFrameCount);
	    experimentRunner.writingBoard.text = "%%%%%";     //  https://unicode-table.com/en/#0025   
	    yield return new WaitForSeconds(0.05f);
	    Data["PrimingDuration"] = Math.Round(Time.deltaTime * thisFrameCount, 3);
	    experimentRunner.writingBoard.text = "";
	    experimentRunner.isTiming = true;
	    yield return null; //required for coroutine
    }


    // Main Trial Execution Code.
    protected override IEnumerator RunMainCoroutine()
    {
	    // TODO: AS SIMPLE AS POSSIBLE!!!
	    // You might want to do a while-loop to wait for participant response: 
        var waitingForParticipantResponse = true;
        while (waitingForParticipantResponse) {   // keep check each frame until waitingForParticipantResponse set to false.

            if (Input.GetKeyDown(KeyCode.Return)) { // check return key pressed
	            waitingForParticipantResponse = false;  // escape from while loop
	            experimentRunner.isTiming = false;
	            Data["Skipped"] = true;
            }
            
            else if (experimentRunner.reactionTimer > 1f) {
	            Data["Skipped"] = false;
	            experimentRunner.isTiming = false;
	            break;
            }

            yield return null; // wait for next frame while allowing rest of program to run (without this the program will hang in an infinite loop)
		}
        
       
    }


    // Optional Post-Trial code. Useful for waiting for the participant to do something after each trial (multiple frames)
    protected override IEnumerator PostCoroutine() {
	    
	  /*  foreach(var numbers in fingerRt)
	    {
		    Debug.Log(numbers);
	    }
	 */
	  
	    experimentRunner.reactionTimer = 0;
	    yield return null;
    }


    // Optional Post-Trial code. useful for writing data to dependent variables and for resetting everything.
    // Executes in a single frame at the end of each trial
    protected override void PostMethod() {
        // How to write results to dependent variables: 
        // Data["MyDependentFloatVariable"] = someFloatVariable;

    }

    /// <summary>
    ///  Priming Duration Value Calibration Method
    ///  Calculates the time of the priming duration
    /// </summary>
    private static void GetPrimeDuration()
    {

    }
    
    
    //GetRandomNumber Method
    //Random.NextDouble returns a double between 0 and 1.
    //Multiply that by the range need to go into (difference between maximum and minimum)
    //and then add that to the base (minimum)

    private static double GetRandomNumber(double minimum, double maximum)
    { 
	    var random = new System.Random();
	    return random.NextDouble() * (maximum - minimum) + minimum;
    }

}

