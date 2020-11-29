using MEI.Controls.Sche;
using System;
using System.Diagnostics;
using System.Text;
using Windows.UI.Xaml.Controls;

namespace MEI.UI
{
    public sealed partial class MakeSche : ContentDialog
    {
        public MakeSche(DateTimeOffset now)
        {
            this.InitializeComponent();
            this.now = now;
        }

        private DateTimeOffset now;

        private new async void PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            string sT = xList.Text;
            string[] lns = sT.Split((char)0x0d);

            ScheManager sManager = new ScheManager();
            foreach(string s in lns)
            {
                if (s == "") continue;
                await sManager.Insert(new ScheData()
                {
                    Content = s,
                    Time = now,
                    Complete = false
                });
            }
        }
    }
}
