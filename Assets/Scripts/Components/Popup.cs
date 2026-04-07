using UnityEngine;
using UnityEngine.UIElements;

namespace Components
{
    [UxmlElement]
    public partial class Popup : VisualElement
    {
        public Popup()
        {
            var asset = Resources.Load<VisualTreeAsset>("UI/Layouts/Popup");
            if (asset != null)
                asset.CloneTree(this);
            else
                Debug.LogError("Не удалось найти Popup.uxml");
        }
    }
}
