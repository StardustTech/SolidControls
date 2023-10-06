using System;
using System.Reflection;
using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace Stardust.OpenSource.SolidControls.Wpf
{
    public class PreviewCommandButton : Button
    {
        private bool _canExecute = true;

        public static readonly RoutedEvent PreviewCanExecuteEvent
            = EventManager.RegisterRoutedEvent(nameof(PreviewCanExecute), RoutingStrategy.Direct, typeof(PreviewCanExecuteRoutedEventHandler), typeof(PreviewCommandButton));

        public event PreviewCanExecuteRoutedEventHandler PreviewCanExecute {
            add { AddHandler(PreviewCanExecuteEvent, value); }
            remove { RemoveHandler(PreviewCanExecuteEvent, value); }
        }

        public PreviewCommandButton() { }

        protected override void OnClick() {
            if (Command is null) {
                base.OnClick();
                return;
            }

            var previewCanExecuteReArgs = new PreviewCanExecuteRoutedEventArgs(this);
            RaiseEvent(previewCanExecuteReArgs);
            _canExecute = previewCanExecuteReArgs.CanExecute;

            if (AutomationPeer.ListenerExists(AutomationEvents.InvokePatternOnInvoked)) {
                var peer = UIElementAutomationPeer.CreatePeerForElement(this);
                peer?.RaiseAutomationEvent(AutomationEvents.InvokePatternOnInvoked);
            }

            if (_canExecute) {
                var onClickMethodPtrFromButtonBase = typeof(ButtonBase).GetMethod(nameof(OnClick), BindingFlags.NonPublic | BindingFlags.Instance).MethodHandle.GetFunctionPointer();
                var onClickMethodActionFromButtonBase = Activator.CreateInstance(typeof(Action), this, onClickMethodPtrFromButtonBase) as Action;

                onClickMethodActionFromButtonBase.Invoke();
            }
            else {
                var clickEventArgs = new RoutedEventArgs(ClickEvent, this);
                RaiseEvent(clickEventArgs);
            }
        }
    }

    public delegate void PreviewCanExecuteRoutedEventHandler(object sender, PreviewCanExecuteRoutedEventArgs e);
    public class PreviewCanExecuteRoutedEventArgs : RoutedEventArgs
    {
        public PreviewCanExecuteRoutedEventArgs(object sender) {
            RoutedEvent = PreviewCommandButton.PreviewCanExecuteEvent;
            Source = sender;
        }

        public bool CanExecute { get; set; } = true;
    }
}
