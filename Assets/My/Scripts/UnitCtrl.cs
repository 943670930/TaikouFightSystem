using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitCtrl : MonoBehaviour {

	[SerializeField]GameObject atkRangeGo;
	public float atkRange = 0.5f;
	public float moveRange = 0.5f;

	void Awake(){
	}

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void MoveTo(Vector3 pos){
		this.transform.position = pos;
	}

	public void ATKRangeDisplay(){
		atkRangeGo.SetActive (true);

	}

	public void ATKRangeHidden(){
		atkRangeGo.SetActive (false);
	}

	public void DirectionChange(float newAngleZ){
		Vector3 newAngle = new Vector3 (0,0,newAngleZ);
		DirectionChange(newAngle);
	}

	public void DirectionChange(Vector3 newAngle){
		atkRangeGo.transform.localEulerAngles = newAngle;
	}

	public Vector3 GetATKDirection(){
		return atkRangeGo.transform.localEulerAngles;
	}

}
