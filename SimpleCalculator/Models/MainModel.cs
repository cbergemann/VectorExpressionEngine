﻿using SimpleCalculator.Business;
using SimpleCalculator.Helper;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;
using VectorExpressionEngine;

namespace SimpleCalculator.Models;

public class MainModel : INotifyPropertyChanged
{
    public event EventHandler<ResultModel> ResultAdded;

    public event EventHandler<ResultModel> HistoryRecalled;

    public event PropertyChangedEventHandler PropertyChanged;

    private int _historyCounter;

    private readonly IContext _ctx;

    public MainModel()
    {
        var lib = new MathLibrary();
        var baseContext = new ReflectionContext(lib);
        _ctx = new ScopedContext(baseContext);

        GoUpInHistory = new RelayCommand(GoUpInHistory_Executed);
        GoDownInHistory = new RelayCommand(GoDownInHistory_Executed);
        Evaluate = new RelayCommand(Evaluate_Executed, Evaluate_CanExecute);
    }

    public ICommand GoUpInHistory { get; }

    private void GoUpInHistory_Executed(object parameter)
    {
        if (Results.Count - _historyCounter <= 0)
        {
            return;
        }

        var result = Results[Results.Count - _historyCounter - 1];
        _historyCounter++;

        ExpressionText = result.Expression;
        HistoryRecalled?.Invoke(this, result);
    }

    public ICommand GoDownInHistory { get; }

    private void GoDownInHistory_Executed(object parameter)
    {
        if (Results.Count - _historyCounter + 1 >= Results.Count)
        {
            ExpressionText = string.Empty;
            _historyCounter = 0;
            return;
        }

        var result = Results[Results.Count - _historyCounter + 1];
        _historyCounter--;

        ExpressionText = result.Expression;
        HistoryRecalled?.Invoke(this, result);
    }

    public ICommand Evaluate { get; }

    private bool Evaluate_CanExecute(object parameter)
    {
        return !string.IsNullOrWhiteSpace(ExpressionText);
    }

    public void Evaluate_Executed(object parameter)
    {
        var exp = ExpressionText.Trim();
        ExpressionText = string.Empty;
        _historyCounter = 0;

        var resultEntry = ResultModel.FromExpression(_ctx, exp);

        Results.Add(resultEntry);
        ResultAdded?.Invoke(this, resultEntry);
    }

    public ObservableCollection<ResultModel> Results { get; } = new();

    private string _expressionText = string.Empty;

    public string ExpressionText
    {
        get => _expressionText;
        set
        {
            _expressionText = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ExpressionText)));
        }
    }
}