using NUnit.Framework;
using SummerRest.Runtime.Authenticate.Appenders;
using SummerRest.Runtime.RequestAdaptor;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.TestTools;
using ISecretRepository = SummerRest.Runtime.Authenticate.Repositories.ISecretRepository;

namespace Tests.Runtime
{
    public class AuthAppenderTests
    {
        
        private class Account
        {
            public string Username { get; set; }
            public bool Equals(Account other)
            {
                return Username == other.Username;
            }
        }

        private const string AccountKey = nameof(AccountKey);
        [Test]
        public void Test_Delete_From_Auth_Repository_Then_Can_Not_Get()
        {
            ISecretRepository.Current.Delete(AccountKey);
            ISecretRepository.Current.TryGet<string>(AccountKey, out var data);
            Assert.That(string.IsNullOrEmpty(data));
        }

        [Test]
        public void Test_Save_To_Auth_Repository_Then_Get()
        {
            var expected = new Account
            {
                Username = "username",
            };
            ISecretRepository.Current.Delete(AccountKey);
            ISecretRepository.Current.Save(AccountKey, expected);
            ISecretRepository.Current.TryGet<Account>(AccountKey, out var data);
            Assert.That(data.Equals(expected));
        }

        [Test]
        public void Test_Bearer_Token_Add_Header_To_Web_Request()
        {
            using var adaptor = RawUnityWebRequestAdaptor<string>.Create(UnityWebRequest.Get(string.Empty));
            const string token = "My token"; 
            IAuthAppender<BearerTokenAuthAppender, string>.GetSingleton().Append(token, adaptor);
            var result = adaptor.GetHeader("Authorization");
            Assert.AreEqual($"Bearer {token}", result);
        }

        [Test]
        public void Test_Log_Warning_When_Auth_Key_Is_Absent()
        {
            using var adaptor = RawUnityWebRequestAdaptor<string>.Create(UnityWebRequest.Get(string.Empty));
            ISecretRepository.Current.Delete(AccountKey);
            ISecretRepository.Current.TryGet<string>(AccountKey, out var data);
            IAuthAppender<BearerTokenAuthAppender, string>.GetSingleton().Append(data, adaptor);
            LogAssert.Expect(LogType.Warning, $@"Bearer token is null or empty");
        }
    }
}