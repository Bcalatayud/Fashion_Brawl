using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{    
    public Text timer;
    public Image P1_H;
    public float gameplayTime;
    public GameObject hudPanel;
    public GameObject pausePanel;
    public GameObject startPanel;
	public Slider volumeSlider;
	public Slider p1;
	public Image SliderFill;

    //
    private float currentTime;
	private float tiempo = 99f;
	private float var = 0f;
	private float damage1= 0f;

    void Start()
    {       
        Time.timeScale = 0f;
		gameplayTime = 10f;
		//P1_H.color = Color.green;
		//SliderFill.color = Color.green;
        ClearUI();
        startPanel.SetActive(true);
    }

    void Update()
    {
		tiempo -= var;
		//Debug.Log ("Tiempo es: "+ tiempo);
		timer.text = "" + tiempo;
		//Debug.Log (volumeSlider.value);
		AudioManager.instance.Volume (volumeSlider.value);
    }

    IEnumerator BarTime()
    {
		
        while (currentTime < gameplayTime)
        {
            yield return new WaitForSeconds(1f);
            //yield return new WaitForSecondsRealtime(1f);
            currentTime++;
			P1_H.fillAmount = 1 - (currentTime / gameplayTime);
			Debug.Log (P1_H.fillAmount);
			if (P1_H.fillAmount < 0.3f) {
				P1_H.color = Color.red;
				//SliderFill.color = Color.red;
			}
        }
        
    }

    void ClearUI()
    {
        hudPanel.SetActive(false);
        pausePanel.SetActive(false);
        startPanel.SetActive(false);
    }

    public void Pause()
    {
        ClearUI();        
        pausePanel.SetActive(true);
		GameManager.instance.ChangeToNewState(GameState.PAUSE);
        Time.timeScale = 0f;  
		var = 0f;
    }

    public void Continue()
    {
        ClearUI();        
        hudPanel.SetActive(true);
		GameManager.instance.ChangeToNewState(GameState.CONTINUE);
        Time.timeScale = 1f;
		var = 0.01f;
    }

    public void Exit()
    {
        ClearUI();
        Time.timeScale = 1f;
        SceneManager.LoadScene(0);
		GameManager.instance.ChangeToNewState(GameState.GAME_OVER);
        Application.Quit();
    }

    public void StartGame()
    {
        Time.timeScale = 0f;
        GameManager.instance.ChangeStateEvent += ShowPanel;
        //StartCoroutine("BarTime");
        GameManager.instance.ChangeToNewState(GameState.PLAYING);
		var = 0.01f;
    }

    void ShowPanel()
    {
        Debug.Log(GameManager.instance.currentState);
        switch (GameManager.instance.currentState)
        {
            case GameState.PLAYING:
                ClearUI();
                hudPanel.SetActive(true);               
                break;
        }

    }

	public void DownHealth()
	{
		//p1.value = p1.value - damage;
		//SliderFill.color = Color.red;
	}
}
