// Copyright (c) Bili Copilot. All rights reserved.

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

using BiliCopilot.UI.Models.Constants;

namespace BiliCopilot.UI.Controls.Settings;

public sealed partial class ScreenshotSettingControl : SettingsPageControlBase
{
    public ScreenshotSettingControl()
    {
        InitializeComponent();
    }

    protected override void OnControlLoaded()
    {
        BehaviorComboBox.SelectedIndex = (int)ViewModel.ScreenshotAction;
    }

    private void OnBehaviorChanged(object sender, SelectionChangedEventArgs e)
    {
        if (!IsLoaded || BehaviorComboBox.SelectedIndex < 0)
        {
            return;
        }

        ViewModel.ScreenshotAction = (ScreenshotAction)BehaviorComboBox.SelectedIndex;
    }
}
