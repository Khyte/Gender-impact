using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Activation : MonoBehaviour {

	// Timers booleans
	public bool startCheck;
	public bool startExp;
	public bool calibrationDone;
	public bool restartExp = true;
	public bool chooseAvatar;

	// Screen tuto
	public GameObject tuto;

	// Timers
	public float checkTimer;
	public float startTimer;

	private float timer = 0;


	// Update is called once per frame
	void Update ()
	{
		if (!startExp && restartExp)
		{
			if (timer < checkTimer)
			{
				timer += Time.deltaTime;
			}
			// Avatar's appearance
			else if (timer >= checkTimer && !startCheck)
			{
				startCheck = true;
			}
			// After calibration
			else if (calibrationDone && timer >= checkTimer && timer < startTimer)
			{
				timer += Time.deltaTime;
				tuto.GetComponent<Animator>().SetBool("NextTuto", true);
			}
			// After observation
			else if (timer >= startTimer)
			{
				calibrationDone = false;
				startExp = true;
				restartExp = false;
				GetComponent<AudioSource>().Play();
				timer = 0;
			}
		}

		if (startCheck)
		{
			// Choose random avatar
			if (!chooseAvatar)
			{
				chooseAvatar = true;
				GetComponent<GameController>().Avatar();
				checkTimer = 2f;
			}
		}
	}

}