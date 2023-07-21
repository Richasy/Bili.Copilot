using System;
using System.Threading;
using System.Threading.Tasks;

using Vortice.DXGI;
using Vortice.Mathematics;

using Bili.Copilot.Libs.Player.MediaFramework.MediaDecoder;
using Bili.Copilot.Libs.Player.MediaFramework.MediaFrame;

using static FlyleafLib.Logger;

namespace Bili.Copilot.Libs.Player.MediaFramework.MediaRenderer;

public unsafe partial class Renderer
{
    bool    isPresenting;
    long    lastPresentAt       = 0;
    long    lastPresentRequestAt= 0;
    object  lockPresentTask     = new();

    public bool Present(VideoFrame frame)
    {
        if (Monitor.TryEnter(lockDevice, 5))
        {
            try
            {
                PresentInternal(frame);
                VideoDecoder.DisposeFrame(LastFrame);
                LastFrame = frame;

                if (child != null)
                    child.LastFrame = frame;

                return true;

            }
            catch (Exception e)
            {
                if (CanWarn) Log.Warn($"Present frame failed {e.Message} | {Device?.DeviceRemovedReason}");
                VideoDecoder.DisposeFrame(frame);

                vpiv?.Dispose();

                return false;

            }
            finally
            {
                Monitor.Exit(lockDevice);
            }
        }

        if (CanDebug) Log.Debug("Dropped Frame - Lock timeout " + (frame != null ? Utils.TicksToTime(frame.timestamp) : ""));
        VideoDecoder.DisposeFrame(frame);

        return false;
    }
    public void Present()
    {
        if (SCDisposed)
            return;

        // NOTE: We don't have TimeBeginPeriod, FpsForIdle will not be accurate
        lock (lockPresentTask)
        {
            if ((Config.Player.player == null || !Config.Player.player.requiresBuffering) && VideoDecoder.IsRunning)
                return;

            if (isPresenting)
            {
                lastPresentRequestAt = DateTime.UtcNow.Ticks;
                return;
            }

            isPresenting = true;
        }

        Task.Run(() =>
        {
            long presentingAt;
            do
            {
                long sleepMs = DateTime.UtcNow.Ticks - lastPresentAt;
                sleepMs = sleepMs < (long)( 1.0/Config.Player.IdleFps * 1000 * 10000) ? (long) (1.0 / Config.Player.IdleFps * 1000) : 0;
                if (sleepMs > 2)
                    Thread.Sleep((int)sleepMs);

                presentingAt = DateTime.UtcNow.Ticks;
                RefreshLayout();
                lastPresentAt = DateTime.UtcNow.Ticks;

            } while (lastPresentRequestAt > presentingAt);

            isPresenting = false;
        });
    }
    internal void PresentInternal(VideoFrame frame)
    {
        if (SCDisposed)
            return;

        // TBR: Replica performance issue with D3D11 (more zoom more gpu overload)
        if (videoProcessor == VideoProcessors.D3D11)
        {
            if (frame.bufRef != null)
            {
                vpivd.Texture2D.ArraySlice = frame.subresource;
                vd1.CreateVideoProcessorInputView(VideoDecoder.textureFFmpeg, vpe, vpivd, out vpiv);
            }
            else
            {
                vpivd.Texture2D.ArraySlice = 0;
                vd1.CreateVideoProcessorInputView(frame.textures[0], vpe, vpivd, out vpiv);
            }

            vpsa[0].InputSurface = vpiv;
            vc.VideoProcessorBlt(vp, vpov, 0, 1, vpsa);
            swapChain.Present(Config.Video.VSync, PresentFlags.None);
            
            vpiv.Dispose();
        }
        else
        {
            context.OMSetRenderTargets(backBufferRtv);
            context.ClearRenderTargetView(backBufferRtv, Config.Video._BackgroundColor);
            context.RSSetViewport(GetViewport);
            context.PSSetShaderResources(0, frame.srvs);
            context.Draw(6, 0);
            swapChain.Present(Config.Video.VSync, PresentFlags.None);
        }

        if (child != null)
            child.PresentInternal(frame);
    }

    public void RefreshLayout()
    {
        if (Monitor.TryEnter(lockDevice, 5))
        {
            try
            {
                if (SCDisposed)
                    return;

                if (LastFrame != null && (LastFrame.textures != null || LastFrame.bufRef != null))
                    PresentInternal(LastFrame);
                else
                {
                    context.ClearRenderTargetView(backBufferRtv, Config.Video._BackgroundColor);
                    swapChain.Present(Config.Video.VSync, PresentFlags.None);
                }
            }
            catch (Exception e)
            {
                if (CanWarn) Log.Warn($"Present idle failed {e.Message} | {Device.DeviceRemovedReason}");
            }
            finally
            {
                Monitor.Exit(lockDevice);
            }
        }
    }
    public void ClearScreen()
    {
        VideoDecoder.DisposeFrame(LastFrame);
        Present();
    }
}
