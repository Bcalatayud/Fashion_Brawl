﻿using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIManager2 : MonoBehaviour {

	public GameObject startPanel;
	public int escena;

	// Use this for initialization
	void Start () {
		startPanel.SetActive(true);
	}

	public void Enter()
	{
		SceneManager.LoadScene(escena+1);
		//Application.Quit();

	}

}
