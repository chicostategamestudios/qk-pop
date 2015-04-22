﻿using UnityEngine;
using System.Collections;
using Debug=FFP.Debug;

public class Quinc : MonoBehaviour
{
	private float pushRate = 0.5f;
	public float pushDistance = 5.0f;
	public float pushModifier = 50.0f;
	private float nextPush = 0.0f;
	private int pushRange = 20;

	private float pullRate = 0.5f;
	public float pullDistance = 5.0f;
	public float pullModifier = 50.0f;
	private float nextPull = 0.0f;
	private int pullRange = 20;

	private int cutRange = 20;

	private int soundThrowRange = 20;
	
	private int stunRange = 20;

	public float smoothing = 1f;

	//! These variables are temporary for testing, can be removed when implementing Targeting into code
	private GameObject pushPullTarget;
	private GameObject cutTarget;
	private GameObject soundThrowTarget;
	private GameObject stunTarget;

	void Start ()
	{
		print ("Press 1: Push");
		print ("Press 2: Pull");
		print ("Press 3: Cut");
		print ("Press 4: SoundThrow");
		print ("Press 5: Stun");
	}
	
	void FixedUpdate ()
	{
		if (Input.GetKeyDown(KeyCode.Alpha1) && Time.time > nextPush)
		{
			string pushStatus = "";
			nextPush = Time.time + pushRate;
			pushPullTarget = GameObject.Find("Crate"); //!> Reference Targeting Script to get current Target

			if(Push(ref pushStatus, pushPullTarget))
			{
				print ("Push status: " + pushStatus);
                pushPullTarget.GetComponent<Item>().pushCounter++;
                pushPullTarget.GetComponent<Item>().quincAffected = true;
			}
			else
			{
				print ("Push Error: " + pushStatus);
			}
		}
		else if (Input.GetKeyDown(KeyCode.Alpha2) && Time.time > nextPull)
		{
			string pullStatus = "";
			nextPull = Time.time + pullRate;
			pushPullTarget = GameObject.Find("Crate"); //!> Reference Targeting Script to get current Target

			if(Pull(ref pullStatus, pushPullTarget))
			{
				print ("Pull Status: " + pullStatus);
                pushPullTarget.GetComponent<Item>().pullCounter++;
                pushPullTarget.GetComponent<Item>().quincAffected = true;
			}
			else
			{
				print ("Pull Error: " + pullStatus);
			}
		}
		else if (Input.GetKeyDown(KeyCode.Alpha3))
		{
			string cutStatus = "";
			cutTarget = GameObject.Find("Rope"); //!> Reference Targeting Script to get current Target

			if(Cut(ref cutStatus, cutTarget))
			{
				print ("Cut Status: " + cutStatus);
                cutTarget.GetComponent<Item>().cutCounter++;
                cutTarget.GetComponent<Item>().quincAffected = true;
			}
			else
			{
				print ("Cut Error: " + cutStatus);
			}
		}
		else if(Input.GetKeyDown(KeyCode.Alpha4))
		{
			string soundStatus = "";
			soundThrowTarget = GameObject.Find("Well"); //!> Reference Targeting Script to get current Target

			if(SoundThrow(ref soundStatus, soundThrowTarget))
			{
				print ("Sound Status: " + soundStatus);
                soundThrowTarget.GetComponent<Item>().soundThrowCounter++;
                soundThrowTarget.GetComponent<Item>().quincAffected = true;
			}
			else
			{
				print ("Sound Error: " + soundStatus);
			}
		}
		else if(Input.GetKeyDown(KeyCode.Alpha5))
		{
			string stunStatus = "";
			stunTarget = GameObject.Find("Enemy"); //!> Reference Targeting Script to get current Target

			if(Stun(ref stunStatus, stunTarget))
			{
				print ("Stun Status: " + stunStatus);
                stunTarget.GetComponent<Item>().stunCounter++;
                stunTarget.GetComponent<Item>().quincAffected = true;
			}
			else
			{
				print ("Stun Error: " + stunStatus);
			}
		}
	}

	//! Function to be called when pushing a box or other heavy object, pushing at intervals (think Ocarina of time)
	bool Push (ref string status, GameObject pushTarget)
	{
		/*
		Check if Object is Push Compatible
		Else return "Object not Compatible"
		Check if Object is with in Push Range
		Else return "Object not in Range"
		Push the object either with force, translation, or grid-based movement
		Play animation for player using Push
		*/
		print("Distance between Player and Target: " + Vector3.Distance(pushTarget.transform.position, transform.position));

		if(pushTarget == null)
		{
			status = "No Target Selected";
			return false;
		}
		if(!pushPullTarget.GetComponent<Item>().pushCompatible)
		{
			status = "Not Compatible";
			return false;
		}
		if(Vector3.Distance(pushTarget.transform.position, transform.position) > pushRange)
		{
			status = "Not In Range";
			return false;
		}

		/* 
			1. Get direction to push
			2. Get position by taking direction multiplied by distance
			3. Use Raycast from Heaven to find Y value for targetPosition that is valid or is above terrain
			4. Call Coroutine to Lerp to target position
		*/

		Vector3 pushDirection = pushTarget.transform.position - transform.position;
		pushDirection.Normalize();
		Vector3 targetPosition = (pushDirection * pushDistance) + pushTarget.transform.position;
		StartCoroutine(MoveSlowly(pushTarget.gameObject, targetPosition, pushDirection));
		status = "Push Successful";
		return true;
	}

