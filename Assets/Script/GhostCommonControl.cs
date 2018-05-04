using UnityEngine;
using System;

public class GhostCommonControl : MonoBehaviour {

	public Sprite DummySprite;
        public Sprite WaitingSprite;
        public Sprite SelectingSprite;
	//public Sprite Sprite;

	public GhostCommonControl() {
		Debug.Log("Constructed!!");
	}

	public void Hide() {
		var rend = GetComponent<SpriteRenderer>();
		rend.enabled = false;
	}

	public void Shadow() {
		Debug.Log("Shadow!!");	
		var rend = GetComponent<SpriteRenderer>();
		rend.sprite = DummySprite;
	}

	public void Selecting() {
		Debug.Log("Selecting!!");	
		//var rend = GetComponent<SpriteRenderer>();
		//rend.sprite = SelectingSprite;
		var anim = GetComponent<Animator>();
		anim.enabled = true;
	}

	public void Waiting() {
		Debug.Log("Waiting!!");	
		var rend = GetComponent<SpriteRenderer>();
		rend.sprite = WaitingSprite;

		var anim = GetComponent<Animator>();
		anim.enabled = false;
	}

	// Use this for initialization
	void Start () {
		Debug.Log("Start!!");	
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void Touch () {
		Debug.Log("Touch!!");
		Selecting();
	}

	public void Event () {
		Vector3 pos = Input.mousePosition;
		Vector3 lpos = Camera.main.ScreenToWorldPoint(pos);
		Vector3 cpos = gameObject.transform.position;
		lpos.z = cpos.z;
		//Debug.Log(lpos);
		gameObject.transform.position = lpos;
	}

	public void DragEnd () {
		Vector3 pos = Input.mousePosition;
		Vector3 lpos = Camera.main.ScreenToWorldPoint(pos);
		lpos.x = 0.5f + (float)Math.Round(lpos.x - 0.5f);
		lpos.y = 0.5f + (float)Math.Round(lpos.y - 0.5f);
		//Math.Truncate(lpos.x / 0.5f);
		Vector3 cpos = gameObject.transform.position;
		lpos.z = cpos.z;
		Debug.Log(lpos);
		gameObject.transform.position = lpos;
	}

	public void SetPosition (Vector3 pos) {
		Debug.LogFormat("SetPosition : {0}", pos);
		gameObject.transform.position = pos;
	}
}

