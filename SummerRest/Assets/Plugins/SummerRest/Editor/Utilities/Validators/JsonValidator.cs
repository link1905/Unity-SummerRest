using System;
using System.Xml;
using SummerRest.Runtime.DataStructures;
using UnityEngine;

namespace SummerRest.Editor.Utilities.Validators
{
    public class JsonValidator : XmlDocument, ISingleton<JsonValidator>
    {
        public bool Validate(string json)
        {
            try
            {
                JsonUtility.FromJson<object>(json);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}