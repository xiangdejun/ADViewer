using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;

namespace ADViewer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            this.txbxId.Focus();
        }

        #region Variables
        
        private User user = new User();

        #endregion
        
        #region Methods

        private void ShowBusy(bool isDisable)
        {
            if (isDisable)
            {
                this.Cursor = Cursors.Wait;
                this.spSearchPanel.IsEnabled = false;
            }
            else
            {
                this.Cursor = Cursors.Arrow;
                this.spSearchPanel.IsEnabled = true;

            }
        }

        private void AsyncAction(string strInput)
        {
            string idPattern = @"xp[0-9]{6,}|[0-9]{7,}";
            string emailPattern = @"\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*";
            string namePattern ="";

            var adHelper = new ADHelper();

            if( Regex.IsMatch(strInput.ToLower(), idPattern))
            {
                user = adHelper.GetUserModel(InputTypes.ID, strInput);
            }
            else if (Regex.IsMatch(strInput, emailPattern))
            {
                user = adHelper.GetUserModel(InputTypes.Email, strInput);
            }
            else if (Regex.IsMatch(strInput, namePattern))
            {
                user = adHelper.GetUserModel(InputTypes.Name, strInput);
            }
        }

        private void AddTab(string tabHeader, User user)
        {
            TabItem tab = GetTabItemByHeader(tabHeader, this.tabs);
            if (tab != null)
            {
                tab.Focus();
                return;
            }

            tab = new TabItem { Header = tabHeader, HeaderTemplate = (DataTemplate)FindResource("dtTabItemHeader") };

            var panel = new StackPanel();
            var scroll = new ScrollViewer();
            scroll.Content = panel;
            scroll.VerticalScrollBarVisibility = ScrollBarVisibility.Visible;
            scroll.HorizontalScrollBarVisibility = ScrollBarVisibility.Visible;
            scroll.PreviewMouseWheel += scroll_PreviewMouseWheel;

            var dg = new DataGrid { AutoGenerateColumns = false, IsReadOnly = true};
            dg.Columns.Add(new DataGridTextColumn { Header = "Name", Binding = new Binding { Path = new PropertyPath("Property"), Mode = BindingMode.OneWay } });
            dg.Columns.Add(new DataGridTextColumn { Header = "Value", Binding = new Binding { Path = new PropertyPath("Value"), Mode = BindingMode.OneWay } });
            dg.HorizontalGridLinesBrush = Brushes.LightGray;
            dg.VerticalGridLinesBrush = Brushes.LightGray;
            dg.ItemsSource = user.Properties;
            panel.Children.Add(dg);

            var label = new Label();
            label.Content = "Groups";
            label.FontWeight = FontWeights.Bold;
            panel.Children.Add(label);

            var lbx = new ListBox {  };
            foreach (var item in user.Groups)
            {
                lbx.Items.Add(item);
            }
            panel.Children.Add(lbx);

            tab.Content = scroll;
            this.tabs.Items.Add(tab);
            tab.Focus();
        }

        private TabItem GetTabItemByHeader(string tabHeader, TabControl tabControl)
        {
            var tab = this.tabs.Items.OfType<TabItem>().Where(t => t.Header == tabHeader).FirstOrDefault();
            return tab;
        }

        #endregion
        
        #region Events

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var id = this.txbxId.Text;

            TabItem tab = GetTabItemByHeader(id, this.tabs);
            if (tab != null)
            {
                tab.Focus();
                return;
            }

            ShowBusy(true);

            var action = new Action<string>(AsyncAction);
            action.BeginInvoke(id,
                r =>
                {
                    Dispatcher.Invoke(new Action(() =>
                    {
                        AddTab(id, user);
                        ShowBusy(false);

                    }));
                },
                null);
        }

        private void CloseTab_Click(object sender, RoutedEventArgs e)
        {
            var tabHeader = (sender as Button).CommandParameter.ToString();
            var tab = GetTabItemByHeader(tabHeader, this.tabs);
            if (tab != null)
            {
                this.tabs.Items.Remove(tab);
            }
        }

        private void scroll_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            ScrollViewer scv = (ScrollViewer)sender;
            scv.ScrollToVerticalOffset(scv.VerticalOffset - e.Delta);
            e.Handled = true;
        }

        #endregion

    }

}
