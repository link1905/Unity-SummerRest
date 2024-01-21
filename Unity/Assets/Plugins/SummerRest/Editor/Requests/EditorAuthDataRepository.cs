using System;
using SummerRest.Editor.Models;
using SummerRest.Runtime.Authenticate.TokenRepositories;

namespace SummerRest.Editor.Requests
{
    /// <summary>
    /// Resolve auth values by user's inputs stored inside <see cref="AuthContainer"/>
    /// Only works if current default auth repository is <see cref="PlayerPrefsAuthDataRepository"/>, in other cases user may prefer to use another resolver
    /// </summary>
    public class EditorAuthDataRepository : IAuthDataRepository, IDisposable
    {
        private readonly IAuthDataRepository _previous;
        private readonly AuthContainer _authContainer;
        public EditorAuthDataRepository(AuthContainer authContainer)
        {
            _previous = IAuthDataRepository.Current;
            _authContainer = authContainer;
            if (_previous is PlayerPrefsAuthDataRepository)
                IAuthDataRepository.Current = this;
        }
        public EditorAuthDataRepository()
        {
        }

        public void Save<TData>(string key, TData data)
        {
        }
        public void Delete(string key)
        {
        }
        public TData Get<TData>(string key)
        {
            if (_previous is PlayerPrefsAuthDataRepository)
                return _authContainer.GetData<TData>();
            return _previous.Get<TData>(key);
        }

        public void Dispose()
        {
            IAuthDataRepository.Current = _previous;
        }
    }
}