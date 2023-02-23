## Ernest Suryś - Homa Case Study

## Solution

I created two systems:

## Automatic content generation using Asset Post Processors

A new ScriptableObject `CharacterCreationPreset` is inside the project.
Inside it developer defines parameters for Character Prefab generation.

Look at: _Assets/ErnSur/CaseStudy/Editor/Character Creation Preset.asset_

Automation behaviour:
When both icon and model are imported to folders specified in "Character Creation Preset" a Prefab will be generated.
All of them need to share the same name for the system to take a match. In this case it is a character name but in more scalable solution this should be altered.

After character prefab is created it is automatically added to a new Scriptable Object `StoreLibrary`.

Look at: _Assets/ErnSur/CaseStudy/Runtime/Store Library.asset_

This scriptable object holds a list of store items to display in the game store.
if the list does not hold an item with the name of a newly created character it will try to download the latest version of the store list csv file form the internet.
Then if the new entry was found it is added to the scriptable object.

There is also another Asset Postprocessor called `EnforcePresetPostProcessor`.
This Postprocessor is used to enforce Preset import settings to all assets inside the directory that the preset is located at.
This ensures that all Store Icon textures are imported with correct settings.

### Summary

1. Import texture with filename "Red Zombie.png" to _Assets/1_Graphics/Store/Icons_
2. Import fbx with filename "Red Zombie.fbx" to _Assets/1_Graphics/Models/Characters_
3. New prefab "Red Zombie" will be created in _Assets/2_Prefabs/Characters_

## Character creation Wizard Window

There is also a Editor Window that can be used as an alternative way to create character prefab by hand.
Look at menu item: _Tools/Character Wizard_

## Notes

- I altered the game code a little bit
- Added assembly definitions
- I did not have time for "automated procedure scanning the whole project
  looking for potentially forgotten assets". But it can be easily created using AssetDatabase and checking dependencies of scenes added to the build scene list. I created something like that in the past.
- Code might look rough around the edges, I did not find the time to clean it up.
- I did not finish the feature of creating new materials from provided texture