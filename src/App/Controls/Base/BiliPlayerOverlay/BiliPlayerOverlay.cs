// Copyright (c) Bili Copilot. All rights reserved.

using System.Collections.Generic;
using Bili.Copilot.App.Controls.Danmaku;
using Bili.Copilot.Models.Constants.App;
using Bili.Copilot.ViewModels;
using Microsoft.UI.Input;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Shapes;

namespace Bili.Copilot.App.Controls.Base;

/// <summary>
/// 哔哩播放器覆盖层.
/// </summary>
public sealed partial class BiliPlayerOverlay : ReactiveControl<PlayerDetailViewModel>
{
    private readonly Dictionary<int, List<DanmakuModel>> _danmakuDictionary;

    private DanmakuView _danmakuView;
    private BiliTransportControls _transportControls;
    private Rectangle _interactionControl;
    private GestureRecognizer _gestureRecognizer;
    private Grid _tempMessageContainer;
    private TextBlock _tempMessageBlock;
    private TextBlock _subtitleBlock;
    private Button _refreshButton;
    private Button _openInBrowserButton;
    private SplitView _rootSplitView;

    private DispatcherTimer _danmakuTimer;
    private DispatcherTimer _unitTimer;

    private double _cursorStayTime;
    private double _tempMessageStayTime;
    private double _transportStayTime;
    private double _nextVideoStayTime;
    private double _progressTipStayTime;
    private double _autoCloseWindowStayTime;

    private double _manipulationDeltaX = 0d;
    private double _manipulationDeltaY = 0d;
    private double _manipulationProgress = 0d;
    private double _manipulationVolume = 0d;
    private double _manipulationUnitLength = 0d;
    private bool _manipulationBeforeIsPlay = false;
    private PlayerManipulationType _manipulationType = PlayerManipulationType.None;

    private bool _isTouch = false;
    private bool _isHolding = false;
    private bool _isForceHiddenTransportControls;

    /// <summary>
    /// Initializes a new instance of the <see cref="BiliPlayerOverlay"/> class.
    /// </summary>
    public BiliPlayerOverlay()
    {
        DefaultStyleKey = typeof(BiliPlayerOverlay);
        _danmakuDictionary = new Dictionary<int, List<DanmakuModel>>();
        InitializeDanmakuTimer();
        InitializeUnitTimer();
        SizeChanged += OnSizeChanged;
        Loaded += OnLoaded;
        Unloaded += OnUnloaded;
    }

    internal override void OnViewModelChanged(DependencyPropertyChangedEventArgs e)
    {
        if (e.OldValue is PlayerDetailViewModel oldViewModel)
        {
            oldViewModel.DanmakuViewModel.DanmakuListAdded -= OnDanmakuListAdded;
            oldViewModel.DanmakuViewModel.RequestClearDanmaku -= OnRequestClearDanmaku;
            oldViewModel.DanmakuViewModel.LiveDanmakuAdded -= OnLiveDanmakuAdded;
            oldViewModel.DanmakuViewModel.SendDanmakuSucceeded -= OnSendDanmakuSucceeded;
            oldViewModel.DanmakuViewModel.PropertyChanged -= OnDanmakuViewModelPropertyChanged;
            oldViewModel.PropertyChanged -= OnViewModelPropertyChanged;
            oldViewModel.RequestShowTempMessage -= OnRequestShowTempMessage;
        }

        if (e.NewValue is PlayerDetailViewModel newViewModel)
        {
            newViewModel.DanmakuViewModel.DanmakuListAdded += OnDanmakuListAdded;
            newViewModel.DanmakuViewModel.RequestClearDanmaku += OnRequestClearDanmaku;
            newViewModel.DanmakuViewModel.LiveDanmakuAdded += OnLiveDanmakuAdded;
            newViewModel.DanmakuViewModel.SendDanmakuSucceeded += OnSendDanmakuSucceeded;
            newViewModel.DanmakuViewModel.PropertyChanged += OnDanmakuViewModelPropertyChanged;
            newViewModel.PropertyChanged += OnViewModelPropertyChanged;
            newViewModel.RequestShowTempMessage += OnRequestShowTempMessage;
        }
    }

    /// <inheritdoc/>
    protected override void OnApplyTemplate()
    {
        _danmakuView = GetTemplateChild("DanmakuView") as DanmakuView;
        _interactionControl = GetTemplateChild("InteractionControl") as Rectangle;
        _transportControls = GetTemplateChild("TransportControls") as BiliTransportControls;
        _tempMessageContainer = GetTemplateChild("TempMessageContainer") as Grid;
        _tempMessageBlock = GetTemplateChild("TempMessageBlock") as TextBlock;
        _subtitleBlock = GetTemplateChild("SubtitleBlock") as TextBlock;
        _refreshButton = GetTemplateChild("RefreshButton") as Button;
        _openInBrowserButton = GetTemplateChild("OpenInBrowserButton") as Button;
        _rootSplitView = GetTemplateChild("RootSplitView") as SplitView;
        var sectionView = GetTemplateChild("SectionNavigationView") as NavigationView;
        var takeScreenshotItem = GetTemplateChild("TakeScreenshotItem") as MenuFlyoutItem;
        var startRecordingItem = GetTemplateChild("StartRecordingItem") as MenuFlyoutItem;

        _gestureRecognizer = new GestureRecognizer
        {
            GestureSettings = GestureSettings.HoldWithMouse | GestureSettings.Hold,
        };

        _interactionControl.ManipulationMode = ManipulationModes.TranslateX | ManipulationModes.TranslateY;
        _interactionControl.Tapped += OnInteractionControlTapped;
        _interactionControl.DoubleTapped += OnInteractionControlDoubleTapped;
        _interactionControl.ManipulationStarted += OnInteractionControlManipulationStarted;
        _interactionControl.ManipulationDelta += OnInteractionControlManipulationDelta;
        _interactionControl.ManipulationCompleted += OnInteractionControlManipulationCompleted;
        _interactionControl.PointerPressed += OnInteractionControlPointerPressed;
        _interactionControl.PointerMoved += OnInteractionControlPointerMoved;
        _interactionControl.PointerReleased += OnInteractionControlPointerReleased;
        _interactionControl.PointerCanceled += OnInteractionControlPointerCanceled;
        _gestureRecognizer.Holding += OnGestureRecognizerHolding;

        _refreshButton.Click += OnRefreshButtonClick;
        _openInBrowserButton.Click += OnOpenInBrowserButtonClick;
        _transportControls.DetailButtonClicked += OnDetailButtonClicked;
        sectionView.ItemInvoked += OnSectionViewItemInvoked;
        takeScreenshotItem.Click += OnTakeScreenshotItemClick;
        startRecordingItem.Click += OnStartRecordingItemClick;

        CheckDanmakuZoom();
        ResizeSubtitle();
    }
}
