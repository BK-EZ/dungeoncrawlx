using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System.IO;
using System.Linq;

// !!! THIS SCRIPT CURRENTLY DOES JACK SHIT !!!
// !!! THIS SCRIPT CURRENTLY DOES JACK SHIT !!!
// !!! THIS SCRIPT CURRENTLY DOES JACK SHIT !!!

// (it works but doesn't actually do anything to impact the game yet)

public class LetterGeneratorSCRIPT : MonoBehaviour
{

    int numOfLetters = 3;
    string[] lines = System.IO.File.ReadAllLines(@".\Assets\Lists\corncob_caps.txt");
    List<char> letters = new List<char>();
    System.Random rnd = new System.Random();
    int results;
    int wordsindic = File.ReadAllLines(@".\Assets\Lists\corncob_caps.txt").Count();
    int minresults = 610;

    public void FillLetters()
    {
        char c = '0';
        letters.Clear();
        letters.TrimExcess();

        for (int i = 0; i < numOfLetters; i++)
        {
            int rndnum = rnd.Next(1, 99);
            
            if (rndnum <= 12)
                c = 'E';
            else if (rndnum <= 21)
                c = 'A';
            else if (rndnum <= 30)
                c = 'I';
            else if (rndnum <= 38)
                c = 'O';
            else if (rndnum <= 44)
                c = 'N';
            else if (rndnum <= 50)
                c = 'R';
            else if (rndnum <= 56)
                c = 'T';
            else if (rndnum <= 60)
                c = 'L';
            else if (rndnum <= 64)
                c = 'S';
            else if (rndnum <= 68)
                c = 'U';
            else if (rndnum <= 72)
                c = 'D';
            else if (rndnum <= 75)
                c = 'G';
            else if (rndnum <= 77)
                c = 'B';
            else if (rndnum <= 79)
                c = 'C';
            else if (rndnum <= 81)
                c = 'M';
            else if (rndnum <= 83)
                c = 'P';
            else if (rndnum <= 85)
                c = 'F';
            else if (rndnum <= 87)
                c = 'H';
            else if (rndnum <= 89)
                c = 'V';
            else if (rndnum <= 91)
                c = 'W';
            else if (rndnum <= 93)
                c = 'Y';
            else if (rndnum <= 94)
                c = 'K';
            else if (rndnum <= 95)
                c = 'J';
            else if (rndnum <= 96)
                c = 'X';
            else if (rndnum <= 97)
                c = 'Q';
            else if (rndnum <= 98)
                c = 'Z';

            letters.Add(c);
            //Console.WriteLine("using " + c + "\n");
        }
    }

    public bool LettersAreValid()
    {
        bool flag1 = false;
        int lnschecked = 0;
        string testword;
        //string testwordcopy;
        string testchar;
        results = 0;
        while (flag1 == false && lnschecked != wordsindic)         //flag1 can be removed here!

        {
            flag1 = true;
            testword = lines[lnschecked];
            //testwordcopy = testword;
            //Console.WriteLine("checking with " + testword + "\n");
            letters.ForEach(delegate(char testc)
            {
                testchar = testc.ToString();
                if (testword.ToUpperInvariant().Contains(testchar) == false)
                {
                    flag1 = false;
                }
                else
                {
                    testword = testword.Remove(testword.IndexOf(testchar), 1);
                }
            });

            if (flag1)
            {
                results++;
                //Console.WriteLine(testwordcopy + " works!\n");
                //return true;
            }
            lnschecked++;
            flag1 = false;
        }
        //Console.WriteLine("no results\n");
        if (results > minresults)
        {
            return true;
        }
        return false;
    }

    void Start()
    {
        bool flagtest = true;
        while (flagtest == true)
        {
            bool flagmain = false;
            while (flagmain == false)
            {
                FillLetters();
                flagmain = LettersAreValid();
            }
            flagtest = false;
        }
    }   

    void OnGUI () 
    {
        //GUI.Box(new Rect (10,10,100,90), "Loader Menu");
    }

}