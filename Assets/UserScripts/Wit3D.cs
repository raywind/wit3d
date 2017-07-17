/***********************************************************************************
MIT License

Copyright (c) 2016 Aaron Faucher

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

	The above copyright notice and this permission notice shall be included in all
	copies or substantial portions of the Software.

	THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
	IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
	FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
	AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
	LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
	OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
	SOFTWARE.

***********************************************************************************/

using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using SimpleJSON;
using System;
using System.Collections.Generic;
//using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
// using System.Web;

public partial class Wit3D : MonoBehaviour
{
    // Class Variables

    // Audio variables
    private AudioClip commandClip;
    int samplerate;

    // API access parameters
    string url;
    string token;
    UnityWebRequest wr;

    // Movement variables
    public float moveTime;
    public float yOffset;
    private int minFreq;
    private int maxFreq;
    private bool micConnected = false;

    // GameObject to use as a default spawn point
    public GameObject spawnPoint;
    private AudioSource goAudioSource;

    // Use this for initialization
    void Start()
    {
        if (Microphone.devices.Length <= 0)
        {
            //Throw a warning message at the console if there isn't
            UnityEngine.Debug.LogWarning("Microphone not connected!");
        }
        else //At least one microphone is present
        {
            //Set 'micConnected' to true
            micConnected = true;

            //Get the default microphone recording capabilities
            Microphone.GetDeviceCaps(null, out minFreq, out maxFreq);

            //According to the documentation, if minFreq and maxFreq are zero, the microphone supports any frequency...
            if (minFreq == 0 && maxFreq == 0)
            {
                //...meaning 44100 Hz can be used as the recording sampling rate
                maxFreq = 44100;
            }

            //Get the attached AudioSource component
            goAudioSource = this.GetComponent<AudioSource>();
        }
        // If you are a Windows user and receiving a Tlserror
        // See: https://github.com/afauch/wit3d/issues/2
        // Uncomment the line below to bypass SSL
        System.Net.ServicePointManager.ServerCertificateValidationCallback = (a, b, c, d) => { return true; };

        // set samplerate to 16000 for wit.ai
        samplerate = 16000;

    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetKeyDown(KeyCode.Space))
        {
            print("Listening for command");
            commandClip = Microphone.Start(null, false, 10, samplerate);  //Start recording (rewriting older recordings)
            goAudioSource.clip = commandClip;
        }


        if (Input.GetKeyUp(KeyCode.Space))
        {
            goAudioSource.Play(); //Playback the recorded audio
            STText.text = "Thinking...";

            StartCoroutine(Communication_IE());
        }

        if (witAiResponse != null)
        {
            Handle(witAiResponse);

            File.Delete(filePath); //Delete the Temporary Wav file
            witAiResponse = null;
        }
    }

    void Testdo()
    {

        // 在新线程上运行的代码
        Loom.RunAsync(() => {

            witAiResponse = GetJSONText(filePath);
            print(witAiResponse);
        });
        //在主线程上运行一些代码        
        Loom.QueueOnMainThread(() =>
        {

        }, 1);
    }
    string witAiResponse = null;
    string filePath = null;
    IEnumerator Communication_IE()
    {
        float filenameRand = UnityEngine.Random.Range(0.0f, 10.0f);

        string filename = "testing" + filenameRand;

        Microphone.End(null); //Stop the audio recording

        Debug.Log("Recording Stopped");

        if (!filename.ToLower().EndsWith(".wav"))
        {
            filename += ".wav";
        }

        filePath = Path.Combine("testing/", filename);
        filePath = Path.Combine(Application.persistentDataPath, filePath);
        Debug.Log("Created filepath string: " + filePath);

        // Make sure directory exists if user is saving to sub dir.
        Directory.CreateDirectory(Path.GetDirectoryName(filePath));
        SavWav.Save(filePath, goAudioSource.clip); //Save a temporary Wav File

        //// Debug
        //print("Thinking ...");

        //// Save the audio file
        //Microphone.End(null);
        //SavWav.Save("sample", commandClip);

        //// At this point, we can delete the existing audio clip
        //commandClip = null;

        ////Grab the most up-to-date JSON file
        //// url = "https://api.wit.ai/message?v=20160305&q=Put%20the%20box%20on%20the%20shelf";
        //token = "NJP2HHQXIUK3IGW53WXL65NRD74GGJ5B";
        token = "2DM3H5XA577I23K5ZYETFTPHAA5S3OWA";
        //Start a coroutine called "WaitForRequest" with that WWW variable passed in as an argument
        //string witAiResponse = GetJSONText("Assets/sample.wav");


        Testdo();

        yield return new WaitForEndOfFrame();// (0.1f);

        //Handle(witAiResponse);

        //File.Delete(filePath); //Delete the Temporary Wav file

    }

    string GetJSONText(string file)
    {

        // get the file w/ FileStream
        FileStream filestream = new FileStream(file, FileMode.Open, FileAccess.Read);
        BinaryReader filereader = new BinaryReader(filestream);
        byte[] BA_AudioFile = filereader.ReadBytes((Int32)filestream.Length);
        filestream.Close();
        filereader.Close();

        // create an HttpWebRequest
        HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://api.wit.ai/speech");


        request.Method = "POST";
        request.Headers["Authorization"] = "Bearer " + token;
        request.ContentType = "audio/wav";
        request.ContentLength = BA_AudioFile.Length;
        request.GetRequestStream().Write(BA_AudioFile, 0, BA_AudioFile.Length);

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

}