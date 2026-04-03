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
            var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/UI/Styles/ScrollField.uss");
            if (styleSheet != null)
                styleSheets.Add(styleSheet);

            var asset = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/UI/Layouts/ScrollField.uxml");
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