using Godot;

public class Menu : Control
{
	public override void _Ready()
	{
		// Hubungkan sinyal tombol ke fungsi yang sesuai
		GetNode<Button>("C/V/ButtonGroup/Center/ButtonLine/About").Connect("pressed", this, nameof(OnButtonAboutPressed));
		GetNode<Button>("C/V/ButtonGroup/Center/Guide").Connect("pressed", this, nameof(OnButtonGuidePressed));
		GetNode<Button>("C/V/ButtonGroup/Center/ButtonLine/Exit").Connect("pressed", this, nameof(OnButtonExitPressed));
		GetNode<Button>("C/V/ButtonGroup/Center/Karya1").Connect("pressed", this, nameof(OnButtonKarya1Pressed));
		GetNode<Button>("C/V/ButtonGroup/Center/Karya2").Connect("pressed", this, nameof(OnButtonKarya2Pressed));
		GetNode<Button>("C/V/ButtonGroup/Center/Karya3").Connect("pressed", this, nameof(OnButtonKarya3Pressed));
		GetNode<Button>("C/V/ButtonGroup/Center/Karya4").Connect("pressed", this, nameof(OnButtonKarya4Pressed));
		
		// Mengubah teks pada Label di dalam CustomButton
		UpdateButtonLabel("C/V/ButtonGroup/Center/ButtonLine/About", "Tentang");
		UpdateButtonLabel("C/V/ButtonGroup/Center/Guide", "Panduan");
		UpdateButtonLabel("C/V/ButtonGroup/Center/ButtonLine/Exit", "Keluar");
		UpdateButtonLabel("C/V/ButtonGroup/Center/Karya1", "Karya 1 (2D)");
		UpdateButtonLabel("C/V/ButtonGroup/Center/Karya2", "Karya 2 (Animasi 2D)");
		UpdateButtonLabel("C/V/ButtonGroup/Center/Karya3", "Karya 3 (Animasi Polygon)");
		UpdateButtonLabel("C/V/ButtonGroup/Center/Karya4", "Karya 4 (Animasi dan Interaksi)");
	}

	private void UpdateButtonLabel(string buttonPath, string newText)
	{
		var button = GetNode<Button>(buttonPath);
		var label = button.GetNode<Label>("Label"); // Mengakses label di dalam button
		label.Text = newText;
	}
	
	private void OnButtonAboutPressed()
	{
		// Pindah ke scene About
		var aboutScene = GD.Load<PackedScene>("res://scenes/about/About.tscn");
		GetTree().ChangeSceneTo(aboutScene);
	}

	private void OnButtonGuidePressed()
	{
		// Pindah ke scene Guide
		var guideScene = GD.Load<PackedScene>("res://scenes/Guide/Guide.tscn");
		GetTree().ChangeSceneTo(guideScene);
	}

	private void OnButtonExitPressed()
	{
		// Keluar dari aplikasi
		GetTree().Quit();
	}

	private void OnButtonKarya1Pressed()
	{
		var karya1Scene = GD.Load<PackedScene>("res://scenes/Karya/Karya1/Karya1.tscn");
		GetTree().ChangeSceneTo(karya1Scene);
	}

	private void OnButtonKarya2Pressed()
	{
		var karya2Scene = GD.Load<PackedScene>("res://scenes/Karya/Karya2/Karya2.tscn");
		GetTree().ChangeSceneTo(karya2Scene);
	}
	
	private void OnButtonKarya3Pressed()
	{
		var karya2Scene = GD.Load<PackedScene>("res://scenes/Karya/Karya3/Karya3.tscn");
		GetTree().ChangeSceneTo(karya2Scene);
	}
	private void OnButtonKarya4Pressed()
	{
		var karya2Scene = GD.Load<PackedScene>("res://scenes/Karya/Karya4/Karya4.tscn");
		GetTree().ChangeSceneTo(karya2Scene);
	}
}
