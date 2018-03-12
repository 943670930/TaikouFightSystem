using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FightSystem : MonoBehaviour {


	#region unitList 人员队列
	List<Unit> unitList = new List<Unit>();

	#endregion

	#region unitActionList 人员行动队列
	List<Unit> unitActionList = new List<Unit>();
	void UnitActionListClear(){
		unitActionList.Clear ();
	}
	void UnitActionListAdd(Unit unit){
		for (int i = 0; i < unitActionList.Count; i++) {
			if (i == unitActionList.Count - 1) {
				unitActionList.Add (unit);
				break;
			} else {
				Unit nextUnit = unitActionList [i + 1];
				if(unit.GetActionLevel() > nextUnit.GetActionLevel()){
					unitActionList.Insert (i,unit);
					Debug.Log ("检查插入顺序是否是按照actionLevel排序");
					break;
				}
			}
		}
	}
	void ExecAction(Unit unit){
		unit.action.Exec ();
	}
	#endregion

	#region
	[SerializeField]InputCtrl inputCtrl;
	[SerializeField]UnitCtrl player;
	[SerializeField]UnitCtrl enemy;
	[SerializeField]GameObject moveRange;

	//两个锚点,用来标注擂台的左下角和右上角坐标
	[SerializeField]Transform frame1;
	[SerializeField]Transform frame2;

	[SerializeField]private Button btnBack;
	[SerializeField]private Button btnSkill;
	[SerializeField]private Button btnConfirm;

	#endregion


	public float moveDistance = 1;

	// Use this for initialization
	void Start () {
		ConfirmBtnClick();
	}

	// Update is called once per frame
	void Update () {
		switch (roundPartState) {
		case 1:
			RoundPart1 ();
			break;
		case 2:
			RoundPart2 ();
			break;
		case 3:
			RoundPart3 ();
			break;
		default:
			break;
		}
	}


	void BtnEnable(){
		//显示按键
		btnBack.interactable = true;
		btnSkill.interactable = true;
		btnConfirm.interactable = true;
	}
	void BtnDisable(){
		//隐藏按键
		btnBack.interactable = false;
		btnSkill.interactable = false;
		btnConfirm.interactable = false;
	}


	#region Round
	//回合开始
	void RoundStart(){
		//遍历所有人员,将每个人都加入行动队列
		for (int i = 0; i < unitList.Count; i++) {
			UnitActionListAdd (unitList [i]);
		}
		//按顺序执行
		for (int i = 0; i < unitActionList.Count; i++) {
			ExecAction(unitActionList[i]);
		}
		//清空使用过的行动队列
		UnitActionListClear ();

	}

	int roundPartState = 0;
	//回合改变
	void RoundChange(int roundNum){
		switch (roundNum) {
		case 1:
			roundPart1State = 1;
			break;
		case 2:
			roundPart2State = 1;
			break;
		case 3:
			break;
		default:
			roundNum = 1;
			break;
		}

		roundPartState = roundNum;
	}

	void RoundToNext(){
		RoundChange (roundPartState+1);

	}


	//回合开始第1部分
	//玩家移动
	int roundPart1State = 1;
	void RoundPart1(){
		/*
		显示移动光标.
		显示移动范围
		隐藏攻击范围
		显示完成,技能按钮
		*/
		BtnDisable ();
		btnSkill.interactable = true;
		btnConfirm.interactable = true;

		if (roundPart1State == 1) {
			//返回之前的位置
			player.transform.position = oldPosition;
			player.DirectionChange (oldDirectionAngle);

			//将移动光标定位在玩家身上
			inputCtrl.TouchSignDisplay (player.transform.position);
			//移动范围参数设置
			moveRange.transform.position = player.transform.position;
			//显示移动范围
			moveRange.SetActive (true);
			//攻击范围隐藏
			player.ATKRangeHidden();
			//开始下一步
			roundPart1State++;
		} else if (roundPart1State == 2) {
			//开始接收玩家操作
			if (inputCtrl.isTouchSign) {
				//开始下一步
				roundPart1State = 3;
			}
		} else {
			//根据玩家输入,调整目标位置
			if (inputCtrl.isTouchSign) {
				//目标位置
				Vector3 targetPos = inputCtrl.touchPosition;
				//起始位置
				Vector3 startPos = moveRange.transform.position;

				//在移动范围内,找到目标位置
				targetPos = InMoveRange (startPos, targetPos, moveDistance);
				//在场地范围内,找到目标位置
				float minX = frame1.position.x;
				float maxX = frame2.position.x;
				float minY = frame1.position.y;
				float maxY = frame2.position.y;
				targetPos.x = GetFInRange (minX, maxX, targetPos.x);
				targetPos.y = GetFInRange (minY, maxY, targetPos.y);

				//将player定位在最终位置
				player.transform.position = targetPos;
			} else {
				//隐藏移动范围
				moveRange.SetActive (false);
				//开始下一步
				roundPart1State = 1;
				RoundToNext ();
			}
		}

	}


	//回合开始第2部分
	//光标定位,显示单位朝向
	int roundPart2State = 1;
	void RoundPart2(){
		/*
		显示移动光标.
		显示攻击范围
		隐藏移动范围
		*/
		BtnDisable ();
		btnBack.interactable = true;
		btnConfirm.interactable = true;

		if (roundPart2State == 1) {
			//将触控指示器定位在玩家前方
			inputCtrl.TouchSignDisplay (player.transform.position+player.transform.right);
			//显示范围
			player.ATKRangeDisplay();
			//隐藏移动范围
			moveRange.SetActive (false);
			//开始下一步
			roundPart2State ++;
		} else if (roundPart2State == 2) {
			//开始接收输入
			if(inputCtrl.isTouchSign){
				//开始下一步
				roundPart2State ++;
			}
		} else {
			//根据输入,调整朝向
			if (inputCtrl.isTouchSign) {
				//目标位置
				Vector3 targetPos = inputCtrl.touchPosition;
				//起始位置
				Vector3 startPos = player.transform.position;

				Vector3 newAngle = GetAngle (startPos,targetPos);

				player.DirectionChange (newAngle);

				//player朝向目标位置
				//可以在这里设置单位的显示动画
				newAngle.z = (newAngle.z +360)%360;
				if (newAngle.z >= 270 || newAngle.z <= 90) {
					//面向右
					Debug.Log ("right");
				} else {
					//面向左
					Debug.Log ("left");
				}
			} else{

				RoundToNext ();
			}
		}
	}

	//回合开始第三部分
	void RoundPart3(){
		//记录现在玩家的位置和朝向,当回滚的时候可以使用
		oldPosition = player.transform.position;
		oldDirectionAngle = player.GetATKDirection ();

		EnemyAction();
		RoundToNext ();
	}

	//敌人行动
	void EnemyAction(){
		//敌人行动一次.
		UnitCtrl selfCtrl = enemy;
		UnitCtrl targetCtrl = player;
		Vector3 targetNewPos = GetNewPosition(targetCtrl.transform.position,selfCtrl.transform.position,targetCtrl.atkRange,targetCtrl.moveRange);
		Vector3 selfNewPos = GetNewPosition(selfCtrl.transform.position,targetNewPos,selfCtrl.atkRange,selfCtrl.moveRange);

		selfCtrl.MoveTo(selfNewPos);
		selfCtrl.DirectionChange(GetAngle(selfNewPos,targetNewPos));
	}

	//点击结束按钮
	Vector3 oldPosition = Vector3.zero;
	Vector3 oldDirectionAngle = Vector3.zero;
	public void ConfirmBtnClick(){
		moveRange.SetActive (false);
		player.ATKRangeHidden ();

		EnemyAction();



		RoundChange (1);
	}

	//点击返回按钮
	public void BackBtnClick(){
		moveRange.SetActive (false);
		player.ATKRangeHidden ();

		RoundChange (1);
	}

	//点击技能按钮
	public void SkillBtnClick(){
		moveRange.SetActive (false);
		player.ATKRangeHidden ();

		RoundChange (1);
	}

	#endregion


	#region AI

	//根据距离远近,决定是自己的移动位置
	Vector3 GetNewPosition(Vector3 selfPos,Vector3 targetPos,float selfAtkRange,float selfMoveRange){
		//站在目标的位置思考,将自己设为敌人,目标设为攻击发起方
//		Vector3 selfPos = targetPos;
		Vector3 targetDir = Vector3.zero;
//		Vector3 targetPos = selfPos;
//		float selfAtkRange = targetAtkRange;
//		float selfMoveRange = targetMoveRange;

		Vector3 selfNewPos;
		Vector3 selfNewDir;
		//根据距离,决定不同的行动方式
		float distance1 = (targetPos-selfPos).magnitude;
		float distance2 = selfAtkRange + selfMoveRange;//未完成,还要考虑目标的移动
		if(distance1 <= distance2){
			//下回合可能攻击到目标
			GetNewPositionClose(targetPos,targetDir,selfPos,selfAtkRange,selfMoveRange,out selfNewPos,out selfNewDir);

		}else{
			//下回合攻击不到目标
			GetNewPositionFar(targetPos,selfPos,selfMoveRange,out selfNewPos,out selfNewDir);
		}

		 return selfNewPos;
	}

	




	//当离目标较远,下回合攻击不到时采取的动作.
	void GetNewPositionFar(Vector3 targetPos,
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
	void GetNewPositionClose(Vector3 targetPos,Vector3 targetDir,
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


	#endregion

	#region Tools
	Vector3 GetAngle(Vector3 startPos,Vector3 targetPos){
		Vector3 newAngle = Vector3.zero;
		newAngle.z = Vector3.Angle(Vector3.right,targetPos-startPos);
		if((targetPos-startPos).y < 0){
			newAngle.z = 360 - newAngle.z;
		}
		return newAngle;
	}
	float GetFInRange(float min,float max,float target){
		if (target >= min && target <= max) {
			return target;
		} else if (target < min) {
			return min;
		} else {
			return max;
		}
	}

	//传入起点和终点,考虑可移动范围,返回实际能到达的位置
	Vector3 InMoveRange(Vector3 startPos,Vector3 targetPos,float moveDistance){
		if ((targetPos - startPos).magnitude <= moveDistance) {
			return targetPos;
		} else {
			return startPos + (targetPos - startPos).normalized * moveDistance;
		}
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




	#endregion


}

public class Action{
	/*
	技能分几个属性
	0.ID
	1.名称
	2.技能等级值
	3.数值
	3.2效果
	4.范围
	4.2生效目标数
	4.3生效目标类型(友或敌)
	5.消耗类型
	6.消耗量
	*/
	/*
	范围类型
	1.自定义范围内全部
	2.自定义范围内一个
	3.自身
	*/

	public int id;
	public string name;
	public float actionLevelBase;
	public EffectType effect1;
	public EffectType effect2;
	public EffectType effect3;
	public float value1;
	public float value2;
	public float value3;
	public int range;
	public int targetNumber = 0;//0是所有目标
	public TargetType targetType = TargetType.Enemy;
	public CostType costType = CostType.MP;
	public int cost;

	public void Exec(){
//		Range
	}

}


/*
技能种类:
防守技,基础值300+攻击力/10
攻击技,基础值200+攻击力/10
Buff/Debuff/恢复/其他,基础值100+攻击力/10
普通攻击/防御,基础值0+攻击力/10
*/
//技能等级基础值
public class ActionLevelBase{
	public const float Def = 300;//防守技
	public const float Atk = 200;//攻击技
	public const float Auxiliary = 100;//辅助
	public const float Base = 0;//基础,攻击防御等
}

public enum TargetType{
	Enemy = 0,
	Friend = 1
}

public enum EffectType{
	Damage = 0,//可以扣血,可以回血
	Vertigo = 1,//眩晕
	Chaos = 2,//混乱
	ATKInc = 3,//攻击力增加
	DEFInc = 4,//防御力增加
	AGIInc = 5,//敏捷(移动力)增加

}

//单位
public class Unit{
	public int unitID;
	public int ATK;
	public Action action;
	public float GetActionLevel(){
		return action.actionLevelBase + ATK;
	}


}

public enum CostType{
	MP = 0,
	HP = 1,
	Gold = 2
}



