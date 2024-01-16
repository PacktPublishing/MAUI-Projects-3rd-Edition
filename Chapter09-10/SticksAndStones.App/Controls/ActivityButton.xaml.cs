using System.Windows.Input;

namespace SticksAndStones.Controls;

public partial class ActivityButton : Frame
{
    public static readonly BindableProperty CommandProperty = BindableProperty.Create(
        propertyName: nameof(Command),
        returnType: typeof(ICommand),
        declaringType: typeof(ActivityButton),
        defaultBindingMode: BindingMode.TwoWay);

    public ICommand Command
    {
        get => (ICommand)GetValue(CommandProperty);
        set { SetValue(CommandProperty, value); }
    }

    public static readonly BindableProperty CommandParameterProperty = BindableProperty.Create(
        propertyName: nameof(CommandParameter),
        returnType: typeof(object),
        declaringType: typeof(ActivityButton),
        defaultBindingMode: BindingMode.TwoWay);

    public object CommandParameter
    {
        get => GetValue(CommandParameterProperty);
        set { SetValue(CommandParameterProperty, value); }
    }

    public static readonly BindableProperty TextProperty = BindableProperty.Create(
        propertyName: nameof(Text),
        returnType: typeof(string),
        declaringType: typeof(ActivityButton),
        defaultValue: "",
        defaultBindingMode: BindingMode.TwoWay);

    public string Text
    {
        get => (string)GetValue(TextProperty);
        set { SetValue(TextProperty, value); }
    }

    public static readonly BindableProperty IsRunningProperty = BindableProperty.Create(
        propertyName: nameof(IsRunning),
        returnType: typeof(bool),
        declaringType: typeof(ActivityButton),
        defaultValue: false);

    public bool IsRunning
    {
        get => (bool)GetValue(IsRunningProperty);
        set { SetValue(IsRunningProperty, value); }
    }

    public static readonly BindableProperty FontFamilyProperty = BindableProperty.Create(
        propertyName: nameof(FontFamily),
        returnType: typeof(string),
        declaringType: typeof(ActivityButton),
        defaultValue: "",
        defaultBindingMode: BindingMode.TwoWay);

    public string FontFamily
    {
        get => (string)GetValue(Label.FontFamilyProperty);
        set { SetValue(Label.FontFamilyProperty, value); }
    }

    public static readonly BindableProperty FontSizeProperty = BindableProperty.Create(
        nameof(FontSize),
        typeof(double),
        typeof(ActivityButton),
        Device.GetNamedSize(NamedSize.Small, typeof(Label)),
        BindingMode.TwoWay);

    public double FontSize
    {
        set { SetValue(FontSizeProperty, value); }
        get { return (double)GetValue(FontSizeProperty); }
    }

    public ActivityButton()
    {
        InitializeComponent();
    }
}