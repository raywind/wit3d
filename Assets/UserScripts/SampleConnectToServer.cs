using UnityEngine;
using System.Collections;
using SimpleJSON;

// Serve this database with the Node json-server prompt:
// dynamicsizedb.json

public class SampleConnectToServer : MonoBehaviour {

	// Global Variables

	// API access parameters
	string url;
	WWW www;

	// GameObjects to hold the results of the Wit sentence
	GameObject subject;
	GameObject destination;
	public float yOffset;


	// Use this for initialization
	void Start () {

	}

	// Update is called once per frame
	void Update () {

		if (Input.GetKeyDown("space")) {

			// Debug
			print("space key was pressed");

			//Grab the most up-to-date JSON file
			url = "http://localhost:3000/json/witresponse.json";
			//Create a WWW variable to store the WWW request to that URL
			www = new WWW(url);

			//Start a coroutine called "WaitForRequest" with that WWW variable passed in as an argument
			StartCoroutine(WaitForRequest(www));
		}


	}

	//Coroutine WaitForRequest
	IEnumerator WaitForRequest(WWW www)
	{

		yield return www;
		// check for errors
		if (www.error == null) {
			Debug.Log ("WWW Ok!: " + www.text);
			//start the "DoParse" function.
			DoParse(www.text);
		} else {
			Debug.Log ("WWW Error: " + www.error);
		}
	}

	void DoParse(string textToParse){

		print (textToParse);
		var N = JSON.Parse (textToParse);
		print ("SimpleJSON: " + N.ToString());

		string subjJson = N["subject"].Value;
		print ("Subject: " + subjJson);

		string destJson = N["destination"].Value;
		print ("Destination: " + destJson);

		string originJson = N["origin"].Value;
		print ("Origin: " + originJson);

		// Find the objects that were specified
		FindObjects (subjJson, destJson);

	}
		
	void FindObjects(string subjName, string destName) {

		print ("FindObjects subject: " + subjName);
		print ("FindObjects destination: " + destName);

		subject = GameObject.Find (subjName);
		destination = GameObject.Find (destName);

		Vector3 subjectLoc = subject.transform.localPosition;
		string subjectLocDebug = subject.transform.localPosition.ToString();
		print ("SubjectLoc: " + subjectLoc);

		Vector3 destLoc = destination.transform.localPosition;
		string destLocDebug = destination.transform.localPosition.ToString();

		// Now move the object
		MoveObject ();

	}

	void MoveObject () {

		subject.transform.position = destination.transform.position + new Vector3(0.0f, yOffset, 0.0f);

	}

}
