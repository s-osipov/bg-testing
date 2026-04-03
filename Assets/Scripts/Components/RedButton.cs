using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Components
{
    [UxmlElement]
    public partial class RedButton : Button
    {
        private Image iconElement;

        [UxmlAttribute]
        public string label
        {
            get => text;
            set => text = value;
        }

        [UxmlAttribute]
        public Object icon
        {
            set
            {
                if (iconElement != null)
                {
                    if (value is Texture2D texture)
                    {
                        iconElement.image = texture;
                    }
                    else if (value is VectorImage vector)
                    {
                        iconElement.vectorImage = vector;
                    }

                    iconElement.style.display = value != null
                        ? DisplayStyle.Flex
                        : DisplayStyle.None;
                }
            }
        }

        [UxmlAttribute]
        public bool round
        {
            set
            {
                if (value)
                {
                    AddToClassList("round");
                }
                else
                {
                    RemoveFromClassList("round");
                }
            }
        }

        public RedButton()
        {
            var styleSheet = Resources.Load<StyleSheet>("UI/Styles/RedButton");
            if (styleSheet != null)
                styleSheets.Add(styleSheet);

            AddToClassList("red-button");

            var content = new VisualElement();
            content.AddToClassList("content");

            iconElement = new Image();
            iconElement.AddToClassList("icon");
            iconElement.style.display = DisplayStyle.None;

            var textElement = this.Q<Label>();
            if (textElement != null)
            {
                textElement.RemoveFromHierarchy();
                content.Add(iconElement);
                content.Add(textElement);
                Add(content);
            }
            else
            {
                content.Add(iconElement);
                Add(content);
            }
        }
    }
}