using Windows.ApplicationModel.Resources;
using Windows.UI;
using Windows.UI.ViewManagement;

namespace MEI.UI
{
    public sealed partial class UIManager
    {
        public readonly static ResourceLoader Rl = ResourceLoader.GetForCurrentView();
        public readonly static UISettings uiSettings = new UISettings();
        public readonly static Color rgba = uiSettings.GetColorValue(UIColorType.Accent);
    }
}
