using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Text;
using System.Text.Json;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using JsonClassNet;
using JsonToClass.Models;

namespace JsonToClass.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    private static readonly string _templateFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "JsonToClass", "templates");

    [ObservableProperty]
    private string? _json;
    [ObservableProperty]
    private string? _classValue;
    [ObservableProperty]
    private ObservableCollection<TemplateEditPanelModel>? _editPanels;
    [ObservableProperty]
    private TemplateEditPanelModel? _selectedEditPanel;
    [ObservableProperty]
    private bool _isPaneOpen;
    [ObservableProperty]
    private string? _templateContent;
    
    private TemplateEditPanelModel? _editInfo;

    [RelayCommand]
    private void Close(TemplateEditPanelModel? model)
    {
        if (model == null) return;
        EditPanels?.Remove(model);
    }

    [RelayCommand]
    private void Edit(TemplateEditPanelModel? model)
    {
        IsPaneOpen = true;
        _editInfo = model;
        TemplateContent = model == null ? "" : model.GenerateContext!.ToString();
    }

    [RelayCommand]
    private void Save()
    {
        if (string.IsNullOrEmpty(TemplateContent)) return;
        
        try
        {
            var context = new GenerateContext(TemplateContent);
            var name = context.GetDefineValue("name");
            if (string.IsNullOrEmpty(name))
            {
                TemplateContent += Environment.NewLine + "@name " +  DateTime.Now.ToString(("yyyyMMddHHmmss"));
                context = new GenerateContext(TemplateContent);
            }
            if (_editInfo == null)
            {
                _editInfo = new TemplateEditPanelModel()
                {
                    Name = name,
                    GenerateContext = context
                };
                EditPanels?.Add(_editInfo);
            }
            else
            {
                _editInfo.Name = name;
                _editInfo.GenerateContext = context;
                var index = EditPanels.IndexOf(_editInfo);
                if (index >= 0)
                {
                    EditPanels[index] = _editInfo;
                }
            }
            
            var filepath = Path.Combine(_templateFolder, $"{name}.txt");
            File.WriteAllBytes(filepath, Encoding.UTF8.GetBytes(TemplateContent));
        }
        catch (Exception e)
        {
            
        }
    }

    [RelayCommand]
    private void Delete()
    {
        if (SelectedEditPanel == null) return;
        var index = EditPanels.IndexOf(SelectedEditPanel);
        try
        {
            var filePath = Path.Combine(_templateFolder, $"{SelectedEditPanel.Name}.txt");
            File.Delete(filePath);
            EditPanels.RemoveAt(index);
            index = index < EditPanels.Count ? index : EditPanels.Count - 1;
            SelectedEditPanel = EditPanels[index];
        }
        catch (Exception e)
        {
        }
    }

    protected override void ViewLoaded()
    {
        if (!Directory.Exists(_templateFolder))
        {
            Directory.CreateDirectory(_templateFolder);
        }
        
        EditPanels ??= new ObservableCollection<TemplateEditPanelModel>();
        var templates = GetTemplates();
        foreach (var template in templates)
        {
            EditPanels.Add(new TemplateEditPanelModel()
            {
                Name = template.GetDefineValue("name"),
                GenerateContext = template
            });
        }

        if (EditPanels.Count > 0)
        {
            SelectedEditPanel = EditPanels[0];
        }
    }

    protected override void OnPropertyChanged(PropertyChangedEventArgs e)
    {
        base.OnPropertyChanged(e);

        if (e.PropertyName is nameof(Json) or nameof(SelectedEditPanel))
        {
            Parse();
        }
    }

    private ICollection<GenerateContext> GetTemplates()
    {
        var files = Directory.GetFiles(_templateFolder);
        var results = new List<GenerateContext>();
        foreach (var t in files)
        {
            try
            {
                var context = new GenerateContext(File.ReadAllBytes(t));
                results.Add(context);
            }
            catch{}
        }

        return results;
    }

    private void Parse()
    {
        if (SelectedEditPanel?.GenerateContext == null || string.IsNullOrEmpty(Json))
        {
            ClassValue = "";
        }
        try
        {
            var jsonReader = new Utf8JsonReader(Encoding.UTF8.GetBytes(Json));
            var classTemplate = new ClassTemplate(SelectedEditPanel.GenerateContext);
            var writers = classTemplate.Generate(ref jsonReader);
            var stringBuilder = new StringBuilder();
            foreach (StringWriter writer in writers)
            {
                stringBuilder.AppendLine(writer.GetStringBuilder().ToString());
            }
                
            ClassValue = stringBuilder.ToString();
        }
        catch (Exception exception)
        {
            ClassValue = "";
        }
    }
}