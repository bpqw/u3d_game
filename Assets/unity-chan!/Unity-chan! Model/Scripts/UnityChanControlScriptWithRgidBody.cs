//
// 带有Rigidbody的控制器示例，用于Mecanim动画数据不在原点移动的情况
// 示例
// 2014/03/13 N.Kobyasahi
//
using UnityEngine;
using System.Collections;

namespace UnityChan
{
// 列出所需的组件
	[RequireComponent(typeof(Animator))]
	[RequireComponent(typeof(CapsuleCollider))]
	[RequireComponent(typeof(Rigidbody))]

	public class UnityChanControlScriptWithRgidBody : MonoBehaviour
	{

		public float animSpeed = 1.5f;				// 动画播放速度设置
		public float lookSmoother = 3.0f;			// 相机运动的平滑设置
		public bool useCurves = true;				// 设置是否在Mecanim中使用曲线调整
		// 如果此开关未打开，则不会使用曲线
		public float useCurvesHeight = 0.5f;		// 曲线校正的有效高度（如果容易穿过地面，则增大此值）

		// 以下是角色控制器的参数
		// 前进速度
		public float forwardSpeed = 7.0f;
		// 后退速度
		public float backwardSpeed = 2.0f;
		// 旋转速度
		public float rotateSpeed = 2.0f;
		// 跳跃力量
		public float jumpPower = 3.0f; 
		// 角色控制器（胶囊碰撞体）的引用
		private CapsuleCollider col;
		private Rigidbody rb;
		// 角色控制器（胶囊碰撞体）的移动量
		private Vector3 velocity;
		// 用于存储CapsuleCollider设置的碰撞体的Height和Center的初始值的变量
		private float orgColHight;
		private Vector3 orgVectColCenter;
		private Animator anim;							// 对附加到角色的动画器的引用
		private AnimatorStateInfo currentBaseState;			// 用于引用动画器在base layer中当前状态的变量

		private GameObject cameraObject;	// 对主摄像机的引用
		
		// 对动画器各状态的引用
		static int idleState = Animator.StringToHash ("Base Layer.Idle");
		static int locoState = Animator.StringToHash ("Base Layer.Locomotion");
		static int jumpState = Animator.StringToHash ("Base Layer.Jump");
		static int restState = Animator.StringToHash ("Base Layer.Rest");

