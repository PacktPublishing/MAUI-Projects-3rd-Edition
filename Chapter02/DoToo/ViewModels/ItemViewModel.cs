namespace DoToo.ViewModels;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DoToo.Models;
using DoToo.Repositories;

public partial class ItemViewModel : ViewModel
{
    private readonly ITodoItemRepository repository;

    [ObservableProperty]
    TodoItem item;

    public ItemViewModel(ITodoItemRepository repository)
    {
        this.repository = repository;
        Item = new TodoItem() { Due = DateTime.Now.AddDays(1) };
    }

    [RelayCommand]
    public async Task SaveAsync()
    {
        await repository.AddOrUpdateAsync(Item);
        await Navigation.PopAsync();
    }

}