	//! Function to be called when pulling an object, pulling at intervals (think Ocarina of time)
	bool Pull (ref string status, GameObject pullTarget)
	{
		/*
		Check if Object is PullCompatible
		Else return "Object not Compatible"
		Check if Object is with in Pull Range
		Else return "Object not in Range"
		Pull the object either with force, translation, or grid-based movement
		Play animation for player using Pull
		*/

		if(pullTarget == null)
		{
			status = "No Target Selected";
			return false;
		}
		if(!pullTarget.GetComponent<Item>().pullCompatible)
		{
			status = "Not Compatible";
			return false;
		}
		if(Vector3.Distance(pullTarget.transform.position, transform.position) > pullRange)
		{
			status = "Not In Range";
			return false;
		}

		Vector3 pullDirection = transform.position - pullTarget.transform.position;
		pullDirection.Normalize();
		Vector3 targetPosition = (pullDirection * pullDistance) + pullTarget.transform.position;
		StartCoroutine(MoveSlowly(pullTarget.gameObject, targetPosition, pullDirection));
		status = "Pull Successful";
		return true;
		
	}

	//! Function to be called when cutting rope
	bool Cut (ref string status, GameObject cutTarget)
	{
		/*
		Check if Object is Cut Compatible
		Else return "Object not Compatible"
		Check if Object is within Cut Range
		Else return "Object not in Range"
		Call Object.cut() function
		Play animation for player using Cut
		Untarget GameObject?
		*/
		if(cutTarget == null)
		{
			status = "No Target Selected";
			return false;
		}
		if(!cutTarget.GetComponent<Item>().cutCompatible)
		{
			status = "Not Compatible";
			return false;
		}
		if(Vector3.Distance(cutTarget.transform.position, transform.position) > cutRange)
		{
			status = "Not In Range";
			return false;
		}
		
		cutTarget.GetComponent<Rope>().Cut();
		
		// Untarget Cuttable Object?
		status = "Cut Successful";
		return true;
		
	}

	//! Function that activates SoundThrow app on Quinc phone that distracts enemies
	bool SoundThrow (ref string status, GameObject soundThrowTarget)
	{
		/*
		Check if Object is SoundThrow compatible
		Else return "Object not Compatible"
		Check if Object is within SoundThrow Range
		Else return "Object not in Range"
		Maybe pick type of sound to be thrown?
		Play sound and animation
		Untarget GameObject?
		*/
		if(soundThrowTarget == null)
		{
			status = "No Target Selected";
			return false;
		}
		if(!soundThrowTarget.GetComponent<Item>().soundThrowCompatible)
		{
			status = "Not Compatible";
			return false;
		}
		if(Vector3.Distance(soundThrowTarget.transform.position, transform.position) > soundThrowRange)
		{
			status = "Not In Range";
			return false;
		}

		// Possibly allow player to select sound, or each object will have it's own sound attached to it
		// Call SoundThrow function in Well script, which will play sound and animation
		soundThrowTarget.GetComponent<Well>().SoundThrow();

		// Untarget GameObject?
		status = "SoundThrow Successful";
		return true;
	}

	//! Function called when activating Stun App on Quinc phone. Only works against enemies.
	bool Stun (ref string status, GameObject stunTarget)
	{
		/*
		Check if Object is Stunnable object
		Else return "Object not Compatible"
		Check if Object is within Stun Range
		Else return "Object not in Range"
		Call Object.stun() funciton
		Play animation for player using stun
		Untarget GameObject?
		*/

		if(stunTarget == null)
		{
			status = "No Target Selected";
			return false;
		}
		if(!stunTarget.GetComponent<Item>().stunCompatible)
		{
			status = "Not Compatible";
			return false;
		}
		if(Vector3.Distance(stunTarget.transform.position, transform.position) > stunRange)
		{
			status = "Not In Range";
			return false;
		}

		stunTarget.GetComponent<Enemy>().Stun();

		//Untarget Enemy?
		status = "Stun Successful";
		return true;
	}

	IEnumerator MoveSlowly(GameObject targetObject, Vector3 targetPosition, Vector3 direction)
	{
		print ("Target Position In CoRoutine: " + targetPosition);

		while(Vector3.Distance(targetObject.transform.position, targetPosition) > 2.0f)
		{
			targetObject.transform.position = Vector3.Lerp(targetObject.transform.position, targetPosition, smoothing * Time.deltaTime);
			yield return null;
		}
		yield return null;

		print ("Target Reached");
	}

}