		// 初始化
		void Start ()
		{
			// 获取Animator组件
			anim = GetComponent<Animator> ();
			// 获取CapsuleCollider组件（胶囊型碰撞体）
			col = GetComponent<CapsuleCollider> ();
			rb = GetComponent<Rigidbody> ();
			// 获取主摄像机
			cameraObject = GameObject.FindWithTag ("MainCamera");
			// 保存CapsuleCollider组件的Height和Center的初始值
			orgColHight = col.height;
			orgVectColCenter = col.center;
		}
	
	
		// 以下是主要处理。由于涉及到刚体，所以在FixedUpdate中进行处理。
		void FixedUpdate ()
		{
			float h = Input.GetAxis ("Horizontal");				// 定义输入设备的水平轴为h
			float v = Input.GetAxis ("Vertical");				// 定义输入设备的垂直轴为v
			anim.SetFloat ("Speed", v);							// 将v传递给Animator中设置的"Speed"参数
			anim.SetFloat ("Direction", h); 						// 将h传递给Animator中设置的"Direction"参数
			anim.speed = animSpeed;								// 将animSpeed设置为Animator的动作播放速度
			currentBaseState = anim.GetCurrentAnimatorStateInfo (0);	// 将Base Layer (0)的当前状态设置到用于引用的状态变量
			rb.useGravity = true;// 跳跃时会关闭重力，除此之外都受重力影响
		
		
		
			// 以下是角色的移动处理
			velocity = new Vector3 (0, 0, v);		// 从上下键输入获取Z轴方向的移动量
			// 转换为角色的本地空间方向
			velocity = transform.TransformDirection (velocity);
			// 以下v的阈值与Mecanim侧的过渡一起调整
			if (v > 0.1) {
				velocity *= forwardSpeed;		// 乘以移动速度
			} else if (v < -0.1) {
				velocity *= backwardSpeed;	// 乘以移动速度
			}
		
			if (Input.GetButtonDown ("Jump")) {	// 如果按下空格键

				// 只有在Locomotion状态下才能跳跃
				if (currentBaseState.nameHash == locoState) {
					// 如果不在状态转换中，则可以跳跃
					if (!anim.IsInTransition (0)) {
						rb.AddForce (Vector3.up * jumpPower, ForceMode.VelocityChange);
						anim.SetBool ("Jump", true);		// 向Animator发送切换到跳跃的标志
					}
				}
			}
		

			// 通过上下键输入移动角色
			transform.localPosition += velocity * Time.fixedDeltaTime;

			// 通过左右键输入使角色在Y轴上旋转
			transform.Rotate (0, h * rotateSpeed, 0);	
	

			// 以下是Animator各状态下的处理
			// Locomotion状态下
			// 当前基础层为locoState时
			if (currentBaseState.nameHash == locoState) {
				// 如果正在使用曲线调整碰撞体，为了安全起见重置一下
				if (useCurves) {
					resetCollider ();
				}
			}
		// JUMP状态下的处理
		// 当前基础层为jumpState时
		else if (currentBaseState.nameHash == jumpState) {
				cameraObject.SendMessage ("setCameraPositionJumpView");	// 切换到跳跃时的摄像机
				// 如果状态不在过渡中
				if (!anim.IsInTransition (0)) {
				
					// 以下是使用曲线调整的情况下的处理
					if (useCurves) {
						// 以下是JUMP00动画中附带的曲线JumpHeight和GravityControl
						// JumpHeight: JUMP00中跳跃的高度（0~1）
						// GravityControl: 1⇒跳跃中（重力无效），0⇒重力有效
						float jumpHeight = anim.GetFloat ("JumpHeight");
						float gravityControl = anim.GetFloat ("GravityControl"); 
						if (gravityControl > 0)
							rb.useGravity = false;	// 在跳跃中切断重力的影响
										
						// 从角色的中心向下投射射线
						Ray ray = new Ray (transform.position + Vector3.up, -Vector3.up);
						RaycastHit hitInfo = new RaycastHit ();
						// 只有当高度大于等于useCurvesHeight时，才使用JUMP00动画中附带的曲线调整碰撞体的高度和中心
						if (Physics.Raycast (ray, out hitInfo)) {
							if (hitInfo.distance > useCurvesHeight) {
								col.height = orgColHight - jumpHeight;			// 调整后的碰撞体高度
								float adjCenterY = orgVectColCenter.y + jumpHeight;
								col.center = new Vector3 (0, adjCenterY, 0);	// 调整后的碰撞体中心
							} else {
								// 当低于阈值时恢复到初始值（以防万一）					
								resetCollider ();
							}
						}
					}
					// 重置Jump布尔值（防止循环）				
					anim.SetBool ("Jump", false);
				}
			}
		// IDLE状态下的处理
		// 当前基础层为idleState时
		else if (currentBaseState.nameHash == idleState) {
				// 如果正在使用曲线调整碰撞体，为了安全起见重置一下
				if (useCurves) {
					resetCollider ();
				}
				// 按下空格键进入Rest状态
				if (Input.GetButtonDown ("Jump")) {
					anim.SetBool ("Rest", true);
				}
			}
		// REST状态下的处理
		// 当前基础层为restState时
		else if (currentBaseState.nameHash == restState) {
				//cameraObject.SendMessage("setCameraPositionFrontView");		// 切换到正面摄像机
				// 如果状态不在过渡中，重置Rest布尔值（防止循环）
				if (!anim.IsInTransition (0)) {
					anim.SetBool ("Rest", false);
				}
			}
		}

		void OnGUI ()
		{
			GUI.Box (new Rect (Screen.width - 260, 10, 250, 150), "交互");
			GUI.Label (new Rect (Screen.width - 245, 30, 250, 30), "上/下箭头：前进/后退");
			GUI.Label (new Rect (Screen.width - 245, 50, 250, 30), "左/右箭头：向左转/向右转");
			GUI.Label (new Rect (Screen.width - 245, 70, 250, 30), "跑步时按空格键：跳跃");
			GUI.Label (new Rect (Screen.width - 245, 90, 250, 30), "停止时按空格键：休息");
			GUI.Label (new Rect (Screen.width - 245, 110, 250, 30), "左Ctrl：前视摄像机");
			GUI.Label (new Rect (Screen.width - 245, 130, 250, 30), "Alt：注视摄像机");
		}


		// 重置角色碰撞体大小的函数
		void resetCollider ()
		{
			// 将组件的Height和Center恢复到初始值
			col.height = orgColHight;
			col.center = orgVectColCenter;
		}
	}
}