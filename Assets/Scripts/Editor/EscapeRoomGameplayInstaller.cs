using System;
using UnityEditor;
using UnityEditorInternal;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public static class EscapeRoomGameplayInstaller
{
    private const string ScenePath = "Assets/Scenes/SampleScene.unity";
    private const string InputActionsPath = "Assets/InputSystem_Actions.inputactions";

    [MenuItem("Tools/Escape Room/Install Gameplay")]
    public static void InstallGameplay()
    {
        Scene scene = EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single);

        EnsureTag("Player");

        UIManager uiManager = CreateUI();
        Transform spawnPoint = FindRequired("Player Spawn Point").transform;
        GameObject player = CreatePlayer(spawnPoint, uiManager, out CharacterController characterController, out SimpleFirstPersonController movement, out PlayerInteractor interactor);
        GameManager gameManager = CreateGameManager(uiManager, player.transform, characterController, movement, interactor, spawnPoint);

        interactor.gameManager = gameManager;
        interactor.uiManager = uiManager;

        ConfigureInteractables(gameManager);

        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Selection.activeGameObject = player;
    }

    private static UIManager CreateUI()
    {
        DestroyIfExists("Escape Room UI");

        GameObject canvasObject = new GameObject("Escape Room UI", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster), typeof(UIManager));
        Canvas canvas = canvasObject.GetComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;

        CanvasScaler scaler = canvasObject.GetComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920f, 1080f);
        scaler.matchWidthOrHeight = 0.5f;

        Font font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        if (font == null)
        {
            font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        }

        Text promptText = CreateText(
            "Prompt Text",
            canvasObject.transform,
            font,
            28,
            TextAnchor.MiddleCenter,
            new Vector2(0.5f, 0f),
            new Vector2(0.5f, 0f),
            new Vector2(0f, 92f),
            new Vector2(760f, 52f),
            Color.white);

        Text messageText = CreateText(
            "Message Text",
            canvasObject.transform,
            font,
            34,
            TextAnchor.MiddleCenter,
            new Vector2(0.5f, 1f),
            new Vector2(0.5f, 1f),
            new Vector2(0f, -70f),
            new Vector2(900f, 72f),
            Color.white);

        Text inventoryText = CreateText(
            "Inventory Text",
            canvasObject.transform,
            font,
            24,
            TextAnchor.UpperLeft,
            new Vector2(0f, 1f),
            new Vector2(0f, 1f),
            new Vector2(28f, -24f),
            new Vector2(380f, 48f),
            Color.white);

        Text crosshairText = CreateText(
            "Crosshair Text",
            canvasObject.transform,
            font,
            28,
            TextAnchor.MiddleCenter,
            new Vector2(0.5f, 0.5f),
            new Vector2(0.5f, 0.5f),
            Vector2.zero,
            new Vector2(44f, 44f),
            Color.white);
        crosshairText.text = "+";

        Text endHelpText = CreateText(
            "End Help Text",
            canvasObject.transform,
            font,
            26,
            TextAnchor.MiddleCenter,
            new Vector2(0.5f, 0.5f),
            new Vector2(0.5f, 0.5f),
            new Vector2(0f, 70f),
            new Vector2(760f, 48f),
            Color.white);

        Button playAgainButton = CreateButton(
            "Play Again Button",
            canvasObject.transform,
            font,
            new Vector2(0.5f, 0.5f),
            new Vector2(0.5f, 0.5f),
            new Vector2(0f, 5f),
            new Vector2(260f, 64f),
            "Play Again");

        promptText.gameObject.SetActive(false);
        messageText.gameObject.SetActive(false);
        endHelpText.gameObject.SetActive(false);
        playAgainButton.gameObject.SetActive(false);
        inventoryText.text = "Keycard: Missing";

        UIManager uiManager = canvasObject.GetComponent<UIManager>();
        uiManager.promptText = promptText;
        uiManager.messageText = messageText;
        uiManager.inventoryText = inventoryText;
        uiManager.endHelpText = endHelpText;
        uiManager.playAgainButton = playAgainButton;

        CreateEventSystem();
        return uiManager;
    }

    private static Text CreateText(
        string name,
        Transform parent,
        Font font,
        int fontSize,
        TextAnchor alignment,
        Vector2 anchorMin,
        Vector2 anchorMax,
        Vector2 anchoredPosition,
        Vector2 sizeDelta,
        Color color)
    {
        GameObject textObject = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(Text), typeof(Outline));
        textObject.transform.SetParent(parent, false);

        RectTransform rectTransform = textObject.GetComponent<RectTransform>();
        rectTransform.anchorMin = anchorMin;
        rectTransform.anchorMax = anchorMax;
        rectTransform.anchoredPosition = anchoredPosition;
        rectTransform.sizeDelta = sizeDelta;

        Text text = textObject.GetComponent<Text>();
        text.font = font;
        text.fontSize = fontSize;
        text.alignment = alignment;
        text.color = color;
        text.raycastTarget = false;
        text.horizontalOverflow = HorizontalWrapMode.Wrap;
        text.verticalOverflow = VerticalWrapMode.Overflow;

        Outline outline = textObject.GetComponent<Outline>();
        outline.effectColor = new Color(0f, 0f, 0f, 0.82f);
        outline.effectDistance = new Vector2(2f, -2f);

        return text;
    }

    private static Button CreateButton(
        string name,
        Transform parent,
        Font font,
        Vector2 anchorMin,
        Vector2 anchorMax,
        Vector2 anchoredPosition,
        Vector2 sizeDelta,
        string label)
    {
        GameObject buttonObject = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(Button));
        buttonObject.transform.SetParent(parent, false);

        RectTransform rectTransform = buttonObject.GetComponent<RectTransform>();
        rectTransform.anchorMin = anchorMin;
        rectTransform.anchorMax = anchorMax;
        rectTransform.anchoredPosition = anchoredPosition;
        rectTransform.sizeDelta = sizeDelta;

        Image image = buttonObject.GetComponent<Image>();
        image.color = new Color(0.08f, 0.18f, 0.24f, 0.92f);

        Button button = buttonObject.GetComponent<Button>();
        ColorBlock colors = button.colors;
        colors.normalColor = image.color;
        colors.highlightedColor = new Color(0.12f, 0.32f, 0.42f, 0.95f);
        colors.pressedColor = new Color(0.05f, 0.13f, 0.18f, 1f);
        colors.selectedColor = colors.highlightedColor;
        button.colors = colors;

        Text labelText = CreateText(
            "Label",
            buttonObject.transform,
            font,
            28,
            TextAnchor.MiddleCenter,
            Vector2.zero,
            Vector2.one,
            Vector2.zero,
            Vector2.zero,
            Color.white);
        labelText.text = label;

        RectTransform labelRect = labelText.GetComponent<RectTransform>();
        labelRect.offsetMin = Vector2.zero;
        labelRect.offsetMax = Vector2.zero;

        return button;
    }

    private static void CreateEventSystem()
    {
        DestroyIfExists("EventSystem");

        GameObject eventSystemObject = new GameObject("EventSystem", typeof(EventSystem), typeof(InputSystemUIInputModule));
        InputSystemUIInputModule inputModule = eventSystemObject.GetComponent<InputSystemUIInputModule>();
        inputModule.moveRepeatDelay = 0.5f;
        inputModule.moveRepeatRate = 0.1f;

        InputActionAsset actionsAsset = AssetDatabase.LoadAssetAtPath<InputActionAsset>(InputActionsPath);
        if (actionsAsset == null)
        {
            throw new InvalidOperationException($"Input actions asset was not found: {InputActionsPath}");
        }

        SerializedObject serializedModule = new SerializedObject(inputModule);
        SetObjectReference(serializedModule, "m_ActionsAsset", actionsAsset);
        SetObjectReference(serializedModule, "m_PointAction", FindActionReference("UI", "Point"));
        SetObjectReference(serializedModule, "m_MoveAction", FindActionReference("UI", "Navigate"));
        SetObjectReference(serializedModule, "m_SubmitAction", FindActionReference("UI", "Submit"));
        SetObjectReference(serializedModule, "m_CancelAction", FindActionReference("UI", "Cancel"));
        SetObjectReference(serializedModule, "m_LeftClickAction", FindActionReference("UI", "Click"));
        SetObjectReference(serializedModule, "m_MiddleClickAction", FindActionReference("UI", "MiddleClick"));
        SetObjectReference(serializedModule, "m_RightClickAction", FindActionReference("UI", "RightClick"));
        SetObjectReference(serializedModule, "m_ScrollWheelAction", FindActionReference("UI", "ScrollWheel"));
        SetObjectReference(serializedModule, "m_TrackedDevicePositionAction", null);
        SetObjectReference(serializedModule, "m_TrackedDeviceOrientationAction", null);
        serializedModule.ApplyModifiedPropertiesWithoutUndo();
    }

    private static void SetObjectReference(SerializedObject serializedObject, string propertyName, UnityEngine.Object reference)
    {
        SerializedProperty property = serializedObject.FindProperty(propertyName);
        if (property == null)
        {
            throw new InvalidOperationException($"Serialized property was not found: {propertyName}");
        }

        property.objectReferenceValue = reference;
    }

    private static InputActionReference FindActionReference(string mapName, string actionName)
    {
        foreach (UnityEngine.Object asset in AssetDatabase.LoadAllAssetsAtPath(InputActionsPath))
        {
            if (asset is not InputActionReference reference || reference.action == null)
            {
                continue;
            }

            if (reference.action.name == actionName && reference.action.actionMap != null && reference.action.actionMap.name == mapName)
            {
                return reference;
            }
        }

        throw new InvalidOperationException($"Input action reference was not found: {mapName}/{actionName}");
    }

    private static GameObject CreatePlayer(
        Transform spawnPoint,
        UIManager uiManager,
        out CharacterController characterController,
        out SimpleFirstPersonController movement,
        out PlayerInteractor interactor)
    {
        Camera camera = Camera.main;
        GameObject oldPlayer = GameObject.Find("Player");
        if (oldPlayer != null)
        {
            if (camera != null && camera.transform.IsChildOf(oldPlayer.transform))
            {
                camera.transform.SetParent(null);
            }

            UnityEngine.Object.DestroyImmediate(oldPlayer);
        }

        if (camera == null)
        {
            camera = UnityEngine.Object.FindAnyObjectByType<Camera>();
        }

        if (camera == null)
        {
            GameObject cameraObject = new GameObject("Main Camera", typeof(Camera), typeof(AudioListener));
            cameraObject.tag = "MainCamera";
            camera = cameraObject.GetComponent<Camera>();
        }

        GameObject player = new GameObject("Player");
        player.tag = "Player";
        player.transform.SetPositionAndRotation(spawnPoint.position, spawnPoint.rotation);

        characterController = player.AddComponent<CharacterController>();
        characterController.height = 1.8f;
        characterController.radius = 0.35f;
        characterController.center = new Vector3(0f, 0.9f, 0f);
        characterController.stepOffset = 0.35f;
        characterController.slopeLimit = 45f;

        movement = player.AddComponent<SimpleFirstPersonController>();
        movement.moveSpeed = 4f;
        movement.lookSensitivity = 0.08f;

        interactor = player.AddComponent<PlayerInteractor>();
        interactor.interactRange = 3.25f;
        interactor.interactMask = Physics.DefaultRaycastLayers;
        interactor.uiManager = uiManager;

        camera.gameObject.tag = "MainCamera";
        camera.transform.SetParent(player.transform, false);
        camera.transform.localPosition = new Vector3(0f, 1.6f, 0f);
        camera.transform.localRotation = Quaternion.identity;
        camera.fieldOfView = 68f;

        if (camera.GetComponent<AudioListener>() == null)
        {
            camera.gameObject.AddComponent<AudioListener>();
        }

        movement.playerCamera = camera;
        interactor.viewCamera = camera;

        return player;
    }

    private static GameManager CreateGameManager(
        UIManager uiManager,
        Transform playerTransform,
        CharacterController characterController,
        SimpleFirstPersonController movement,
        PlayerInteractor interactor,
        Transform spawnPoint)
    {
        DestroyIfExists("Game Manager");

        GameObject gameManagerObject = new GameObject("Game Manager", typeof(GameManager));
        GameManager gameManager = gameManagerObject.GetComponent<GameManager>();
        gameManager.uiManager = uiManager;
        gameManager.playerTransform = playerTransform;
        gameManager.playerController = characterController;
        gameManager.firstPersonController = movement;
        gameManager.playerInteractor = interactor;
        gameManager.playerSpawnPoint = spawnPoint;
        return gameManager;
    }

    private static void ConfigureInteractables(GameManager gameManager)
    {
        GameObject keycardObject = FindRequired("Keycard Placeholder");
        Keycard keycard = GetOrAddComponent<Keycard>(keycardObject);
        keycard.gameManager = gameManager;

        GameObject doorObject = FindRequired("Locked Door");
        Door door = GetOrAddComponent<Door>(doorObject);
        door.gameManager = gameManager;
        door.doorPivot = FindRequired("Main Door Pivot").transform;
        door.openLocalEulerAngles = new Vector3(0f, 95f, 0f);
        door.openDuration = 0.75f;

        Trap trap = ConfigureTrap(gameManager);

        GameObject buttonObject = FindRequired("Button Switch Placeholder");
        ButtonSwitch button = GetOrAddComponent<ButtonSwitch>(buttonObject);
        button.gameManager = gameManager;
        button.targetTrap = trap;
        button.buttonVisual = buttonObject.transform.Find("Blue Button");
        button.buttonRenderer = button.buttonVisual != null ? button.buttonVisual.GetComponent<Renderer>() : null;

        GameObject noteObject = FindRequired("Hint Note Placeholder");
        Note note = GetOrAddComponent<Note>(noteObject);
        note.gameManager = gameManager;
        note.noteText = "Hint: the blue wall button disables the laser.";

        ConfigureEscapeZone(gameManager);
    }

    private static Trap ConfigureTrap(GameManager gameManager)
    {
        GameObject trapObject = FindRequired("Laser Trap Placeholder");
        trapObject.transform.localPosition = new Vector3(0f, 0f, 4.55f);

        BoxCollider triggerCollider = GetOrAddComponent<BoxCollider>(trapObject);
        triggerCollider.isTrigger = true;
        triggerCollider.center = new Vector3(0f, 0.6f, 0f);
        triggerCollider.size = new Vector3(8.8f, 1.4f, 0.75f);

        Rigidbody rigidbody = GetOrAddComponent<Rigidbody>(trapObject);
        rigidbody.isKinematic = true;
        rigidbody.useGravity = false;

        Trap trap = GetOrAddComponent<Trap>(trapObject);
        trap.gameManager = gameManager;
        return trap;
    }

    private static void ConfigureEscapeZone(GameManager gameManager)
    {
        GameObject escapePad = FindRequired("Escape Zone Placeholder");
        Collider padCollider = escapePad.GetComponent<Collider>();
        if (padCollider != null)
        {
            padCollider.enabled = false;
        }

        DestroyIfExists("Escape Zone Trigger");

        GameObject triggerObject = new GameObject("Escape Zone Trigger", typeof(BoxCollider), typeof(Rigidbody), typeof(EscapeZone));
        triggerObject.transform.SetParent(escapePad.transform.parent, false);
        triggerObject.transform.position = new Vector3(escapePad.transform.position.x, 1f, escapePad.transform.position.z);
        triggerObject.transform.rotation = Quaternion.identity;
        triggerObject.transform.localScale = Vector3.one;

        BoxCollider triggerCollider = triggerObject.GetComponent<BoxCollider>();
        triggerCollider.isTrigger = true;
        triggerCollider.size = new Vector3(1.8f, 2f, 0.8f);
        triggerCollider.center = Vector3.zero;

        Rigidbody rigidbody = triggerObject.GetComponent<Rigidbody>();
        rigidbody.isKinematic = true;
        rigidbody.useGravity = false;

        EscapeZone escapeZone = triggerObject.GetComponent<EscapeZone>();
        escapeZone.gameManager = gameManager;
    }

    private static T GetOrAddComponent<T>(GameObject gameObject) where T : Component
    {
        T component = gameObject.GetComponent<T>();
        if (component == null)
        {
            component = gameObject.AddComponent<T>();
        }

        return component;
    }

    private static GameObject FindRequired(string objectName)
    {
        GameObject gameObject = GameObject.Find(objectName);
        if (gameObject == null)
        {
            throw new InvalidOperationException($"Required scene object was not found: {objectName}");
        }

        return gameObject;
    }

    private static void DestroyIfExists(string objectName)
    {
        GameObject gameObject = GameObject.Find(objectName);
        if (gameObject != null)
        {
            UnityEngine.Object.DestroyImmediate(gameObject);
        }
    }

    private static void EnsureTag(string tag)
    {
        foreach (string existingTag in InternalEditorUtility.tags)
        {
            if (existingTag == tag)
            {
                return;
            }
        }

        UnityEngine.Object[] tagManagerAssets = AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset");
        if (tagManagerAssets.Length == 0)
        {
            return;
        }

        SerializedObject tagManager = new SerializedObject(tagManagerAssets[0]);
        SerializedProperty tags = tagManager.FindProperty("tags");
        if (tags == null)
        {
            return;
        }

        for (int i = 0; i < tags.arraySize; i++)
        {
            if (tags.GetArrayElementAtIndex(i).stringValue == tag)
            {
                return;
            }
        }

        tags.InsertArrayElementAtIndex(tags.arraySize);
        tags.GetArrayElementAtIndex(tags.arraySize - 1).stringValue = tag;
        tagManager.ApplyModifiedProperties();
    }
}
