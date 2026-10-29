using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class EscapeRoomRoomBuilder
{
    private const string ScenePath = "Assets/Scenes/SampleScene.unity";
    private const string LayoutRootName = "Escape Room Layout";
    private const string MaterialFolder = "Assets/Materials";

    [MenuItem("Tools/Escape Room/Rebuild Room Layout")]
    public static void BuildRoom()
    {
        Scene scene = EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single);

        GameObject existingLayout = GameObject.Find(LayoutRootName);
        if (existingLayout != null)
        {
            Object.DestroyImmediate(existingLayout);
        }

        EnsureMaterialFolder();

        Material floorMaterial = GetOrCreateMaterial("EscapeRoom_Floor", new Color(0.23f, 0.25f, 0.27f));
        Material wallMaterial = GetOrCreateMaterial("EscapeRoom_Wall", new Color(0.70f, 0.72f, 0.70f));
        Material trimMaterial = GetOrCreateMaterial("EscapeRoom_Trim", new Color(0.15f, 0.17f, 0.18f));
        Material doorMaterial = GetOrCreateMaterial("EscapeRoom_Door", new Color(0.12f, 0.28f, 0.42f));
        Material keycardMaterial = GetOrCreateMaterial("EscapeRoom_Keycard", new Color(1.0f, 0.84f, 0.12f));
        Material tableMaterial = GetOrCreateMaterial("EscapeRoom_Table", new Color(0.36f, 0.25f, 0.16f));
        Material buttonMaterial = GetOrCreateMaterial("EscapeRoom_Button", new Color(0.04f, 0.42f, 0.82f));
        Material trapMaterial = GetOrCreateMaterial("EscapeRoom_Trap", new Color(0.95f, 0.06f, 0.05f));
        Material exitMaterial = GetOrCreateMaterial("EscapeRoom_Exit", new Color(0.04f, 0.72f, 0.35f));
        Material noteMaterial = GetOrCreateMaterial("EscapeRoom_Note", new Color(0.92f, 0.88f, 0.70f));
        Material lightMaterial = GetOrCreateMaterial("EscapeRoom_Light", new Color(1.0f, 0.95f, 0.70f));

        GameObject layoutRoot = CreateGroup(LayoutRootName);
        GameObject shellRoot = CreateGroup("Room Shell", layoutRoot.transform);
        GameObject propsRoot = CreateGroup("Props", layoutRoot.transform);
        GameObject interactablesRoot = CreateGroup("Interactable Placeholders", layoutRoot.transform);
        GameObject hazardRoot = CreateGroup("Hazard Placeholder", layoutRoot.transform);
        GameObject exitRoot = CreateGroup("Exit Area", layoutRoot.transform);

        BuildRoomShell(shellRoot.transform, floorMaterial, wallMaterial, trimMaterial);
        BuildDoor(interactablesRoot.transform, doorMaterial, trimMaterial);
        BuildTableAndKeycard(propsRoot.transform, interactablesRoot.transform, tableMaterial, keycardMaterial);
        BuildButton(interactablesRoot.transform, buttonMaterial, trimMaterial);
        BuildTrap(hazardRoot.transform, trapMaterial, trimMaterial);
        BuildNote(interactablesRoot.transform, noteMaterial, trimMaterial);
        BuildExit(exitRoot.transform, exitMaterial, trimMaterial, floorMaterial);
        BuildLighting(layoutRoot.transform, lightMaterial);
        BuildSpawnPoint(layoutRoot.transform);
        PositionStarterCamera();

        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    private static void BuildRoomShell(Transform parent, Material floorMaterial, Material wallMaterial, Material trimMaterial)
    {
        CreateCube("Floor", parent, new Vector3(0f, -0.1f, 0f), Vector3.zero, new Vector3(10f, 0.2f, 12f), floorMaterial);
        CreateCube("Back Wall", parent, new Vector3(0f, 1.75f, -6f), Vector3.zero, new Vector3(10f, 3.5f, 0.2f), wallMaterial);
        CreateCube("Left Wall", parent, new Vector3(-5f, 1.75f, 0f), Vector3.zero, new Vector3(0.2f, 3.5f, 12f), wallMaterial);
        CreateCube("Right Wall", parent, new Vector3(5f, 1.75f, 0f), Vector3.zero, new Vector3(0.2f, 3.5f, 12f), wallMaterial);

        CreateCube("Front Wall Left", parent, new Vector3(-3f, 1.75f, 6f), Vector3.zero, new Vector3(4f, 3.5f, 0.2f), wallMaterial);
        CreateCube("Front Wall Right", parent, new Vector3(3f, 1.75f, 6f), Vector3.zero, new Vector3(4f, 3.5f, 0.2f), wallMaterial);
        CreateCube("Doorway Header", parent, new Vector3(0f, 3f, 6f), Vector3.zero, new Vector3(2f, 1f, 0.2f), wallMaterial);

        CreateCube("Back Floor Trim", parent, new Vector3(0f, 0.08f, -5.88f), Vector3.zero, new Vector3(10f, 0.16f, 0.08f), trimMaterial);
        CreateCube("Left Floor Trim", parent, new Vector3(-4.88f, 0.08f, 0f), Vector3.zero, new Vector3(0.08f, 0.16f, 12f), trimMaterial);
        CreateCube("Right Floor Trim", parent, new Vector3(4.88f, 0.08f, 0f), Vector3.zero, new Vector3(0.08f, 0.16f, 12f), trimMaterial);
        CreateCube("Door Threshold", parent, new Vector3(0f, 0.04f, 5.9f), Vector3.zero, new Vector3(2f, 0.08f, 0.4f), trimMaterial);
    }

    private static void BuildDoor(Transform parent, Material doorMaterial, Material trimMaterial)
    {
        GameObject pivot = CreateGroup("Main Door Pivot", parent);
        pivot.transform.localPosition = new Vector3(-0.9f, 0f, 5.82f);

        GameObject door = CreateCube("Locked Door", pivot.transform, new Vector3(0.8f, 1.25f, 0f), Vector3.zero, new Vector3(1.6f, 2.5f, 0.18f), doorMaterial);
        CreateCube("Door Handle", door.transform, new Vector3(0.36f, 0f, -0.6f), Vector3.zero, new Vector3(0.12f, 0.12f, 0.18f), trimMaterial);
    }

    private static void BuildTableAndKeycard(Transform propsParent, Transform interactablesParent, Material tableMaterial, Material keycardMaterial)
    {
        GameObject table = CreateGroup("Keycard Table", propsParent);
        table.transform.localPosition = new Vector3(-3.55f, 0f, -3.4f);

        CreateCube("Table Top", table.transform, new Vector3(0f, 0.82f, 0f), Vector3.zero, new Vector3(1.7f, 0.16f, 0.9f), tableMaterial);
        CreateCube("Table Leg Front Left", table.transform, new Vector3(-0.7f, 0.4f, 0.32f), Vector3.zero, new Vector3(0.12f, 0.8f, 0.12f), tableMaterial);
        CreateCube("Table Leg Front Right", table.transform, new Vector3(0.7f, 0.4f, 0.32f), Vector3.zero, new Vector3(0.12f, 0.8f, 0.12f), tableMaterial);
        CreateCube("Table Leg Back Left", table.transform, new Vector3(-0.7f, 0.4f, -0.32f), Vector3.zero, new Vector3(0.12f, 0.8f, 0.12f), tableMaterial);
        CreateCube("Table Leg Back Right", table.transform, new Vector3(0.7f, 0.4f, -0.32f), Vector3.zero, new Vector3(0.12f, 0.8f, 0.12f), tableMaterial);

        CreateCube("Keycard Placeholder", interactablesParent, new Vector3(-3.55f, 0.94f, -3.4f), Vector3.zero, new Vector3(0.55f, 0.04f, 0.32f), keycardMaterial);
    }

    private static void BuildButton(Transform parent, Material buttonMaterial, Material trimMaterial)
    {
        GameObject buttonRoot = CreateGroup("Button Switch Placeholder", parent);
        buttonRoot.transform.localPosition = new Vector3(4.88f, 1.25f, 3.8f);
        buttonRoot.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);

        CreateCube("Button Backplate", buttonRoot.transform, new Vector3(-0.01f, 0f, 0f), Vector3.zero, new Vector3(0.08f, 0.55f, 0.55f), trimMaterial);
        GameObject button = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        button.name = "Blue Button";
        button.transform.SetParent(buttonRoot.transform, false);
        button.transform.localPosition = new Vector3(-0.08f, 0f, 0f);
        button.transform.localRotation = Quaternion.Euler(0f, 0f, 90f);
        button.transform.localScale = new Vector3(0.28f, 0.08f, 0.28f);
        ApplyMaterial(button, buttonMaterial);
    }

    private static void BuildTrap(Transform parent, Material trapMaterial, Material trimMaterial)
    {
        GameObject trapRoot = CreateGroup("Laser Trap Placeholder", parent);
        trapRoot.transform.localPosition = new Vector3(0f, 0f, 4.55f);

        CreateCube("Laser Beam", trapRoot.transform, new Vector3(0f, 1.1f, 0f), Vector3.zero, new Vector3(8.8f, 0.08f, 0.08f), trapMaterial);
        CreateCube("Laser Emitter Left", trapRoot.transform, new Vector3(-4.55f, 1.1f, 0f), Vector3.zero, new Vector3(0.28f, 0.42f, 0.28f), trimMaterial);
        CreateCube("Laser Emitter Right", trapRoot.transform, new Vector3(4.55f, 1.1f, 0f), Vector3.zero, new Vector3(0.28f, 0.42f, 0.28f), trimMaterial);
        CreateCube("Danger Floor Marker", trapRoot.transform, new Vector3(0f, 0.01f, 0f), Vector3.zero, new Vector3(8.6f, 0.02f, 0.7f), trapMaterial);
    }

    private static void BuildNote(Transform parent, Material noteMaterial, Material trimMaterial)
    {
        GameObject noteRoot = CreateGroup("Hint Note Placeholder", parent);
        noteRoot.transform.localPosition = new Vector3(-1.6f, 1.6f, -5.88f);

        CreateCube("Pinned Note", noteRoot.transform, Vector3.zero, Vector3.zero, new Vector3(0.72f, 0.52f, 0.04f), noteMaterial);
        CreateCube("Note Pin", noteRoot.transform, new Vector3(0f, 0.21f, -0.04f), Vector3.zero, new Vector3(0.08f, 0.08f, 0.08f), trimMaterial);
    }

    private static void BuildExit(Transform parent, Material exitMaterial, Material trimMaterial, Material floorMaterial)
    {
        CreateCube("Exit Hall Floor", parent, new Vector3(0f, -0.1f, 7.35f), Vector3.zero, new Vector3(2.3f, 0.2f, 2.7f), floorMaterial);
        CreateCube("Exit Left Rail", parent, new Vector3(-1.15f, 0.65f, 7.35f), Vector3.zero, new Vector3(0.14f, 1.3f, 2.7f), trimMaterial);
        CreateCube("Exit Right Rail", parent, new Vector3(1.15f, 0.65f, 7.35f), Vector3.zero, new Vector3(0.14f, 1.3f, 2.7f), trimMaterial);
        CreateCube("Escape Zone Placeholder", parent, new Vector3(0f, 0.02f, 8.35f), Vector3.zero, new Vector3(1.8f, 0.04f, 0.8f), exitMaterial);
    }

    private static void BuildLighting(Transform parent, Material lightMaterial)
    {
        GameObject fixture = CreateCube("Ceiling Light Fixture", parent, new Vector3(0f, 3.25f, 0f), Vector3.zero, new Vector3(1.2f, 0.08f, 1.2f), lightMaterial);
        Object.DestroyImmediate(fixture.GetComponent<BoxCollider>());

        GameObject lightObject = new GameObject("Room Point Light");
        lightObject.transform.SetParent(parent, false);
        lightObject.transform.localPosition = new Vector3(0f, 3.05f, 0f);
        Light roomLight = lightObject.AddComponent<Light>();
        roomLight.type = LightType.Point;
        roomLight.color = new Color(1f, 0.94f, 0.78f);
        roomLight.intensity = 8f;
        roomLight.range = 12f;
        roomLight.shadows = LightShadows.None;
    }

    private static void BuildSpawnPoint(Transform parent)
    {
        GameObject spawnPoint = CreateGroup("Player Spawn Point", parent);
        spawnPoint.transform.localPosition = new Vector3(0f, 0f, -4.4f);
    }

    private static void PositionStarterCamera()
    {
        Camera camera = Camera.main;
        if (camera == null)
        {
            GameObject cameraObject = new GameObject("Main Camera");
            cameraObject.tag = "MainCamera";
            camera = cameraObject.AddComponent<Camera>();
            cameraObject.AddComponent<AudioListener>();
        }

        camera.transform.position = new Vector3(0f, 1.6f, -4.4f);
        camera.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
        camera.fieldOfView = 68f;
    }

    private static GameObject CreateGroup(string name, Transform parent = null)
    {
        GameObject group = new GameObject(name);
        if (parent != null)
        {
            group.transform.SetParent(parent, false);
        }

        return group;
    }

    private static GameObject CreateCube(string name, Transform parent, Vector3 localPosition, Vector3 localEulerAngles, Vector3 localScale, Material material)
    {
        GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cube.name = name;
        cube.transform.SetParent(parent, false);
        cube.transform.localPosition = localPosition;
        cube.transform.localRotation = Quaternion.Euler(localEulerAngles);
        cube.transform.localScale = localScale;
        ApplyMaterial(cube, material);
        return cube;
    }

    private static void ApplyMaterial(GameObject gameObject, Material material)
    {
        MeshRenderer renderer = gameObject.GetComponent<MeshRenderer>();
        if (renderer != null)
        {
            renderer.sharedMaterial = material;
        }
    }

    private static void EnsureMaterialFolder()
    {
        if (!AssetDatabase.IsValidFolder(MaterialFolder))
        {
            AssetDatabase.CreateFolder("Assets", "Materials");
        }
    }

    private static Material GetOrCreateMaterial(string materialName, Color color)
    {
        string assetPath = $"{MaterialFolder}/{materialName}.mat";
        Material material = AssetDatabase.LoadAssetAtPath<Material>(assetPath);
        if (material == null)
        {
            Shader shader = Shader.Find("Universal Render Pipeline/Lit");
            if (shader == null)
            {
                shader = Shader.Find("Standard");
            }

            material = new Material(shader)
            {
                name = materialName
            };
            AssetDatabase.CreateAsset(material, assetPath);
        }

        if (material.HasProperty("_BaseColor"))
        {
            material.SetColor("_BaseColor", color);
        }
        else
        {
            material.color = color;
        }

        EditorUtility.SetDirty(material);
        return material;
    }
}
