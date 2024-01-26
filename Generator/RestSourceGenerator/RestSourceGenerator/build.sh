#!/bin/bash

#!/bin/bash

# Run the dotnet test command

dotnet test "../RestSourceGenerator.Tests"

# Check the exit status of the dotnet test command
if [ $? -ne 0 ]; then
    echo "Error: Tests failed. Exiting the script."
    # Add any error-specific message or log here
    exit 1  # Exit the script with an error code
fi

# Step 1: Build the .NET project
dotnet build

# Check if the build was successful before proceeding
if [ $? -eq 0 ]; then
    echo "Build successful $(pwd)"
    # Step 2: Move all output DLL files to <MyFolder>
    OutputDirectory="../../../SummerRest/Assets/Plugins/SummerRest/Runtime"
    
    # Create the output directory if it doesn't exist
    mkdir -p "$OutputDirectory"

    # Move DLL files to the output directory
    find . -name '*.dll' -exec mv {} "$OutputDirectory" \;

    echo "DLL files moved to $OutputDirectory."
else
    echo "Build failed. Check the build output for errors."
fi