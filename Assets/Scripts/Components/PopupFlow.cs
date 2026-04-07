using System;
using Enums;
using Helpers;
using States;
using UnityEngine;
using UnityEngine.UIElements;

namespace Components
{
    public class PopupFlow
    {
        class Title
        {
            public string text;
            public string resultText;
            public CardState resultState;

            public Title(string text, string resultText, CardState state)
            {
                this.text = text;
                this.resultText = resultText;
                this.resultState = state;
            }
        }

        private readonly Title[] titles;
        private readonly RatingManager rm = new();

        private Label subtitle;
        private VisualElement navigationEl;
        private VisualElement closeEl;
        private VisualElement textEl;
        private VisualElement ratingEl;
        private Image eyeIcon;
        private RedButton leftBtn;
        private RedButton rightBtn;
        private RedButton chooseBtn;
        private RedButton closeEmptyBtn;
        private RedButton closeTextBtn;
        private Rating rating;
        private Popup popupRoot;
        private ScrollField textField;
        private EventCallback<GeometryChangedEvent> onPopupGeometryChanged;
        private IVisualElementScheduledItem subtitleSwapTask;
        private IVisualElementScheduledItem subtitleCleanupTask;

        private int index;
        private CardState currentState;
        private Action<int> onRatingChanged;

        public PopupFlow(StringData strings)
        {
            titles = new Title[]
            {
                new(strings.Get("dispatcher"), strings.Get("think"), CardState.Result),
                new(strings.Get("mean"), "", CardState.FullText)
            };
        }

        public void Attach(Popup popup, StringData strings)
        {
            popupRoot = popup;
            subtitle = popup.Q<Label>("subtitle");
            navigationEl = popup.Q<VisualElement>("navigationEl");
            closeEl = popup.Q<VisualElement>("closeEl");
            textEl = popup.Q<VisualElement>("textEl");
            ratingEl = popup.Q<VisualElement>("ratingEl");
            rating = popup.Q<Rating>("rating");
            textField = popup.Q<ScrollField>("textField");

            leftBtn = popup.Q<RedButton>("leftBtn");
            rightBtn = popup.Q<RedButton>("rightBtn");
            chooseBtn = popup.Q<RedButton>("chooseBtn");
            chooseBtn.text = strings.Get("choose");

            closeEmptyBtn = popup.Q<RedButton>("closeEmptyBtn");
            closeTextBtn = popup.Q<RedButton>("closeTextBtn");

            eyeIcon = popup.Q<Image>("icon");

            leftBtn.clicked += OnLeft;
            rightBtn.clicked += OnRight;
            chooseBtn.clicked += OnChoose;
            closeEmptyBtn.clicked += OnClose;
            closeTextBtn.clicked += OnClose;

            onRatingChanged = value => rm.SaveRating(currentState, value);
            rating.OnValueChanged += onRatingChanged;

            onPopupGeometryChanged = _ => UpdateScrollFieldBounds();
            popup.RegisterCallback(onPopupGeometryChanged);
            UpdateScrollFieldBounds();

            index = 0;
            UpdateText();
            SetState(CardState.Selection);
        }

        public void Detach()
        {
            if (popupRoot != null && onPopupGeometryChanged != null)
                popupRoot.UnregisterCallback(onPopupGeometryChanged);

            if (leftBtn != null) leftBtn.clicked -= OnLeft;
            if (rightBtn != null) rightBtn.clicked -= OnRight;
            if (chooseBtn != null) chooseBtn.clicked -= OnChoose;
            if (closeEmptyBtn != null) closeEmptyBtn.clicked -= OnClose;
            if (closeTextBtn != null) closeTextBtn.clicked -= OnClose;
            if (rating != null && onRatingChanged != null)
                rating.OnValueChanged -= onRatingChanged;

            leftBtn = rightBtn = chooseBtn = closeEmptyBtn = closeTextBtn = null;
            rating = null;
            textField = null;
            popupRoot = null;
            onPopupGeometryChanged = null;
            onRatingChanged = null;
            subtitleSwapTask?.Pause();
            subtitleCleanupTask?.Pause();
            subtitleSwapTask = null;
            subtitleCleanupTask = null;
        }

        private void UpdateScrollFieldBounds()
        {
            if (popupRoot == null || textField == null)
                return;

            float popupWidth = popupRoot.resolvedStyle.width;
            float popupHeight = popupRoot.resolvedStyle.height;
            if (float.IsNaN(popupWidth) || popupWidth <= 0f || float.IsNaN(popupHeight) || popupHeight <= 0f)
                return;

            float adaptiveWidth = Mathf.Clamp(popupWidth * 0.7f, 160f, 320f);
            float adaptiveHeight = Mathf.Clamp(popupHeight * 0.36f, 80f, 220f);

            textField.style.maxWidth = adaptiveWidth;
            textField.style.maxHeight = adaptiveHeight;
        }

        void OnLeft()
        {
            index--;
            if (index < 0) index = titles.Length - 1;
            UpdateText(-1);
        }

        void OnRight()
        {
            index = (index + 1) % titles.Length;
            UpdateText(1);
        }

        void OnChoose()
        {
            SetState(titles[index].resultState);
            Fading.Rotate(eyeIcon);
            ratingEl.AddToClassList("visible");
        }

        void OnClose()
        {
            SetState(CardState.Selection);
            UpdateText();
            Fading.DeRotate(eyeIcon);
            ratingEl.RemoveFromClassList("visible");
        }

        void UpdateText(int direction = 0)
        {
            if (direction == 0)
            {
                subtitle.text = titles[index].text;
                return;
            }

            AnimateSubtitle(direction);
        }

        void AnimateSubtitle(int direction)
        {
            if (subtitle == null || direction == 0)
                return;

            const int animationDurationMs = 100;

            subtitleSwapTask?.Pause();
            subtitleCleanupTask?.Pause();

            subtitle.RemoveFromClassList("subtitle-out-left");
            subtitle.RemoveFromClassList("subtitle-out-right");
            subtitle.RemoveFromClassList("subtitle-in-left");
            subtitle.RemoveFromClassList("subtitle-in-right");

            string outClass = direction < 0 ? "subtitle-out-left" : "subtitle-out-right";
            string inClass = direction < 0 ? "subtitle-in-left" : "subtitle-in-right";

            subtitle.AddToClassList(outClass);

            subtitleSwapTask = subtitle.schedule.Execute(() =>
            {
                subtitle.RemoveFromClassList(outClass);
                subtitle.text = titles[index].text;
                subtitle.AddToClassList(inClass);
                subtitleCleanupTask = subtitle.schedule.Execute(() =>
                {
                    subtitle.RemoveFromClassList(inClass);
                });
                subtitleCleanupTask.ExecuteLater(animationDurationMs);
            });
            subtitleSwapTask.ExecuteLater(animationDurationMs);
        }

        void SetState(CardState state)
        {
            currentState = state;
            Fading.FadeOut(navigationEl);
            Fading.FadeOut(closeEl);
            Fading.FadeOut(textEl);
            Fading.FadeOut(subtitle);

            switch (state)
            {
                case CardState.Selection:
                    Fading.FadeIn(navigationEl);
                    Fading.FadeIn(subtitle);
                    break;

                case CardState.Result:
                    subtitle.text = titles[index].resultText;
                    Fading.FadeIn(closeEl);
                    Fading.FadeIn(subtitle);
                    break;

                case CardState.FullText:
                    Fading.FadeIn(textEl);
                    break;
            }

            int savedRating = rm.LoadRating(currentState);
            rating.SetRating(savedRating, false);
        }
    }
}
