using System.Collections.Specialized;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace ErnSur.CaseStudy.Editor
{
    class CharacterAssetsProcessor : AssetPostprocessor
    {
        static CharacterCreationPreset _characterCreationPreset;
        static StoreLibrary _storeLibrary;

        [MenuItem("Tools/Run character prefab generation")]
        static void RunCharacterPrefabGeneration()
        {
            if (_characterCreationPreset == null)
                return;

            var icons = AssetDatabase.FindAssets("t:Texture2D", new[] { _characterCreationPreset.iconDirectory })
                .Select(AssetDatabase.GUIDToAssetPath);
            foreach (var icon in icons)
            {
                AssetDatabase.ImportAsset(icon);
            }
        }

        [InitializeOnLoadMethod]
        static void GetAllProcessors()
        {
            _characterCreationPreset = AssetDatabase.FindAssets($"t:{nameof(CharacterCreationPreset)}")
                .Select(AssetDatabase.GUIDToAssetPath)
                .Select(AssetDatabase.LoadAssetAtPath<CharacterCreationPreset>)
                .FirstOrDefault();
            _storeLibrary = AssetDatabase.FindAssets($"t:{nameof(StoreLibrary)}")
                .Select(AssetDatabase.GUIDToAssetPath)
                .Select(AssetDatabase.LoadAssetAtPath<StoreLibrary>)
                .FirstOrDefault();
        }

        static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets,
            string[] movedFromAssetPaths)
        {
            if (_characterCreationPreset == null)
                return;

            var importedOrMovedAssetPaths = importedAssets.Union(movedAssets);

            foreach (var path in importedOrMovedAssetPaths)
            {
                if (_storeLibrary != null && path.StartsWith(_characterCreationPreset.characterPrefabsDirectory))
                {
                    var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
                    if (prefab != null && prefab.TryGetComponent<StoreItemViewModel>(out var storeComponent))
                    {
                        var fileName = Path.GetFileNameWithoutExtension(path);
                        _storeLibrary.AddNewCharacter(fileName, storeComponent);
                    }
                }
                var isIconOrModelPath = path.StartsWith(_characterCreationPreset.iconDirectory) ||
                                        path.StartsWith(_characterCreationPreset.modelDirectory);
                if (!isIconOrModelPath)
                    return;

                TryCreateCharacterPrefab(path, _characterCreationPreset);
            }
        }

        static void TryCreateCharacterPrefab(string path, CharacterCreationPreset creationPreset)
        {
            var fileName = Path.GetFileNameWithoutExtension(path);
            var model = AssetDatabase.LoadAssetAtPath<GameObject>($"{creationPreset.modelDirectory}/{fileName}.fbx");
            var icon = AssetDatabase.LoadAssetAtPath<Texture2D>($"{creationPreset.iconDirectory}/{fileName}.png");
            var character =
                AssetDatabase.LoadAssetAtPath<GameObject>(
                    $"{creationPreset.characterPrefabsDirectory}/{fileName}.prefab");
            if (model != null && icon != null && character == null)
            {
                creationPreset.CreateNewCharacter(new CharacterCreationArgs
                {
                    model = model,
                    shopIcon = icon
                }, fileName);
                Debug.Log($"Created new Character Prefab {fileName}");
            }
        }
    }
}