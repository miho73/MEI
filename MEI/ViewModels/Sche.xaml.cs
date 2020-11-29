using MEI.Controls.Sche;
using System;
using System.Collections.Generic;
using Windows.UI.Xaml.Controls;
using Windows.UI.Text;
using Windows.UI.Xaml;

namespace MEI.UI
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Sche : Page
    {
        private ScheManager sManager = new ScheManager();

        public Sche()
        {
            this.InitializeComponent();
            UpdateList();
        }

        public async void UpdateList()
        {
            await sManager.InitDB();
            DateTimeOffset Date = DateTimeOffset.Now;
            if (DateSel.SelectedDates.Count > 0) Date = DateSel.SelectedDates[0];
            else return;
            string format = Date.ToString("yyyy/MM/dd");
            List<ScheData> sches = await sManager.GetSche(format);
            DateDisplay.Text = Date.ToString("yyyy/MM/dd dddd");
            ScheList.Items.Clear();
            if(sches.Count == 0)
            {
                TextBlock nothing = new TextBlock()
                {
                    Text = UIManager.Rl.GetString("ScheduleNothing"),
                    FontSize = 20,
                    Height = 30,
                    FontWeight = FontWeights.Bold
                };
                ScheList.Items.Add(nothing);
            }
            else
            {
                sches.Sort((a, b) =>
                {
                    if (a.Complete && !b.Complete) return 1;
                    else if (!a.Complete && b.Complete) return -1;
                    else return 0;
                });
                foreach(ScheData data in sches)
                {
                    DaySchedule sch = new DaySchedule()
                    {
                        TodoC = data.Content,
                        IsComplete = data.Complete,
                        ID = data.ID,
                        Width = ScheList.Width,
                        Margin = new Thickness(0)
                    };
                    ScheList.Items.Add(sch);
                }
            }
            ListViewItem addNew = new ListViewItem()
            {
                Height = 30,
                Content = UIManager.Rl.GetString("AddNewSchedule"),
                FontSize = 15,
                Tag = "MkNew"
            };
            ScheList.Items.Add(addNew);
        }

        private void DateChanged(CalendarView sender, CalendarViewSelectedDatesChangedEventArgs args)
        {
            UpdateList();
        }

        private async void ScheList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Control ed = ScheList.SelectedItem as Control;
            if (ed == null) return;
            if(ed.Tag != null)
            {
                if (ed.Tag.ToString() == "MkNew")
                {
                    MakeSche makeSche = new MakeSche(DateSel.SelectedDates[0]);
                    await makeSche.ShowAsync();
                    UpdateList();
                }
            }
            else
            {
                DaySchedule sel = ScheList.SelectedItem as DaySchedule;
                await sManager.ChangeComplete(sel.ID);
                UpdateList();
            }
            ScheList.SelectedItem = null;
        }
    }
}
