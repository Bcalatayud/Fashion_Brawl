using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class CharControl1 : MonoBehaviour 
{

	//---------------
	// private
	//---------------
	private CharacterController m_char = null;
	private Animator	m_ani = null;

	void Awake()
	{
		m_ani = GetComponent<Animator>();
	}

	void Start () 
	{
		m_ani.SetTrigger("CatWalk");
	}
				
}