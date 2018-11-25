using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour {

	// Data & list
	public int maximumWalls;
	public int wallCount = 0;
	public List<GameObject> wallsList = new List<GameObject>();
	public float totalTimer = 0;

	// Emissive mat color
	public Material[] emissiveMat;
	private float timerEmissive = 0;
	public Material[] wallsMat;
	private Material saveMat;
	public GameObject[] lightSpheres;

	// Avatar
	public List<int> avatarNbr = new List<int>();
	public int actualAvatar = 0;
	public GameObject avatars;
	public GameObject holograms;

	public GameObject screen;
	private Animator screenAnim;

	// Variables
	private int randomWall = 0;
	private int lastWall = 99;
	private int difficulty = 0;

	private float timer = 99f;

	private bool questionTime = false;

	private Vector3 initPos;

	public Questionnaire questionnaire;
	public GameObject holo;

	private Activation activation;
	private SaveData saveData;


	// Initialization
	private void Awake()
	{
		// List of avatars
		for (int i = 1 ; i <= 4 ; i++)
		{
			avatarNbr.Add(i);
		}

		// Initial emissive color
		for (int i = 0 ; i < emissiveMat.Length ; i++)
		{
			emissiveMat[i].SetColor("_EmissionColor", Color.cyan);
		}

		activation = GetComponent<Activation>();
		saveData = GetComponent<SaveData>();

		// Add walls to the list
		foreach (Transform child in transform)
		{
			foreach (Transform wallChild in child)
			{
				wallsList.Add(wallChild.gameObject);
			}
		}

		// The initial pos of all walls
		initPos = wallsList[0].transform.localPosition;

		// Deactivate walls display
		for (int i = 0 ; i < wallsList.Count ; i++)
		{
			wallsList[i].SetActive(false);
		}

		// Screen animator
		screenAnim = screen.GetComponent<Animator>();
		MoveScreen("active");
	}

	// Update is called once per frame
	private void Update()
	{
		// Push the questionnaire (admin mode)
		if (Input.GetKeyDown(KeyCode.Q))
		{
			Questionnaire();
		}

		totalTimer += Time.deltaTime;

		// Actual exp
		if (wallCount <= maximumWalls && activation.startExp)
		{
			timer += Time.deltaTime;

			MovingWalls();
		}
		// End actual exp and start questionnaire
		else if (wallCount > maximumWalls)
		{
			SoftReset();
			Questionnaire();
		}

		// Questionnaire
		if (questionTime)
		{
			timerEmissive += Time.deltaTime;

			for (int i = 0 ; i < emissiveMat.Length ; i++)
			{
				emissiveMat[i].SetColor("_EmissionColor", Color.Lerp(Color.yellow, Color.green, timerEmissive * 0.6f));
			}
		}

		// End of exp
		if (questionTime && questionnaire.endQuestion && avatarNbr.Count == 0)
		{
			questionTime = false;
			saveData.SaveState();
			lightSpheres[0].SetActive(false);
			lightSpheres[1].SetActive(false);
			for (int i = 0 ; i < emissiveMat.Length ; i++)
			{
				emissiveMat[i].SetColor("_EmissionColor", new Color(0, 0, 0));
			}
			holo.transform.parent.gameObject.SetActive(false);
		}
		// Start the next exp
		else if (questionTime && questionnaire.endQuestion)
		{
			questionnaire.endQuestion = false;
			questionnaire.incQuestion = true;
			activation.startExp = false;
			activation.restartExp = true;
			activation.startCheck = false;
			activation.chooseAvatar = false;
			questionTime = false;
			saveData.SaveState();
			timerEmissive = 0;

			for (int i = 0 ; i < emissiveMat.Length ; i++)
			{
				emissiveMat[i].SetColor("_EmissionColor", Color.cyan);
			}

			MoveScreen("unactive");
		}
	}

	// Moving 10 walls trial
	private void MovingWalls()
	{
		// Reset last wall & launch a new wall
		if (timer >= 15f)
		{
			if (wallCount > 0)
			{
				wallsList[randomWall].transform.localPosition = initPos;
				wallsList[randomWall].GetComponent<Walls>().meshCollider.enabled = true;
				wallsList[randomWall].SetActive(false);
			}

			wallCount++;

			if (wallCount >= 5 && wallCount <= 8)
			{
				difficulty = 6;
			}
			else if (wallCount > 8)
			{
				difficulty = 12;
			}

			do
			{
				randomWall = Random.Range(0 + difficulty, 6 + difficulty);
			}
			while (lastWall == randomWall);

			if (wallCount <= maximumWalls)
			{
				lastWall = randomWall;
				wallsList[randomWall].SetActive(true);
				timer = 0;
			}
		}
		else
		{
			timerEmissive += Time.deltaTime;

			if (wallCount == 1)
			{
				MoveScreen("unactive");
			}

			for (int i = 0 ; i < emissiveMat.Length ; i++)
			{
				emissiveMat[i].SetColor("_EmissionColor", Color.Lerp(Color.cyan, Color.yellow, timerEmissive * 0.6f));
			}

			if (wallsList[randomWall].transform.localPosition.z >= -31f)
			{
				wallsList[randomWall].transform.Translate(0, 0, -4f * Time.deltaTime);

				// Change mat after animation
				string value = wallsList[randomWall].GetComponent<Renderer>().material.name;
				value = value.Substring(0, value.Length - 11);
				if (wallsList[randomWall].name != value)
				{
					if (wallsList[randomWall].GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).normalizedTime > 1 && !wallsList[randomWall].GetComponent<Animator>().IsInTransition(0))
					{
						for (int i = 0 ; i < wallsMat.Length ; i++)
						{
							if (wallsList[randomWall].name == wallsMat[i].name)
							{
								saveMat = wallsList[randomWall].GetComponent<Renderer>().material;
								wallsList[randomWall].GetComponent<Renderer>().material = wallsMat[i];
							}
						}
					}
				}
			}
			// Wall disparition
			else if (wallsList[randomWall].transform.localPosition.y >= -3)
			{
				wallsList[randomWall].transform.Translate(0, -25 * Time.deltaTime, 0);
			}
			else
			{
				wallsList[randomWall].GetComponent<Animator>().Play("WallApparitionAnim", -1, 0f);
				wallsList[randomWall].GetComponent<Renderer>().material = saveMat;
			}
		}
	}

	// Choose a random avatar
	public void Avatar()
	{
		int value = Random.Range(0, avatarNbr.Count - 1);
		actualAvatar = avatarNbr[value];
		avatarNbr.RemoveAt(value);
		foreach (Transform child in avatars.transform)
		{
			child.gameObject.SetActive(false);
		}
		foreach (Transform child in holograms.transform)
		{
			child.gameObject.SetActive(false);
		}
		avatars.transform.GetChild(actualAvatar - 1).gameObject.SetActive(true);
		holograms.transform.GetChild(actualAvatar - 1).gameObject.SetActive(true);
	}

	// Soft reset between each trials
	private void SoftReset()
	{
		for (int i = 0 ; i < wallsList.Count ; i++)
		{
			wallsList[i].transform.localPosition = initPos;
			wallsList[i].SetActive(false);
		}

		activation.startExp = false;
		lastWall = 99;
		difficulty = 0;
		wallCount = 0;
		timer = 99f;
		timerEmissive = 0;
	}

	// Questionnaire after each trials
	private void Questionnaire()
	{
		questionTime = true;
		MoveScreen("active");
		holo.transform.GetChild(1).gameObject.SetActive(false);
		holo.transform.GetChild(2).gameObject.SetActive(true);
	}

	// Move screen
	private void MoveScreen(string text)
	{
		if (text == "active")
		{
			screenAnim.Play("Screen");
		}
		else if (text == "unactive")
		{
			screenAnim.Play("ScreenDown");
			screenAnim.speed = 2f;
		}
	}

}