using Windows.UI;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace MEI.UI
{
    public sealed partial class DaySchedule : UserControl
    {
        public DaySchedule()
        {
            this.InitializeComponent();
        }

        public string TodoC { get; set; }
        public bool IsComplete { get; set; }
        public long ID { get; set; }
        public Brush AccentColor = new SolidColorBrush(UIManager.rgba);
    }
}
