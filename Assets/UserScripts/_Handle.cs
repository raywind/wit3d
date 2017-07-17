using UnityEngine;
using System.Collections;
using SimpleJSON;
using UnityEngine.UI;
using System.Text.RegularExpressions;

public partial class Wit3D : MonoBehaviour
{
    public Text STText;

    void Handle(string textToParse)
    {

        print(textToParse);
        var N = JSON.Parse(textToParse);
        print("SimpleJSON: " + N.ToString());

        //string intent = N["outcomes"][0]["intent"].Value.ToLower();

        string intent = N["entities"]["intent"][0]["value"].Value.ToLower();
        Debug.Log(intent);

        intent = Regex.Replace(intent, @"\s+", " ").Trim();

        string sTText = N["_text"].Value;
        STText.text = FunctionStr(sTText)+".";

        // what function should I call?
        switch (intent)
        {
            case "turn off":
                Lighting(N, false);
                break;
            case "turn on":
                Lighting(N,true);
                break;
            case "move":
            case "move_object":
                print("Intent is MOVE OBJECT");
                MoveObject(textToParse);
                break;
            case "build":
            case "great":
            case "create":
            case "create_object":
                print("Intent is CREATE OBJECT");
                CreateObjectHandler(textToParse);
                break;
            default:
                print("Sorry, didn't understand your intent.");
                break;
        }
    }


    void Lighting(JSONNode textToParse,bool isOn)
    {
        string subjJson = textToParse["entities"]["subject"][0]["value"].Value.ToLower();

        print("Subject: " + subjJson);

        // Find the objects
        string s = Regex.Replace(subjJson, @"\s+", " ").Trim();
        GameObject subject = GameObject.Find(s);

        if (subject.GetComponent<Light>())
            subject.GetComponent<Light>().enabled = isOn;

    }

    public static string FunctionStr(string str)
    {
        string functionStr = str.Substring(0, 1).ToUpper() + str.Substring(1);
        return functionStr;
    }
}
