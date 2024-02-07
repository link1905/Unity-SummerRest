using UnityEngine;
using UnityEngine.UI;

namespace SummerRestSample.Managers
{
    public abstract class BaseRequestManager : MonoBehaviour
    {
        [SerializeField] protected Button coroutineRequest;
        [SerializeField] protected Button taskRequest;

        private void Awake()
        {
            coroutineRequest.onClick.AddListener(OnCoroutineClicked);
#if SUMMER_REST_TASK
            taskRequest.onClick.AddListener(OnTaskClicked);
#else
            taskRequest.gameObject.SetActive(false);
#endif
        }

        protected abstract void OnCoroutineClicked();
        protected abstract void OnTaskClicked();
    }
}