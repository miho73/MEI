using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace MEI.UI
{
    public sealed partial class Classroom : UserControl
    {
        public Classroom()
        {
            this.InitializeComponent();
        }

        public string TBText { get; set; }
        public long ID { get; set; }
        //THESE WILL NOT BE USED.
        private Visibility _ControlVisibility;
        private Visibility _RealVisibility;
        public Visibility ControlVisibility
        {
            get
            {
                return _ControlVisibility;
            }
            set
            {
                _ControlVisibility = value;
                _RealVisibility = value == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible;
            }
        }

        public void _UpContent(object sender, RoutedEventArgs args)
        {
            UpContent.Invoke(sender, args);
        }
        public void _DownContent(object sender, RoutedEventArgs args)
        {
            DownContent.Invoke(sender, args);
        }
        public void _RemoveContent(object sender, RoutedEventArgs args)
        {
            RemoveContent.Invoke(sender, args);
        }

        public Visibility Reverse(Visibility oper)
        {
            if (oper == Visibility.Visible) return Visibility.Collapsed;
            else return Visibility.Visible;
        }

        public Action<object, RoutedEventArgs> UpContent { get; set; }
        public Action<object, RoutedEventArgs> DownContent { get; set; }
        public Action<object, RoutedEventArgs> RemoveContent { get; set; }
    }
}
