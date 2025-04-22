using Godot;

public class Guide : Control
{
	private TextBox _textbox;

	public override void _Ready()
	{
		_textbox = GetNode<TextBox>("TextBoxGuide");

		if (_textbox != null)
		{
			_textbox.AddText("Tekan Tombol Karya untuk Karya yang Ingin Ditampilkan\nTekan Tombol Kembali untuk Kembali ke Main Menu");
		}
		else
		{
			GD.Print("Error: Textbox node not found!");
		}

		GetNode<Button>("BackButton1").Connect("pressed", this, nameof(OnButtonBackPressed));
		
		UpdateButtonLabel("BackButton1", "Kembali");
	}
	
	private void UpdateButtonLabel(string buttonPath, string newText)
	{
		var button = GetNode<Button>(buttonPath);
		var label = button.GetNode<Label>("Label"); // Mengakses label di dalam button
		label.Text = newText;
	}

	private void OnButtonBackPressed()
	{
		// Kembali ke scene Menu
		var menuScene = GD.Load<PackedScene>("res://scenes/Menu/Menu.tscn");
		GetTree().ChangeSceneTo(menuScene);
	}
}
