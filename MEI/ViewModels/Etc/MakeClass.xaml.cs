using Windows.UI.Xaml.Controls;
using MEI.Controls.MyClass;
using System.Diagnostics;

namespace MEI.UI
{
    public sealed partial class MakeClass : ContentDialog
    {
        public MakeClass(long tailOrder)
        {
            this.InitializeComponent();
            Order = tailOrder;
        }

        private long Order { get; set; }
        private string ClassName { get; set; }
        private string _ClassLink;
        private string ClassLink
        {
            get
            {
                return _ClassLink;
            }
            set
            {
                string lv = value.ToLower();
                if (!(lv.StartsWith("http://") || lv.StartsWith("https://"))) {
                    _ClassLink = "http://" + value;
                }
                else
                {
                    _ClassLink = value;
                }
            }
        }

        private void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            ClassObj obj = new ClassObj()
            {
                Order = Order,
                DisTxt = ClassName,
                Link = ClassLink
            };
            new ClassManager().AddData(obj);
        }
    }
}
