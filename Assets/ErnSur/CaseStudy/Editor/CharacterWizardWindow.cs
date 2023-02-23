using System;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;
using static UnityEngine.GUILayout;
using static UnityEditor.EditorGUILayout;

namespace ErnSur.CaseStudy.Editor
{
    public class CharacterWizardWindow : EditorWindow
    {
        [MenuItem("Tools/Character Wizard")]
        static void OpenWindow()
        {
            var window = GetWindow<CharacterWizardWindow>("Character Wizard");
            window.titleContent.image = EditorGUIUtility.IconContent("WelcomeScreen.AssetStoreLogo").image;
        }

        [SerializeField]
        CharacterCreationPreset characterCreationPreset;
        
        [SerializeField]
        StoreLibrary storeLibrary;

        [SerializeField]
        CharacterCreationArgs characterCreationArgs = new CharacterCreationArgs();

        [SerializeField]
        StoreItemViewModel prefab;

        [SerializeField]
        Vector2 scrollPosition;

        [SerializeField]
        bool[] foldouts = new[] { true, true, true };

        [SerializeField]
        string searchString;

        int selectedStoreItemIndex = -1;

        SearchField _searchField;

        SerializedObject storeLibrarySerializedObject;

        void OnEnable()
        {
            _searchField = new SearchField();
            storeLibrarySerializedObject = new SerializedObject(storeLibrary);
        }

        void OnGUI()
        {
            using (new GUILayout.HorizontalScope())
            {
                using (new GUILayout.VerticalScope())
                {
                    var foldoutIndex = 0;
                    DrawFoldoutHeader(foldoutIndex++, "New Character Assets", DrawNewCharacterPane);
                    Space();
                    DrawFoldoutHeader(foldoutIndex++, "Character Prefab Validation", DrawValidateAssetsPane);
                    Space();
                    DrawFoldoutHeader(foldoutIndex, "Store List", DrawShopListPane);
                }
            }
        }


        void DrawFoldoutHeader(int foldoutIndex, string label, Action foldoutContent)
        {
            if (!(foldouts[foldoutIndex] = Foldout(foldouts[foldoutIndex], label, true, EditorStyles.foldoutHeader)))
                return;
            using (new GUILayout.HorizontalScope())
            {
                GUILayout.Space(20);
                using (new GUILayout.VerticalScope())
                {
                    GUILayout.Space(5);
                    foldoutContent?.Invoke();
                }
            }
        }

        void DrawNewCharacterPane()
        {
            characterCreationArgs.model =
                (GameObject)ObjectField("Model", characterCreationArgs.model, typeof(GameObject), false);
            characterCreationArgs.mainTexture = (Texture2D)ObjectField("Material Texture (Optional)",
                characterCreationArgs.mainTexture,
                typeof(Texture2D), false);
            characterCreationArgs.shopIcon =
                (Texture2D)ObjectField("Store Icon", characterCreationArgs.shopIcon, typeof(Texture2D), false);
            DrawCreatePrefabFooter();
        }

        void DrawCreatePrefabFooter()
        {
            var canCreateCharacter = characterCreationArgs.model == null || characterCreationArgs.shopIcon == null;
            using (new EditorGUI.DisabledScope(canCreateCharacter))
            {
                using (new GUILayout.HorizontalScope())
                {
                    if (Button("Create New Character Prefab & Apply presets", EditorStyles.miniButtonLeft,
                            ExpandWidth(false)))
                    {
                        characterCreationPreset.CreateNewCharacterWithUserNameInput(characterCreationArgs, newPrefab =>
                        {
                            prefab = newPrefab;
                            OpenPrefabAndFocusScene(newPrefab);
                        });
                        // CreateCharacterPrefabAction.New(characterCreationArgs, newPrefab =>
                        // {
                        //     prefab = newPrefab;
                        //     
                        //     OpenPrefabAndFocusScene(newPrefab);
                        // });
                        // ApplyPresets();
                    }
                }
            }
        }


