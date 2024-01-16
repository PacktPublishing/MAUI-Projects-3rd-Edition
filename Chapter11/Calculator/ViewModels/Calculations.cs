using System.Collections.ObjectModel;

namespace Calculator.ViewModels;

public class Calculations : ObservableCollection<Calculation>
{
}

public class Calculation : Tuple<string, string>
{
    public Calculation(string expression, string result) : base(expression, result) { }

    public string Expression => this.Item1;
    public string Result => this.Item2;
}

