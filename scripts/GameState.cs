using Godot;
using System.Collections.Generic;
using System.Text.Json;

public partial class GameState : Node
{
	public bool EditMode = false;

	public List<string> SliderTitles { get; private set; } = new List<string> { "Slider 1", "Slider 2", "Slider 3", "Slider 4" };

	private string sliderTitlesPath = "res://quizzes/slider_titles.json";

	public override void _Ready()
	{
		LoadSliderTitles();
	}

	public void SetSliderTitles(List<string> newTitles)
	{
		SliderTitles = newTitles;
		SaveSliderTitles();
	}

	public void LoadSliderTitles()
	{
		if (FileAccess.FileExists(sliderTitlesPath))
		{
			using var file = FileAccess.Open(sliderTitlesPath, FileAccess.ModeFlags.Read);
			string json = file.GetAsText();
			SliderTitles = JsonSerializer.Deserialize<List<string>>(json);
		}
	}

	public void SaveSliderTitles()
	{
		string json = JsonSerializer.Serialize(SliderTitles, new JsonSerializerOptions { WriteIndented = true });
		using var file = FileAccess.Open(sliderTitlesPath, FileAccess.ModeFlags.Write);
		file.StoreString(json);
	}
}
