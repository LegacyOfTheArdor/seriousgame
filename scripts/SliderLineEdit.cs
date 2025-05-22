using Godot;

public partial class SliderLineEdit : LineEdit
{
	[Export] public NodePath SliderPath { get; set; }

	private HSlider _slider;

	public override void _Ready()
	{
		// Get the slider node using the exported NodePath
		if (!string.IsNullOrEmpty(SliderPath))
		{
			_slider = GetNode<HSlider>(SliderPath);

			// Set the LineEdit text to the slider's current value at startup
			Text = ((int)_slider.Value).ToString();

			// Connect slider's value_changed to update this LineEdit
			_slider.ValueChanged += OnSliderValueChanged;
		}

		// Connect LineEdit's text_changed to update the slider
		TextChanged += OnLineEditTextChanged;
	}

	private void OnSliderValueChanged(double value)
	{
		// Update LineEdit when the slider changes
		string valueStr = ((int)value).ToString();
		if (Text != valueStr)
			Text = valueStr;
	}

	private void OnLineEditTextChanged(string text)
	{
		// Update slider when the LineEdit changes, if input is valid
		if (_slider != null && int.TryParse(text, out int val))
		{
			val = Mathf.Clamp(val, (int)_slider.MinValue, (int)_slider.MaxValue);
			if ((int)_slider.Value != val)
				_slider.Value = val;
		}
	}
}
