#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.PackageManager.UI;
using UnityEngine;
using UnityEngine.UIElements;

namespace CupkekGames.Luna.Editor
{
    public class LunaUIWindow : EditorWindow
    {
        [SerializeField] private VisualTreeAsset _parent = default;
        [SerializeField] private VisualTreeAsset _sample = default;
        [SerializeField] private VisualTreeAsset _requirementLine = default;
        private static string[] _demoGameStandartScenePaths = new string[] {
        "Game/LunaUIDemoStandartInitialization",
        "Game/Scenes/LunaUIDemoStandartMainMenu",
        "Game/Scenes/LunaUIDemoStandartBase"
        };
        private static string[] _demoGameAddressableScenePaths = new string[] {
        "Game Addressables/LunaUIDemoAddressablesInitialization",
        "Game Addressables/Scenes/LunaUIDemoAddressablesMainMenu",
        "Game Addressables/Scenes/LunaUIDemoAddressablesGameManager",
        "Game Addressables/Scenes/LunaUIDemoAddressablesBase"
        };
        private static string[] _demoGameMobileScenePaths = new string[] {
        "Mobile/LunaUIDemoMobileInitialization",
        "Mobile/Scenes/LunaUIDemoMobileMainMenu",
        };
        private static string[] _sampleNames = new string[] {
        "Library",
        "Components",
        "Game",
        "Game Addressables",
        "Localization",
        "Ink",
        "Newtonsoft",
        "Mobile",
        };

        private static VisualElement _root;

        [MenuItem("Tools/CupkekGames/LunaUI Panel", false, 5)]
        public static void ShowUI()
        {
            LunaUIWindow wnd = GetWindow<LunaUIWindow>();
            wnd.titleContent = new GUIContent("LunaUI Panel");
            wnd.minSize = new Vector2(480, 480);
        }

        public void CreateGUI()
        {
            _root = rootVisualElement;

            CreateGUIInner(_root);
        }

        [InitializeOnLoadMethod]
        static void OnProjectLoadedInEditor()
        {
            IEnumerable<Sample> samples = Sample.FindByPackage("com.cupkekgames.luna", null);

            bool anyNewImport = false;
            List<int> newImports = new();

            int index = 0;

            foreach (Sample sample in samples)
            {
                string key = "LunaUI_" + sample.displayName + "_Imported";
                bool imported = EditorPrefs.GetBool(key, false);

                if (imported != sample.isImported)
                {
                    // new import or remove
                    EditorPrefs.SetBool(key, sample.isImported);

                    if (sample.isImported)
                    {
                        // new import
                        anyNewImport = true;
                        newImports.Add(index);
                    }
                }

                index++;
            }

            if (anyNewImport)
            {
                CloseWindows();

                ShowUI();

                if (_root != null)
                {
                    List<Foldout> foldouts = _root.Query<Foldout>("SampleName").ToList();

                    for (int i = 0; i < foldouts.Count; i++)
                    {
                        Foldout foldout = foldouts[i];

                        if (newImports.Contains(i))
                        {
                            foldout.value = true;
                        }
                        else
                        {
                            foldout.value = false;
                        }
                    }
                }
            }
        }

        // [MenuItem("Tools/CupkekGames/Close LunaUI Panels")]
        static void CloseWindows()
        {
            while (EditorWindow.HasOpenInstances<LunaUIWindow>())
            {
                EditorWindow window = EditorWindow.GetWindow(typeof(LunaUIWindow));
                window.Close();
            }
        }

