using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Questionnaire : MonoBehaviour {

	public MeshRenderer[] answers;
	public GameObject[] question;

	public bool validation;
	public bool endQuestion;

	// Question number and answer (for the save)
	public string actualQuestion;
	public string answerData;

	// Incrementation of the question
	public bool incQuestion = true;

	private int questionNbr = 1;


	// Update is called once per frame
	private void Update()
	{
		// Get the question number
		actualQuestion = question[questionNbr - 1].name;
	}

	// Swap for the next question
	public void SwapQuestion()
	{
		// If there's at least one question, swap
		if (questionNbr < question.Length)
		{
			questionNbr++;
			question[questionNbr - 2].SetActive(false);
			question[questionNbr - 1].SetActive(true);
		}
		// Else, end the questionnaire
		else
		{
			question[questionNbr - 1].SetActive(false);
			questionNbr = 1;
			question[0].SetActive(true);
			endQuestion = true;
		}
	}

}