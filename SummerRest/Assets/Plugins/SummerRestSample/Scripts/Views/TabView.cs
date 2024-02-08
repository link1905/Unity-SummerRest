using System;
using UnityEngine;
using UnityEngine.UI;

namespace SummerRestSample.Views
{
    public class TabView : MonoBehaviour
    {
        [Serializable]
        public class TabContainer
        {
            public Button button;
            public GameObject container;
        }
        [SerializeField] private TabContainer[] tabs;
        private void Awake()
        {
            for (var i = 0; i < tabs.Length; i++)
            {
                var index = i;
                tabs[i].button.onClick.AddListener(() => ShowTab(index));
            }
        }
        private void ShowTab(int index)
        {
            foreach (var tab in tabs)
                tab.container.SetActive(false);
            tabs[index].container.SetActive(true);
        }
    }
}