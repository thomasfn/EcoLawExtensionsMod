# Eco Law Extensions Mod
A server mod for Eco 9.4 that extends the law system with a number of helpful utility game values.

Added game values:
 - Citizen Population - the current citizen count of a title or demographic
 - Distance to Closest World Object - the closest distance to the nearest world object of a specific type
 - Distance to Closest Plant - the closest distance to the nearest plant of a specific type
 - Government Account Holding - the current amount of a particular currency held in a government account
 - Skill Rate - the current xp multiplier of a citizen from either food, housing or total
 - Citizen Skill Count - the current number of skills learnt/specialised by a citizen

## Installation
1. Download `EcoLawExtensionsMod.dll` from the [latest release](https://github.com/thomasfn/EcoLawExtensionsMod/releases).
2. Copy the `EcoLawExtensionsMod.dll` file to `Mods` folder of the dedicated server.
3. Restart the server.

## Usage

The added game values will show up under the existing headings in the law editor. Do not remove the mod if any features of the mod have been used in a law, even a law that has been removed, as this may corrupt the world save.

### Citizens

#### Citizen Population

Counts the number of citizens that hold a title or are a member of a demographic.

| Property Name | Type | Description |
| - | - | - |
| Target | Alias | The title or demographic to count. If a user is passed in here, this will always return 1. |

#### Citizen Skill Count

Counts the number of skills that a citizen has learnt (just read the scroll) or specialised (invested a star in). Always includes Self Improvement.

| Property Name | Type | Description |
| - | - | - |
| Citizen | User | The citizen to count skills of. |
| IncludeUnspecialised | Yes/No | Whether to include skills which are learnt but not specialised. This will also include base skills that don't need a scroll, e.g. logging and mining. |

#### Skill Rate

Gets the current XP multiplier of a citizen, with options to enable or disable consideration of food or housing bonuses. Will return 0 if "No" is passed to both.

| Property Name | Type | Description |
| - | - | - |
| Citizen | User | The user to evaluate skill rate of. |
| ConsiderFood | Yes/No | Whether to consider their food bonus. |
| ConsiderHousing | Yes/No | Whether to consider their housing bonus. |

### World

#### Distance to Closest World Object

Finds the closest world object matching a filter to a location and gets the distance to it. If no world objects exist that match the filter, a very large number is returned (basically infinite).

| Property Name | Type | Description |
| - | - | - |
| Location | Vector3 | The location to test. Usually this is passed in context from the law trigger. |
| ObjectType | Object Picker | A filter for objects to search for. |

#### Distance to Closest Plant

Finds the closest plant matching a filter to a location and gets the distance to it. If no plants exist that match the filter, a very large number is returned (basically infinite).

| Property Name | Type | Description |
| - | - | - |
| Location | Vector3 | The location to test. Usually this is passed in context from the law trigger. |
| PlantType | Plant Species Picker | A filter for plants to search for. |

### Government

#### Government Account Holding

Gets the amount of a currency held in a government bank account.

| Property Name | Type | Description |
| - | - | - |
| Currency | Currency | The currency to retrieve the holding for. |
| Account | Government Bank Account | The account to retrieve the holding from. |

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