
# DefineArch

A .NET 8 command-line tool for analyzing and organizing DLL files by their architecture (x86/x64).

## Features

- Detects DLL architecture (x86/x64) using PE headers and Mono.Cecil
- Generates a CSV report (`report.csv`) on your Desktop
- Optionally organizes DLLs into architecture-specific folders
- Implements retry logic for file operations

## Usage
- `<path-to-folder>`: Path to the directory containing DLL files to analyze

## Output

- A `report.csv` file on your Desktop with the following format:
- If enabled in code, DLLs are moved into `x86` and `x64` subfolders.

## How It Works

1. The tool scans the specified folder for `.dll` files.
2. For each DLL, it determines the architecture using both PE headers and Mono.Cecil.
3. It writes the results to `report.csv`.
4. Optionally, it moves DLLs into `x86` or `x64` folders.

## Building

- Requires .NET 8 SDK
- Run `dotnet build` in the project directory

## Customization

- To enable moving DLLs into architecture folders, set the `moveToArchDir` parameter to `true` in `OrganizeDllFilesByArchitecture`.

## Project Structure

- `Program.cs`: Entry point, argument parsing
- `OrganizeByArchitecture.cs`: Core logic for analysis and organization

## Notes

- File operations use retry logic (8 attempts, 200ms delay) to handle locked files.
- Only DLL files in the specified folder are processed (no recursion).
