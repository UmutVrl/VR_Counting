using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;
using bmlTUX.Scripts.ExperimentParts;
using Leap;
using System;
using System.CodeDom;
using LeapInternal;
using TMPro;
using UnityEditor.UI;

/// <summary>
/// LeapFingerCounting Project
/// Main_Experiment_Part
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
public class MP_CanonicalFingersTrialScript : Trial {


    // // You usually want to store a reference to your experiment runner
    // YourCustomExperimentRunner myRunner;

    private readonly MP_CanonicalFingersExperimentRunner experimentRunner;

    private readonly Controller controller = new Controller();
    private Frame current;
    private Hand handRight;
	private Hand handLeft;
    
	//private List<float> fingerRt = new List<float>();
	
	
    // Required Constructor. Good place to set up references to objects in the unity scene
    public MP_CanonicalFingersTrialScript(ExperimentRunner runner, DataRow data) : base(runner, data)
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
	    var thisTrialsNumber = (int) Data["PrimeNumbers"];
	    var thisDistance = (int) Data["Distance"];
	    var thisCountingOrder = (string) Data["CountingOrder"];
	    var thisTargetNumber = thisTrialsNumber + thisDistance;
	    var thisPrimingDurationConst = (int) Data["PrimingDurationConst"];
	    
	    Data["FixCrossDuration"] = fixationCrossDuration;
	    Data["ShowedNumber"] = thisTargetNumber;
	    Data["PrimingDuration"] = Math.Round(Time.deltaTime * thisPrimingDurationConst, 3);
	    
	    experimentRunner.writingBoard.color = Color.white;
	    experimentRunner.writingBoard.alignment = TextAlignmentOptions.Midline;
	    experimentRunner.writingBoard.text = ".";         
	    yield return new WaitForSeconds(fixationCrossDuration);
	    experimentRunner.writingBoard.text = "%"; //  https://unicode-table.com/en/#0025   
	    yield return new WaitForSeconds(0.05f);
	    experimentRunner.writingBoard.text = thisTrialsNumber.ToString();
	    yield return MP_CanonicalFingersExperimentRunner.WaitFor.Frames(thisPrimingDurationConst);
	    experimentRunner.writingBoard.text = "%";     
	    yield return new WaitForSeconds(0.05f);
	    experimentRunner.writingBoard.text = "";
	    experimentRunner.CallModelPrefab(thisTargetNumber, thisCountingOrder);
	    experimentRunner.isTiming = true;
	    yield return null; //required for coroutine
    }


    // Main Trial Execution Code.
    protected override IEnumerator RunMainCoroutine()
    {
	    var thisTrialsNumber = (int) Data["PrimeNumbers"];
	    var thisDistance = (int) Data["Distance"];
	    var thisTargetNumber = thisTrialsNumber + thisDistance;
	    var thisTrialNo = (int) Data["Trial"];
	
	    // You might want to do a while-loop to wait for participant response: 
        var waitingForParticipantResponse = true;
        while (waitingForParticipantResponse) {   // keep check each frame until waitingForParticipantResponse set to false.

            if (Input.GetKeyDown(KeyCode.Return)) { // check return key pressed
	            waitingForParticipantResponse = false;  // escape from while loop
	            experimentRunner.isTiming = false;
	            Data["Skipped"] = true;
	            MP_CanonicalFingersExperimentRunner.DestroyModelPrefab();
            }
           
	        else if((int) DigitDetector()[10] == thisTargetNumber)
            {
	            
	            waitingForParticipantResponse = false;
	            experimentRunner.isTiming = false;
	            
	            // Append filename to folder name (format is 'id01Trial12shot.png"')
	            var name = $"{experimentRunner.folder}/id{experimentRunner.id}Trial{thisTrialNo}shot.png";

	            // Capture the screenshot to the specified file
	            ScreenCapture.CaptureScreenshot(name);

	            //fingerRt = DigitDetector();
	            MP_CanonicalFingersExperimentRunner.DestroyModelPrefab();
            }
	        yield return null; // wait for next frame while allowing rest of program to run (without this the program will hang in an infinite loop)
        }
        
       
    }


    // Optional Post-Trial code. Useful for waiting for the participant to do something after each trial (multiple frames)
    protected override IEnumerator PostCoroutine() {
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
    ///  Calculates total number of fingers which are extended.
    ///  Additionally saves RTs of each extended finger inside a list.
    /// </summary>
    private List<float> DigitDetector() //Hand Digit Calculator
    	{
	        //TODO: EXTENSION THRESHOLDS CHECK OF EACH FINGER

	        int a = 0, b = 0, c = 0, d = 0, e = 0 ;
    		int f = 0, g = 0, h = 0, i = 0, j = 0;
            var fingerRTs = new List<float>(){0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f};
    		current = controller.Frame(); 
    		
    		switch (current.Hands.Count)
            {
	            case 1 when current.Hands[0].IsRight:
		            handRight = current.Hands[0];
		            handLeft = null;
		            break;
	            case 1 when current.Hands[0].IsLeft:
					handLeft = current.Hands[0];
			        handRight = null;
		            break;
	            case 2:
		            handRight = current.Hands[0];
		            handLeft = current.Hands[1];
		            break;
	            case 0:
		            handRight = null;
		            handLeft = null;
		            break;
            }
    
    		if (handRight != null)
    		{
    			if (handRight.Fingers[0].IsExtended)
    				a = 1;
                // fingerRTs[0] = experimentRunner.reactionTimer;
	            if (handRight.Fingers[1].IsExtended)
    				b = 1;
	            if (handRight.Fingers[2].IsExtended)
    				c = 1;
    			if (handRight.Fingers[3].IsExtended)
    				d = 1;
	            if (handRight.Fingers[4].IsExtended)
  					e = 1;
	        }
    
    		if (handLeft != null)
    		{
	            if (handLeft.Fingers[0].IsExtended)
    				f = 1;
    			if (handLeft.Fingers[1].IsExtended)
    				g = 1;
    			if (handLeft.Fingers[2].IsExtended)
    				h = 1;
    			if (handLeft.Fingers[3].IsExtended)
    				i = 1;
    			if (handLeft.Fingers[4].IsExtended)
    				j = 1;
    		}
    		var sum = a + b + c + d + e + f + g + h + i + j;
    		fingerRTs[10] = sum;
			return fingerRTs;
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

