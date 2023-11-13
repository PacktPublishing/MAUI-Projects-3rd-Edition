namespace DoToo.ViewModels;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DoToo.Models;
using DoToo.Repositories;
using DoToo.Views;
using System.Collections.ObjectModel;

public partial class MainViewModel : ViewModel
{
    private readonly ITodoItemRepository repository;
    private readonly IServiceProvider services;

    [ObservableProperty]
    ObservableCollection<TodoItemViewModel> items;

    [ObservableProperty]
    bool showAll;

    [RelayCommand]
    public async Task ToggleFilterAsync()
    {
        ShowAll = !ShowAll;
        await LoadDataAsync();
    }

    public MainViewModel(ITodoItemRepository repository, IServiceProvider services)
    {
        this.repository = repository;
        repository.OnItemAdded += (sender, item) => Items.Add(CreateTodoItemViewModel(item));
        repository.OnItemUpdated += (sender, item) => Task.Run(async () => await LoadDataAsync());

        this.services = services;
        Task.Run(async () => await LoadDataAsync());
    }

    private async Task LoadDataAsync()
    {
        var items = await repository.GetItemsAsync(); 
        if (!ShowAll)
        {
            items = items.Where(x => x.Completed == false).ToList();
        }

        var itemViewModels = items.Select(i => CreateTodoItemViewModel(i));
        Items = new ObservableCollection<TodoItemViewModel>(itemViewModels);
    }

    private TodoItemViewModel CreateTodoItemViewModel(TodoItem item)
    {
        var itemViewModel = new TodoItemViewModel(item);
        itemViewModel.ItemStatusChanged += ItemStatusChanged;
        return itemViewModel;
    }

    private void ItemStatusChanged(object sender, EventArgs e)
    {
        if (sender is TodoItemViewModel item)
        {
            if (!ShowAll && item.Item.Completed)
            {
                Items.Remove(item);
            }

            Task.Run(async () => await repository.UpdateItemAsync(item.Item));
        }
    }

    [RelayCommand]
    public async Task AddItemAsync() => await Navigation.PushAsync(services.GetRequiredService<ItemView>());

    [ObservableProperty]
    TodoItemViewModel selectedItem;

    partial void OnSelectedItemChanging(TodoItemViewModel value)
    {
        if (value == null)
        {
            return;
        }

        MainThread.BeginInvokeOnMainThread(async () =>
        {
            await NavigateToItemAsync(value);
        });
    }

    private async Task NavigateToItemAsync(TodoItemViewModel item)
    {
        var itemView = services.GetRequiredService<ItemView>();
        var vm = itemView.BindingContext as ItemViewModel;
        vm.Item = item.Item;
        itemView.Title = "Edit todo item";

        await Navigation.PushAsync(itemView);
    }
}
