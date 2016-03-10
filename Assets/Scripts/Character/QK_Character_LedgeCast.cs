﻿using UnityEngine;
using System.Collections;

public class QK_Character_LedgeCast : MonoBehaviour {
	public Vector3 col_point;
	private bool ledge_found = false;
	private GameObject player;
	private int ticker = 0;
	void OnTriggerEnter(Collider col){
		if (col.gameObject.GetComponent<QK_Ledge> ()) {
			col_point = col.ClosestPointOnBounds (this.transform.position);
			player = GameObject.FindGameObjectWithTag ("Player");
			//apply hanging calculation to col_point
			col_point.y -= 1.8f;
			//player.GetComponent<QK_Character_Movement>()._stateModifier = CharacterStates.Hang;
			player.GetComponent<QK_Character_Movement>().ledge = col.gameObject;
			player.GetComponent<QK_Character_Movement>().endPos = col_point;
			player.transform.position = col_point;
			player.GetComponent<QK_Character_Movement>()._stateModifier = CharacterStates.Hang;

		}
	}
	public void FixedUpdate(){
		ticker++;
		if (ticker > 2) {//ToDo need to modify, might need to modify to keep the player from popping off the first time
			Erase();
		}
	}
	public void Start(){
		player = GameObject.FindGameObjectWithTag ("Player");
		//this.GetComponent<MeshRenderer>().enabled = false;
		Physics.IgnoreCollision(this.transform.GetComponent<Collider>(), player.transform.GetComponent<Collider>(), true);
	}
	public bool Check(){
		return ledge_found;
	}
	public void Erase(){
		Destroy (this.gameObject);
	}

	public Vector3 HangCoord(){
		return col_point;
	}
}