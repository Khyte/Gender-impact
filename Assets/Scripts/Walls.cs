using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Walls : MonoBehaviour {

	public SaveData saveData;

	public MeshCollider meshCollider;

	private AudioSource audioSource;


	private void Start()
	{
		meshCollider = GetComponent<MeshCollider>();
		audioSource = GetComponent<AudioSource>();
	}

	private void OnEnable()
	{
		saveData = GameObject.FindGameObjectWithTag("GameController").GetComponent<SaveData>();
	}

	// If the wall collider touch the player (or his hand), this wall is touched (and disable the wall collider)
	private void OnTriggerEnter(Collider other)
	{
		if (other.tag == "Player" || other.tag == "Hand")
		{
			saveData.nbrTouchedWalls++;
			meshCollider.enabled = false;
			audioSource.Play();
		}
	}

}