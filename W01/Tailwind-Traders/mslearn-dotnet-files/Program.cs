using System.IO;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;


// Get the directory where the program is currently running.
var currentDirectory = Directory.GetCurrentDirectory();

// Build the full path to the stores folder.
var storesDirectory = Path.Combine(currentDirectory, "stores");

// Build the full path to the output folder.
var salesTotalDir = Path.Combine(currentDirectory, "salesTotalDir");

// Create the output folder if it does not already exist.
Directory.CreateDirectory(salesTotalDir);

// Find all JSON files under the stores directory.
var salesFiles = FindFiles(storesDirectory);

// Read the files and calculate the combined total.
var salesTotal = CalculateSalesTotal(salesFiles);

// Build the full path to totals.txt.
var totalsFile = Path.Combine(salesTotalDir, "totals.txt");

// Append the grand total to totals.txt.
File.AppendAllText(
    totalsFile,
    $"{salesTotal}{Environment.NewLine}");

// Build the full path to the new sales summary report.
var summaryFile = Path.Combine(salesTotalDir, "salesSummary.txt");

// Generate the detailed sales summary report.
GenerateSalesSummary(
    salesFiles,
    storesDirectory,
    summaryFile);


// Finds JSON files in the supplied directory and all subdirectories.
IEnumerable<string> FindFiles(string directoryPath)
{
    List<string> jsonFiles = new List<string>();

    var foundFiles = Directory.EnumerateFiles(
        directoryPath,
        "*",
        SearchOption.AllDirectories);

    foreach (var file in foundFiles)
    {
        var extension = Path.GetExtension(file);

        if (extension.Equals(".json", StringComparison.OrdinalIgnoreCase))
        {
            jsonFiles.Add(file);
        }
    }

    return jsonFiles;
}


// Reads each JSON file and adds its Total value to the running total.
double CalculateSalesTotal(IEnumerable<string> salesFiles)
{
    double salesTotal = 0;

    foreach (var file in salesFiles)
    {
        // Read the JSON text from the file.
        string salesJson = File.ReadAllText(file);

        // Convert the JSON into a SalesData object.
        SalesData? data =
            JsonConvert.DeserializeObject<SalesData?>(salesJson);

        // Add the Total value, or 0 if the JSON could not be deserialized.
        salesTotal += data?.Total ?? 0;
    }

    return salesTotal;
}


// Generates a report containing the grand total
// and the individual total from each sales file.
void GenerateSalesSummary(
    IEnumerable<string> salesFiles,
    string storesDirectory,
    string outputFile)
{
    // StringBuilder builds the report in memory.
    StringBuilder report = new StringBuilder();

    double grandTotal = 0;

    report.AppendLine("Sales Summary");
    report.AppendLine("----------------------------");

    // Store the detail lines temporarily so the grand total
    // can appear before the Details section.
    StringBuilder details = new StringBuilder();

    foreach (var file in salesFiles)
    {
        // Read the contents of the current JSON file.
        string salesJson = File.ReadAllText(file);

        // Deserialize the JSON into a SalesData object.
        SalesData? data =
            JsonConvert.DeserializeObject<SalesData?>(salesJson);

        // Use 0 if the file did not contain valid sales data.
        double fileTotal = data?.Total ?? 0;

        // Add this file's amount to the grand total.
        grandTotal += fileTotal;

        // Use a relative path so files with the same name
        // can still be distinguished by their store directory.
        string fileName =
            Path.GetRelativePath(storesDirectory, file);

        // Add this file and its formatted total to the Details section.
        details.AppendLine(
            $"  {fileName}: {fileTotal:C}");
    }

    // Add the grand total using currency formatting.
    report.AppendLine($" Total Sales: {grandTotal:C}");
    report.AppendLine();
    report.AppendLine(" Details:");

    // Add all of the individual file detail lines.
    report.Append(details);

    // Write the completed report to the output file.
    File.WriteAllText(outputFile, report.ToString());
}


// Defines the structure expected in each sales JSON file.
record SalesData(double Total);