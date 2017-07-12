

namespace HealthPredictor.Common.Constants
{
    public static class Constants
    {
        public const string START_INTENT = "MyHealthScoreStartIntent";
        public const string AGE_INTENT = "MyHealthScoreAgeIntent";
        public const string GENDER_INTENT = "MyHealthScoreGenderIntent";
        public const string SMOKER_INTENT = "MyHealthScoreSmokerIntent";
        public const string GRADUATE_INTENT = "MyHealthScoreGraduateIntent";

        public const string AGE_SLOT = "Age";
        public const string GENDER_SLOT = "Gender";
        public const string SMOKER_SLOT = "Smoker";
        public const string GRADUATE_SLOT = "Graduate";

        public const string START_INTENT_RESPONSE = "Sure, I can help you with that. Please tell me your age. ";
        public const string AGE_INTENT_RESPONSE = "Ok, got it. Please tell me your gender.";
        public const string GENDER_INTENT_RESPONSE = "I got that. Do you smoke?";
        public const string SMOKER_INTENT_RESPONSE = "One last question. Did you graduate from college?";
        public const string GRADUATE_INTENT_RESPONSE = "Thank you. Based on your answers, on a scale of one to hundred, your health score is {0}";

        public const string INTENT_NOT_FOUND_RESPONSE = "I don't know how to handle this intent. Please say something like Alexa, start Health Scorer.";
        public const string INCOMPLETE_DATA_RECEIVED_RESPONSE = "I am sorry. I did not get all the data to make a prediction. Let's start again. Please tell me your age.";
        public const string ALL_OTHER_ERROR_RESPONSE = "I am having trouble in getting the results for you right now. Please try again.";
        public const string REPROMPT_RESPONSE = "If you want to try this skill again, please say tell me more about my health. To exit, say, exit.";
    }
}
