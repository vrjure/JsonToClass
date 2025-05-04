using System.Collections;
using CommunityToolkit.Mvvm.ComponentModel;
using JsonClassNet;

namespace JsonToClass.Models;

public partial class TemplateEditPanelModel : ObservableObject
{
    [ObservableProperty]
    private string? _name;
    [ObservableProperty]
    private string? _value;
    [ObservableProperty]
    private GenerateContext? _generateContext;
}