/*
 An extend Button inherit from origin button class for multiple item list selection
 */

using System.Collections.Generic;
using System.Windows.Controls;

namespace DRnamespace
{
    internal class ListButton: Button
    {
        int id = -1;
        public readonly List<ListBoxItem> items;
        public ListButton(int col=0, int row=0, Grid parant=null, bool switch_by_click=true)
        {
            items = new List<ListBoxItem>();
            MouseWheel += SwitchButton_MouseWheel;
            if (switch_by_click)
                Click += SwitchButton_Click;

            if (parant !=null)
                parant.Children.Add(this);

            Grid.SetRow(this, row);
            Grid.SetColumn(this, col);
        }
        public void Enable()
        {
            IsEnabled = true;
        }

        public void Disable()
        {
            IsEnabled = false;
        }

        private void SwitchButton_MouseWheel(object sender, System.Windows.Input.MouseWheelEventArgs e)
        {
            if (e.Delta > 0)
                SelectedIndex(id + 1);
            else if (e.Delta < 0)
                SelectedIndex(id - 1);
        }

        private void SwitchButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            SelectedIndex(id + 1);
        }

        public ListBoxItem CurrentItem()
        {
            if (items.Count == 0)
                return null;
            return items[id];
        }

        public int SelectedIndex()
        {
            return id;
        }
        public bool SelectedIndex(int id)
        {
            if (items.Count==0) return false;

            if (id < 0)
                id = items.Count - 1;
            else if (id >= items.Count)
                id = 0;

            this.id=id;
            Content = items[id].Content;

            return true;
        }

        public void AddItem(ListBoxItem item) 
        {
            items.Add(item);
            if (items.Count == 1)
                SelectedIndex(0);
        }

        public void RemoveItem(ListBoxItem item)
        {
            items.Remove(item);

            //reinitialize
            SelectedIndex(id);
        }

        public void RemoveAll() 
        {
            items.Clear();
            id = -1;
        }
    }
}