        private void CreateGUIInner(VisualElement root)
        {
            // Instantiate UXML
            VisualElement tree = _parent.Instantiate();
            tree.style.flexGrow = new StyleFloat(1f);
            root.Add(tree);

            ScrollView scrollView = tree.Q<ScrollView>("LunaUIScroll");

            VisualElement firstContainer = scrollView.Q<VisualElement>("FirstContainer");

            Button openDocs = firstContainer.Q<Button>("OpenUrlDocs");
            openDocs.clicked += () =>
            {
                Application.OpenURL("https://docs.cupkek.games/");
            };

            bool imported = false;
#if UNITY_INPUT
            imported = true;
#endif
            firstContainer.Add(new Label("Input System is Optional."));
            firstContainer.Add(new Label("Required for full functionality of the InputPrompt component."));
            AddRequirementLine(firstContainer, "Input System", imported);

            int i = 0;

            IEnumerable<Sample> samples = Sample.FindByPackage("com.cupkekgames.luna", null);
            bool libraryImported = false;
            foreach (Sample sample in samples)
            {
                if (_sampleNames[i] == "Library")
                {
                    libraryImported = sample.isImported;
                }

                VisualElement container = _sample.Instantiate();
                scrollView.Add(container);

                if (i % 2 == 1)
                {
                    container.AddToClassList("odd_line");
                }
                else
                {
                    container.AddToClassList("even_line");
                }

                Foldout foldout = container.Q<Foldout>("SampleName");
                foldout.text = sample.displayName;
                foldout.value = false;
                container.Q<Label>("Description").text = sample.description;

                string importedIcon = sample.isImported ? "icon_checkmark" : "icon_cross";
                container.Q<VisualElement>("ImportedIcon").AddToClassList(importedIcon);

                HandleSample(container, sample.displayName, libraryImported);

                i++;
            }
        }

