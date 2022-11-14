using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using Random = System.Random;

public class BallMovement : MonoBehaviour

{
    // total number of trials
    public int trial = 30;
    // location of ball, correlated to what's defined in unity
    public List<string> loc_list = new List<string> { "+0", "+30", "-30","+45","-45" };
    // location of log file
    string logFile = "TimeLog.txt";


    Random rnd = new Random();
    // t is index of trial
    public int t = 0;
    // lists of randomized color and position of ball, total length = trial
    public List<int> color_list = new List<int>();
    public List<int> pos_list = new List<int>();
    // save start time and response time (in ms)
    public List<double> start_list = new List<double>();
    public List<double> resp_list = new List<double>();
    // this is to prevent things get update many times in multiple coherent frames when button is pressed down
    public bool getkey = true;


    // Start is called before the first frame update
    void Start()
    {
        transform.position = GameObject.Find("disappear").transform.position;
        for (int i = 0; i < trial; i++)
        {
            color_list.Add(rnd.Next(3));
            pos_list.Add(rnd.Next(loc_list.Count));
        }
        File.WriteAllText(logFile, "Color" + "\t" + "Eccentricity" + "\t" + "StartTime" + "\t" + "ResponseTime" + "\n");
    }

    IEnumerator change(int rnd1, int rnd2)
    {
        transform.position = GameObject.Find("disappear").transform.position;
        if (rnd1 == 0)
        {
            GetComponent<Renderer>().material.color = Color.red;
        }
        else if (rnd1 == 1)
        {
            GetComponent<Renderer>().material.color = Color.green;
        }
        else
        {
            GetComponent<Renderer>().material.color = Color.blue;
        }
        yield return new WaitForSecondsRealtime(2);
        t++;
        transform.position = GameObject.Find(loc_list[rnd2]).transform.position;
        start_list.Add(DateTimeOffset.Now.ToUnixTimeMilliseconds());
        getkey = true;
    }

    IEnumerator wait_before_quit()
    {
        yield return new WaitForSecondsRealtime(2);
    }

    // Update is called once per frame

    void Update()
    {
        if (getkey && (Input.GetKey(KeyCode.V) | Input.GetKey(KeyCode.B) | Input.GetKey(KeyCode.N)))
        {
            resp_list.Add(DateTimeOffset.Now.ToUnixTimeMilliseconds());
            getkey = false;
            if (true)
            {
                StartCoroutine(change(color_list[t], pos_list[t]));
            }
        }
        else if ((Input.GetKey(KeyCode.Escape)) | (t > trial))
        {
            resp_list.RemoveAt(0);
            for (int i = 0; i < start_list.Count; i++)
            {
                string text;
                text = color_list[i].ToString() + '\t' + loc_list[pos_list[i]] + '\t';
                text += start_list[i].ToString() + '\t' + resp_list[i].ToString() + '\n';
                File.AppendAllText(logFile, text);
            }
            StartCoroutine(wait_before_quit());
            Application.Quit();
        }
    }
}
