using System;
using System.Linq;
using System.Collections.Generic;
using Surveyors_Calculator.View;

namespace Surveyors_Calculator.ViewModel;

public partial class MySavePopupViewModel : BaseViewModel
{

    public ObservableCollection<string> CoordinateSystems { get; } = new();
    public ObservableCollection<string> Units { get; } = new();



    public MySavePopupViewModel()
    {
        Units.Add("metres");
        Units.Add("Inches");
        Units.Add("Cape Feet");
        Units.Add("English Feet");
        Units.Add("Cape Roods");

        CoordinateSystems.Add("Local");
        CoordinateSystems.Add("Lo 27°");
        CoordinateSystems.Add("Lo 29°");
        CoordinateSystems.Add("Lo 31°");
        CoordinateSystems.Add("UTM Zone 35S");
        //CoordinateSystems.Add("UTM Zone 35N");
        //CoordinateSystems.Add("Geographic");
        CoordinateSystems.Add("BLO");
    }


    [ObservableProperty]
    private string selectedCoordSys;
    [ObservableProperty]
    private string selectedUnits;
    [ObservableProperty]
    private string fileName;
    [ObservableProperty]
    private string missingData;

    // Reference to the View
    public MySavePopup Instance { get; set; }

    [RelayCommand]
    public void OkButton()
    {

        if (string.IsNullOrEmpty(FileName) || string.IsNullOrEmpty(SelectedCoordSys) || string.IsNullOrEmpty(SelectedUnits))
        {
            MissingData = "Fill all the fields";
        }
        else
        {
            FileData fileData = new FileData
            {
                fileName = FileName,
                coordSystem = SelectedCoordSys,
                units = SelectedUnits
            };
            Instance?.Close(fileData);
        }

    }


}
