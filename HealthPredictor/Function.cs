using System.Net.Http;
using Amazon.Lambda.Core;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Alexa.NET.Request;
using Alexa.NET.Response;
using Alexa.NET.Request.Type;
using HealthPredictor.Common.Enums;
using HealthPredictor.Common.Constants;

using HealthPredictor.Model;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.Json.JsonSerializer))]

namespace HealthPredictor
{
    public class Function
    {
        //This should be in a configuration file
        const string RMODEL_API = "http://34.224.67.204:8000/alexa";

        public UserResponse inputData;
        public async Task<SkillResponse> FunctionHandler(SkillRequest input, ILambdaContext context)
        {
            var requestType = input.GetRequestType();
            if (requestType == typeof(LaunchRequest))
            {
                return MakeSkillResponse(Constants.START_INTENT_RESPONSE,false);
            }

            if (requestType == typeof(IntentRequest))
            {
                Dictionary<string, object> sessionAttributes;
                if (input.Session.Attributes == null)
                {
                    sessionAttributes = new Dictionary<string, object>();
                }
                else
                {
                    sessionAttributes = input.Session.Attributes;
                }

                IntentRequest intentRequest = input.Request as IntentRequest;
                if (intentRequest.Intent.Name == Constants.START_INTENT)
                {
                    return MakeSkillResponse(Constants.START_INTENT_RESPONSE, false);
                }
                else if (intentRequest.Intent.Name == Constants.AGE_INTENT)
                {
                    return MakeSkillResponse(Constants.AGE_INTENT_RESPONSE, false, 
                        UpdateSessionDictionary(intentRequest, Constants.AGE_SLOT, sessionAttributes, context));
                }
                else if(intentRequest.Intent.Name == Constants.GENDER_INTENT)
                {
                    return MakeSkillResponse(Constants.GENDER_INTENT_RESPONSE,false, 
                        UpdateSessionDictionary(intentRequest, Constants.GENDER_SLOT, sessionAttributes, context));
                }
                else if (intentRequest.Intent.Name == Constants.SMOKER_INTENT)
                {
                    return MakeSkillResponse(Constants.SMOKER_INTENT_RESPONSE, false, 
                        UpdateSessionDictionary(intentRequest, Constants.SMOKER_SLOT, sessionAttributes, context));
                }
                else if (intentRequest.Intent.Name == Constants.GRADUATE_INTENT)
                {
                    if ((sessionAttributes.ContainsKey(Constants.AGE_SLOT) && sessionAttributes.ContainsKey(Constants.GENDER_SLOT)
                         && sessionAttributes.ContainsKey(Constants.SMOKER_SLOT)))
                    {
                        var inputData = MapInputData(UpdateSessionDictionary(intentRequest, Constants.GRADUATE_SLOT, sessionAttributes, context));
                        var healthIndex = await GetHealthIndex(context, inputData);

                        return MakeSkillResponse(string.Format(Constants.GRADUATE_INTENT_RESPONSE, (Math.Round(Convert.ToDecimal(healthIndex), 0) * 100).ToString()),
                            false, UpdateSessionDictionary(intentRequest, Constants.GRADUATE_SLOT, sessionAttributes, context));
                    }
                    return MakeSkillResponse(Constants.INCOMPLETE_DATA_RECEIVED_RESPONSE,
                        false, UpdateSessionDictionary(intentRequest, Constants.GRADUATE_SLOT, sessionAttributes, context));
                }
                else 
                {
                    return MakeSkillResponse(Constants.ALL_OTHER_ERROR_RESPONSE, true);
                }
            }
            else
            {
                return MakeSkillResponse(Constants.INTENT_NOT_FOUND_RESPONSE,true);
            }
        }
        private UserResponse MapInputData(Dictionary<string,object> sessionAttributes)
        {
            UserResponse inputData = new UserResponse();
            var age = (sessionAttributes[Constants.AGE_SLOT]).ToString();
            inputData.Age = Convert.ToInt32(age.Substring(0,1));
            inputData.Education = (int)((NoYes)Enum.Parse(typeof(NoYes), Convert.ToString(sessionAttributes[Constants.GRADUATE_SLOT])));
            inputData.Smoking = (int)((NoYes)Enum.Parse(typeof(NoYes), Convert.ToString(sessionAttributes[Constants.SMOKER_SLOT])));
            inputData.Sex = (int)((Gender)Enum.Parse(typeof(Gender), Convert.ToString(sessionAttributes[Constants.GENDER_SLOT])));
            inputData.BMI = 3;
            inputData.BaselineAMD = 3;
            inputData.ZincGroup = 3;
            inputData.AntioxidentGroup = 3;
            return inputData;
        }
        private Dictionary<string, object> UpdateSessionDictionary(IntentRequest intentRequest, string slotName, Dictionary<string, object> sessionAttributes, ILambdaContext context)
        {
            try
            {
                if (sessionAttributes!= null && sessionAttributes.ContainsKey(slotName))
                {
                    sessionAttributes[slotName] = intentRequest.Intent.Slots[slotName].Value;
                }
                else
                {
                    sessionAttributes.Add(intentRequest.Intent.Slots[slotName].Name, intentRequest.Intent.Slots[slotName].Value);
                }
            }
            catch (Exception ex)
            {
                context.Logger.LogLine(ex.Message);
                return null;
            }            
            
            return sessionAttributes;
        }
        private async Task<string> GetHealthIndex(ILambdaContext context, UserResponse inputData)
        {
            string healthIndex = "empty";

            string json = await Task.Run(() => JsonConvert.SerializeObject(inputData));
            using (var stringContent = new StringContent(json, System.Text.Encoding.UTF8, "application/json"))
           
            using (var client = new HttpClient())
            {
                try
                {
                    var response = await client.PostAsync(RMODEL_API, stringContent);
                    context.Logger.LogLine(healthIndex);
                    healthIndex = await response.Content.ReadAsStringAsync();
                    healthIndex = healthIndex.Substring(1, healthIndex.Length - 2);
                    context.Logger.LogLine(healthIndex);
                }
                catch (Exception ex)
                {
                    context.Logger.LogLine(ex.Message);
                }
                return healthIndex;
            }
        }
        private SkillResponse MakeSkillResponse(string outputSpeech, bool shouldEndSession)
        {
            return MakeSkillResponse(outputSpeech, shouldEndSession, null);
        }
        private SkillResponse MakeSkillResponse(string outputSpeech, bool shouldEndSession, Dictionary<string, object> sessionAttributes) 
        {
            string repromptText = Constants.REPROMPT_RESPONSE;

            var response = new ResponseBody
            {
                ShouldEndSession = shouldEndSession,
                OutputSpeech = new PlainTextOutputSpeech { Text = outputSpeech }
              
            };

            if (repromptText != null)
            {
                response.Reprompt = new Reprompt() { OutputSpeech = new PlainTextOutputSpeech() { Text = repromptText } };
            }

            
            var skillResponse = new SkillResponse
            {
                Response = response,
                Version = "1.0",
            };

            if (sessionAttributes != null)
            {
                skillResponse.SessionAttributes = sessionAttributes;
            }
            return skillResponse;
        }
    }
    
}