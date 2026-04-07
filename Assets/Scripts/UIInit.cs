using Helpers;
using UnityEngine;
using UnityEngine.UIElements;
using Components;

public class UIInit : MonoBehaviour
{
    public UIDocument uiDocument;

    private StringData strings;
    private PopupFlow flow;
    private EventCallback<GeometryChangedEvent> responsiveCallback;

    private const float MobileWidthThreshold = 600f;

    void Awake()
    {
        strings = StringData.Load();
        flow = new PopupFlow(strings);
    }

    void OnEnable()
    {
        var root = uiDocument.rootVisualElement;
        ApplyResponsiveClass(root);

        responsiveCallback = _ => ApplyResponsiveClass(root);
        root.RegisterCallback(responsiveCallback);

        var popup = root.Q<Popup>("popup");
        flow.Attach(popup, strings);
    }

    void OnDisable()
    {
        if (uiDocument != null && responsiveCallback != null)
            uiDocument.rootVisualElement.UnregisterCallback(responsiveCallback);

        flow?.Detach();
    }

    private static void ApplyResponsiveClass(VisualElement root)
    {
        if (root == null)
            return;

        float width = root.resolvedStyle.width;
        if (float.IsNaN(width) || width <= 0f)
            width = Screen.width;

        bool isMobile = width > 0f && width <= MobileWidthThreshold;

        if (isMobile) root.AddToClassList("is-mobile");
        else root.RemoveFromClassList("is-mobile");
    }
}
