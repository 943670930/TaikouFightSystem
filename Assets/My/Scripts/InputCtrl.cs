using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InputCtrl : MonoBehaviour {
	/*
	UI怎么实现呢.
	将UI和单位的控制分离,靠游戏控制将二者联系在一起.
	操作分几种
	1.拖动操作,范围暂定为全图,拖到屏幕边缘时不可再拖动
	2.点击操作,用在选择单位的时候
	3.
	
	*/
	/*
	1.拖动操作
	屏幕上出现一个可拖动的
	*/

	[SerializeField]private Transform touchSign;

	/// <summary>
	/// The is touch.正在点击屏幕
	/// </summary>
//	public bool isTouch = false;
	/// <summary>
	/// The is touch sign.正在点击"触碰按钮"
	/// </summary>
	public bool isTouchSign = false;
	public Vector3 touchPosition;




	// Use this for initialization
	void Start () {
		

	}

	// Update is called once per frame
	void Update () {

		isTouchSign = false;
		if(Input.GetMouseButton(0)){
			if (touchSign.GetComponent<TouchSignMove> ().isTouch) {
				isTouchSign = true;
			}
		}


		if(isTouchSign){
			touchPosition = touchSign.position;
		}



	}

	public void TouchSignDisplay(Vector3 position){
		
		touchSign.gameObject.SetActive (true);
		touchSign.position = position;

	}

	public void MoveSignHidden(Vector3 position){

		touchSign.gameObject.SetActive (false);
//		moveSign.position = position;

	}

}

