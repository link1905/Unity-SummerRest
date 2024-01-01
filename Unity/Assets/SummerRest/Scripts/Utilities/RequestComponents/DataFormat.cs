using System;

namespace SummerRest.Scripts.Utilities.RequestComponents
{
    [Serializable]
    public enum DataFormat
    {
        Json = 0, PlainText = 1, Bson = 2, Xml = 3, 
    }
}