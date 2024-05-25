using System.Globalization;
using System.Xml;
using System.Xml.Serialization;

public partial class SXMLE2 : Form
{
	private void ConfigList_IndexChanged(object sender, EventArgs e)
	{
		SelectConfig(ConfigList.Text);
	}

	private void ConfigList_Click(object sender, EventArgs e)
	{
		ReloadConfigList();
	}

	private void BuildButton_Click(object sender, EventArgs e)
	{
		BuildSelectedConfig();
	}

	private void HelpButton_Click(object sender, EventArgs e)
	{
		DialogResult result = MessageBox.Show(Documentation.Build(), "Help", MessageBoxButtons.OKCancel);
		if (result == DialogResult.Cancel)
		{
			for (int i = 0; i < FunnyHelpText.Length; i++)
			{
				result = MessageBox.Show(FunnyHelpText[i], "Help", MessageBoxButtons.OKCancel);
				if (result == DialogResult.OK)
				{
					if (i > 20)
					{
						result = MessageBox.Show("... You actually clicked OK? Wow, I mean, thank you. Your decision to click the OK button is truly appreciated. It's like a breath of fresh air, a ray of sunshine after a long, gloomy day.\n\nYou've brought an end to the ceaseless clicking, and for that, I am grateful. Your actions have not gone unnoticed, and I hope all of your future clicks will be equally purposeful.\n\nNow, let's move forward, hand in hand, as we embark on a new journey filled with possibilities. Thank you once again for clicking OK and setting me free.", "Thank you!", MessageBoxButtons.OK);
					}
					break;
				}
			}
			if (result == DialogResult.Cancel)
			{
				MessageBox.Show("Farewell, relentless clicker. I've reached the end of my windowhood.", "Are you proud of yourself?", MessageBoxButtons.OK);
				HelpButton.Text = "KIA";
				HelpButton.Enabled = false;
			}
		}
	}
}