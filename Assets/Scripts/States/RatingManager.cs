using Enums;
using UnityEngine;

namespace States
{
    public class RatingManager
    {
        private string GetRatingKey(CardState state)
        {
            return $"rating_{state}";
        }
        public void SaveRating(CardState state, int value)
        {
            PlayerPrefs.SetInt(GetRatingKey(state), value);
            PlayerPrefs.Save();
        }

        public int LoadRating(CardState state)
        {
            return PlayerPrefs.GetInt(GetRatingKey(state), 0);
        }
    }



}