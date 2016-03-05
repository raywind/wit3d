using UnityEngine;
using System.Collections;
using SimpleJSON;

// Serve this database with the Node json-server prompt:
// dynamicsizedb.json

public class SampleConnectToServer : MonoBehaviour {

	// Global Variables

	// GameObjects to hold the results of the Wit sentence
	GameObject subject;
	GameObject destination;

	// Use this for initialization
	void Start () {

	}

	// Update is called once per frame
	void Update () {

		//Create a string variable to store the URL
		string url = "http://localhost:3000/json/witresponse.json";
		//Create a WWW variable to store the WWW request to that URL
		WWW www = new WWW(url);
		//Start a coroutine called "WaitForRequest" with that WWW variable passed in as an argument
		StartCoroutine(WaitForRequest(www));

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

		// Move the subject 
		// MoveSubject();

	}
		
	void FindObjects(string subjName, string destName) {

		print ("FindObjects subject: " + subjName);
		print ("FindObjects destination: " + destName);

		subject = GameObject.Find (subjName);
		destination = GameObject.Find (destName);

		string subjectLoc = subject.transform.localPosition.ToString();
		print ("SubjectLoc: " + subjectLoc);

	}

}
