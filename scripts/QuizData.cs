using System;
using System.Collections.Generic;

[Serializable]
public class QuizData
{
	public string title = "Untitled Quiz";
	public List<string> sliderTitles = new List<string>();
	public List<Question> questions = new List<Question>();
}

[Serializable]
public class Question
{
	public string questionTitle = "";
	public List<Answer> answers = new List<Answer>();
}

[Serializable]
public class Answer
{
	public string answerTitle = "";
	public string description = "";
	public string upside = "";
	public string downside = "";
	public List<float> sliderValues = new List<float>();
}
