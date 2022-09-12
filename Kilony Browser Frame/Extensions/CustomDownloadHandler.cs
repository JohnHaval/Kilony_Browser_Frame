﻿using System;
using CefSharp;
using StatusTracking;

namespace Kilony_Browser_Frame.Extensions
{
    public class CustomDownloadHandler:IDownloadHandler
    {
        public event EventHandler<DownloadItem> OnBeforeDownloadFired;

        public event EventHandler<DownloadItem> OnDownloadUpdatedFired;

        public bool CanDownload(IWebBrowser chromiumWebBrowser, IBrowser browser, string url, string requestMethod)
        {
            return true;
        }

        public void OnBeforeDownload(IWebBrowser chromiumWebBrowser, IBrowser browser, DownloadItem downloadItem, IBeforeDownloadCallback callback)
        {
            OnBeforeDownloadFired?.Invoke(this, downloadItem);

            if (!callback.IsDisposed)
            {
                using (callback)
                {
                    DownloadTracking.IsDownload = true;
                    callback.Continue(DownloadTracking.FileName = downloadItem.SuggestedFileName, showDialog: true);//Присвоение имени для TextBlock
                }
            }
        }
        public void OnDownloadUpdated(IWebBrowser chromiumWebBrowser, IBrowser browser, DownloadItem downloadItem, IDownloadItemCallback callback)
        {
            DownloadTracking.CurrentSpeed = downloadItem.CurrentSpeed / 1024;
            DownloadTracking.CurrentSize = downloadItem.ReceivedBytes / 1024;
            DownloadTracking.FullSize = downloadItem.TotalBytes / 1024;
            DownloadTracking.DownloadStatus = DownloadTracking.CurrentSize / DownloadTracking.FullSize * 100;
            OnDownloadUpdatedFired?.Invoke(this, downloadItem);
        }
    }
}
