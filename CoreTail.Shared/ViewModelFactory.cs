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
            return new FileReaderViewModel(
                _openFileDialogService, 
                args.Length == 0 ? null : args[0]);
        }
    }
}
