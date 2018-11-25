using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandDetection : MonoBehaviour {

	public Questionnaire questionnaire;


	private void OnTriggerEnter(Collider other)
	{
		// Answers colliders
		if (other.tag == "Answer")
		{
			// Activate an answer and disable all the others
			for (int i = 0 ; i < questionnaire.answers.Length ; i++)
			{
				if (other.name == questionnaire.answers[i].name)
				{
					questionnaire.answers[i].enabled = true;
				}
				else
				{
					questionnaire.answers[i].enabled = false;
				}
			}
			// Play a sound when you touch an answer
			other.GetComponent<AudioSource>().Play();
		}

		// Validation collider
		if (other.tag == "Validate")
		{
			// Disable all the answers, go the next question (or end the questionnaire) and play a sound
			for (int i = 0 ; i < questionnaire.answers.Length ; i++)
			{
				if (questionnaire.answers[i].enabled)
				{
					questionnaire.answers[i].enabled = false;
					questionnaire.answerData = questionnaire.answers[i].name;
					questionnaire.validation = true;
					other.GetComponent<AudioSource>().Play();
					return;
				}
			}
		}
	}

}