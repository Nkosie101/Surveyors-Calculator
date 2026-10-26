using System;
using System.Linq;
using System.Collections.Generic;
using Surveyors_Calculator.View;

namespace Surveyors_Calculator.ViewModel;
//[QueryProperty(nameof(tempCoordinates), "coordinatesKey")]
public partial class MyFileListPopupViewModel : BaseViewModel
{
    public MyFileListPopup Instance { get; set; }

    public ObservableCollection<FileData> FileNames { get; } = new();

    //public ObservableCollection<Coordinate> Coordinates { get; set; } = new();

    public List<FileData> files = new List<FileData>();

    private readonly SqliteConnectionFactory _connectionFactory;

    [ObservableProperty]
    private string deleteFile;

    public MyFileListPopupViewModel(List<FileData> filed, SqliteConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;

        FileNames = new ObservableCollection<FileData>();

        files = new List<FileData>();
        files = filed;
        LoadFileCommand.Execute(null);

    }

    [RelayCommand]
    public void OkButton()
    {
        // Perform your logic here (e.g., saving the name)

        /*FileData fileData = new FileData
        {
            fileName = FileName,
            coordSystem = SelectedCoordSys,
            units = SelectedUnits
        };*/
        // Close the popup
        Instance?.Close();
    }


    [RelayCommand]
    async Task Delete(FileData file)
    {

        if (IsBusy)
            return;
        ISQLiteAsyncConnection database = _connectionFactory.CreateConnection();
        try
        {
            IsBusy = true;
            if (file is null)
                return;
            DeleteFile = file.fileName;
            var result = await AppShell.Current.DisplayAlert("Alert", "Are you sure you want to delete the selected file(s)?", "Yes", "No");
            if (result)
            {
                List<CoordinateDTO> coordinateDTOs = await database.Table<CoordinateDTO>().ToListAsync();

                var tempCoordinates = new List<Coordinate>();

                //await AppShell.Current.DisplayAlert("Error", $"{file1name}", "OK");
                //show();
                foreach (CoordinateDTO dto in coordinateDTOs)
                {

                    var coord = new Coordinate
                    {
                        //FirstName = "Nkosilamandla", // Setting properties after creation

                        //name = dto.name
                        id = dto.id,
                        name = dto.name,
                        y = dto.y,
                        x = dto.x,
                        z = dto.z,
                        coordSystem = dto.coordSystem,
                        units = dto.units,
                        fileName = dto.fileName

                    };
                    if (dto.fileName == DeleteFile)
                    {
                        await database.DeleteAsync(dto);
                    }
                }
                FileNames.Remove(new FileData { fileName = file.fileName, coordSystem = file.coordSystem, units = file.units });
            }
        }
        catch
        {

        }
        finally
        {

            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task LoadFile()
    {
        var tempFileNames = new List<FileData>();

        MainThread.BeginInvokeOnMainThread(() =>
        {
            FileNames.Clear();
            foreach (var co in files)
            {
                co.IsChecked = false;
                FileNames.Add(co);
            }


        });
    }
    [ObservableProperty]
    private string file1Name;
    [ObservableProperty]
    private string file2Name;
    FileData singleFile = new FileData();
    string Key1 = string.Empty;
    string Key2 = string.Empty;
    string Key3 = string.Empty;

    [RelayCommand]
    private async Task OpenFiles()
    {

        //AppShell.Current.DisplayAlert("Error", $"{singleFile.units}", "OK");
        try
        {
            if (string.IsNullOrEmpty(File1Name) && !string.IsNullOrEmpty(File2Name))
            {
                await AppShell.Current.GoToAsync($"{nameof(Input)}",
                new Dictionary<string, object>
                {
                    ["singleFile"] = singleFile
                });
                return;

            }
            else if (!string.IsNullOrEmpty(File1Name) && string.IsNullOrEmpty(File2Name))
            {

                await AppShell.Current.GoToAsync($"{nameof(Input)}",
                new Dictionary<string, object>
                {
                    ["singleFile"] = singleFile
                });
                return;
                //await AppShell.Current.GoToAsync($"{nameof(Input)}?key1={Key1}&key2={Key2}&key3={Key3}, ");
                //return;

            }
            else
            {

                await AppShell.Current.GoToAsync($"{nameof(CalculationsPage)}?file1Key={File1Name}&file2Key={File2Name}");
            }
        }
        catch (Exception ex)
        {
            await AppShell.Current.DisplayAlert("Error!", ex.Message, "OK");
        }
        finally
        {
            //await AppShell.Current.DisplayAlert("Error!", $"The names {File1Name} {File2Name}", "OK");
            File1Name = string.Empty;
            File2Name = string.Empty;
            Instance?.Close();
        }
    }

    int checkedCount = 0;
    [RelayCommand]
    private void ToggleFile(FileData checkedFile)
    {
        //AppShell.Current.DisplayAlert("Error!", $"count one {checkedCount} checked", "OK");
        checkedCount = FileNames.Count(f => f.IsChecked);
        if (checkedCount == 2 && !checkedFile.IsChecked)
        {
            //AppShell.Current.DisplayAlert("Location!", $"{checkedCount} 1", "OK");
        }
        else if (/*checkedCount == 2 && */checkedFile.IsChecked)
        {
            //AppShell.Current.DisplayAlert("Location!", $"{checkedCount} 2", "OK");
            // If two files are already checked and the user is trying to uncheck one, allow it
            checkedFile.IsChecked = false;
            if (checkedFile.fileName == File1Name)
            {
                File1Name = null;
            }
            else if (checkedFile.fileName == File2Name)
            {
                File2Name = null;
            }
            return;
        }
        else if (checkedCount < 2 && !checkedFile.IsChecked)
        {
            //AppShell.Current.DisplayAlert("Location!", $"{checkedCount} 3", "OK");
            checkedFile.IsChecked = true;
            //return;
        }
        //checkedFile.IsChecked = !checkedFile.IsChecked;

        //AppShell.Current.DisplayAlert("Error!", $"count {checkedCount} checked", "OK");
        if (checkedCount < 2)
        {
            if (File1Name is null)
            {
                //AppShell.Current.DisplayAlert("Location!", $"{checkedCount} 4", "OK");
                singleFile.fileName = checkedFile.fileName;
                singleFile.units = checkedFile.units;
                singleFile.coordSystem = checkedFile.coordSystem;
                Key1 = checkedFile.fileName;
                Key2 = checkedFile.units;
                Key3 = checkedFile.coordSystem;
                File1Name = checkedFile.fileName;
            }
            else if (File2Name is null)
            {
                //AppShell.Current.DisplayAlert("Location!", $"{checkedCount} 5", "OK");
                singleFile.fileName = checkedFile.fileName;
                singleFile.units = checkedFile.units;
                singleFile.coordSystem = checkedFile.coordSystem;
                Key1 = checkedFile.fileName;
                Key2 = checkedFile.units;
                Key3 = checkedFile.coordSystem;
                File2Name = checkedFile.fileName;
            }
            //AppShell.Current.DisplayAlert("Error!", $"Here {File1Name} {File2Name}", "OK");
        }
        //var match = To.FirstOrDefault(s => s.name == checkedCoordinate.name);


    }


}

//List<CoordinateDTO> coordinateDTOs = await database.Table<CoordinateDTO>().ToListAsync();

//await AppShell.Current.DisplayAlert("Error", $"{file1name}", "OK");
//show();
/*foreach (CoordinateDTO dto in coordinateDTOs)
{
    var coord = new Coordinate
    {
        //FirstName = "Nkosilamandla", // Setting properties after creation

        //name = dto.name
        id = dto.id,
        name = dto.name,
        y = dto.y,
        x = dto.x,
        z = dto.z,
        coordSystem = dto.coordSystem,
        units = dto.units,
        fileName = dto.fileName

    };
    tempCoordinates.Add(coord);
    //From.Add(coord);
    // if (file2name == "")
}*/

/*if (fileNames.Count == 0)
{
    fileNames.Clear();
    //await AppShell.Current.DisplayAlert("Error", $" {File1[1]}", "OK");
    var fileNamesQueried = tempCoordinates.DistinctBy(s => s.fileName);
    foreach (var fileNamesQuery in fileNamesQueried)
    {
        fileNames.Add(fileNamesQuery.fileName);
        //await AppShell.Current.DisplayAlert("Error", $" {fileNamesQuery}", "OK");
    }
}*/