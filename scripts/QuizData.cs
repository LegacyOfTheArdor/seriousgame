using System.Collections.Generic;

public class QuizData
{
	public List<QuestionData> questions { get; set; } = new();
}

public class QuestionData
{
	public string text { get; set; }
	public string image { get; set; }
	public List<AnswerData> answers { get; set; } = new();
}

public class AnswerData
{
	public string text { get; set; }
	public string image { get; set; }
	public List<OptionData> options { get; set; } = new();
}

public class OptionData
{
	public string title { get; set; }
	public string description { get; set; }
	public string positive { get; set; }
	public string negative { get; set; }
	public string value_1 { get; set; }
	public string value_2 { get; set; }
	public string value_3 { get; set; }
	public string value_4 { get; set; }
	public double score1 { get; set; }
	public double score2 { get; set; }
	public double score3 { get; set; }
	public double score4 { get; set; }
}
