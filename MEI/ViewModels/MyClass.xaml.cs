using System.Collections.Generic;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using MEI.Controls.MyClass;
using System.Threading.Tasks;
using System.Diagnostics;
using Windows.UI.Core;
using System;
using Windows.System;
using Windows.UI.Popups;
using HangulLib;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace MEI.UI
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MyClass : Page
    {
        public MyClass()
        {
            this.InitializeComponent();
            UpdateClassList();
        }

        private readonly ClassManager classManager = new ClassManager();

        private int ClSortFunc(ClassObj a, ClassObj b)
        {
            return a.Order > b.Order ? 1 : (a.Order == b.Order ? 0 : -1);
        }

        private async void UpdateClassList()
        {
            await classManager.InitDB();
            List<ClassObj> lList = await classManager.ReadData();
            lList.Sort(ClSortFunc);
            ClassroomList.Items.Clear();
            foreach (ClassObj element in lList)
            {
                Classroom cl = new Classroom()
                {
                    ID = element.ID,
                    Tag = element.Link,
                    TBText = element.DisTxt,
                    Margin = new Thickness(0, 0, 0, 0),
                    OrderDis = element.Order,
                    controlVisible = Visibility.Visible
                };
                cl.UpContent = new Action<object, RoutedEventArgs>(ClUpContent);
                cl.DownContent = new Action<object, RoutedEventArgs>(ClDownContent);
                cl.RemoveContent = new Action<object, RoutedEventArgs>(ClDeleteContent);
                ClassroomList.Items.Add(cl);
            }
        }

        private async void ClassroomList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ClassroomList.SelectedItem == null) return;
                
            string link = ((Classroom)ClassroomList.SelectedItem).Tag.ToString();
            ClassroomList.SelectedItem = null;
            await Launcher.LaunchUriAsync(new Uri(link));
        }

        private async void AddNewClass(object sednder, RoutedEventArgs args)
        {
            MakeClass makeClass = new MakeClass(await classManager.GetNumberOfClasses());
            await makeClass.ShowAsync();
            UpdateClassList();
        }

        private Hangul hangul = new Hangul();

        private async void SearchClass(object sender, TextChangedEventArgs e)
        {
            await classManager.InitDB();
            List<ClassObj> lList = await classManager.ReadData();
            lList.Sort(ClSortFunc);
            ClassroomList.Items.Clear();
            string Query = hangul.ReplaceHangulSeparated(SearchQuery.Text);
            Visibility visible = Query == "" ? Visibility.Visible : Visibility.Collapsed;
            foreach (ClassObj element in lList)
            {
                if (!hangul.ReplaceHangulSeparated(element.DisTxt).Contains(Query)) continue;
                Classroom cl = new Classroom()
                {
                    ID = element.ID,
                    Tag = element.Link,
                    TBText = element.DisTxt,
                    Margin = new Thickness(0, 0, 0, 0),
                    controlVisible = visible
                };
                cl.UpContent = new Action<object, RoutedEventArgs>(ClUpContent);
                cl.DownContent = new Action<object, RoutedEventArgs>(ClDownContent);
                cl.RemoveContent = new Action<object, RoutedEventArgs>(ClDeleteContent);
                ClassroomList.Items.Add(cl);
            }
        }

        private async void ClUpContent(object sender, RoutedEventArgs args)
        {
            long cOrder = long.Parse(((Button)sender).Tag.ToString());
            if (cOrder == 0) return;
            await classManager.UpContent(cOrder);
            UpdateClassList();
        }
        private async void ClDownContent(object sender, RoutedEventArgs args)
        {
            long cOrder = long.Parse(((Button)sender).Tag.ToString());
            if (cOrder == await classManager.GetNumberOfClasses()-1) return;
            await classManager.DownContent(cOrder);
            UpdateClassList();
        }
        private async void ClDeleteContent(object sender, RoutedEventArgs args)
        {
            MessageDialog dialog = new MessageDialog(UIManager.Rl.GetString("ClassRemoveConfirmDialogContent"));

            dialog.Commands.Add(new UICommand(UIManager.Rl.GetString("DeleteClassConfirmMsg"), new UICommandInvokedHandler(async (IUICommand cmd) =>
            {
                await classManager.RemoveData(int.Parse(((Button)sender).Tag.ToString()));
            })));
            dialog.Commands.Add(new UICommand(UIManager.Rl.GetString("DeleteClassCancelMsg")));

            await dialog.ShowAsync();
            UpdateClassList();
            return;
        }
    }
}
