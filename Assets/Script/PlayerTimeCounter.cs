using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerTimeCounter : MonoBehaviour {
	public Text timerText;

	public float totalTime = 0.0f;

	// Use this for initialization
	void Start () {
		timerText = this.GetComponent<Text>();

	}

	// Update is called once per frame
	void Update () {
		if (totalTime < 0.0) {
			return;
		}
		totalTime -= Time.deltaTime;
		//var seconds = Math.Round(totalTime, 2);
		var seconds = (int)totalTime;
		timerText.text= seconds.ToString();
	}
}

