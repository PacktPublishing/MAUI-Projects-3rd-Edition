using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HotdogOrNot.Models;
using HotdogOrNot.ImageClassifier;

namespace HotdogOrNot.ViewModels;

public partial class MainViewModel : ObservableObject
{
    IClassifier classifier;
    Task initTask;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(TakePhotoCommand))]
    [NotifyCanExecuteChangedFor(nameof(PickPhotoCommand))]
    private bool isClassifying;

    public MainViewModel()
    {
        _ = InitAsync();
    }

    public Task InitAsync()
    {
        if (initTask == null || initTask.IsFaulted)
            initTask = InitTask();

        return initTask;
    }

    Task InitTask() => Task.Run(async () =>
    {        
        // Get model
        using var modelStream = await FileSystem.OpenAppPackageFileAsync("hotdog-or-not.onnx");
        using var modelMemoryStream = new MemoryStream();

        modelStream.CopyTo(modelMemoryStream);
        var model = modelMemoryStream.ToArray();

        classifier = new MLNetClassifier(model);
    });

    [RelayCommand(CanExecute = nameof(CanExecuteClassification))]
    public async void TakePhoto()
    {
        if (MediaPicker.Default.IsCaptureSupported)
        {
            var status = await AppPermissions.CheckAndRequestRequiredPermissionAsync<Permissions.Camera>();
            if (status == PermissionStatus.Granted) {
                status = await AppPermissions.CheckAndRequestRequiredPermissionAsync<Permissions.StorageWrite>();
            }
            if (status == PermissionStatus.Granted)
            {
                FileResult photo = await MediaPicker.Default.CapturePhotoAsync(new MediaPickerOptions() { Title = "Hotdog or Not?" });
                var imageToClassify = await ConvertPhotoToBytes(photo);
                var result = await RunClassificationAsync(imageToClassify);
                await MainThread.InvokeOnMainThreadAsync(async () => await
                    Shell.Current.GoToAsync("Result", new Dictionary<string, object>() { { "result", result } })
                );
            }
        }
    }

    [RelayCommand(CanExecute = nameof(CanExecuteClassification))]
    public async void PickPhoto()
    {
        var status = await AppPermissions.CheckAndRequestRequiredPermissionAsync<Permissions.Photos>();
        if (status == PermissionStatus.Granted)
        {
            FileResult photo = await MediaPicker.Default.PickPhotoAsync();
            var imageToClassify = await ConvertPhotoToBytes(photo);
            var result = await RunClassificationAsync(imageToClassify);
            await MainThread.InvokeOnMainThreadAsync(async () => await
                Shell.Current.GoToAsync("Result", new Dictionary<string, object>() { { "result", result } })
            );
        }
    }

    private bool CanExecuteClassification() => !IsClassifying;

    private async Task<byte[]> ConvertPhotoToBytes(FileResult photo)
    {
        if (photo == null) return Array.Empty<byte>();

        using var stream = await photo.OpenReadAsync();
        using MemoryStream memoryStream = new();
        stream.CopyTo(memoryStream);

        return memoryStream.ToArray();
    }

    async Task<Result> RunClassificationAsync(byte[] imageToClassify)
    {
        IsClassifying = true;

        try
        {
            await InitAsync().ConfigureAwait(false);
            var result = classifier.Classify(imageToClassify);
            return new Result()
            {
                IsHotdog = result.TopResultLabel == "hotdog",
                Confidence = result.TopResultScore,
                PhotoBytes = result.Image
            };
        }
        catch 
        {
            return new Result
            {
                IsHotdog = false,
                Confidence = 0.0f,
                PhotoBytes = imageToClassify
            };
        }
        finally
        {
            MainThread.BeginInvokeOnMainThread(() => IsClassifying = false);
        }
    }
}

