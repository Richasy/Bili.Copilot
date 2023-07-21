using System.Diagnostics;

namespace Bili.Copilot.Libs.Player.MediaPlayer;

public class Activity : NotifyPropertyChanged
{
    /* Player Activity Mode ( Idle / Active / FullActive )
     * 
     * Required Engine's Thread (UIRefresh)
     * 
     * TODO: Static?
     */

    public ActivityMode Mode
        {
        get => mode;
        set {

            if (value == mode)
                return;

            mode = value;

            if (value == ActivityMode.Idle)
            {
                swKeyboard.Reset();
                swMouse.Reset();
            }
            else if (value == ActivityMode.Active)
                swKeyboard.Restart();
            else
                swMouse.Restart();

            Utils.UI(() => SetMode());
            }
        }
    internal ActivityMode _Mode = ActivityMode.FullActive, mode = ActivityMode.FullActive;

    /// <summary>
    /// Should use Timeout to Enable/Disable it. Use this only for temporary disable.
    /// </summary>
    public bool IsEnabled           { get => _IsEnabled;
        set {

            if (value && _Timeout <= 0)
            {
                if (_IsEnabled)
                {
                    _IsEnabled = false;
                    RaiseUI(nameof(IsEnabled));
                }
                else
                    _IsEnabled = false;
                
            }
            else
            {
                if (_IsEnabled == value)
                    return;

                if (value)
                {
                    swKeyboard.Restart();
                    swMouse.Restart();
                }

                _IsEnabled = value;
                RaiseUI(nameof(IsEnabled));
            }
            }
        }
    bool _IsEnabled;

    public int  Timeout             { get => _Timeout; set { _Timeout = value; IsEnabled = value > 0; } }
    int _Timeout;

    Player player;
    Stopwatch swKeyboard = new();
    Stopwatch swMouse = new();

    public Activity(Player player) => this.player = player;

    /// <summary>
    /// Updates Mode UI value and shows/hides mouse cursor if required
    /// Must be called from a UI Thread
    /// </summary>
    internal void SetMode()
    {
        _Mode = mode;
        Raise(nameof(Mode));
        player.Log.Trace(mode.ToString());
    }

    /// <summary>
    /// Refreshes mode value based on current timestamps
    /// </summary>
    internal void RefreshMode()
    {
        if (!IsEnabled)
            mode = ActivityMode.FullActive;
        else mode = swMouse.IsRunning && swMouse.ElapsedMilliseconds < Timeout
            ? ActivityMode.FullActive
            : swKeyboard.IsRunning && swKeyboard.ElapsedMilliseconds < Timeout ? ActivityMode.Active : ActivityMode.Idle;
    }

    /// <summary>
    /// Sets Mode to Idle
    /// </summary>
    public void ForceIdle()
    {
        if (Timeout > 0)
            Mode = ActivityMode.Idle;
    }
    /// <summary>
    /// Sets Mode to Active
    /// </summary>
    public void ForceActive()       => Mode = ActivityMode.Active;
    /// <summary>
    /// Sets Mode to Full Active
    /// </summary>
    public void ForceFullActive()   => Mode = ActivityMode.FullActive;

    /// <summary>
    /// Updates Active Timestamp
    /// </summary>
    public void RefreshActive()     => swKeyboard.Restart();

    /// <summary>
    /// Updates Full Active Timestamp
    /// </summary>
    public void RefreshFullActive() => swMouse.Restart();
}

public enum ActivityMode
{
    Idle,
    Active,     // Keyboard only
    FullActive  // Mouse
}
