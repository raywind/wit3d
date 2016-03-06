using UnityEngine;
using System.Collections;

public class RecordAudio : MonoBehaviour {


	public AudioClip myClip;
	int samplerate;

	void Start() {

		samplerate = 16000;

	}

	void Update(){

		if (Input.GetKey (KeyCode.P)) {
			print ("Start recording");
			myClip = Microphone.Start(null, false, 10, samplerate);  //Start recording (rewriting older recordings)
		}
		if (Input.GetKey (KeyCode.O)) {    //Stop recording
			print ("Stop recording");
			Microphone.End(null);
			SavWav.Save("sample", myClip);
		}
	}

}
