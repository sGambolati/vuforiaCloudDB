using System;
using System.Collections;
using System.Collections.Generic;
using Assets;
using UnityEngine;
using Vuforia;

public class SimpleCloudHandler : MonoBehaviour, ICloudRecoEventHandler
{

	private CloudRecoAbstractBehaviour cloudRecoBehavior;
	private bool isScanning = false;
	private string targetMetaData = string.Empty;

	public ImageTargetBehaviour imageTargetTemplate;

	public void OnInitError(TargetFinder.InitState initError)
	{
		throw new NotImplementedException();
	}

	public void OnInitialized()
	{
		Debug.Log("Cloud reco initialized");
	}
	//Here we handle a cloud target recognition event
	public void OnNewSearchResult(TargetFinder.TargetSearchResult targetSearchResult)
	{
		//do something with the metadata.
		this.targetMetaData = targetSearchResult.MetaData;

		if (!string.IsNullOrEmpty(targetSearchResult.MetaData))
		{
			var cardInfo = JsonUtility.FromJson<CardInfo>(targetSearchResult.MetaData);
            if (cardInfo != null)
            {

            }
		}

		//Stop the target finder (i.e. stop scanning the cloud)
		this.cloudRecoBehavior.CloudRecoEnabled = false;

		//Build augmentation based on target
		if (this.imageTargetTemplate)
		{
			//Enable the new result with the same ImageTargetBehavior
			ObjectTracker tracker = TrackerManager.Instance.GetTracker<ObjectTracker>();
			ImageTargetBehaviour imageTargetBehavior = (ImageTargetBehaviour)tracker.TargetFinder.EnableTracking(targetSearchResult, this.imageTargetTemplate.gameObject);
		}
	}

	public void OnStateChanged(bool scanning)
	{
		this.isScanning = scanning;
		if (this.isScanning)
		{
			// Clear all known trackables
			ObjectTracker tracker = TrackerManager.Instance.GetTracker<ObjectTracker>();
			tracker.TargetFinder.ClearTrackables(false);
		}
	}

	public void OnUpdateError(TargetFinder.UpdateState updateError)
	{
		Debug.Log("Cloud reco update error " + updateError.ToString());
	}

	void OnGUI()
	{
		//Display current scanning status
		GUI.Box(new Rect(100, 100, 200, 50), this.isScanning ? "Scanning" : "Not Scanning");
		//Displat metadata of lastest detected cloud-target
		GUI.Box(new Rect(100, 200, 200, 50), "MetaData: " + this.targetMetaData);
		//if not scanning, show button
		//so thar user can restart cloud scanning
		if (!this.isScanning)
		{
			if (GUI.Button(new Rect(100, 300, 200, 50), "Restart Scanning"))
			{
				//Restart TargetFinder
				this.cloudRecoBehavior.CloudRecoEnabled = true;
			}
		}
	}

	// Use this for initialization
	void Start ()
	{
		this.cloudRecoBehavior = GetComponent<CloudRecoBehaviour>();

		if (this.cloudRecoBehavior)
		{
			this.cloudRecoBehavior.RegisterEventHandler(this);
		}
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
