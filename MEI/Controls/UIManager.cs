using Windows.ApplicationModel.Resources;
using Windows.UI.Xaml.Controls;

namespace MEI.UI
{
    public sealed partial class UIManager
    {
        public readonly static ResourceLoader Rl = ResourceLoader.GetForCurrentView();
    }
}
