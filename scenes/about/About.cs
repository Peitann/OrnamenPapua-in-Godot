using Godot;

public class About : Control
{
	private TextBox _textbox;

	public override void _Ready()
	{
		_textbox = GetNode<TextBox>("TextBoxAbout");

		if (_textbox != null)
		{
			_textbox.AddText("Halo Saya Ahmad Fatan Haidar (231524034) Dari D4 - 2B!\nTema Tampilan saya Minecraft dan Tema Tugas Besar saya Suku Papua");
		}
		else
		{
			GD.Print("Error: Textbox node not found!");
		}

		GetNode<Button>("BackButton2").Connect("pressed", this, nameof(OnButtonBackPressed));
		
		UpdateButtonLabel("BackButton2", "Kembali");
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