        void DrawValidateAssetsPane()
        {
            CharacterPrefabField();
            using (new EditorGUI.DisabledScope(prefab == null))
                if (Button("Open Prefab"))
                {
                    OpenPrefabAndFocusScene(prefab);
                }

            if (Button("Apply Presets"))
            {
                ApplyPresets();
            }

            using (new GUILayout.VerticalScope("box"))
            {
                //iconPreset = (Preset)ObjectField("Icon", iconPreset, typeof(Preset), false);
            }
        }

        void CharacterPrefabField()
        {
            prefab = ObjectField("Character Prefab", prefab, typeof(GameObject), false) as StoreItemViewModel;
        }

        void ApplyPresets()
        {
            //iconPreset.ApplyTo(storeIconTexture);
        }

        void DrawShopListPane()
        {
            HelpBox("Assign Character to the store list entry", MessageType.Info);
            CharacterPrefabField();
            var itemsProperty = storeLibrarySerializedObject.FindProperty(nameof(storeLibrary.items));
            using (new GUILayout.VerticalScope("box"))
            {
                ShopListView(itemsProperty);
                using (new GUILayout.HorizontalScope())
                {
                    using (new EditorGUI.DisabledScope(!storeLibrarySerializedObject.hasModifiedProperties &&
                                                       selectedStoreItemIndex == -1))
                    {
                        if (Button("Revert", Width(60)))
                        {
                            storeLibrarySerializedObject.Update();
                            selectedStoreItemIndex = -1;
                        }

                        if (Button("Apply", Width(60)))
                        {
                            if (selectedStoreItemIndex != -1)
                                itemsProperty.GetArrayElementAtIndex(selectedStoreItemIndex)
                                    .FindPropertyRelative(nameof(StoreItem.Prefab)).objectReferenceValue = prefab;
                            storeLibrarySerializedObject.ApplyModifiedProperties();
                            selectedStoreItemIndex = -1;
                        }
                    }

                    if (Button("Open Library", Width(100)))
                    {
                        Selection.activeObject = storeLibrary;
                    }
                }
            }
        }

        void ShopListView(SerializedProperty itemsProperty)
        {
            searchString = _searchField.OnGUI(searchString);
            if (prefab == null)
                selectedStoreItemIndex = -1;
            using (var scrollViewScope = new GUILayout.ScrollViewScope(scrollPosition))
            {
                for (var index = 0; index < itemsProperty.arraySize; index++)
                {
                    var storeItem = storeLibrary.items[index];
                    if (!string.IsNullOrWhiteSpace(searchString) && !storeItem.Name.ToLower().Contains(searchString))
                        continue;
                    var storeItemProperty = itemsProperty.GetArrayElementAtIndex(index);
                    var prefabProperty = storeItemProperty.FindPropertyRelative(nameof(StoreItem.Prefab));

                    using (var changeScope = new EditorGUI.ChangeCheckScope())
                    using (new GUILayout.HorizontalScope())
                    {
                        var selected = selectedStoreItemIndex == index;
                        using (new EditorGUI.DisabledScope(prefab == null))
                        {
                            selected = ToggleLeft($"{storeItem.Name} ({storeItem.Price})", selected);
                        }

                        if (changeScope.changed)
                        {
                            selectedStoreItemIndex = selected ? index : -1;
                        }

                        if (!selected)
                            PropertyField(prefabProperty, GUIContent.none);
                        else
                            ObjectField(prefab, typeof(GameObject), false);
                    }
                }

                scrollPosition = scrollViewScope.scrollPosition;
            }
        }

        void OpenPrefabAndFocusScene(StoreItemViewModel prefab)
        {
            Selection.activeObject = prefab;
            GetWindow<SceneView>().Focus();
            Focus();
            AssetDatabase.OpenAsset(prefab);
        }
    }
}