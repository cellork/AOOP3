using System;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace AOOP3.ViewModels;

public abstract partial class ViewModelBase : ObservableObject
{
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsNotBusy))]
    private bool _isBusy;

    public bool IsNotBusy => !IsBusy;

    protected IRelayCommand CreateCommand(Action execute, Func<bool>? canExecute = null)
    {
        return canExecute == null 
            ? new RelayCommand(execute) 
            : new RelayCommand(execute, canExecute);
    }

    protected IAsyncRelayCommand CreateAsyncCommand(Func<Task> execute, Func<bool>? canExecute = null)
    {
        return canExecute == null 
            ? new AsyncRelayCommand(execute) 
            : new AsyncRelayCommand(execute, canExecute);
    }
}