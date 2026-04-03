using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Components
{
    [UxmlElement]
    public partial class Rating : VisualElement
    {
        private VisualElement[] stars;
        private int currentRating = 0;
        private int maxStars = 3;

        [SerializeField] private VectorImage starEmpty;
        [SerializeField] private VectorImage starActive;

        [UxmlAttribute("max-stars")]
        public int MaxStars
        {
            get => maxStars;
            set
            {
                maxStars = value;
                Init(maxStars);
            }
        }

        [UxmlAttribute]
        public int value
        {
            get => currentRating;
            set => SetRating(value);
        }

        private static StyleSheet styleSheet;

        public Rating()
        {
            LoadStyle();
        }

        private void LoadStyle()
        {
            if (styleSheet == null)
            {
                styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/UI/Styles/Rating.uss");
            }

            if (styleSheet != null && !styleSheets.Contains(styleSheet))
            {
                styleSheets.Add(styleSheet);
            }
        }
        public void Init(int count)
        {
            Clear();

            starEmpty = Resources.Load<VectorImage>("Icons/star-empty");
            starActive = Resources.Load<VectorImage>("Icons/star-active");

            stars = new VisualElement[count];

            AddToClassList("rating");

            for (int i = 0; i < count; i++)
            {
                int index = i;

                var star = new VisualElement();
                star.AddToClassList("star");

                if (index == count - 1)
                    star.AddToClassList("star--last");

                SetStarImage(star, false);

                star.RegisterCallback<ClickEvent>(_ =>
                {
                    int newValue = index + 1;

                    if (currentRating == newValue)
                        SetRating(0);
                    else
                        SetRating(newValue);
                });

                star.RegisterCallback<MouseEnterEvent>(_ =>
                {
                    PreviewRating(index + 1);
                });

                stars[i] = star;
                Add(star);
            }

            RegisterCallback<MouseLeaveEvent>(_ =>
            {
                RestoreRating();
            });
        }


        void SetStarImage(VisualElement star, bool active)
        {
            star.style.backgroundImage = new StyleBackground(
                active ? starActive : starEmpty
            );
        }

        public void SetRating(int value, bool notify = true)
        {
            currentRating = value;

            if (stars == null)
                return;

            for (int i = 0; i < stars.Length; i++)
            {
                SetStarImage(stars[i], i < value);
            }

            OnValueChanged?.Invoke(currentRating);

            if (notify)
                OnValueChanged?.Invoke(currentRating);
        }

        private void PreviewRating(int value)
        {
            if (stars == null)
                return;

            for (int i = 0; i < stars.Length; i++)
            {
                SetStarImage(stars[i], i < value);
            }
        }

        private void RestoreRating()
        {
            SetRating(currentRating);
        }

        public System.Action<int> OnValueChanged;
    }
}