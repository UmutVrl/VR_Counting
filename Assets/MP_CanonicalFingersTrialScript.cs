using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;
using bmlTUX.Scripts.ExperimentParts;
using Leap;
using System;


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

    // TODO:LEAP RELATED
    private readonly Controller controller = new Controller();
    private Frame current;
    private Hand handRight;
	private Hand handLeft;
    
	private List<float> fingerRT = new List<float>(); //TODO:CURRENTLY DISABLED
	
	
    // Required Constructor. Good place to set up references to objects in the unity scene
    public MP_CanonicalFingersTrialScript(ExperimentRunner runner, DataRow data) : base(runner, data)
    {
	    // myRunner = (YourCustomExperimentRunner)runner;  //cast the generic runner to your custom type.
        // GameObject myGameObject = myRunner.MyGameObject;  // get reference to gameObject stored in your custom runner
        
        experimentRunner = (MP_CanonicalFingersExperimentRunner)runner;
    }


    // Optional Pre-Trial code. Useful for setting unity scene for trials. Executes in one frame at the start of each trial
    protected override void PreMethod() {
	    
	    fingerRT = DigitDetector();
	    
	    // float thisTrialsDistanceValue = (float)Data["MyDistanceFloatVariableName"]; // Read values of independent variables
        // myGameObject.transform.position = new Vector3(thisTrialsDistanceValue, 0, 0); // set up scene based on value
    }


    // Optional Pre-Trial code. Useful for waiting for the participant to
    // do something before each trial (multiple frames). Also might be useful for fixation points etc.
    protected override IEnumerator PreCoroutine()
    {

	    var fixationCrossDuration = (float)GetRandomNumber(0.5, 1.1);
	    
	    experimentRunner.writingBoard.text = "+";         
	    yield return new WaitForSeconds(fixationCrossDuration);
	    experimentRunner.writingBoard.text = "#";     
	    yield return new WaitForSeconds(0.07f);       
	    experimentRunner.writingBoard.text = "4";    //TODO: Addition NEUTRAL and NUMERICAL Primes
	    yield return new WaitForSeconds(0.07f);        
	    experimentRunner.writingBoard.text = "#";     
	    yield return new WaitForSeconds(0.07f);        
	    experimentRunner.writingBoard.text = "MODEL HAND"; //TODO: Hand models
	    
	    yield return null; //required for coroutine
    }


    // Main Trial Execution Code.
    protected override IEnumerator RunMainCoroutine()
    {
	
	    
	    
	    // TODO: AS SIMPLE AS POSSIBLE!!!
	    // You might want to do a while-loop to wait for participant response: 
        bool waitingForParticipantResponse = true;
        while (waitingForParticipantResponse) {   // keep check each frame until waitingForParticipantResponse set to false.

            if (Input.GetKeyDown(KeyCode.Return)) { // check return key pressed
	            //TODO: SKIP CURRENT TRIAL WITH DEFAULT RT -999 MESSAGE
                waitingForParticipantResponse = false;  // escape from while loop
            }
           
	        else if((int) DigitDetector()[10] == 4){
	           waitingForParticipantResponse = false;
            }
          
            yield return null; // wait for next frame while allowing rest of program to run (without this the program will hang in an infinite loop)
        }
    
    }


    // Optional Post-Trial code. Useful for waiting for the participant to do something after each trial (multiple frames)
    protected override IEnumerator PostCoroutine() {
	    
	    /*
	    
	    //TODO: Feedback
	    if (true)  //TODO: COMPARISON 
	    {
		    experimentRunner.writingBoard.color = Color.green;
		    experimentRunner.writingBoard.text = "+";
		    yield return new WaitForSeconds(0.3f);      
		    experimentRunner.writingBoard.text = "";   
		    yield return new WaitForSeconds(0.3f);
	    }
	    else
	    {
		    experimentRunner.writingBoard.color = Color.red;
		    experimentRunner.writingBoard.text = "+";
		    yield return new WaitForSeconds(0.3f);
		    experimentRunner.writingBoard.text = "";
		    yield return new WaitForSeconds(0.3f);
	    }
	    experimentRunner.writingBoard.color = Color.white;
	  
	    */
	    
        yield return null;
    }


    // Optional Post-Trial code. useful for writing data to dependent variables and for resetting everything.
    // Executes in a single frame at the end of each trial
    protected override void PostMethod() {
        // How to write results to dependent variables: 
        // Data["MyDependentFloatVariable"] = someFloatVariable;
    }
    
    //TODO: DigitDetector Method
    //Calculates total number of fingers which are extended.
    //Additionally saves RTs of each extended finger inside a list.
    private List<float> DigitDetector() //Hand Digit Calculator
    	{
	        //TODO: EXTENSION THRESHOLDS CHECK OR RT ELIMINATION DUE TO OUTPUT LIST
	        
	        
    		int a = 0, b = 0, c = 0, d = 0, e = 0, sum = 0;
    		int f = 0, g = 0, h = 0, i = 0, j = 0;

            var reactionTimer = 0; //TODO: RT CALCULATION IS CURRENTLY DISABLED
            
    		var fingerRTs = new List<float>(){0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f};
    		var rtSum = 0.0f;
            
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
    				fingerRTs[0] = reactionTimer;
    			}
				if (handRight.Fingers[1].IsExtended)
    			{
    				b = 1;
    				fingerRTs[1] = reactionTimer;
    			}
				if (handRight.Fingers[2].IsExtended)
    			{
    				c = 1;
    				fingerRTs[2] = reactionTimer;
    			}
				if (handRight.Fingers[3].IsExtended)
    			{
    				d = 1;
    				fingerRTs[3] = reactionTimer;
    			}
    			if (handRight.Fingers[4].IsExtended)
    			{
    				e = 1;
    				fingerRTs[4] = reactionTimer;
    			}
    			
    		}
    
    		if (handLeft != null)
    		{
	            if (handLeft.Fingers[0].IsExtended)
    			{
    				f = 1;
    				fingerRTs[5] = reactionTimer;
    			}
	            if (handLeft.Fingers[1].IsExtended)
    			{
    				g = 1;
    				fingerRTs[6] = reactionTimer;
    			}
	            if (handLeft.Fingers[2].IsExtended)
    			{
    				h = 1;
    				fingerRTs[7] = reactionTimer;
    			}
	            if (handLeft.Fingers[3].IsExtended)
    			{
    				i = 1;
    				fingerRTs[8] = reactionTimer;
    			}
	            if (handLeft.Fingers[4].IsExtended)
    			{
    				j = 1;
    				fingerRTs[9] = reactionTimer;
    			}
    		}
    		sum = a + b + c + d + e + f + g + h + i + j;
    		fingerRTs[10] = sum;
    
    		for(i = 0; i < 10; i++)
    			rtSum += fingerRTs[i];      
    		fingerRTs[11] = rtSum;
    		
    		return fingerRTs;
    	}
    
    
    //TODO: GetRandomNumber Method
    //Random.NextDouble returns a double between 0 and 1.
    //Multiply that by the range need to go into (difference between maximum and minimum)
    //and then add that to the base (minimum)
    
    public double GetRandomNumber(double minimum, double maximum)
    { 
	    var random = new System.Random();
	    return random.NextDouble() * (maximum - minimum) + minimum;
    }
    
    
}

