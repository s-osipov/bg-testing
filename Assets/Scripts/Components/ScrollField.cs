using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using Helpers;

namespace Components
{
    [UxmlElement]
    public partial class ScrollField : VisualElement
    {
        private ScrollView _scrollView;
        private Label _label;
        private TextField contentField;
        public string Text
        {
            get => contentField.value;
            set => contentField.value = value;
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
            }
            else
            {
                Debug.LogError("Не удалось найти ScrollField.uxml");
            }

            var data = StringData.Load();
            Text = data.GetBigText();
        }
    }
}