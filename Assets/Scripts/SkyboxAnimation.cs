using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkyboxAnimation : MonoBehaviour {

	public Material skyboxMat;

	private float timerExposure = 0;
	private bool exposure = false;


	// Update is called once per frame
	private void Update()
	{
		// Skybox' rotation
		skyboxMat.SetFloat("_Rotation", Time.time * 0.2f);

		timerExposure += Time.deltaTime * 0.1f;

		// Skybox' exposure
		if (skyboxMat.GetFloat("_Exposure") > 1.3f)
		{
			exposure = true;
			timerExposure = 0;
		}
		else if (skyboxMat.GetFloat("_Exposure") < 0.7f)
		{
			exposure = false;
			timerExposure = 0;
		}
		if (exposure)
		{
			skyboxMat.SetFloat("_Exposure", -timerExposure + 1.3f);
		}
		else
		{
			skyboxMat.SetFloat("_Exposure", timerExposure + 0.7f);
		}
	}

}