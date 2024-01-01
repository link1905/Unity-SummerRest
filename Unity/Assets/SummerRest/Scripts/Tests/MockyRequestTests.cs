using System.Collections;
using SummerRest.Runtime.Request;
using SummerRest.Scripts.Utilities.RequestComponents;
using UnityEngine.TestTools;

namespace SummerRest.Scripts.Tests
{
    public class MockyService
    {
        public class SimpleRequest : BaseRequest<SimpleRequest>
        {
            public SimpleRequest() : base("https://run.mocky.io/v3/1e9c6d93-72c2-4cf2-a94a-4e00888cb692")
            {
                Method = HttpMethod.Get;
                Init();
            }
        }
    }

    public class MockyRequestTests
    {
        [UnityTest]
        public IEnumerator Test_Simple_Request_Return_200_And_SuccessMessage()
        {
            var request = MockyService.SimpleRequest.Create();
            yield return request.DetailedResponseCoroutine<string>(d =>
            {
                
            });
        }
    }
}
