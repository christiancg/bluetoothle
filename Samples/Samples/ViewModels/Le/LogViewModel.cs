﻿using System;
using System.Reactive.Linq;
using System.Windows.Input;
using Acr.Ble;
using Acr.Ble.Plugins;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Samples.Services;
using Xamarin.Forms;
using Command = Acr.Command;


namespace Samples.ViewModels.Le
{
    public class LogViewModel : AbstractViewModel
    {
        readonly IAdapter adapter;
        IDisposable logger;


        public LogViewModel(IAdapter adapter, IAppSettings settings)
        {
            this.adapter = adapter;
            this.Clear = new Command(() => Device.BeginInvokeOnMainThread(() => this.Log = String.Empty));

            this.IsBackgroundLoggingEnabled = settings.IsBackgroundLoggingEnabled;
            this.WhenAnyValue(x => x.IsBackgroundLoggingEnabled)
                .Skip(1)
                .Subscribe(enabled => settings.IsBackgroundLoggingEnabled = enabled);
        }


        public ICommand Clear { get; }
        [Reactive] public bool IsBackgroundLoggingEnabled { get; set; }
        [Reactive] public string Log { get; private set; }
        // TODO: Save/Share log with another app (ie. Dropbox)
        // TODO: email log
        // TODO: clear log

        public override void OnActivate()
        {
            base.OnActivate();
            this.logger = this.adapter
                .WhenActionOccurs()
                .Subscribe(this.Write);
        }


        public override void OnDeactivate()
        {
            base.OnDeactivate();
            this.logger.Dispose();
        }


        void Write(string msg)
        {
            Device.BeginInvokeOnMainThread(() =>
                this.Log += Environment.NewLine + msg
            );
        }
    }
}
