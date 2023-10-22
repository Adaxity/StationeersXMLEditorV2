SXMLE2 is a simple mass editor for Stationeers XML data. It is currently in development, so please expect bugs and changes. If you encounter any bugs, have suggestions or feedback, please contact me on Discord (username is decxi).

1. Installation & Info:
	- Put 'SXMLE2.exe' file into your Stationeers folder and launch it. A 'SXMLE_Configs' folder will be generated in your Stationeers folder, and will contain a 'vanilla.xml' and 'default.xml' configs. These files will regenerate when deleted.

2. Configs:
	- The 'vanilla.xml' config will always match up with vanilla Stationeers XML data. Don't bother editing/deleting it, because it will regenerate anyway.
	- The 'default.xml' config has set values from when it is created. You can edit the settings in this config to your liking using your favourite text editor. The values will be saved. If you wish to reset it to the default 'default.xml', delete the file and restart SXMLE2.
	- You can create new configs by copy-pasting a config. All configs will show up in the dropbox in SXMLE2 by their filenames.

4. Usage:
	- Upon selecting a desired config in SXMLE2 via the dropdown menu, its settings will be shown in the textbox. Now we can move onto building a config, I highly recommend to verify integrity of files on Steam before doing this.
	- When you press 'Build Config' for the first time or after a game update changes file(s) in 'StreamingAssets\Data', SXMLE2 will automatically back up the original vanilla files into 'StreamingAssets\DATA_ORIGINAL'.
	- SXMLE2 will then read the files from 'StreamingAssets\DATA_ORIGINAL' to apply the config and replaces the files in 'StreamingAssets\Data'.
	- SXMLE2 knows which files were edited by itself, so you don't have to worry about multiplier overlap or having modified/non-vanilla files in 'StreamingAssets\DATA_ORIGINAL', unless you manually edited files in 'StreamingAssets\Data' and didn't verify integrity of files.
	- Info about what each setting in a config file changes can be browsed by pressing the 'Need Help?' button in SXMLE2.
