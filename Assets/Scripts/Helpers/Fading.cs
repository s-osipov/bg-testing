
using UnityEngine.UIElements;

namespace Helpers
{
    public static class Fading
    {
        public static void FadeOut(VisualElement el)
        {
            el.RemoveFromClassList("fade");
            el.AddToClassList("hidden");
            el.style.display = DisplayStyle.None;
        }

        public static void FadeIn(VisualElement el)
        {
            el.style.display = DisplayStyle.Flex;
            el.AddToClassList("fade");
            el.RemoveFromClassList("hidden");
        }

        public static void Rotate(VisualElement el)
        {
            el.AddToClassList("rotate-180");
        }

        public static void DeRotate(VisualElement el)
        {
            el.RemoveFromClassList("rotate-180");
        }
    }
}