        private void HandleSample(VisualElement container, string name, bool libraryImported)
        {
            if (string.IsNullOrEmpty(name))
            {
                return;
            }

            VisualElement req = container.Q<VisualElement>("RequirementContainer");
            VisualElement setup = container.Q<VisualElement>("SetupContainer");
            VisualElement troubleshoot = container.Q<VisualElement>("TroubleshootContainer");

            Label setupTitle = setup.Q<Label>();

            if (name == _sampleNames[0])
            {
                req.style.display = DisplayStyle.None;
                setupTitle.style.display = DisplayStyle.None;
                troubleshoot.style.display = DisplayStyle.None;
            }
            else if (name == _sampleNames[1])
            {
                setupTitle.style.display = DisplayStyle.None;
                troubleshoot.style.display = DisplayStyle.None;

                AddRequirementLine(req, "Import Library sample", libraryImported);
            }
            else if (name == _sampleNames[2])
            {
                bool inputImported = false;
#if UNITY_INPUT
                inputImported = true;
#endif
                AddRequirementLine(req, "Input System (Required for input rebinding in settings)", inputImported);
                AddRequirementLine(req, "Import Library sample", libraryImported);

                setupTitle.text = "Setup:";

                bool sceneSetup = SceneSetupStandart();

                VisualElement sceneSetupReqVE = AddRequirementLine(setup, "Add demo scenes to the Build Settings", sceneSetup);

                VisualElement buttonContainer = new VisualElement();
                setup.Add(buttonContainer);
                buttonContainer.style.flexDirection = FlexDirection.Row;

                Button add = new Button();
                buttonContainer.Add(add);
                add.SetEnabled(libraryImported);
                add.style.flexGrow = new StyleFloat(1f);
                add.text = "Add Scenes";
                add.clicked += () =>
                {
                    BuildSettingsUtility.AddScenesToBuildSettingsInFolderAtStartWithIndexCheck(_demoGameStandartScenePaths, "Assets/Samples");
                    sceneSetup = SceneSetupStandart();
                    SetRequirementIcon(sceneSetupReqVE, sceneSetup);
                };

                Button remove = new Button();
                buttonContainer.Add(remove);
                remove.SetEnabled(sceneSetup);
                remove.style.flexGrow = new StyleFloat(1f);
                remove.text = "Remove Scenes";
                remove.clicked += () =>
                {
                    BuildSettingsUtility.RemoveScenesFromBuildSettings(_demoGameStandartScenePaths);
                    sceneSetup = SceneSetupStandart();
                    SetRequirementIcon(sceneSetupReqVE, sceneSetup);
                };

                troubleshoot.Add(new Label("If inputs doesn't work, make sure Project Settings > Input System Package > Project-wide Actions is set to Luna_InputSystem_Actions."));
            }
            else if (name == _sampleNames[3])
            {
                bool inputImported = false;
#if UNITY_INPUT
                inputImported = true;
#endif
                AddRequirementLine(req, "Input System (Required for input rebinding in settings)", inputImported);

                bool importedAddr = false;
#if UNITY_ADDRESSABLES
                importedAddr = true;
#endif
                AddRequirementLine(req, "Addressables", importedAddr);

                AddRequirementLine(req, "Import Library sample", libraryImported);

                setupTitle.text = "Setup:";


                bool setupAddr = false;
#if UNITY_ADDRESSABLES
                setupAddr = AddressableUtility.IsAddressableSettingsCreated();
#endif
                AddRequirementLine(setup, "Create Addressables Settings", setupAddr);
                if (!setupAddr)
                {
                    setup.Add(new Label("- Open: Window > Asset Management > Addressables > Groups"));
                    setup.Add(new Label("- Click: Create Addressable Settings"));
                }

                bool sceneSetup = SceneSetupAddressables();
                VisualElement sceneSetupReqVE = AddRequirementLine(setup, "Make demo scenes addressable", sceneSetup);

                VisualElement buttonContainer = new VisualElement();
                setup.Add(buttonContainer);
                buttonContainer.style.flexDirection = FlexDirection.Row;

                Button add = new Button();
                buttonContainer.Add(add);
                add.SetEnabled(libraryImported && setupAddr && importedAddr);
                add.style.flexGrow = new StyleFloat(1f);
                add.text = "Make Addressable";
#if UNITY_ADDRESSABLES
                add.clicked += () =>
                {
                    AddressableUtility.MakeScenesAddressable(_demoGameAddressableScenePaths, "Assets/Samples");
                    sceneSetup = SceneSetupAddressables();
                    SetRequirementIcon(sceneSetupReqVE, sceneSetup);
                };
#endif

                Button remove = new Button();
                buttonContainer.Add(remove);
                buttonContainer.SetEnabled(inputImported && setupAddr && importedAddr);
                remove.style.flexGrow = new StyleFloat(1f);
                remove.text = "Remove Addressable";
#if UNITY_ADDRESSABLES
                remove.clicked += () =>
                {
                    AddressableUtility.RemoveScenesAddressable(_demoGameAddressableScenePaths, "Assets/Samples");
                    sceneSetup = SceneSetupAddressables();
                    SetRequirementIcon(sceneSetupReqVE, sceneSetup);
                };
#endif

                troubleshoot.Add(new Label("If inputs doesn't work, make sure Project Settings > Input System Package > Project-wide Actions is set to Luna_InputSystem_Actions."));
            }
            else if (name == _sampleNames[4])
            {
                bool imported = false;
#if UNITY_ADDRESSABLES
                imported = true;
#endif
                AddRequirementLine(req, "Addressables", imported);

                imported = false;
#if UNITY_LOCALIZATION
                imported = true;
#endif
                AddRequirementLine(req, "Unity Localization", imported);
                AddRequirementLine(req, "Import Library sample", libraryImported);

                setupTitle.style.display = DisplayStyle.None;

                troubleshoot.Add(new Label("If it doesn't work, make sure Project Settings > Localization > Active Settings is assigned."));
            } else if (name == _sampleNames[5]) {
                // req.style.display = DisplayStyle.None;
                setupTitle.style.display = DisplayStyle.None;
                troubleshoot.style.display = DisplayStyle.None;

                Label reqLabel = new Label($"Requires ink-unity-integration package from asset store.");
                req.Add(reqLabel);
            } else if (name == _sampleNames[6]) {
                // req.style.display = DisplayStyle.None;
                setupTitle.style.display = DisplayStyle.None;
                troubleshoot.style.display = DisplayStyle.None;

                Label reqLabel = new Label($"Requires {RichTextColor.AQUA}'com.unity.nuget.newtonsoft-json'{RichTextColor.CLOSING_TAG} package.\n" +
                                          $"Implements a working {RichTextColor.LIME}save/load system{RichTextColor.CLOSING_TAG}.\n" +
                                          $"If you don't want to switch to another serialization solution, you can remove the {RichTextColor.YELLOW}serialization folder {RichTextColor.CLOSING_TAG}, " +
                                          $"and then update the {RichTextColor.ORANGE}UI code{RichTextColor.CLOSING_TAG} accordingly.");                                          
                req.Add(reqLabel);
                Label reqLabel2 = new Label($"You probably already have it installed, but if not, you can install it from the Package Manager.\n" +
                                          $"Click on the '+' button in the top left and select 'Install package by name'. Then enter {RichTextColor.AQUA}'com.unity.nuget.newtonsoft-json'{RichTextColor.CLOSING_TAG}.");
                reqLabel2.style.marginTop = 10;
                req.Add(reqLabel2);
            }
            else if (name == _sampleNames[7])
            {
                AddRequirementLine(req, "Import Library sample", libraryImported);

                setupTitle.text = "Setup:";

                bool sceneSetup = SceneSetupMobile();

                VisualElement sceneSetupReqVE = AddRequirementLine(setup, "Add mobile scenes to the Build Settings", sceneSetup);

                VisualElement buttonContainer = new VisualElement();
                setup.Add(buttonContainer);
                buttonContainer.style.flexDirection = FlexDirection.Row;

                Button add = new Button();
                buttonContainer.Add(add);
                add.SetEnabled(libraryImported);
                add.style.flexGrow = new StyleFloat(1f);
                add.text = "Add Scenes";
                add.clicked += () =>
                {
                    BuildSettingsUtility.AddScenesToBuildSettingsInFolderAtStartWithIndexCheck(_demoGameMobileScenePaths, "Assets/Samples");
                    sceneSetup = SceneSetupMobile();
                    SetRequirementIcon(sceneSetupReqVE, sceneSetup);
                };

                Button remove = new Button();
                buttonContainer.Add(remove);
                remove.SetEnabled(sceneSetup);
                remove.style.flexGrow = new StyleFloat(1f);
                remove.text = "Remove Scenes";
                remove.clicked += () =>
                {
                    BuildSettingsUtility.RemoveScenesFromBuildSettings(_demoGameMobileScenePaths);
                    sceneSetup = SceneSetupMobile();
                    SetRequirementIcon(sceneSetupReqVE, sceneSetup);
                };

                troubleshoot.Add(new Label("If inputs doesn't work, make sure Project Settings > Input System Package > Project-wide Actions is set to Luna_InputSystem_Actions."));
            }
        }

