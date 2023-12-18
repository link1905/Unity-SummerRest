#!/bin/bash

# Step 1: Build the .NET project
dotnet build

# Check if the build was successful before proceeding
if [ $? -eq 0 ]; then
    echo "Build successful $(pwd)"
    # Step 2: Move all output DLL files to <MyFolder>
    OutputDirectory="../../../Unity/Assets/Plugins/SourceGenerators"
    
    # Create the output directory if it doesn't exist
    mkdir -p "$OutputDirectory"

    # Move DLL files to the output directory
    find . -name '*.dll' -exec mv {} "$OutputDirectory" \;

    echo "DLL files moved to $OutputDirectory."
else
    echo "Build failed. Check the build output for errors."
fi