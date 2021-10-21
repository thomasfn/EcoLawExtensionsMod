# Eco Law Extensions Mod
A server mod for Eco 9.4 that extends the law system with a number of helpful utility game values and legal actions.
TODO: list the new game values and legal actions with a brief overview of what they do here
 - Wealth of government account
 - Population of demographic / title
 - Nearest distance to world object
 - Current skill rate of citizen

## Installation
1. Download `EcoLawExtensionsMod.dll` from the [latest release](https://github.com/thomasfn/EcoLawExtensionsMod/releases).
2. Copy the `EcoLawExtensionsMod.dll` file to `Mods` folder of the dedicated server.
3. Restart the server.

## Usage

TODO: Document the new game values and legal actions in more detail
## Building Mod from Source

### Windows

1. Login to the [Eco Website](https://play.eco/) and download the latest modkit
2. Extract the modkit and copy the dlls from `ReferenceAssemblies` to `eco-dlls` in the root directory (create the folder if it doesn't exist)
3. Open `EcoLawExtensionsMod.sln` in Visual Studio 2019
4. Build the `EcoLawExtensionsMod` project in Visual Studio
5. Find the artifact in `EcoLawExtensionsMod\bin\{Debug|Release}\net5.0`

### Linux

1. Run `ECO_BRANCH="release" MODKIT_VERSION="0.9.4.3-beta" fetch-eco-reference-assemblies.sh` (change the modkit branch and version as needed)
2. Enter the `EcoLawExtensionsMod` directory and run:
`dotnet restore`
`dotnet build`
3. Find the artifact in `EcoLawExtensionsMod/bin/{Debug|Release}/net5.0`

## License
[MIT](https://choosealicense.com/licenses/mit/)