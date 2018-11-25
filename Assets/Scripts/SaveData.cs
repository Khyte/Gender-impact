using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class SaveData : MonoBehaviour {

	public int subjectNbr;
	public int nbrTouchedWalls;

	private int totalTouchedWalls = 0;

	public int[] wallsTouched;

	private string fileNameWalls;
	private string fileNameQuestion;

	public Questionnaire questionnaire;
	private GameController gameController;


	// Initialization
	private void Awake()
	{
		// Create a folder if it doesn't exist
		if (!Directory.Exists("SavedData"))
		{
			Directory.CreateDirectory("SavedData");
		}
		// Read the text file (subject number)
		StreamReader sr = new StreamReader("SavedData/SubjectNbr.txt");
		string fileContents = sr.ReadToEnd();
		sr.Close();

		subjectNbr = int.Parse(fileContents);

		// Create two csv files (excel)
		fileNameWalls = "SavedData/SavedData_Subject_number_" + subjectNbr + "_Walls.csv";
		fileNameQuestion = "SavedData/SavedData_Subject_number_" + subjectNbr + "_Questionnaires.csv";
		File.WriteAllText(fileNameWalls, "Touched walls MA ; Touched walls MM ; Touched walls FM ; Touched walls FA ; Total Touched walls ; Total time \r\n");
		File.WriteAllText(fileNameQuestion, "Avatar number ; Questions");

		gameController = GetComponent<GameController>();

		nbrTouchedWalls = 0;
	}

	// Update is called once per frame
	private void Update()
	{
		// If a question is complete, save this question and the answer
		if (questionnaire.validation)
		{
			questionnaire.validation = false;
			// If it's a new questionnaire, change the avatar number
			if (questionnaire.incQuestion)
			{
				questionnaire.incQuestion = false;
				File.AppendAllText(fileNameQuestion, "\r\n" + gameController.actualAvatar + " ; ");
			}
			File.AppendAllText(fileNameQuestion, questionnaire.actualQuestion + " : " + questionnaire.answerData + " ; ");

			questionnaire.SwapQuestion();
		}
	}

	// Keep the value of the number of the touched walls for each avatars
	public void SaveState()
	{
		wallsTouched[gameController.actualAvatar - 1] = nbrTouchedWalls;
		totalTouchedWalls += nbrTouchedWalls;
		nbrTouchedWalls = 0;
	}

	// Save this value if you leave the application
	void OnApplicationQuit()
	{
		for (int i = 0 ; i < wallsTouched.Length ; i++)
		{
			File.AppendAllText(fileNameWalls, wallsTouched[i] + " ; ");
		}
		File.AppendAllText(fileNameWalls, totalTouchedWalls + " ; ");
		File.AppendAllText(fileNameWalls, Mathf.Round(gameController.totalTimer * 100f) / 100f + "\r\n");
	}
}