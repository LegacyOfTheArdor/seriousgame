using System.Collections.Generic;

public class QuizData
{
	public List<string> sliderTitles { get; set; } = new List<string>();
	public List<Question> questions { get; set; } = new List<Question>();
}

public class Question
{
	public string questionTitle { get; set; }
	public List<Answer> answers { get; set; } = new List<Answer>();
}

public class Answer
{
	public string answerTitle { get; set; }
	public string description { get; set; }
	public string upside { get; set; }
	public string downside { get; set; }
	public List<float> sliderValues { get; set; } = new List<float>();
}
