using Godot;
using System.Collections.Generic;

public partial class QuizResource : Resource
{
	[Export]
	public string QuizName { get; set; } = "";

	[Export]
	public Godot.Collections.Array<Question> Questions { get; set; } = new();
}

[GlobalClass]
public partial class Question : Resource
{
	[Export]
	public string Text { get; set; } = "";

	[Export]
	public Godot.Collections.Array<string> Answers { get; set; } = new();

	[Export]
	public int CorrectAnswerIndex { get; set; } = 0;
}
