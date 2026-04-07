using UnityEngine;
using UnityEngine.UIElements;
using Helpers;

namespace Components
{
    [UxmlElement]
    public partial class ScrollField : VisualElement
    {
        private TextField contentField;
        public string Text
        {
            get => contentField?.value ?? string.Empty;
            set
            {
                if (contentField != null)
                    contentField.value = value ?? string.Empty;
            }
        }

        public ScrollField()
        {
            var styleSheet = Resources.Load<StyleSheet>("UI/Styles/ScrollField");
            if (styleSheet != null)
                styleSheets.Add(styleSheet);

            var asset = Resources.Load<VisualTreeAsset>("UI/Layouts/ScrollField");
            if (asset != null)
            {
                asset.CloneTree(this);
                contentField = this.Q<TextField>("content-field");
                if (contentField == null)
                    Debug.LogError("Не удалось найти 'content-field' в ScrollField.uxml");
            }
            else
            {
                Debug.LogError("Не удалось найти ScrollField.uxml");
            }

            var data = StringData.Load();
            if (data != null)
                Text = data.GetBigText() ?? string.Empty;
            else
                Debug.LogError("Не удалось загрузить StringData для ScrollField");
        }
    }
}