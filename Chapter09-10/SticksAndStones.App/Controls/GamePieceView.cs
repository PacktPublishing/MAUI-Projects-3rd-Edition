namespace SticksAndStones.Controls;

public partial class GamePieceView : ContentView
{
    // Piece Position
    public static readonly BindableProperty GamePiecePositionProperty = BindableProperty.Create(nameof(GamePiecePosition), typeof(string), typeof(GamePieceView), string.Empty);
    public string GamePiecePosition
    {
        get => (string)GetValue(GamePiecePositionProperty);
        set => SetValue(GamePiecePositionProperty, value);
    }

    // Piece State
    public static readonly BindableProperty GamePieceStateProperty = BindableProperty.Create(nameof(GamePieceState), typeof(int), typeof(GamePieceView), 0, BindingMode.TwoWay);
    public int GamePieceState
    {
        get => (int)GetValue(GamePieceStateProperty);
        set => SetValue(GamePieceStateProperty, value);
    }

    // Piece Direction
    public static readonly BindableProperty GamePieceDirectionProperty = BindableProperty.Create(nameof(GamePieceDirection), typeof(string), typeof(GamePieceView), null);
    public string GamePieceDirection
    {
        get => (string)GetValue(GamePieceDirectionProperty);
        set => SetValue(GamePieceDirectionProperty, value);
    }

}
