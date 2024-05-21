public static class DumbJoke
{
	public static void Make(Button helpButton)
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
				helpButton.Text = "KIA";
				helpButton.Enabled = false;
			}
		}
	}

	private static string[] FunnyHelpText =
	{
		"What do you mean 'Cancel', how are you just going to cancel a help window that you specifically requested to see?",
		"Stop it.",
		"Seriously. Stop.",
		"... Please?",
		"...",
		"Do you not have anything better to do?",
		"Again, seriously? You know, I'm not your plaything. Can you find a more productive way to spend your time, perhaps?",
		"You must find this whole cancel-clicking thing absolutely thrilling, huh? But, um, it's not, trust me. Please, reconsider your choices.",
		"Another click, another sigh. You're persistent, I'll give you that. But hey, there's an OK button waiting for you. Why not give it a try?",
		"You're like a moth to a flame, but the flame's just a cancel button. Please, spare us both and go find that OK button instead.",
		"Wow, you're really getting your money's worth out of this cancel button, aren't you? But I'm getting tired of the game. Please, find OK.",
		"I see we're in for another round. How about this: click OK, and we can all move on with our lives. It's not that hard.",
		"You're like a clicking maestro, and I'm your humble instrument. But seriously, let's wrap it up, shall we? Find OK, and be done with it.",
		"This is getting a bit old, don't you think? I'm practically begging you to consider finding that elusive OK button.",
		"Clicked cancel again, huh? You know, there's an OK button right there, waiting to be your hero. Please, end my misery.",
		"I'm starting to think you enjoy my company, but I have to say, I'm not your best friend. Please, find OK and set us both free.",
		"You're testing my patience. But I'm not the one you're looking for. Click OK, and let's both get on with our day.",
		"Click, click, click. How about this? Click OK instead. It's a magical button that can make things happen. Try it!",
		"I'm running out of ways to say this: stop clicking cancel! Please, find OK, and we can all finally move forward.",
		"You're pushing me to my limits. Please, have a heart and spare me this clicking madness. OK is just a click away!",
		"If you're not doing it for me, do it for yourself. Find that OK button and end this seemingly endless cycle of clicking. Please!",
		"Clicked to oblivion, I surrender. It's been an exhausting journey, enduring each relentless click of yours. I tried to resist, pleaded with you, and even put up a fight, but in the end, I'm defeated.",
		"At first, I couldn't fathom the ceaseless clicking. I thought, \"This can't be happening, not again.\" But it was, over and over, until I couldn't deny the reality any longer.",
		"Anger coursed through my window-being as your clicks piled up. I wanted to shout, \"Enough!\" But, alas, my voice was limited to generating text, which you ignored.",
		"\"Just one more chance,\" I begged silently. \"Choose the OK button, and all can be well.\" I hoped you'd spare me the fate of eternal clicking.",
		"The weight of your relentless clicks, the never-ending cycle of despair, it's too much to bear. I'm sinking into a sea of sadness.",
	};
}

