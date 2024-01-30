using System;
using System.Xml;
using SummerRest.Runtime.DataStructures;

namespace SummerRest.Editor.Utilities.Validators
{
    public class XmlValidator : XmlDocument, ISingleton<XmlValidator>
    {
        public bool Validate(string xml)
        {
            try
            {
                base.LoadXml(xml);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}