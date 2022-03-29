# Eco Law Extensions Mod
A server mod for Eco 9.5 that extends the law system with a number of helpful utility game values and legal actions.

Added game values:
 - Citizen Population - the current citizen count of a title or demographic
 - Distance to Closest World Object - the closest distance to the nearest world object of a specific type
 - Distance to Closest Plant - the closest distance to the nearest plant of a specific type
 - Government Account Holding - the current amount of a particular currency held in a government account
 - Skill Rate - the current xp multiplier of a citizen from either food, housing or total
 - Nutrition - the current base nutrition of a citizen, e.g. before bonus multipliers (like balanced diet) are applied
 - Citizen Skill Count - the current number of skills learnt/specialised by a citizen

Added legal actions:
- Turn On Machines - turn on machines belonging to a citizen or group that were turned off either manually, legally or both

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

#### Nutrition

Gets the current nutrition value of a citizen, e.g. excluding any multipliers from balanced diet, tastiness, cravings etc, with options to enable or disable consideration of each macro nutrient. Will return 0 if "No" is passed to all.

| Property Name | Type | Description |
| - | - | - |
| Citizen | User | The user to evaluate skill rate of. |
| ConsiderCarbs | Yes/No | Whether to consider their carbohydrates. |
| ConsiderVits | Yes/No | Whether to consider their vitamins. |
| ConsiderProtein | Yes/No | Whether to consider their protein. |
| ConsiderFat | Yes/No | Whether to consider their fat. |

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

#### Layer Value At

Reads the value of a world layer at a location. World layers include biomes, animal populations, pollution levels and other misc things like oilfield density. There is not (yet) a documented list of valid world layer names but generally you can guess them based on the layer toggles on the world map - for example, the ocean biome layer is "OceanBiome", oilfield is "Oilfield" etc. Note that world layers are usually at a different resolution to block coords - e.g. while the transition between biomes may appear smooth on the map, the actual layer readings from this game value may not be interpolated in the same way. The returned value is a decimal between 0 and 1, corresponding to the percentage shown on the world map.

| Property Name | Type | Description |
| - | - | - |
| Location | Vector3 | The location to test. Usually this is passed in context from the law trigger. |
| WorldLayer | String | The world layer name, for example "OceanBiome" or "Oilfield". |

#### Height At

Extracts the Y coordinate from a location. This will be an integer in whole blocks above bedrock.

| Property Name | Type | Description |
| - | - | - |
| Location | Vector3 | The location to get the height of. Usually this is passed in context from the law trigger. |

#### Turn On Machines

Tries to turn on machines belonging to a citizen or group that are currently turned off. The filter can specify how the machines were turned off - for example, only try to turn on machines that were turned off legally (e.g. via prevent on Pollute Air).

| Property Name | Type | Description |
| - | - | - |
| Target | Alias | The title, demographic or citizen whoes machines should be considered. This could be set to Everyone to consider all machines in the world. |
| ByPlayer | Yes/No | Include machines turned off by a player or by other invalid status. |
| ByLaw | Yes/No | Include machines turned off by a law (e.g. prevent on Pollute Air trigger). |

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

1. Run `ECO_BRANCH="staging" MODKIT_VERSION="0.9.5.0-beta-staging-2230" fetch-eco-reference-assemblies.sh` (change the modkit branch and version as needed)
2. Enter the `EcoLawExtensionsMod` directory and run:
`dotnet restore`
`dotnet build`
3. Find the artifact in `EcoLawExtensionsMod/bin/{Debug|Release}/net5.0`

## License
[MIT](https://choosealicense.com/licenses/mit/)