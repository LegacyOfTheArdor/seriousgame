using System.Collections.Generic;

// Root quiz data class
public class QuizData
{
	public string quizTitle { get; set; } = "Untitled Quiz";
	public List<string> sliderTitles { get; set; } = new List<string> { "Slider 1", "Slider 2", "Slider 3", "Slider 4" };
	public List<Question> questions { get; set; } = new List<Question>();
}

// Each question in the quiz
public class Question
{
	public string questionTitle { get; set; } = "Untitled Question";
	public List<Answer> answers { get; set; } = new List<Answer>();
}

// Each answer for a question
public class Answer
{
	public string answerTitle { get; set; } = "Untitled Answer";
	public string description { get; set; } = "";
	public string upside { get; set; } = "";
	public string downside { get; set; } = "";
	public List<float> sliderValues { get; set; } = new List<float> { 0, 0, 0, 0 };
}
