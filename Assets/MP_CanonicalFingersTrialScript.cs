using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;
using bmlTUX.Scripts.ExperimentParts;
using Leap;
using System;
using System.CodeDom;
using System.Globalization;
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
	    var fixationCrossDuration = (float)GetRandomNumber(1f, 1.5f);
	    var thisTrialsNumber = (int) Data["PrimeNo"];
	    var thisDistance = (int) Data["Distance"];
	    var thisCountingOrder = (string) Data["CountingOrder"];
	    var thisPrimingDurationConst = (int) Data["PrimingConst"];

	    if ((thisTrialsNumber == 1 & thisDistance == -2) || (thisTrialsNumber == 2 & thisDistance == -2))
	    {
		    thisDistance = 1;
	    }
	    else if ((thisTrialsNumber == 8 & thisDistance == 2) || (thisTrialsNumber == 9 & thisDistance == 2))
	    {
		    thisDistance = -1;
	    }

	    var thisTargetNumber = thisTrialsNumber + thisDistance;

	    Data["FixCrossTime"] = fixationCrossDuration;
	    Data["ShowedNo"] = thisTargetNumber;
	    Data["PrimingTime"] = Math.Round(Time.deltaTime * thisPrimingDurationConst, 3);
	    Data["FingersExpect"] = FingersExpected(thisTargetNumber, thisCountingOrder);
	    
	    experimentRunner.writingBoard.color = Color.white;
	    experimentRunner.writingBoard.alignment = TextAlignmentOptions.Midline;
	    experimentRunner.writingBoard.text = ".";         
	    yield return new WaitForSeconds(fixationCrossDuration);
	    experimentRunner.writingBoard.text = "$"; //  https://unicode-table.com/en/#0025   
	    yield return new WaitForSeconds(0.66f);
	    experimentRunner.writingBoard.text = thisTrialsNumber.ToString();
	    yield return MP_CanonicalFingersExperimentRunner.WaitFor.Frames(thisPrimingDurationConst);
	    experimentRunner.writingBoard.text = "$";     
	    yield return new WaitForSeconds(0.66f);
	    experimentRunner.writingBoard.text = "";
	    experimentRunner.CallModelPrefab(thisTargetNumber, thisCountingOrder);
	    experimentRunner.isTiming = true;
	    yield return null; //required for coroutine
    }


    // Main Trial Execution Code.
    protected override IEnumerator RunMainCoroutine()
    {
	    var thisTrialsNumber = (int) Data["PrimeNo"];
	    var thisDistance = (int) Data["Distance"];
	    
	    if ((thisTrialsNumber == 1 & thisDistance == -2) || (thisTrialsNumber == 2 & thisDistance == -2))
	    {
		    thisDistance = 1;
	    }
	    else if ((thisTrialsNumber == 8 & thisDistance == 2) || (thisTrialsNumber == 9 & thisDistance == 2))
	    {
		    thisDistance = -1;
	    }
	    
	    var thisTargetNumber = thisTrialsNumber + thisDistance;
	    
	    // var thisTrialNo = (int) Data["Trial"];
	
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
	            //var name = $"{experimentRunner.folder}/id{experimentRunner.id}Trial{thisTrialNo}shot.png";
	            // Capture the screenshot to the specified file
	            //ScreenCapture.CaptureScreenshot(name);

	            //fingerRt = DigitDetector();
	            MP_CanonicalFingersExperimentRunner.DestroyModelPrefab();
            }
	        yield return null; // wait for next frame while allowing rest of program to run (without this the program will hang in an infinite loop)
        }
        
    }


    // Optional Post-Trial code. Useful for waiting for the participant to do something after each trial (multiple frames)
    protected override IEnumerator PostCoroutine() {
	    var fingerLayoutRight = DigitDetector()[1].ToString(CultureInfo.CurrentCulture);
	    var fingerLayoutLeft = DigitDetector()[0].ToString(CultureInfo.CurrentCulture);
		Data["FingersPerform"] = GetPerformedFingerConfiguration(fingerLayoutLeft, fingerLayoutRight);    
		experimentRunner.reactionTimer = 0;
	    yield return new WaitForSeconds(0.5f);
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
    ///  Returns a string to define fingers which expected to be extended
    ///  For Right Hand >> a= thumb, b= index, c= middle, d= ring, e= little 
    ///  For Left Hand  >> f= thumb, g= index, h= middle, i= ring, j= little
    ///  Changed to binary set: 1 means finger is triggered, 0 means finger is not triggered 
    /// </summary>
    private List<float> DigitDetector() //Hand Digit Calculator
    	{
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
	            {
		            a = 1;
		            // fingerRTs[0] = experimentRunner.reactionTimer;
		            fingerRTs[0] += 10000f;
	            }
	            if (handRight.Fingers[1].IsExtended)
	            {
		            b = 1;
		            fingerRTs[0] += 1000f;
	            }
	            if (handRight.Fingers[2].IsExtended)
	            {
		            c = 1;
		            fingerRTs[0] += 100f; 
	            }
	            if (handRight.Fingers[3].IsExtended)
	            {
		            d = 1;
		            fingerRTs[0] += 10f;
	            }
	            if (handRight.Fingers[4].IsExtended)
	            {
		            e = 1;
		            fingerRTs[0] += 1f;
	            }
            }
    
    		if (handLeft != null)
    		{
	            if (handLeft.Fingers[0].IsExtended)
	            {
		            f = 1;
		            fingerRTs[1] += 1f;
	            }
	            if (handLeft.Fingers[1].IsExtended)
	            {
		            g = 1;
		            fingerRTs[1] += 10f;
	            }
	            if (handLeft.Fingers[2].IsExtended)
	            {
		            h = 1;
		            fingerRTs[1] += 100f;
	            }
	            if (handLeft.Fingers[3].IsExtended)
	            {
		            i = 1;
		            fingerRTs[1] += 1000f;
	            }
	            if (handLeft.Fingers[4].IsExtended)
	            {
		            j = 1;
		            fingerRTs[1] += 10000f;
	            }
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

    //GetPerformedFingerConfiguration Method
    //Returns a string to define fingers which were triggered
    //Combine both hand finger configurations and returns a binary string(For example:1111100001)
    //Changed to binary set: 1 means finger is triggered, 0 means finger is not triggered
    
    private static string GetPerformedFingerConfiguration(string leftHandSet, string rightHandSet)
    {
	    switch (leftHandSet.Length)
	    {
		    case 1:
			    leftHandSet = "0000" + leftHandSet;
			    break;
		    case 2:
			    leftHandSet = "000" + leftHandSet;
			    break;
		    case 3:
			    leftHandSet = "00" + leftHandSet;
			    break;
		    case 4:
			    leftHandSet = "0" + leftHandSet;
			    break;
		    case 5:
			    leftHandSet = "" + leftHandSet;
			    break;
	    }
	    switch (rightHandSet.Length)
	    {
		    case 1:
			    rightHandSet = "0000" + rightHandSet;
			    break;
		    case 2:
			    rightHandSet = "000" + rightHandSet;
			    break;
		    case 3:
			    rightHandSet = "00" + rightHandSet;
			    break;
		    case 4:
			    rightHandSet = "0" + rightHandSet;
			    break;
		    case 5:
			    rightHandSet = "" + rightHandSet;
			    break;
	    }

	    var finalSet = leftHandSet + rightHandSet;
	    return finalSet;
    }
    
    
    
    //FingersExpected Method
    //Returns a string to define fingers which expected to be extended
    //Changed to binary set: 1 means finger is triggered, 0 means finger is not triggered 
    
    private static string FingersExpected(int a, string b)
    {
	    var fingersExpected = "Null";
	    
	    switch (a)
	    {
		    case 1 when b == "RightHandFirst":
			    fingersExpected = "0000000001";
			    break;
		    case 2 when b == "RightHandFirst":
			    fingersExpected = "0000000011";
			    break;
		    case 3 when b == "RightHandFirst":
			    fingersExpected = "0000000111";
			    break;
		    case 4 when b == "RightHandFirst":
			    fingersExpected = "0000001111";
			    break;
		    case 5 when b == "RightHandFirst":
			    fingersExpected = "0000011111";
			    break;
		    case 6 when b == "RightHandFirst":
			    fingersExpected = "1000011111";
			    break;
		    case 7 when b == "RightHandFirst":
			    fingersExpected = "1100011111";
			    break;
		    case 8 when b == "RightHandFirst":
			    fingersExpected = "1110011111";
			    break;
		    case 9 when b == "RightHandFirst":
			    fingersExpected = "1111011111";
			    break;
	    }
	    
	    switch (a)
	    {
		    case 1 when b == "LeftHandFirst":
			    fingersExpected = "1000000000";
			    break;
		    case 2 when b == "LeftHandFirst":
			    fingersExpected = "1100000000";
			    break;
		    case 3 when b == "LeftHandFirst":
			    fingersExpected = "1110000000";
			    break;
		    case 4 when b == "LeftHandFirst":
			    fingersExpected = "1111000000";
			    break;
		    case 5 when b == "LeftHandFirst":
			    fingersExpected = "1111100000";
			    break;
		    case 6 when b == "LeftHandFirst":
			    fingersExpected = "1111100001";
			    break;
		    case 7 when b == "LeftHandFirst":
			    fingersExpected = "1111100011";
			    break;
		    case 8 when b == "LeftHandFirst":
			    fingersExpected = "1111100111";
			    break;
		    case 9 when b == "LeftHandFirst":
			    fingersExpected = "1111101111";
			    break;
	    }
	    
	    return fingersExpected;
    }


}