        private VisualElement AddRequirementLine(VisualElement parent, string text, bool imported)
        {
            VisualElement line = _requirementLine.Instantiate();
            parent.Add(line);

            line.Q<Label>().text = text;
            string importedIcon = imported ? "icon_checkmark" : "icon_cross";
            line.Q<VisualElement>("ImportedIcon").AddToClassList(importedIcon);

            return line;
        }

        private bool SceneSetupStandart()
        {
            bool sceneSetup = false;
            for (int i = 0; i < _demoGameStandartScenePaths.Length; i++)
            {
                sceneSetup = BuildSettingsUtility.IsDesiredSceneAtIndex(i, _demoGameStandartScenePaths[i]);
                if (!sceneSetup)
                {
                    break;
                }
            }
            return sceneSetup;
        }
        private bool SceneSetupMobile()
        {
            bool sceneSetup = false;
            for (int i = 0; i < _demoGameMobileScenePaths.Length; i++)
            {
                sceneSetup = BuildSettingsUtility.IsDesiredSceneAtIndex(i, _demoGameMobileScenePaths[i]);
                if (!sceneSetup)
                {
                    break;
                }
            }
            return sceneSetup;
        }
        private bool SceneSetupAddressables()
        {
            bool sceneSetup = false;
#if UNITY_ADDRESSABLES
            sceneSetup = AddressableUtility.AreScenesAddressable(_demoGameAddressableScenePaths, "Assets/Samples");
#endif
            return sceneSetup;
        }

        private void SetRequirementIcon(VisualElement requirement, bool value)
        {
            string importedIcon = value ? "icon_checkmark" : "icon_cross";
            VisualElement icon = requirement.Q<VisualElement>("ImportedIcon");
            icon.ClearClassList();
            icon.AddToClassList(importedIcon);
        }
    }
}
#endif