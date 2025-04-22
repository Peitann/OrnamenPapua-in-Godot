using Godot;

public class TextBox : CanvasLayer
{
	private const float CharReadRate = 0.05f;

	private Control _textboxContainer;
	private Label _startSymbol;
	private Label _endSymbol;
	private Label _label;
	private Tween _tween;

	public override void _Ready()
	{
		_textboxContainer = GetNode<Control>("TextBoxContainer");
		_startSymbol = GetNode<Label>("TextBoxContainer/MarginContainer/HBoxContainer/Start");
		_endSymbol = GetNode<Label>("TextBoxContainer/MarginContainer/HBoxContainer/End");
		_label = GetNode<Label>("TextBoxContainer/MarginContainer/HBoxContainer/Label");
		_tween = GetNode<Tween>("Tween");

		HideTextbox();
	}

	private void HideTextbox()
	{
		_startSymbol.Text = "";
		_endSymbol.Text = "";
		_label.Text = "";
		_textboxContainer.Hide();
	}

	private void ShowTextbox()
	{
		_startSymbol.Text = "^";
		_textboxContainer.Show();
	}

	public void AddText(string nextText)
	{
		_label.Text = nextText;
		ShowTextbox();
		_tween.InterpolateProperty(_label, "percent_visible", 0.0f, 1.0f, nextText.Length * CharReadRate, Tween.TransitionType.Linear, Tween.EaseType.InOut);
		_tween.Start();
	}

	private void OnTweenTweenAllCompleted()
	{
		_endSymbol.Text = "v";
	}
}
