using UnityEngine;
using UnityEngine.UIElements;
using Helpers;
using Components;
using Enums;
using States;

public class UIInit : MonoBehaviour
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

    public UIDocument uiDocument;
    private VisualElement root;
    private VisualElement navigationEl;
    private VisualElement closeEl;
    private VisualElement textEl;
    private VisualElement ratingEl;
    private Image eyeIcon;
    private Label subtitle;

    private RedButton leftBtn;
    private RedButton rightBtn;
    private RedButton chooseBtn;
    private RedButton closeEmptyBtn;
    private RedButton closeTextBtn;
    private Rating rating;

    private int index = 0;

    private StringData strings;
    private Title[] titles;

    private RatingManager rm;

    private CardState currentState;

    void Awake()
    {
        strings = StringData.Load();

        rm = new();

        titles = new Title[]
        {
            new(strings.Get("dispatcher"), strings.Get("think"), CardState.Result),
            new(strings.Get("mean"), "", CardState.FullText)
        };
    }

    void OnEnable()
    {
        root = uiDocument.rootVisualElement;

        subtitle = root.Q<Label>("subtitle");
        navigationEl = root.Q<VisualElement>("navigationEl");
        closeEl = root.Q<VisualElement>("closeEl");
        textEl = root.Q<VisualElement>("textEl");
        ratingEl = root.Q<VisualElement>("ratingEl");
        rating = root.Q<Rating>("rating");

        leftBtn = root.Q<RedButton>("leftBtn");
        rightBtn = root.Q<RedButton>("rightBtn");
        chooseBtn = root.Q<RedButton>("chooseBtn");
        chooseBtn.text = strings.Get("choose");

        closeEmptyBtn = root.Q<RedButton>("closeEmptyBtn");
        closeTextBtn = root.Q<RedButton>("closeTextBtn");

        eyeIcon = root.Q<Image>("icon");

        leftBtn.clicked += OnLeft;
        rightBtn.clicked += OnRight;
        chooseBtn.clicked += OnChoose;

        closeEmptyBtn.clicked += OnClose;
        closeTextBtn.clicked += OnClose;

        rating.OnValueChanged += (value) =>
        {
            rm.SaveRating(currentState, value);
        };

        index = 0;
        UpdateText();

        SetState(CardState.Selection);
    }

    void OnLeft()
    {
        index--;
        if (index < 0) index = titles.Length - 1;
        UpdateText();
    }

    void OnRight()
    {
        index = (index + 1) % titles.Length;
        UpdateText();
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

    void UpdateText()
    {
        subtitle.text = titles[index].text;
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