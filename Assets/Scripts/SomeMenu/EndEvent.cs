using System;

namespace DefaultNamespace
{
    public class EndEvent
    {
        private MenuButtons _menuButtons;
        private event Action EndAction;
        
        public EndEvent(MenuButtons menuButtons, Action endAction)
        {
            _menuButtons = menuButtons;
            EndAction = endAction;
        }

        public void BeginEvent(string menuText, string buttonText)
        {
            _menuButtons.RemoveButtons();
            
            _menuButtons.AddButton(buttonText,
                () => EndAction?.Invoke());
            
            _menuButtons.ShowMenu(menuText);
        }
    }
}