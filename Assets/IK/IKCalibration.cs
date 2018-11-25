using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RootMotion.FinalIK;

public class IKCalibration : MonoBehaviour
{
    private VRIK m_VRIK;
    private SteamVR_Controller.Device m_controller_1, m_controller_2;

    private bool m_userCalibrated = false;

    private GameObject m_headIKTarget;
    private GameObject m_leftHandIKTarget;
    private GameObject m_rightHandIKTarget;
    private GameObject m_pelvisIKTarget;
    private GameObject m_leftToesIKtarget;
    private GameObject m_rightToesIKTarget;

    public Transform headTracker;
    public Transform leftHandTracker;
    public Transform rightHandTracker;

    public List<Transform> otherTrackers;

	public float avatarScale;
	public GameController gameController;
	public Activation activation;

    private Transform m_pelvisTracker;
    private Transform m_leftToesTracker;
    private Transform m_RightToesTracker;

    //public PointOfViewHandler pointOfViewHandler;

    public bool UserCalibrated
    {
        get { return m_userCalibrated; }
    }

    void Start ()
    {
        m_VRIK = transform.GetComponent<VRIK>();
        m_VRIK.solver.IKPositionWeight = 0;

		try
        {
            m_controller_1 = SteamVR_Controller.Input(SteamVR_Controller.GetDeviceIndex(SteamVR_Controller.DeviceRelation.Leftmost));
            m_controller_2 = SteamVR_Controller.Input(SteamVR_Controller.GetDeviceIndex(SteamVR_Controller.DeviceRelation.Rightmost));
        }
        catch (System.Exception e)
        {
            Debug.LogWarning("Please connect the Vive controllers " + e.ToString());
        }

        headTracker.GetChild(0).gameObject.SetActive(true);
        leftHandTracker.GetChild(0).gameObject.SetActive(true);
        rightHandTracker.GetChild(0).gameObject.SetActive(true);

        foreach (Transform tracker in otherTrackers)
            tracker.GetChild(0).gameObject.SetActive(true);
    }

    void Update ()
    {
        if(!m_userCalibrated && m_controller_1 != null && m_controller_2 != null)
        {
            SetAvatarHeight();

            if ((m_controller_1 != null && m_controller_1.GetPressDown(SteamVR_Controller.ButtonMask.Grip)) || (m_controller_1 != null && m_controller_2.GetPressDown(SteamVR_Controller.ButtonMask.Grip)))
            {
                SetIKTrackingPoints();
				activation.calibrationDone = true;
				foreach (GameObject walls in gameController.wallsList)
				{
					walls.transform.localScale = new Vector3(avatarScale, avatarScale, avatarScale);
				}

				// Changement de point de vue automatique lors de la calibration de l'avatar.
				//pointOfViewHandler.SwhitchPOV(true);
			}
        }

        if (Input.GetKeyDown(KeyCode.Space))
            SetIKTrackingPoints();
    }

    public void SetAvatarHeight ()
    {
        float actualEyeHeight = Camera.main.transform.localPosition.y;

        actualEyeHeight = Mathf.Clamp(actualEyeHeight, 0.7f, 2.5f);

        float scaleFactor = 0.6f;
        avatarScale = actualEyeHeight * scaleFactor;
        transform.localScale = new Vector3(avatarScale, avatarScale, avatarScale);
    }

    public void SetIKTrackingPoints ()
    {
        m_pelvisTracker = otherTrackers[0];
        int pelvisTrackerID = 0;

        for (int i = 1; i < otherTrackers.Count; i++)
        {
            if (otherTrackers[i].position.y > m_pelvisTracker.position.y)
            {
                m_pelvisTracker = otherTrackers[i];
                pelvisTrackerID = i;
            }
        }

        otherTrackers.Remove(otherTrackers[pelvisTrackerID]);

        if (otherTrackers.Count == 2)
        {
            if (otherTrackers[0].position.z > otherTrackers[1].position.z)
            {
                m_RightToesTracker = otherTrackers[0];
                m_leftToesTracker = otherTrackers[1];
            }
            else
            {
                m_RightToesTracker = otherTrackers[1];
                m_leftToesTracker = otherTrackers[0];
            }
        }

        else
            Debug.Log("Incorrect number of trackers");

        m_headIKTarget = Instantiate(Resources.Load("IKTarget", typeof(GameObject))) as GameObject;
        m_headIKTarget.transform.position = m_VRIK.references.head.transform.position;
        m_headIKTarget.transform.localRotation = m_VRIK.references.head.transform.rotation;
        m_headIKTarget.transform.parent = headTracker;
        //headTracker.GetChild(0).gameObject.SetActive(false);
        m_VRIK.solver.spine.headTarget = m_headIKTarget.transform;

        m_pelvisIKTarget = Instantiate(Resources.Load("IKTarget", typeof(GameObject))) as GameObject;
        m_pelvisIKTarget.transform.position = m_VRIK.references.pelvis.transform.position;
        m_pelvisIKTarget.transform.localRotation = m_VRIK.references.pelvis.transform.rotation;
        m_pelvisIKTarget.transform.parent = m_pelvisTracker;
        m_pelvisTracker.GetChild(0).gameObject.SetActive(false);
        m_VRIK.solver.spine.pelvisTarget = m_pelvisIKTarget.transform;

        m_leftHandIKTarget = Instantiate(Resources.Load("IKTarget", typeof(GameObject))) as GameObject;
        m_leftHandIKTarget.transform.position = m_VRIK.references.leftHand.transform.position;
        m_leftHandIKTarget.transform.localRotation = m_VRIK.references.leftHand.transform.rotation;
        m_leftHandIKTarget.transform.parent = leftHandTracker;
        leftHandTracker.GetChild(0).gameObject.SetActive(false);
        m_VRIK.solver.leftArm.target = m_leftHandIKTarget.transform;

        m_rightHandIKTarget = Instantiate(Resources.Load("IKTarget", typeof(GameObject))) as GameObject;
        m_rightHandIKTarget.transform.position = m_VRIK.references.rightHand.transform.position;
        m_rightHandIKTarget.transform.rotation = m_VRIK.references.rightHand.transform.rotation;
        m_rightHandIKTarget.transform.parent = rightHandTracker;
        rightHandTracker.GetChild(0).gameObject.SetActive(false);
        m_VRIK.solver.rightArm.target = m_rightHandIKTarget.transform;

        m_leftToesIKtarget = Instantiate(Resources.Load("IKTarget", typeof(GameObject))) as GameObject;
        m_leftToesIKtarget.transform.position = m_VRIK.references.leftToes.transform.position;
        m_leftToesIKtarget.transform.localRotation = m_VRIK.references.leftToes.transform.rotation;
        m_leftToesIKtarget.transform.parent = m_leftToesTracker;
        m_leftToesTracker.GetChild(0).gameObject.SetActive(false);
        m_VRIK.solver.leftLeg.target = m_leftToesIKtarget.transform;

        m_rightToesIKTarget = Instantiate(Resources.Load("IKTarget", typeof(GameObject))) as GameObject;
        m_rightToesIKTarget.transform.position = m_VRIK.references.rightToes.transform.position;
        m_rightToesIKTarget.transform.localRotation = m_VRIK.references.rightToes.transform.rotation;
        m_rightToesIKTarget.transform.parent = m_RightToesTracker;
        m_RightToesTracker.GetChild(0).gameObject.SetActive(false);
        m_VRIK.solver.rightLeg.target = m_rightToesIKTarget.transform;

        m_VRIK.solver.IKPositionWeight = 1;

        m_userCalibrated = true;
    }
}
