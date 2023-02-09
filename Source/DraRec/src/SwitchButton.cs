/*
 An extend Button inherit from origin button class for switch-like button
 */

using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace DRnamespace
{
    class SwitchButton: Button
    {
        bool activated;
        Brush activeColor, normalColor;

        public static readonly RoutedEvent ActivatedClickEvent = EventManager.RegisterRoutedEvent(
            name: "ActivatedClick",
            routingStrategy: RoutingStrategy.Bubble,
            handlerType: typeof(RoutedEventHandler),
            ownerType: typeof(SwitchButton));

        public static readonly RoutedEvent DeactivatedClickEvent = EventManager.RegisterRoutedEvent(
            name: "DeactivatedClick",
            routingStrategy: RoutingStrategy.Bubble,
            handlerType: typeof(RoutedEventHandler),
            ownerType: typeof(SwitchButton));

        public SwitchButton(Brush activate_color, Brush normal_color, int col = 0, int row = 0, Grid parant = null)
        {
            if (parant != null)
                parant.Children.Add(this);

            Grid.SetRow(this, row);
            Grid.SetColumn(this, col);

            activated = false;
            activeColor = activate_color;
            normalColor = normal_color;

            Click += SwitchButton_Click;

            Background = normal_color;
        }

        public void Enable()
        {
            IsEnabled = true;
        }

        public void Disable()
        {
            IsEnabled = false;
        }

        public event RoutedEventHandler ActivateClick
        {
            add { AddHandler(ActivatedClickEvent, value); }
            remove { RemoveHandler(ActivatedClickEvent, value); }
        }

        public event RoutedEventHandler DeactivateClick
        {
            add { AddHandler(DeactivatedClickEvent, value); }
            remove { RemoveHandler(DeactivatedClickEvent, value); }
        }

        void RaiseActivateClickEvent()
        {
            RoutedEventArgs routedEventArgs = new RoutedEventArgs(routedEvent: ActivatedClickEvent);
            RaiseEvent(routedEventArgs);
        }
        void RaiseDeactivateClickEvent()
        {
            RoutedEventArgs routedEventArgs = new RoutedEventArgs(routedEvent: DeactivatedClickEvent);
            RaiseEvent(routedEventArgs);
        }

        private void SwitchButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {

            if (!activated)
                Activate();
            else
                Deactivate();
        }

        public void Activate()
        {
            activated = true;
            Background = activeColor;

            RaiseActivateClickEvent();
        }
        public void Deactivate()
        {
            activated = false;
            Background = normalColor;

            RaiseDeactivateClickEvent();
        }

        public bool On()
        {
            return activated;
        }
    }
}
