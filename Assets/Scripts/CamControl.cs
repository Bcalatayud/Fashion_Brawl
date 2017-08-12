using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamControl : MonoBehaviour {

	public GameObject target;
	Vector3		m_defaultPosition = Vector3.zero;

	// Use this for initialization
	void Start () {
		m_defaultPosition = target.transform.position;
	}
	
	// Update is called once per frame
	void Update () {
		//m_defaultPosition = target.transform.position;
		m_defaultPosition.y = -1.1f;
		m_defaultPosition.x = target.transform.position.x - 6;
		m_defaultPosition.z = 8.46f;
		transform.position = m_defaultPosition;
		//transform.rotation.xyz = Vector3 (0,-270,0);
	}
}
