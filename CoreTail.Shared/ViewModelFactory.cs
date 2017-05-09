using System;
using System.Collections.Generic;
using System.Text;

namespace CoreTail.Shared
{
    public class ViewModelFactory
    {
        private readonly IDispatcher _dispatcher;
        private readonly IOpenFileDialogService _openFileDialogService;

        public ViewModelFactory(IDispatcher dispatcher, IOpenFileDialogService openFileDialogService)
        {
            _dispatcher = Guard.ArgumentNotNull(dispatcher, nameof(dispatcher));
            _openFileDialogService = Guard.ArgumentNotNull(openFileDialogService, nameof(openFileDialogService));
        }

        public object CreateMainWindowViewModel(string[] args)
        {
            return args.Length == 0
                ? new FileReaderViewModel(_openFileDialogService, null)
                : new FileReaderViewModel(_openFileDialogService, args[0]);
        }
    }
}
