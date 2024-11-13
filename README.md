# Eco Law Extensions Mod
A server mod for Eco 11.0 that extends the law system with a number of helpful utility game values and legal actions.

Added game values:
 - Citizen Population - the current citizen count of a title or demographic
 - Distance to Closest World Object - the closest distance to the nearest world object of a specific type
 - Distance to Closest Plant - the closest distance to the nearest plant of a specific type
 - Government Account Holding - the current amount of a particular currency held in a government account
 - Skill Rate - the current xp multiplier of a citizen from either food, housing or total
 - Nutrition - the current base nutrition of a citizen, e.g. before bonus multipliers (like balanced diet) are applied
 - Citizen Skill Count - the current number of skills learnt/specialised by a citizen
 - Height At - the Y coordinate of a location
 - Layer Value At - the value of a world layer at a location

Added legal actions:
 - Turn On Machines - turn on machines belonging to a citizen or group that were turned off either manually, legally or both

Added law triggers:
 - Power Generated - triggers every "power law tick" (by default 30s) for every object generating mechanical or electrical power
 - Power Consumed - triggers every "power law tick" (by default 30s) for every object consuming mechanical or electrical power

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
| IgnoreAtLocation | Yes/No | Ignore any world objects directly at the location, e.g. with a distance of 0. This is useful to find the next nearest world object from a world object. Defaults to No. |

#### Distance to Closest Plant

Finds the closest plant matching a filter to a location and gets the distance to it. If no plants exist that match the filter, a very large number is returned (basically infinite).

| Property Name | Type | Description |
| - | - | - |
| Location | Vector3 | The location to test. Usually this is passed in context from the law trigger. |
| PlantType | Plant Species Picker | A filter for plants to search for. |
| IgnoreAtLocation | Yes/No | Ignore any plants directly at the location, e.g. with a distance of 0. This is useful to find the next nearest plant from a plant. Defaults to No. |

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

#### X/Z Coord At

Extracts the X/Z coordinate from a location. This will be an integer in whole blocks.

| Property Name | Type | Description |
| - | - | - |
| Location | Vector3 | The location to get the X/Z coord of. Usually this is passed in context from the law trigger. |

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

### Power

#### Power Generated

Triggered every "power law tick" for every object generating some kind of power. The "power law tick" is a server configurable interval defaulting to 30 seconds. Triggers for objects generating mechanical power (e.g. waterwheels) and electrical power (e.g. steam engines). Also triggers for objects that could be generating power but aren't because they've been turned off by the power grid to save resources. Does not trigger for objects that are turned off by a player or are otherwise disabled (e.g. out of fuel or issue with pipe connections).

| Property Name | Type | Description |
| - | - | - |
| Citizen | User | The citizen responsible for generating the power. This is the owner of the plot on which the object resides. If owned by a multi-user title or demographic, the first user in the list is used. |
| ActionLocation | Vector3 | The location of the object that generated the power. |
| PowerGenerator | Item | The type of object that generated the power. |
| TimeGenerating | Number | How long since the previous power law tick. This will normally be the same as the configured interval but will vary slightly and may be higher when the server is under heavy load. |
| IsElectrical | Boolean | Whether the object is generating electrical power. |
| IsMechanical | Boolean | Whether the object is generating mechanical power. |
| PowerAvailable | Number | How much power the object is making available to the grid. This will be constant, even if the object is disabled by the power grid to save resources. Measured in Joules. |
| PowerProduced | Number | How much power the object is currently generating. This may be 0 if the object is disabled by the power grid to save resources. Measured in Joules. |

Note that the `PowerAvailable` and `PowerProduced` values are measured in Joules and are units of power over time. This is because the law trigger tracks all power activity over a time interval. For example, a steam engine generating 1000 Watts of power will generate 30,000 Joules of power over the course of 30 seconds. This value will change if the server configurable "power law tick" interval is changed. You can get back to the wattage of the object by dividing by `TimeGenerating`, however depending on your goal it may make more sense to use these values as-is to apply any sort of tax or payment, as they are compensated for time implicitly by the law trigger automatically.

#### Power Consumed

Triggered every "power law tick" for every object consuming some kind of power. The "power law tick" is a server configurable interval defaulting to 30 seconds. Triggers for objects consuming mechanical power (e.g. mills) and electrical power (e.g. lights). Does not trigger for objects that are trying to use power but are turned off due to overloaded power grid.

| Property Name | Type | Description |
| - | - | - |
| Citizen | User | The citizen responsible for consuming the power. This is the owner of the plot on which the object resides. If owned by a multi-user title or demographic, the first user in the list is used. |
| ActionLocation | Vector3 | The location of the object that consumed the power. |
| PowerConsumer | Item | The type of object that consumed the power. |
| TimeConsuming | Number | How long since the previous power law tick. This will normally be the same as the configured interval but will vary slightly and may be higher when the server is under heavy load. |
| IsElectrical | Boolean | Whether the object is consuming electrical power. |
| IsMechanical | Boolean | Whether the object is consuming mechanical power. |
| PowerUsed | Number | How much power the object is currently using. Measured in Joules. |

Note that the `PowerAvailable` and `PowerProduced` values are measured in Joules and are units of power over time. This is because the law trigger tracks all power activity over a time interval. For example, a steam engine generating 1000 Watts of power will generate 30,000 Joules of power over the course of 30 seconds. This value will change if the server configurable "power law tick" interval is changed. You can get back to the wattage of the object by dividing by `TimeGenerating`, however depending on your goal it may make more sense to use these values as-is to apply any sort of tax or payment, as they are compensated for time implicitly by the law trigger automatically.

## Building Mod from Source

### Windows

1. Login to the [Eco Website](https://play.eco/) and download the latest modkit
2. Extract the modkit and copy the dlls from `ReferenceAssemblies` to `eco-dlls` in the root directory (create the folder if it doesn't exist)
3. Open `EcoLawExtensionsMod.sln` in Visual Studio 2019/2022
4. Build the `EcoLawExtensionsMod` project in Visual Studio
5. Find the artifact in `EcoLawExtensionsMod\bin\{Debug|Release}\net7.0`

### Linux

1. Run `ECO_BRANCH="release" MODKIT_VERSION="0.11.0.0-beta" fetch-eco-reference-assemblies.sh` (change the modkit branch and version as needed)
2. Enter the `EcoLawExtensionsMod` directory and run:
`dotnet restore`
`dotnet build`
3. Find the artifact in `EcoLawExtensionsMod/bin/{Debug|Release}/net7.0`

## License
[MIT](https://choosealicense.com/licenses/mit/)