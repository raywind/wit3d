using UnityEngine;
using UnityEngine.Experimental.Networking;
using System.Collections;
using SimpleJSON;


public class HttpConnectToServer : MonoBehaviour {

	// Global Variables

	// API access parameters
	string url;
	string token;
	UnityWebRequest wr;
	UnityWebRequest retrieveAudio;

	// GameObjects to hold the results of the Wit sentence
	GameObject subject;
	GameObject destination;

	//Temporary: public request variables
	public string witMessage;
	byte[] data;

	// Public movement paramaters
	public float yOffset;
	public float moveTime;


	// Use this for initialization
	void Start () {

		// What's the WIT api token?
		token = "NJP2HHQXIUK3IGW53WXL65NRD74GGJ5B";

	}

	// Update is called once per frame
	void Update () {

		// Press 'space' to get the audio file

		if (Input.GetKeyDown("space")) {

			// Debug
			print("space key was pressed");

			//Grab the most up-to-date JSON file
			url = "https://api.wit.ai/speech?v=20141022";

			//Start a coroutine called "WaitForRequest" with that WWW variable passed in as an argument
			//StartCoroutine(GetJSONText());
			StartCoroutine(GetAudioFile());
		}

		// Press 'M' to see if the audio retrieval is done...
		// if it is, ship it off to Wit

		if (Input.GetKeyDown("m")) {

			// Debug
			print("m key was pressed");

			// See if it's done
			if (retrieveAudio.downloadHandler.isDone) {
				print ("audio retrieval done");
				StartCoroutine (GetJSONText ());
			} else {
				print ("download not done yet");
			}

		}


	}


	IEnumerator GetAudioFile() {

		string audioFileLocation = "http://localhost:3000/sample.wav";

		// get the audio file and encode it into byte[] data
		retrieveAudio = UnityWebRequest.Get (audioFileLocation);
		yield return retrieveAudio.Send ();

		if (retrieveAudio.isError) {
			print ("ERRA CAN't get audio");
		} else {
			print ("Got eem");
			// use a download handler here
			data = retrieveAudio.downloadHandler.data;
			// string dataString = data.ToString ();
			// print (dataString);
		}

	}

	IEnumerator GetJSONText() {

		wr = UnityWebRequest.Post (url,"test");

		// uploadhandler will include this in the body
		// UploadHandlerRaw upHandler = new UploadHandlerRaw(data);

		string authString = "Bearer " + token;
		wr.SetRequestHeader ("Authorization", authString);

		wr.SetRequestHeader ("Content-Type", "audio/wav");
		yield return wr.Send ();

		if(wr.isError) {
			print(wr.error);
			print ("ERROR!");
		}
		else {
			// Show results as text
			print(wr.downloadHandler.text);
			// DoParse (wr.downloadHandler.text);

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
