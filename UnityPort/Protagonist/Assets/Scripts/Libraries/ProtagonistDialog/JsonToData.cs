﻿using System.Linq;
using Newtonsoft.Json.Linq;

namespace Assets.Scripts.Libraries.ProtagonistDialog
{
    /**
     * Simple helper class to turn JSON into Dictionaries and Lists instead of normal objects.
     */
    public static class JsonToData
    {
        public static object Deserialize(string json)
        {
            return ToObject(JToken.Parse(json));
        }

        private static object ToObject(JToken token)
        {
            switch (token.Type)
            {
                case JTokenType.Object:
                    return token.Children<JProperty>()
                                .ToDictionary(prop => prop.Name,
                                              prop => ToObject(prop.Value));

                case JTokenType.Array:
                    return token.Select(ToObject).ToList();

                default:
                    return ((JValue)token).Value;
            }
        }
    }
}
