using System;
using System.Linq;
using Windows.UI.Xaml.Controls;
using MEI.Controls.MyClass;
using System.Collections.Generic;
using System.Threading.Tasks;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace MEI.UI
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            InitializeComponent();
            NavView.SelectedItem = NavView.MenuItems.ElementAt(1);
        }

        private void NavigationView_SelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
        {
            if (sender is null)
            {
                throw new ArgumentNullException(nameof(sender));
            }
            if (args is null)
            {
                throw new ArgumentNullException(nameof(args));
            }

            string item = (args.SelectedItem as NavigationViewItem).Tag.ToString();
            switch(item)
            {
                case "class":
                    NavView.Header = UIManager.Rl.GetString("NavHeaderMyClass");
                    ContentFrame.Navigate(typeof(MyClass));
                    break;
                case "todo":
                    NavView.Header = UIManager.Rl.GetString("NavHeaderTODO");
                    ContentFrame.Navigate(typeof(TODO));
                    break;
                case "schedule":
                    NavView.Header = UIManager.Rl.GetString("NavHeaderSchedule");
                    ContentFrame.Navigate(typeof(Sche));
                    break;
            }
        }
    }
}
