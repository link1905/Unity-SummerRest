using NUnit.Framework;
using SummerRest.Runtime.Authenticate.Appenders;
using SummerRest.Runtime.Authenticate.TokenRepositories;
using SummerRest.Runtime.RequestAdaptor;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.TestTools;

namespace SummerRest.Tests
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
            IAuthDataRepository.Current.Delete(AccountKey);
            var data = IAuthDataRepository.Current.Get<string>(AccountKey);
            Assert.That(string.IsNullOrEmpty(data));
        }

        [Test]
        public void Test_Save_To_Auth_Repository_Then_Get()
        {
            var expected = new Account
            {
                Username = "username",
            };
            IAuthDataRepository.Current.Delete(AccountKey);
            IAuthDataRepository.Current.Save(AccountKey, expected);
            var data = IAuthDataRepository.Current.Get<Account>(AccountKey);
            Assert.That(data.Equals(expected));
        }

        [Test]
        public void Test_Bearer_Token_Add_Header_To_Web_Request()
        {
            using var adaptor = RawUnityWebRequestAdaptor<string>.Create(UnityWebRequest.Get(string.Empty));
            const string token = "My token"; 
            IAuthDataRepository.Current.Save(AccountKey, token);
            IAuthAppender<BearerTokenAuthAppender>.GetSingleton().Append(AccountKey, adaptor);
            var result = adaptor.GetHeader("Authorization");
            Assert.AreEqual($"Bearer {token}", result);
        }

        [Test]
        public void Test_Log_Warning_When_Auth_Key_Is_Absent()
        {
            using var adaptor = RawUnityWebRequestAdaptor<string>.Create(UnityWebRequest.Get(string.Empty));
            IAuthDataRepository.Current.Delete(AccountKey);
            IAuthAppender<BearerTokenAuthAppender>.GetSingleton().Append(AccountKey, adaptor);
            LogAssert.Expect(LogType.Warning, $@"The auth key ""{AccountKey}"" does not exist in the program");
        }
    }
}