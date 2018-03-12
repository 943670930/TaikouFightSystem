using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeDisplay : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		IsTouch ();
	}

	public void Touch(){
		this.GetComponent<SpriteRenderer> ().color = Color.red;
	}

	public void EnTouch(){
		this.GetComponent<SpriteRenderer> ().color = Color.white;
	}

	//判断是否被技能范围触碰到
	bool IsTouch(){
		if (Physics2D.CircleCast (this.transform.position, 0.2f, Vector2.zero, 0f, LayerMask.GetMask ("Enemy"))) {
			Touch ();
			return true;
		} else {
			EnTouch ();
			return false;
		}


	}

}



