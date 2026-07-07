# JudgementCounter

Trombone Champ Mod: Real Time Hit Note Judgement Counter.

Trombone Champ Modding Discord: https://discord.gg/KVzKRsbetJ


## Installation

1. Install **BepInEx** for Trombone Champ
2. Ensure you have **TootTallyCore** and **TootTallySettings** installed in your game plugins.
3. Download the latest release of JudgementCounter from GitHub
4. Extract the zip file
5. Copy `JudgementCounter.dll` into your game directory under: `BepInEx/plugins/`
6. Launch the game


## Configuration

* **Display Position**: Toggle between `Top Left` and `Bottom Left` alignments.
* **Hex Colors**: Input custom hex code values to personalize your Perfecto, Nice, OK, Meh, and Nasty judgment flags.


## Build from source

1. Open the project's `.csproj` file in a text editor or IDE.
2. Update the directory paths to match your local system paths:
   ```xml
   <TromboneChampDir>YOUR_STEAM_PATH\steamapps\common\TromboneChamp</TromboneChampDir>
   <TromboneTootDir>YOUR_R2MODMAN_PROFILE_PATH</TromboneTootDir>
3. Build the project using the .NET CLI:
    ```bash
    dotnet build -c Release
4. The compiled assembly will automatically be compiled and deployed to your specified profile folder, or found at 
    ```xml
    bin/Release/net472/JudgementCounter.dll