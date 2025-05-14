using System.Collections.Generic;

public class QuizData
{
	public List<string> sliderTitles = new List<string>();
	public List<Question> questions = new List<Question>();
}

public class Question
{
	public string questionTitle = "";
	public List<Answer> answers = new List<Answer>();
}

public class Answer
{
	public string answerTitle = "";
	public string description = "";
	public string upside = "";
	public string downside = "";
	public List<float> sliderValues = new List<float>();
}
