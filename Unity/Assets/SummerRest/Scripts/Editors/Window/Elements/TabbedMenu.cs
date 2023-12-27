using UnityEngine;
using UnityEngine.UIElements;

namespace SummerRest.Editors.Window.Elements
{
    public class TabbedMenu : SelectableListElement<TabbedMenu, TabSwitchButton, VisualElement>
    {
        private VisualElement _tabLabel;
        private VisualElement _tabContent;
        private int? _currentSelectedTabIdx;
        public override void Init(Color selectColor)
        {
            base.Init(selectColor);
            _tabLabel = this.Q<VisualElement>();
            _tabContent = this.Q<VisualElement>("contents");
            for (int i = 0; i < _tabContent.childCount && i < _tabLabel.childCount; i++)
            {
                var tab = _tabLabel[i].Q<TabSwitchButton>();
                var content = _tabContent[i];
                AddChild(tab, content, false);
                tab.Index = i;
            }
        }
        public new class UxmlFactory : UxmlFactory<TabbedMenu, UxmlTraits>
        {
        }
        public TabbedMenu()
        {
            
        }
    }
}