using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class CharControl : MonoBehaviour 
{

	private Vector2 touchDirection;
	private Vector2 startTouchPosition;
	private float gestureTime;
	private float swipeTime = 50f;
	private float jumpForce = 10f;
	private int actionValue2 = 0;

	Touch fingerTouch;

	[System.Serializable]
	public struct Action
	{
		public string	m_name;		//Action Animation Name
		public KeyCode	m_keyCode;	//Input Keycode
	};


	//---------------
	// public
	//---------------
	public string		m_characterName;

	public bool			m_enableControl = true;

	public float		m_turnSpeed = 10.0f;
	public float		m_moveSpeed = 2.0f;
	public float		m_runSpeedScale = 2.0f;	

	public Vector3		m_attackOffset = Vector3.zero;
	public float		m_attackRadius = 1.0f;
	public float		m_airTime = 0.0f;

	public GameObject	m_hitEffect = null;
	public GameObject	m_cameraTarget = null;

	public string[]		m_damageReaction;
	public Action[]		m_actionList;


	//---------------
	// private
	//---------------
	private CharacterController m_char = null;
	private Animator	m_ani = null;
	private Vector3		m_moveDir = Vector3.zero;
	private bool		m_isRun = false;
	private float		m_moveSpeedScale = 1.0f;
	private Collider m_coll;


	public Slider P2_H;
	public Image color;
	public int escena;

	private float var = 0;

	void Awake()
	{
		m_ani = GetComponent<Animator>();
		m_char = GetComponent<CharacterController>();
		//m_coll = GetComponent<Collider> ();
	}

	void Start () 
	{
		if( m_enableControl == true )
		{
			Select();
		}
	}


	public void Select()
	{
		m_enableControl = true;
		Message.Broadcast("OnChangeCharControl", this);
	}
	

	// Update is called once per frame
	void Update () 
	{
		//------------------
		//Parameters Reset
		//------------------
		m_moveDir = Vector3.zero;
		//P1_H.value -= 0.1f; 

		//------------------
		//Update Control
		//------------------
		if( m_enableControl == true )
		{
			m_moveSpeedScale = m_ani.GetFloat("SpeedScale");
			UpdateControl();
		}

		//------------------
		//Parameters sync
		//------------------
		float speed = m_moveDir.magnitude;
		if( m_isRun == true ) speed *= m_runSpeedScale;

		m_ani.SetFloat("Speed", speed, 0.05f, Time.deltaTime);
		m_ani.SetBool("Ground", m_char.isGrounded);
		m_ani.SetFloat("AirTime", m_airTime);

	}


	void FixedUpdate()
	{
		m_airTime = m_ani.GetBool("Ground") ? 0.0f : m_airTime + Time.deltaTime;
	}


	private void UpdateControl()
	{
		UpdateMoveControl();
		UpdateActionControl();
	}

	
	private void UpdateMoveControl()
	{
		Vector3 dir = Vector3.zero;
		Vector3 move = Vector3.zero;

		float h = Input.GetAxis("Horizontal");
		float v = Input.GetAxis("Vertical");

		#if UNITY_EDITOR

		dir.x = h;
		dir.z = 0; //Antes v

		#endif

		#if UNITY_IOS
		if (Input.touchCount == 1){
			fingerTouch = Input.GetTouch(0);

			if ((fingerTouch.position.x > Screen.width * 0.5f) && (fingerTouch.position.y < Screen.height * 0.5f)){
				transform.Translate (new Vector3 (0,0,1.0f * Time.deltaTime * 1.0f));
				//m_ani.SetTrigger("Walk");
				m_ani.SetFloat("Speed", 1.5f, 0.05f, Time.deltaTime);
				dir = Camera.main.transform.TransformDirection(0f,0f,1f);
			}
			else if ((fingerTouch.position.x < Screen.width * 0.5f) && (fingerTouch.position.y < Screen.height * 0.5f)){
				transform.Translate (new Vector3 (0,0,-1.0f * Time.deltaTime * 1.0f));
				//m_ani.SetFloat("Speed", fingerTouch.position.x, 0.05f, Time.deltaTime);
				m_ani.SetFloat("Speed", 1.8f, 0.05f, Time.deltaTime);
				dir = Camera.main.transform.TransformDirection(0f,0f,-1f);
			}
			else if((fingerTouch.position.x > Screen.width * 0.5f) && (fingerTouch.position.y > Screen.height * 0.5f)){
				//m_ani.SetTrigger("Jump");
				actionValue2 = actionValue2 + 1; 
				m_ani.SetInteger("Action", 1);
			}
			switch (fingerTouch.phase) {

				case TouchPhase.Began:
					startTouchPosition = fingerTouch.position;
					gestureTime = Time.time;
					break;

				case TouchPhase.Ended:
					touchDirection = fingerTouch.position - startTouchPosition;
					gestureTime = Time.time - gestureTime;
					if (gestureTime <= swipeTime && (touchDirection.magnitude >= Screen.height * 0.2f) && 
						Vector2.Angle (Vector2.up, touchDirection) <= 45) {
						m_ani.SetTrigger("Jump");
				} 
					break;
			}

		}
		/*if(Input.touchCount >1)
			m_ani.SetTrigger("Jump");*/


		#endif

		dir = Camera.main.transform.TransformDirection(dir);
		dir.y = 0.0f;

		dir = Vector3.ClampMagnitude(dir, 1.0f);


		m_moveDir = dir;

		//Run key check
		m_isRun = Input.GetKey(KeyCode.LeftShift);

		// run
		if( m_isRun == true )
		{
			dir *= m_runSpeedScale;
		}

	
		// default gravity
		move = Vector3.down * 0.5f * Time.deltaTime;

		
		// turn
		if( dir.magnitude > 0.01f )
		{
			transform.forward = Vector3.RotateTowards(transform.forward, dir, m_turnSpeed * Time.deltaTime, m_turnSpeed);
			move += dir * Time.deltaTime * m_moveSpeed * m_moveSpeedScale;
		}
		
		// jump
		if( Input.GetButtonDown("Jump") == true && m_char.isGrounded == true )
		{
			m_ani.SetTrigger("Jump");
		}

		
		// move
		if( move != Vector3.zero )
		{
			m_char.Move(move);
		}
			

	
	}


	// Check Action Input
	private void UpdateActionControl()
	{
		int actionValue = 0;

		for(int i = 0 ; i < m_actionList.Length ; i ++ )
		{
			#if UNITY_IOS
			if (Input.touchCount == 1){
				if((fingerTouch.position.x > Screen.width * 0.5f) && (fingerTouch.position.y > Screen.height * 0.5f)){
					actionValue = i + 1;
					switch (fingerTouch.phase) {
						case TouchPhase.Began:
							AudioManager.instance.PlayShot(0);
							break;
						case TouchPhase.Ended:
							AudioManager.instance.PlayShot(1);
							break;
						}
					break;
				}
			}
			#endif


			if(Input.GetKey(m_actionList[i].m_keyCode) == true )
			{
				actionValue = i + 1;
				break;
			}
		}

		m_ani.SetInteger("Action", actionValue);
	}



	//Animation Events
	void EventSkill(string skillName)
	{
		SendMessage(skillName, SendMessageOptions.DontRequireReceiver);
	}


	//Animation Events
	void EventAttack()
	{
		Vector3 center = transform.TransformPoint(m_attackOffset);
		float radius = m_attackRadius;


		Debug.DrawRay(center, transform.forward ,Color.red , 0.5f);

		Collider[] cols = Physics.OverlapSphere(center, radius);


		//------------------------
		//Check Enemy Hit Collider
		//------------------------
		foreach( Collider col in cols )
		{
			CharControl charControl  = col.GetComponent<CharControl>();
			if( charControl == null )
				continue;

			if( charControl == this)
				continue;

			charControl.TakeDamage(this, center , transform.forward , 1.0f);
		}
	}


	public void TakeDamage(CharControl other,Vector3 hitPosition,  Vector3 hitDirection, float amount)
	{

		//--------------------
		// direction example
		if( other != null )
		{
			transform.forward = -other.transform.forward;
		}
		else
		{
			hitDirection.y = 0.0f;
			transform.forward = -hitDirection.normalized;
		}

		//--------------------
		// reaction animation example
		string reaction = m_damageReaction[0/*Random.Range(0, m_damageReaction.Length)*/];	// random damage animation test
		m_ani.CrossFade(reaction, 0.1f, 0, 0.0f);
		//Health.fillAmount = 1 - (amount);

		/*
		P1_H.value = 1 - var;
		var += 0.2f;
		*/

		P2_H.value = 1 - var ;
		var += 0.1f;
		if (P2_H.value < 0.25) {
			color.color = Color.red;
		}
		if (P2_H.value == 0) { 			m_ani.SetTrigger ("Die");
			SceneManager.LoadScene(escena+1);
		}
		
		//--------------------
		// hitFX example
		GameObject.Instantiate(m_hitEffect, hitPosition, Quaternion.identity);
	}


	public string GetHelpText()
	{
		string text = "";

		foreach( Action action in m_actionList )
		{
			text += action.m_keyCode.ToString() + " : " + action.m_name + "\n";
		}

		return text;
	}


}
