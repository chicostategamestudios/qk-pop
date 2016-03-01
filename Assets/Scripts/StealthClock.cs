﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class StealthClock : MonoBehaviour
{
    /*!
        \file  StealthClock.cs
	    \brief  This class attaches to a clock face.
	
        In order for the minigame success/fail conditions to work properly,
	    the clock face y axis must be inverted so the y axis is pointing down
        (y rotation = 180, z rotation = 180).
	*/

    //reference to the player
    Transform player;

    //distance from player to search for enemies
	public float searchSize = 20f;

    //list of enemies searching near player
	public List<Enemy> enemyList = new List<Enemy>();

	private AIManager aiMan;

	//start of green area in degrees from x axis
	private int _startDegree;
	//end of green area in degrees from x axis
	private int _endDegree;

	public float startDegree
	{
		get
		{
			return _startDegree;
		}
	}

	public float endDegree
	{
		get
		{
			return _endDegree;
		}
	}

	//clock hand speed multiplyer, used by StealthHand object
	public float _clockSpeed = 1f;

	public float clockSpeed
	{
		get
		{
			return _clockSpeed;
		}
	}

	//size of green area in degrees
	private int _easySize = 45;
	private int _mediumSize = 30;
	private int _hardSize = 20;
	private int _hellSize = 10;

	//success/fail counts
	private int _currentSuccess;
	private int _currentFail;

	//success/fail limits
	public int maxSuccess;
	public int maxFail;

    //number of successes necessary for a win
    private int easySuccess = 3;
    private int mediumSuccess = 3;
    private int hardSuccess = 3;
    private int hellSuccess = 3;

    //number of failures necessary for a loss
    private int easyFail = 1;
    private int mediumFail = 1;
    private int hardFail = 1;
    private int hellFail = 1;

    //speed of clock hand in rpms
    public int easySpeed = 20;
    private int mediumSpeed = 20;
    private int hardSpeed = 20;
    private int hellSpeed = 40;

	//width of red/green lines start is center of circle, end is edge of circle
	private float _startWidth;
	private float _endWidth;

	//number of guards near player
	private int _numberOfGuards;

	public int numberOfGuards
	{
		get
		{
			return _numberOfGuards;
		}
	}

	//length of lines that make up red/green areas
	private float _lineLength;
	//degrees in a circle
	private int _degrees = 360;

	//starting point of each line, above center of clock face
	public Vector3 startPos;

	//array of empty objects to hold line renderers, one object/line for each degree
	public GameObject[] lines;
	
    //hand of clock, child of this gameObject
	private GameObject _clockHand;

	void Awake()
	{

		//get the location of the player
		player = GameObject.FindGameObjectWithTag("Player").transform;

		aiMan = GameObject.FindObjectOfType<AIManager>();



		//check for nearby enemies that are searching and not chasing player
//AI

//TEMP
//		_numberOfGuards = 1;
//TEMP
		
//		Collider[] enemyColliders = Physics.OverlapSphere(player.position, searchSize);

//		for(int i = 0; i < enemyColliders.Length; i++)
//		{

			//check for enemy tag
//			if(enemyColliders[i].CompareTag("enemy"))
//			{

				//if enemy is searching for player
//AI				if(enemyColliders[i].GetComponent<AIMain>().enemyCurrentState == enemy.searchingState)
//AI				{

					//add to list
//					enemyList.Add(enemyColliders[i].GetComponent<Enemy>());

//AI				}
//			}
//		}

		//get the number of guards actively searching for player
//		_numberOfGuards = enemyList.Count;
		
		//set difficulty
		setDifficulty();

//TESTING
		Debug.Log("StealthClock Awake() complete");
//END TESTING

	}

	// Use this for initialization
	void Start()
	{

		//initialize line parameters
		_lineLength = transform.localScale.x / 2f;
		startPos = transform.position;
        //should set line y distance above clock face based on size of clock
		startPos = new Vector3(startPos.x, startPos.y + transform.lossyScale.y * 2.0f, startPos.z);
		_startWidth = 0f;
		_endWidth = transform.localScale.x * Mathf.PI / 360f;

		//array of empty gameobjects to hold a single line renderer each
		lines = new GameObject[_degrees];

		//initialize lines for arc
		lineSetup();
		
		//set red and green zones accordingly
		setColors();

		//get reference to clockHand
		_clockHand = GetComponentInChildren<StealthHand>().gameObject;
		
	}

	// Update is called once per frame
	void Update()
	{

		//check to see if player is found
		hideCheck();

		//check for at least 1 suspicous enemy
//AI
        //if(suspicious < 1)
        //kill self

		//check for no enemies chasing player
//AI
        //if(chasing > 0)
        //kill self

//should be if(Input.GetButtonDown("Action"))
		if(Input.GetButtonDown("Jump"))
		{

			if(_startDegree < _endDegree)
			{
				//check for success/failure
				if((startDegree <= _clockHand.transform.localEulerAngles.y) && (_clockHand.transform.localEulerAngles.y <= endDegree))
				{
                    
                    //increment success count
					_currentSuccess++;

//TESTING
					print("SUCCESS: " + _clockHand.transform.localEulerAngles.y);
					print("startDegree: " + startDegree);
					print("endDegree: " + endDegree);
//END TESTING
					//check to see if zones need to be reset
					if(_currentSuccess < maxSuccess)
						{
                            
                            //reset the red and green zones
							setZones();

						}

				}
				else
				{

                    //increment fail count
					_currentFail++;

//TESTING
					print("FAIL: " + _clockHand.transform.localEulerAngles.y);
					print("startDegree: " + startDegree);
					print("endDegree: " + endDegree);
//END  TESTING

				}

			}
			//endDegree <= startDegree
			else
			{
				if((_startDegree <= _clockHand.transform.localEulerAngles.y) || (_clockHand.transform.localEulerAngles.y <= endDegree))
				{

                    //increment success count
					_currentSuccess++;

					//check to see if zones need to be reset
					if(_currentSuccess < maxSuccess)
					{

                        //reset the zones
						setZones();

					}

				}
				else
				{

                    //increment fail count
					_currentFail++;

				}

			}

			//check for win condition
			if(_currentSuccess >= maxSuccess)
			{
				
				Debug.Log("StealthClock: mini game success");

                //enemies return to normal
                //				foreach(Enemy guard in enemyList)
                //				{

                //AI				guard.enemyCurrentState = enemy.patrolState;

                //				}

                //deactivate lines
                deactivateLines();

                //deactivate self
                gameObject.SetActive(false);

			}
			else if(_currentFail >= maxFail)
			{

				Debug.Log("StealthClock: mini game fail");

                //alert enemies to player position
                //				foreach(Enemy guard in enemyList)
                //				{

                //AI				guard.enemyCurrentState = clockFace.chaseState;

                //				}

                //deactivate lines
                deactivateLines();

                //deactivate self
                gameObject.SetActive(false);

			}

		}

	}

    /*!
        \brief Creates a line renderer for each object in the lines array.

        Creates a line renderer for each object in the lines array. Each line starts
        in the the center of the clock face and a moves outward, one line for each 
        degree of the clock face. Line widths start at a point and expand as they
        move away from the center of the clock face to form a continuous circle.

        \return void
    */
    void lineSetup()
	{

		//create a line renderer for each object in the array
		for(int i = 0; i < lines.Length; i++)
		{

			float lineX;
			float lineZ;
			Vector3 linePos;

			//create the gameObject to hold the LineRenderer
			lines[i] = new GameObject();

			//set the position of each game object with line renderers
			lines[i].transform.position = startPos;

			//add a line renderer to the empty game object
			LineRenderer lRend = lines[i].AddComponent<LineRenderer>();
			//set line widths
			lRend.SetWidth(_startWidth, _endWidth);
			//set line renderer material
			lRend.material = new Material(Shader.Find("Particles/Additive"));

  //          lRend.material = new Material(Shader.Find("Transparent/Diffuse"));

            //only one line per game object
            lRend.SetVertexCount(2);
			
			//do math to point lines in correct directions, find end point for each line
			//initialize end point to current position of gameObject
			linePos = startPos;

			//find x and z position of end of line, y will remain the same
			lineX = startPos.x + _lineLength * Mathf.Cos(Mathf.Deg2Rad * i);
			linePos[0] = lineX;
			lineZ = startPos.z + _lineLength * Mathf.Sin(Mathf.Deg2Rad * i);
			linePos[2] = lineZ;

			//set start and end points for the line
			lRend.SetPosition(0, startPos);
			lRend.SetPosition(1, linePos);
			
		}
		
	}


    /*! \brief Sets proper difficulty level based on enemies and resets the red and green zones.
        
        Just calls setDifficulty and setColors.
    */
	void setZones()
	{

		setDifficulty();
		setColors();

	}

	/*! \brief  Sets the minigame difficulty.
    
        Sets the location and size of green area, successes needed to win, fails allowed
        before losing, and speed of clock hand based on the number of guards presently
        searching for player.

        Calls getEndDegree(int randomStart, int range)

        \return void
    */
	void setDifficulty()
	{

		//call AI manager for suspicious guards
//AI
		_numberOfGuards = 1;

		//choose random angle to start the green zone
		_startDegree = Random.Range(0, 360);

		//1 guard
		if(_numberOfGuards < 2)
		{

			_endDegree = getEndDegree(_startDegree, _easySize);
			maxSuccess = easySuccess;
			maxFail = easyFail;
			_clockSpeed = easySpeed;

		}
		//2-3 guards
		else if(_numberOfGuards < 4)
		{

			_endDegree = getEndDegree(_startDegree, _mediumSize);
			maxSuccess = mediumSuccess;
			maxFail = mediumFail;
			_clockSpeed = mediumSpeed;

		}
		//4-9 guards
		else if(_numberOfGuards < 10)
		{

			_endDegree = getEndDegree(_startDegree, _hardSize);
			maxSuccess = hardSuccess;
			maxFail = hardFail;
			_clockSpeed = hardSpeed;

		}
		//10 or more guards
		else
		{

			_endDegree = getEndDegree(_startDegree, _hellSize);
			maxSuccess = hellSuccess;
			maxFail = hellFail;
			_clockSpeed = hellSpeed;

		}

	}

    /*!
	    \brief  Calculates the end degree of the green zone.
        
	    \param randomStart the start degree of the green area 0-359
        \param range the size of the green area in degrees

        Called by setDifficulty()

        /return int
	*/
    int getEndDegree(int randomStart, int range)
	{

		int end;

		//check for range
		if(randomStart < 0)
		{

			randomStart = 0;

		}

		//find end degree
		end = randomStart + range;

		//check for upper bound
		while(end > 360)
		{

			//move end within bounds
			end = end - 360;

			//check for same start and end degrees
			if(randomStart == end)
			{

				//just set to easy range
				_startDegree = 0;
				end = _startDegree + _easySize;
				
				Debug.Log("StealthClock.getEndDegree: start == end");

			}

		}
		
		return end;

	}

    /*!
	    \brief Sets the colors for each line renderer that make up the red and green zones.
	    
        Cycles through each object in the lines array and sets its line renderer to the proper
        color based on _startDegree and _endDegree

        \return void
    */
    void setColors()
	{

		//set green and red with reduced opacity
		Color colorG = new Color(0.0f, 1.0f, 0.0f, 0.9f);
		Color colorR = new Color(1.0f, 0.0f, 0.0f, 0.9f);

		LineRenderer lRend;

		//set line colors accordingly
		//check start and end boundaries
		//green area fills upward from startDegree to endDegree
		for(int i = 0; i < lines.Length; i++)
		{

			lRend = lines[i].GetComponent<LineRenderer>();

			if(_startDegree < _endDegree)
			{

				if((_startDegree <= i) && (i <= _endDegree))
				{

					lRend.SetColors(colorG, colorG);
				}
				else
				{

					lRend.SetColors(colorR, colorR);

				}

			}
			else
			{
				if((_startDegree <= i) || (i <= _endDegree))
				{

					lRend.SetColors(colorR, colorG);

				}
				else
				{

					lRend.SetColors(Color.clear, colorR);

				}

			}

		}

	}

    /*!
        \brief  Deactivates all of the lines created by StealthClock.

        Iterates through the lines[] array and deactivates each game object.

        \return void
    */
    void deactivateLines()
    {

        for(int i = 0; i < lines.Length; i++)
        {

            if(lines[i] != null)
            {

                lines[i].SetActive(false);

            }
        }

    }

	/*!
		\brief Checks the aiManager to see if the player is hidden from enemies.
		
		Deactivates this gameObject if ai known to the AIManager have found the player.
		
		\return void
	*/
	void hideCheck()
	{

		//if the player is not hidden
		if(!aiMan.checkForPlayer())
		{
			
			//deactivate the miniGame
			gameObject.SetActive(false);
			
		}
		else
		{

			int searchingAI = 0;
			//check for at least one enemy looking for player
			for(int i = 0; i < aiMan.AiChildren.Length; i++)
			{

				//if searching for player
				if(aiMan.AiChildren[i].GetComponent<StatePatternEnemy>().currentState.ToString() == "ChasingPlayer");
				{

					searchingAI++;

				}
				if(searchingAI > 0)
				{

					gameObject.SetActive(false);

				}

			}

		}

	}

}
