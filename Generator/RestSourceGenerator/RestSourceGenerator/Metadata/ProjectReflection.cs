namespace RestSourceGenerator.Metadata
{
    public class ProjectReflection
    {

        public static class SummerRest
        {
            public const string SummerRestRoot = nameof(SummerRest);
            public static class Runtime
            {
                public const string RuntimeRoot = SummerRestRoot + "." + nameof(Runtime);
                public static class RequestComponents
                {
                    public const string RequestComponentsRoot = RuntimeRoot + "." + nameof(RequestComponents);
                    public static class ContentType
                    {
                        public const string ContentTypeRoot = RequestComponentsRoot + "." + nameof(ContentType);
                        public const string Encodings = ContentTypeRoot + "." + nameof(Encodings);
                        public static class MediaTypeNames
                        {
                            public const string MediaTypeNamesRoot = ContentTypeRoot + "." + nameof(MediaTypeNames);
                            public const string Application = MediaTypeNamesRoot + "." + nameof(Application);
                            public const string Text = MediaTypeNamesRoot + "." + nameof(Text);
                            public const string Multipart = MediaTypeNamesRoot + "." + nameof(Multipart);
                        }
                    }
                }
                public static class Requests
                {
                    public const string RequestsRoot = RuntimeRoot + "." + nameof(Requests);
                    public const string BaseDataRequest = RequestsRoot + "." + nameof(BaseDataRequest);
                    public const string BaseMultipartRequest = RequestsRoot + "." + nameof(BaseMultipartRequest);
                }
                public static class Authenticate
                {
                    public const string AuthenticateRoot = RuntimeRoot + "." + nameof(Authenticate);
                    public const string AuthKeys = AuthenticateRoot + "." + nameof(AuthKeys);
                }
            }
        }
        public static class SummerRestConfiguration
        {
            public static class Domain
            {
            }
            public static class Service
            {
            }
            public static class Request
            {
                public const string Keys = nameof(Keys);
                public const string Headers = nameof(Headers);
                public const string Params = nameof(Params);
                public const string MultipartFormSections = nameof(MultipartFormSections);
                public const string Values = nameof(Values);
            }
        }

    }
}