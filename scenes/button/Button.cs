using Godot;
using System;

[Tool] // Memungkinkan script digunakan di editor Godot
public class CustomButton : Godot.Button // Gunakan nama kelas yang unik
{
	[Export]
	private string ButtonText
	{
		get => _buttonText;
		set => SetButtonText(value);
	}

	private string _buttonText = "Button";
	private AudioStreamPlayer2D _audioPlayer;
	private Label _label;

	public override void _Ready()
	{
		// Inisialisasi node
		_audioPlayer = GetNode<AudioStreamPlayer2D>("AudioStreamPlayer2D");
		_label = GetNode<Label>("Label");

		// Set teks awal
		SetButtonText(_buttonText);

		// Hubungkan sinyal pressed ke metode OnButtonPressed
		Connect("pressed", this, nameof(OnButtonPressed));
	}

	private void SetButtonText(string newText)
	{
		_buttonText = newText;
		if (_label != null)
		{
			_label.Text = newText;
		}
	}

	private string GetButtonText()
	{
		return _buttonText;
	}

	private void OnButtonPressed()
	{
		_audioPlayer.Play();
	}
}
