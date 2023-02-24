using UnityEngine;
using System.Collections;

public class ClockScript : MonoBehaviour {

	float[] timer;
	public UnityEngine.UI.Text[] TimeText;
	// Use this for initialization
	void Start () {
		timer = new float[2];
		UpdateTime();
    }

	public string secondsToTime(int i)
	{
		int m= (int)(timer[i] / 60);
		string min=m.ToString();
		float s = Mathf.Round((timer[i] - 60f * m) * 100f) / 100f;
        string sec = s.ToString();
		if(s<1)
			sec= "0"+sec;
		else if (s < 10)
			sec = "0" + sec;
		if(sec.Length==2)
			sec= sec+".00";
		else if(sec.Length==4)
            sec = sec + "0";
        return min+":"+sec;
	}

	public void UpdateTime()
	{
		timer[0] = GameControl.singleton.ClockSlider.value * 60;
		timer[1] = timer[0];
		GameControl.singleton.transform.transform.GetChild(3).GetChild(0).GetChild(1).GetComponent<UnityEngine.UI.Text>().text = "Game in " + GameControl.singleton.ClockSlider.value.ToString() + " min";
		TimeText[0].text = secondsToTime(0);
		TimeText[1].text = secondsToTime(1);
	}

	// Update is called once per frame
	void Update () {
		if (GameControl.singleton.CurrentMode == GameControl.GameMode.Play)
		{
			int index = 0;
			if (!GameControl.singleton.playerOne)
				index = 1;
			timer[index] -= Time.deltaTime;
			TimeText[index].text = secondsToTime(index);
			if(timer[index]<=0)
			{
				GameControl.singleton.CurrentMode= GameControl.GameMode.Over;
				TimeText[index].text = "Loss on Time";
			}
		}
	}
}
