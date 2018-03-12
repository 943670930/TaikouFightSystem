using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCtrl : MonoBehaviour {

	/*
	问题:移动距离有点短
	解决方法:在可移动范围内,尽量取大的值
	*/

//	private Vector3 oldPosition;
	public float selfAtkRange = 1.5f;
	public float selfMoveRange = 2;
	public float targetAtkRange = 0.5f;
	public float targetMoveRange = 0.5f;
	public GameObject targetGo;


	public UnitCtrl unitCtrl;
	void Awake(){

//		oldPosition = this.transform.position;
	}
	// Use this for initialization
	void Start () {
//		AI();

	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetKeyDown(KeyCode.K)){
			AI();
		}
	}

	void AI(){
//		Vector3 targetPos = targetGo.transform.position;
//		Vector3 targetDir = Vector3.zero;
//		Vector3 selfPos = this.transform.position;
//
//		Vector3 selfNewPos;
//		Vector3 selfNewDir;
//		//根据距离,决定不同的行动方式
//		float distance1 = (targetPos-selfPos).magnitude;
//		float distance2 = selfAtkRange + selfMoveRange;//未完成,还要考虑目标的移动
//		if(distance1 <= distance2){
//			//下回合可能攻击到目标
//			GetNewPosition(targetPos,targetDir,selfPos,selfAtkRange,selfMoveRange,out selfNewPos,out selfNewDir);
//
//		}else{
//			//下回合攻击不到目标
//			GetNewPosition(targetPos,selfPos,selfMoveRange,out selfNewPos,out selfNewDir);
//		}
//
//		MoveTo(selfNewPos);
//		unitCtrl.ATKDirectionChange (GetAngle (selfNewPos,targetPos));
		ForecastTargetNewPos();
		AttackTargetPos ();
	}

	//猜测目标下回合可能移动到的位置
	void ForecastTargetNewPos(){
		Vector3 selfPos = targetGo.transform.position;
		Vector3 targetDir = Vector3.zero;
		Vector3 targetPos = this.transform.position;

		Vector3 selfNewPos;
		Vector3 selfNewDir;
		//根据距离,决定不同的行动方式
		float distance1 = (targetPos-selfPos).magnitude;
		float distance2 = targetAtkRange + targetMoveRange;//未完成,还要考虑目标的移动
		if(distance1 <= distance2){
			//下回合可能攻击到目标
			GetNewPosition(targetPos,targetDir,selfPos,selfAtkRange,selfMoveRange,out selfNewPos,out selfNewDir);

		}else{
			//下回合攻击不到目标
			GetNewPosition(targetPos,selfPos,selfMoveRange,out selfNewPos,out selfNewDir);
		}

		targetNextPos = selfNewPos;
//		MoveTo(selfNewPos);
//		unitCtrl.ATKDirectionChange (GetAngle (selfNewPos,targetPos));
	}

	//向目标位置,发起攻击
	Vector3 targetNextPos = Vector3.zero;
	void AttackTargetPos(){
		Vector3 targetPos = targetNextPos;
		Vector3 targetDir = Vector3.zero;
		Vector3 selfPos = this.transform.position;

		Vector3 selfNewPos;
		Vector3 selfNewDir;
		//根据距离,决定不同的行动方式
		float distance1 = (targetPos-selfPos).magnitude;
		float distance2 = selfAtkRange + selfMoveRange;//未完成,还要考虑目标的移动
		if(distance1 <= distance2){
			//下回合可能攻击到目标
			GetNewPosition(targetPos,targetDir,selfPos,selfAtkRange,selfMoveRange,out selfNewPos,out selfNewDir);

		}else{
			//下回合攻击不到目标
			GetNewPosition(targetPos,selfPos,selfMoveRange,out selfNewPos,out selfNewDir);
		}

		MoveTo(selfNewPos);
		unitCtrl.DirectionChange (GetAngle (selfNewPos,targetPos));
	}

	//选择一个新的随机位置,该位置能攻击到目标
	void GetNewRandomPosition(Vector3 selfPos,Vector3 targetPos,Vector3 targetDir){
//		Vector3 targetPos = targetNextPos;
//		Vector3 targetDir = Vector3.zero;
//		Vector3 selfPos = this.transform.position;

		Vector3 selfNewPos;
		Vector3 selfNewDir;
		//根据距离,决定不同的行动方式
		float distance1 = (targetPos-selfPos).magnitude;
		float distance2 = selfAtkRange + selfMoveRange;//未完成,还要考虑目标的移动
		if(distance1 <= distance2){
			//下回合可能攻击到目标
			GetNewPosition(targetPos,targetDir,selfPos,selfAtkRange,selfMoveRange,out selfNewPos,out selfNewDir);

		}else{
			//下回合攻击不到目标
			GetNewPosition(targetPos,selfPos,selfMoveRange,out selfNewPos,out selfNewDir);
		}

		MoveTo(selfNewPos);
		unitCtrl.DirectionChange (GetAngle (selfNewPos,targetPos));

	}


	//当离目标较远,下回合攻击不到时采取的动作.
	void GetNewPosition(Vector3 targetPos,
		Vector3 selfPos,float selfMoveRange,
		out Vector3 selfNewPos,out Vector3 selfNewDir){

		selfNewPos = (targetPos - selfPos).normalized * selfMoveRange + selfPos;
		selfNewDir = GetAngle (selfPos,targetPos);

	}

	/// <summary>
	/// Gets the new position.
	/// 当下回合,可能攻击到目标时采取的动作.
	/// </summary>
	/// <param name="targetPos">Target position.目标的位置</param>
	/// <param name="selfPos">Self position.自己的位置</param>
	/// <param name="selfAtkRange">Self atk range.自身攻击范围</param>
	/// <param name="selfMoveRange">Self move range.自身移动范围</param>
	/// <param name="targetDir">Target dir.目标朝向</param>
	/// <param name="selfNewPos">Self new position.自身新的位置</param>
	/// <param name="selfNewDir">Self new dir.自身新的朝向</param>
	void GetNewPosition(Vector3 targetPos,Vector3 targetDir,
		Vector3 selfPos,float selfAtkRange,float selfMoveRange,
		out Vector3 selfNewPos,out Vector3 selfNewDir
	){
		selfNewPos = Vector3.zero;
		selfNewDir = Vector3.zero;
		/*
		以目标位置为圆心cA,自身攻击范围为半径rA,制造圆A
		以自身位置为圆心cB,自身移动范围为半径rB,制造圆B
		则两个圆相交的部分,即为自己可以攻击到目标的位置
		以下算法则是为了随机取一个这个范围内的点
		*/
		/*
		如何取这样的点呢.
		首先两个圆要相交,计算圆心距离即可,在相交的前提下.
		连接cA,cB,做线段AB,
		从cA出发,向cB做一条线段,长度范围为(AB长-rB)~rA.记线段的终点为C.
		过C做AB的垂线DE,DE与两圆相交,比较DE与两圆的交点,取较短距离的交汇点为FG.
		取FG上随机一点G,该点即为所求.
		*/
		//实现
		Vector3 cA = targetPos;
		Vector3 cB = selfPos;
		float rA = selfAtkRange;
		float rB = selfMoveRange;
		float AB = (cB - cA).magnitude;
		float rangeA = 0;
		if (AB >= rB) {
			rangeA = AB >= rB ? AB - rB : 0;
		} else if(AB < rB){
			rangeA = rB - AB;
		}

		float rangeB = 0;
		if (AB >= rA) {
			rangeB = AB >= rA ? rA : AB;
		} else if(AB < rA){
			rangeB = rA - AB;
		}
//		if(rangeB > rA){
//			rangeB = rA;
//		}
		float randomLength = Random.Range(rangeA,rangeB);
		Vector3 C = (cB - cA).normalized * randomLength;//从cA到C的线段
		Vector3 cAC = C + cA;
		float FG1 = rA*rA + randomLength*randomLength;
		float FG2 = rB*rB + (AB-randomLength)*(AB-randomLength);
		float FG = FG1 < FG2 ? Mathf.Sqrt(FG1) : Mathf.Sqrt(FG2);

		Vector3 G = RotationMatrix(cAC,90).normalized * Random.Range(-FG/2,FG/2);
		G += cAC;

		selfNewPos = G;
	}

	private void MoveTo(Vector3 position){
		this.transform.position = position;

//		oldPosition = position;
	}




	/// <summary>
	/// 旋转向量，使其方向改变，大小不变
	/// </summary>
	/// <param name="v">需要旋转的向量</param>
	/// <param name="angle">旋转的角度</param>
	/// <returns>旋转后的向量</returns>
	private Vector2 RotationMatrix(Vector2 v, float angle)
	{
		float x = v.x;
		float y = v.y;
		float sin = Mathf.Sin(Mathf.PI * angle / 180);
		float cos = Mathf.Cos(Mathf.PI * angle / 180);
		float newX = x * cos + y * sin;
		float newY = x * -sin + y * cos;
		return new Vector2((float)newX, (float)newY);
	}



	Vector3 GetAngle(Vector3 startPos,Vector3 targetPos){
		Vector3 newAngle = Vector3.zero;
		newAngle.z = Vector3.Angle(Vector3.right,targetPos-startPos);
		if((targetPos-startPos).y < 0){
			newAngle.z = 360 - newAngle.z;
		}
		return newAngle;
	}
}
