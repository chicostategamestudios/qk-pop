﻿using UnityEngine;
using UnityEditor;
using System.Collections;
using UnityEngine.Events;
using UnityEngine.UI;

/*
 * Scene Setup
 * 
 * This script's goal is to setup a new scene with every objects with its dependancies
 * necessary to start a level. It will spawn the player which should be playable. It should
 * resolve any error messages such as null references.
 */

public class Scene_Setup : EditorWindow {

	[MenuItem("Custom Tools/Setup Scene")]
	static void setup(){
		GameObject go;
		// managers
		GameObject objManager = GameObject.Find ("_ObjectManager");
		GameObject masterManager = GameObject.Find ("_MasterSceneManager");
		GameObject inputManager = GameObject.Find ("_InputManager");
        GameObject audioManager = GameObject.Find("_AudioManager");
        GameObject hudManager = GameObject.Find("_HUDManager");
		// player
		GameObject player = GameObject.Find ("_Player");
		GameObject camera = GameObject.Find("_Main Camera");
		GameObject abilitydockanim = GameObject.Find ("AbilityDock");
		// UI
		GameObject ui = GameObject.Find("UI");

		if (objManager == null) {
			go = new GameObject();
			go.AddComponent<ObjectManager>();
			go.name = "_ObjectManager";
			objManager = GameObject.Find ("_ObjectManager");
			ObjectManager.AddSavedObject(go.transform);
		}

		if (masterManager == null) {
			go = new GameObject();
			go.AddComponent<MasterSceneManager>();
			go.name = "_MasterSceneManager";
			masterManager = GameObject.Find ("_MasterSceneManager");
			MasterSceneManager manager = masterManager.GetComponent<MasterSceneManager>();
			ObjectManager.AddSavedObject(go.transform);
			manager.InitScenesDictionary();
		}

		if (inputManager == null) {
			go = new GameObject();
			go.AddComponent<InputManager>();
			go.name = "_InputManager";
			inputManager = GameObject.Find ("_InputManager");
			ObjectManager.AddSavedObject(go.transform);
		}

		if (audioManager == null) {
			go = new GameObject();
			go.AddComponent<AudioManager>();
			go.name = "_AudioManager";
			audioManager = GameObject.Find ("_AudioManager");
			ObjectManager.AddSavedObject(go.transform);
		}

        if (ui == null)
        {
            Object prefab = AssetDatabase.LoadAssetAtPath("Assets/Prefabs/UI/UI.prefab", typeof(GameObject));
            go = Instantiate(prefab, Vector3.zero, Quaternion.identity) as GameObject;
            go.name = "UI";
            ui = go;
        }

        if (hudManager == null)
        {
            go = new GameObject();
            GameHUD gameHud = go.AddComponent<GameHUD>();
            PauseMenu pauseMenu = go.AddComponent<PauseMenu>();
			DialogueManager dialogueManager = go.AddComponent<DialogueManager>();
            gameHud.accessManager = pauseMenu;
            pauseMenu.GHud = gameHud;
            go.name = "_HUDManager";
            hudManager = GameObject.Find("_HUDManager");
            ObjectManager.AddSavedObject(go.transform);
            GameHUD hud = go.GetComponent<GameHUD>();
            hud.pauseMenu = GameObject.Find("pauseMenu");
        }



        abilitydockanim = GameObject.Find("AbilityDock");
        if (abilitydockanim == null){
			Object prefab = AssetDatabase.LoadAssetAtPath("Assets/Prefabs/UI/AbilityDock.prefab", typeof(GameObject));
			go = Instantiate(prefab, Vector3.zero, Quaternion.identity) as GameObject;
			go.name = "AbilityDock";
			abilitydockanim = go;
            go.transform.parent = ui.transform;
		}

		if (player == null) {
			Object prefab = AssetDatabase.LoadAssetAtPath("Assets/Prefabs/_Player.prefab", typeof(GameObject));
			go = Instantiate(prefab, Vector3.zero, Quaternion.identity) as GameObject;
			go.name = "_Player";
			player = go;
            go.layer = LayerMask.NameToLayer("Player");
			Quinc quinc = go.GetComponent<Quinc>();
            quinc.abilitySelector = abilitydockanim.GetComponent<AbilityDockController>();
			ObjectManager.AddSavedObject(go.transform);
		}

		if (camera == null) {
			go = new GameObject();
			GameObject old = GameObject.Find("Main Camera");
			DestroyImmediate(old);
			go.AddComponent<Camera>();
			go.AddComponent<PoPCamera>();
			PoPCamera popc = go.GetComponent<PoPCamera>();
			popc.target = player.transform;
			go.name = "_Main Camera";
			go.tag = "MainCamera";
			camera = GameObject.Find("_Main Camera");
            Transform cameraTarget = GameObject.Find("CameraTarget").transform;
            if (cameraTarget != null)
            {
                popc.target = cameraTarget;
            }

            //Object icon_pull = AssetDatabase.LoadAssetAtPath("Assets/Prefabs/UI/HUDIcons/abilityButtonPull.prefab", typeof(GameObject));
            //Object icon_push = AssetDatabase.LoadAssetAtPath("Assets/Prefabs/UI/HUDIcons/abilityButtonPush.prefab", typeof(GameObject));
            //Object icon_shock = AssetDatabase.LoadAssetAtPath("Assets/Prefabs/UI/HUDIcons/abilityButtonShock.prefab", typeof(GameObject));

            //hud.hudAbilityIcons.Add(Instantiate(icon_pull, Vector3.zero, Quaternion.identity) as GameObject);
            //hud.hudAbilityIcons.Add(Instantiate(icon_push, Vector3.zero, Quaternion.identity) as GameObject);
            //hud.hudAbilityIcons.Add(Instantiate(icon_shock, Vector3.zero, Quaternion.identity) as GameObject);
			ObjectManager.AddSavedObject(go.transform);
		}

    }



}
