using Godot;
using System.Collections.Generic;

public partial class QuizSelection : Control
{
	private VBoxContainer quizList;
	private Button addQuizButton;
	private bool editMode = false;

	public override void _Ready()
	{
		quizList = GetNode<VBoxContainer>("ScrollContainer/VBoxContainer");
		addQuizButton = GetNode<Button>("AddQuizButton");
		addQuizButton.Visible = editMode;

		LoadQuizzes();
	}

	public void SetEditMode(bool isEditMode)
	{
		editMode = isEditMode;
		addQuizButton.Visible = editMode;
	}

	private void LoadQuizzes()
	{
		var dir = DirAccess.Open("user://quizzes");
		if (dir != null)
		{
			dir.ListDirBegin();
			string fileName = dir.GetNext();
			while (fileName != "")
			{
				if (!dir.CurrentIsDir() && fileName.EndsWith(".tres"))
				{
					AddQuizButton(fileName.TrimSuffix(".tres"));
				}
				fileName = dir.GetNext();
			}
		}
	}

	private void AddQuizButton(string quizName)
	{
		var button = new Button();
		button.Text = quizName;
		button.Pressed += () => OnQuizButtonPressed(quizName);
		quizList.AddChild(button);
	}

	private void OnQuizButtonPressed(string quizName)
	{
		GetTree().ChangeSceneToFile("res://QuizEditor.tscn");
		GetNode<QuizEditor>("/root/QuizEditor").LoadQuiz(quizName);
	}

	private void OnAddQuizButtonPressed()
	{
		string newQuizName = "New Quiz " + (quizList.GetChildCount() + 1);
		var newQuiz = new QuizResource { QuizName = newQuizName };
		ResourceSaver.Save(newQuiz, $"user://quizzes/{newQuizName}.tres");
		GetTree().ChangeSceneToFile("res://QuizEditor.tscn");
		GetNode<QuizEditor>("/root/QuizEditor").LoadQuiz(newQuizName);
	}
}
