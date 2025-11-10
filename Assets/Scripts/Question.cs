using UnityEngine;

[System.Serializable]
public class Question
{
    public Sprite flagImage;
    public string questionText;
    public string[] answers = new string[3];
    public int correctAnswerIndex;
    public string funFact;
}
