using UnityEngine;
using UnityEngine.Experimental.Networking;
using System.Collections;
using SimpleJSON;

// Serve this database with the Node json-server prompt:
// dynamicsizedb.json

public class SampleConnectToServer : MonoBehaviour {

	// Global Variables

	// API access parameters
	string url;
	UnityWebRequest wr;

	// GameObjects to hold the results of the Wit sentence
	GameObject subject;
	GameObject destination;

	// Public movement paramaters
	public float yOffset;
	public float moveTime;


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

			//Start a coroutine called "WaitForRequest" with that WWW variable passed in as an argument
			StartCoroutine(GetJSONText());
		}


	}

	IEnumerator GetJSONText() {

		wr = UnityWebRequest.Get(url);
		yield return wr.Send ();

		if(wr.isError) {
			Debug.Log(wr.error);
		}
		else {
			// Show results as text
			Debug.Log(wr.downloadHandler.text);
			DoParse (wr.downloadHandler.text);

			// Or retrieve results as binary data
			// byte[] results = wr.downloadHandler.data;
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

		Vector3 destLoc = destination.transform.localPosition + new Vector3 (0.0f, yOffset, 0.0f);
		string destLocDebug = destination.transform.localPosition.ToString();

		// Now move the object
		// MoveObject ();
		StartCoroutine(MoveToPosition(destLoc, moveTime));

	}

	void MoveObject () {

		subject.transform.position = destination.transform.position + new Vector3(0.0f, yOffset, 0.0f);

		// subject.transform.position = Vector3.Lerp (subjectOriginalPosition, destination.transform.position, speed);


	}

	IEnumerator MoveToPosition(Vector3 newPosition, float time)
	{
		float elapsedTime = 0;
		Vector3 startingPos = subject.transform.position;
		while (elapsedTime < time)
		{
			print ("moving!");
			subject.transform.position = Vector3.Lerp(startingPos, newPosition, (elapsedTime / time));
			elapsedTime += Time.deltaTime;
			yield return null;
		}
	}


}
