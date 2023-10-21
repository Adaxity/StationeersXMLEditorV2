
partial class SXMLE2
{
	/// <summary>
	/// Required designer variable.
	/// </summary>
	private System.ComponentModel.IContainer components = null;

	/// <summary>
	/// Clean up any resources being used.
	/// </summary>
	/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
	protected override void Dispose(bool disposing)
	{
		if (disposing && (components != null))
		{
			components.Dispose();
		}
		base.Dispose(disposing);
	}

	#region Windows Form Designer generated code

	/// <summary>
	/// Required method for Designer support - do not modify
	/// the contents of this method with the code editor.
	/// </summary>
	private void InitializeComponent()
	{
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SXMLE2));
		ConfigSettingsBox = new RichTextBox();
		button2 = new Button();
		ConfigList = new ComboBox();
		HelpButton = new Button();
		SuspendLayout();
		// 
		// ConfigSettingsBox
		// 
		ConfigSettingsBox.Enabled = false;
		ConfigSettingsBox.Location = new Point(12, 36);
		ConfigSettingsBox.Name = "ConfigSettingsBox";
		ConfigSettingsBox.Size = new Size(523, 285);
		ConfigSettingsBox.TabIndex = 1;
		ConfigSettingsBox.Text = "";
		// 
		// button2
		// 
		button2.Location = new Point(416, 7);
		button2.Name = "button2";
		button2.Size = new Size(119, 23);
		button2.TabIndex = 4;
		button2.Text = "Build Config";
		button2.UseVisualStyleBackColor = true;
		button2.Click += BuildButton_Click;
		// 
		// ConfigList
		// 
		ConfigList.FormattingEnabled = true;
		ConfigList.Location = new Point(99, 7);
		ConfigList.Name = "ConfigList";
		ConfigList.Size = new Size(311, 23);
		ConfigList.TabIndex = 5;
		ConfigList.SelectedIndexChanged += ConfigList_IndexChanged;
		ConfigList.Click += ConfigList_Click;
		// 
		// HelpButton
		// 
		HelpButton.Location = new Point(12, 7);
		HelpButton.Name = "HelpButton";
		HelpButton.Size = new Size(81, 23);
		HelpButton.TabIndex = 6;
		HelpButton.Text = "Need Help?";
		HelpButton.UseVisualStyleBackColor = true;
		HelpButton.Click += HelpButton_Click;
		// 
		// SXMLE2
		// 
		AutoScaleDimensions = new SizeF(7F, 15F);
		AutoScaleMode = AutoScaleMode.Font;
		ClientSize = new Size(547, 333);
		Controls.Add(HelpButton);
		Controls.Add(ConfigList);
		Controls.Add(button2);
		Controls.Add(ConfigSettingsBox);
		Icon = (Icon)resources.GetObject("$this.Icon");
		Name = "SXMLE2";
		Text = "SXMLE 2";
		TransparencyKey = Color.Lime;
		ResumeLayout(false);
	}

	#endregion

	private RichTextBox ConfigSettingsBox;
	private Button button2;
	private ComboBox ConfigList;
	new private Button HelpButton;
}
