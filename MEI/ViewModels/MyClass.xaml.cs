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
                    ControlVisibility = Visibility.Collapsed
                };
                ClassroomList.Items.Add(cl);
            }
        }

        private async void ClassroomList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ClassroomList.SelectedItem == null) return;
            if(isRemoving)
            {
                //MessageDialog dialog = new MessageDialog(UIManager.Rl.GetString("ClassRemoveConfirmDialogContent"),
                //    UIManager.Rl.GetString("ClassRemoveConfirmDialogTitle"));
                MessageDialog dialog = new MessageDialog(UIManager.Rl.GetString("ClassRemoveConfirmDialogContent"));

                dialog.Commands.Add(new UICommand(UIManager.Rl.GetString("DeleteClassConfirmMsg"), new UICommandInvokedHandler(async (IUICommand cmd) =>
                {
                    await classManager.RemoveData(((Classroom)ClassroomList.SelectedItem).ID);
                })));
                dialog.Commands.Add(new UICommand(UIManager.Rl.GetString("DeleteClassCancelMsg")));

                await dialog.ShowAsync();
                UpdateClassList();
                return;
            }
            string link = ((Classroom)ClassroomList.SelectedItem).Tag.ToString();
            ClassroomList.SelectedItem = null;
            await Launcher.LaunchUriAsync(new Uri(link));
        }

        private void EditingTrigger(object sender, RoutedEventArgs args)
        {
            Visibility visiChange = Visibility.Visible;
            if (editBtn.Tag.ToString() == "ctrlo")
            {
                visiChange = Visibility.Visible;
                editBtn.Tag = "ctrli";
                editBtn.Content = UIManager.Rl.GetString("EditClassConfirm");
            }
            else if (editBtn.Tag.ToString() == "ctrli")
            {
                visiChange = Visibility.Collapsed;
                editBtn.Tag = "ctrlo";
                editBtn.Content = UIManager.Rl.GetString("EditClassListAlone");
            }

            foreach (object obj in ClassroomList.Items)
            {
                Classroom classroom = obj as Classroom;
                classroom.ControlVisibility = visiChange;
            }
        }

        private async void AddNewClass(object sednder, RoutedEventArgs args)
        {
            MakeClass makeClass = new MakeClass(ClassroomList.Items.Count);
            await makeClass.ShowAsync();
            UpdateClassList();
        }

        private bool isRemoving = false;
        private void RemoveClass(object sedner, RoutedEventArgs args)
        {
            if (isRemoving)
            {
                removeThat.Content = UIManager.Rl.GetString("RemoveClassBtnAlone");
                isRemoving = false;
            }
            else
            {
                removeThat.Content = UIManager.Rl.GetString("RemoveClassConfirm");
                isRemoving = true;
            }
        }

        private async void SearchClass(object sender, TextChangedEventArgs e)
        {
            await classManager.InitDB();
            List<ClassObj> lList = await classManager.ReadData();
            lList.Sort(ClSortFunc);
            ClassroomList.Items.Clear();
            foreach (ClassObj element in lList)
            {
                //TODO: 한글 초중종 나눠서 비교하기
                if (!element.DisTxt.Contains(SearchQuery.Text)) continue;
                Classroom cl = new Classroom()
                {
                    ID = element.ID,
                    Tag = element.Link,
                    TBText = element.DisTxt,
                    Margin = new Thickness(0, 0, 0, 0),
                    ControlVisibility = Visibility.Collapsed
                };
                ClassroomList.Items.Add(cl);
            }
        }
    }
}
