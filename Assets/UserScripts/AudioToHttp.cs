using UnityEngine;
using UnityEngine.Experimental.Networking;
using System.Collections;
using SimpleJSON;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
// using System.Web;

public class AudioToHttp : MonoBehaviour {

	// Class Variables

	// Audio variables
	public AudioClip commandClip;
	int samplerate;

	// API access parameters
	string url;
	string token;
	UnityWebRequest wr;

	// GameObjects to hold the results of the Wit sentence
	GameObject subject;
	GameObject destination;

	// Public movement paramaters
	public float yOffset;
	public float moveTime;


	// Use this for initialization
	void Start () {

		// set samplerate to 16000 for wit.ai
		samplerate = 16000;

	}

	// Update is called once per frame
	void Update () {

		if (Input.GetKeyDown (KeyCode.Space)) {
			print ("Listening for command");
			commandClip = Microphone.Start(null, false, 10, samplerate);  //Start recording (rewriting older recordings)
		}


		if (Input.GetKeyUp (KeyCode.Space)) {

			// Debug
			print("Thinking ...");

			// Save the audio file
			Microphone.End(null);
			SavWav.Save("sample", commandClip);

			// At this point, we can delete the existing audio clip
			commandClip = null;

			//Grab the most up-to-date JSON file
			// url = "https://api.wit.ai/message?v=20160305&q=Put%20the%20box%20on%20the%20shelf";
			token = "NJP2HHQXIUK3IGW53WXL65NRD74GGJ5B";

			//Start a coroutine called "WaitForRequest" with that WWW variable passed in as an argument
			string witAiResponse = GetJSONText("Assets/sample.wav");
			print (witAiResponse);
			DoParse (witAiResponse);
		}


	}

	string GetJSONText(string file) {

		// get the file w/ FileStream
		FileStream filestream = new FileStream (file, FileMode.Open, FileAccess.Read);
		BinaryReader filereader = new BinaryReader (filestream);
		byte[] BA_AudioFile = filereader.ReadBytes ((Int32)filestream.Length);
		filestream.Close ();
		filereader.Close ();

		// create an HttpWebRequest
		HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://api.wit.ai/speech");

		request.Method = "POST";
		request.Headers ["Authorization"] = "Bearer " + token;
		request.ContentType = "audio/wav";
		request.ContentLength = BA_AudioFile.Length;
		request.GetRequestStream ().Write (BA_AudioFile, 0, BA_AudioFile.Length);

		//		// Delete the temp file
		//		try
		//		{
		//			File.Delete(file);
		//		}
		//		catch
		//		{
		//			print("Unable to delete the temp file!" + Environment.NewLine + "Please do so yourself: " + file);
		//		}

		// Process the wit.ai response
		try
		{
			HttpWebResponse response = (HttpWebResponse)request.GetResponse();
			if (response.StatusCode == HttpStatusCode.OK)
			{
				print("Http went through ok");
				StreamReader response_stream = new StreamReader(response.GetResponseStream());
				return response_stream.ReadToEnd();
			}
			else
			{
				return "Error: " + response.StatusCode.ToString();
				return "HTTP ERROR";
			}
		}
		catch (Exception ex)
		{
			return "Error: " + ex.Message;
			return "HTTP ERROR";
		}       
	}

	void DoParse(string textToParse){

		print (textToParse);
		var N = JSON.Parse (textToParse);
		print ("SimpleJSON: " + N.ToString());

		string subjJson = N["outcomes"][0]["entities"]["subject"][0]["value"].Value.ToLower();
		print ("Subject: " + subjJson);

		string destJson = N["outcomes"][0]["entities"]["destination"][0]["value"].Value.ToLower();
		print ("Destination: " + destJson);

		//		string originJson = N["origin"].Value;
		//		print ("Origin: " + originJson);

		// Find the objects that were specified
		FindObjects (subjJson, destJson);

	}

	void FindObjects(string subjName, string destName) {

		if (subjName == "" || destName == "") {

			print ("Didn't understand you, try again.");

		} else {

			print ("FindObjects subject: " + subjName);
			print ("FindObjects destination: " + destName);

			if (GameObject.Find (subjName) != null && GameObject.Find (destName) != null) {

				subject = GameObject.Find (subjName);
				destination = GameObject.Find (destName);

				Vector3 subjectLoc = subject.transform.localPosition;
				string subjectLocDebug = subject.transform.localPosition.ToString ();
				print ("SubjectLoc: " + subjectLoc);

				Vector3 destLoc = destination.transform.localPosition + new Vector3 (0.0f, yOffset, 0.0f);
				string destLocDebug = destination.transform.localPosition.ToString ();

				// Now move the object
				// MoveObject ();
				StartCoroutine (MoveToPosition (destLoc, moveTime));
			
			} else {

				print("Either " + subjName + " or " + destName + " is confusing me.");

			}

		}

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