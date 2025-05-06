#!/bin/bash

app_name="JsonToClass.app"

# Define the runtime identifiers
runtimes=("osx-arm64" "osx-x64" "win-x64")
pack_app_runtimes=("osx-arm64" "osx-x64" "osx-arm64-self" "osx-x64-self")
pack_zip_runtimes=("win-x64" "win-x64-self")
package_dir="packages"

# Publish the .NET project for each runtime
for runtime in "${runtimes[@]}"; do
  echo "remove ./publish/$runtime"
  rm -rf ./publish/$runtime
  dotnet publish ./JsonToClass/JsonToClass.csproj -c Release -f net8.0 -r "$runtime" -o "./publish/$runtime"
  dotnet publish ./JsonToClass/JsonToClass.csproj -c Release -f net8.0 -r "$runtime" --sc true -p:PublishTrimmed=true -o "./publish/${runtime}-self"
  echo "Project published successfully for $runtime to ./publish/$runtime directory."
done

echo "packageing..."

if [ ! -d "$package_dir" ]
then
  mkdir "$package_dir"
fi

cd "$package_dir"

for runtime in "${pack_app_runtimes[@]}"; do
  if [ -d "$runtime" ]
  then
    rm -rf "$runtime"
  fi

  mkdir "$runtime"
  mkdir "$runtime/$app_name"
  mkdir "$runtime/$app_name/Contents"
  mkdir "$runtime/$app_name/Contents/MacOS"
  mkdir "$runtime/$app_name/Contents/Resources"
  cp "../JsonToClass/Info.plist" "$runtime/$app_name/Contents/Info.plist"
  cp "../JsonToClass/Resources/JsonToClass.icns" "$runtime/$app_name/Contents/Resources/JsonToClass.icns"
  cp -a "../publish/$runtime/." "$runtime/$app_name/Contents/MacOS"

  if [ -f "${runtime}.zip" ]
  then
    rm -rf "${runtime}.zip"
  fi

  zip -r "${runtime}.zip" "$runtime"

  rm -rf "$runtime"

  echo "pack $runtime/$app_name success"
done

cd "../publish"

for runtime in "${pack_zip_runtimes[@]}"; do
  if [ -f "../$package_dir/${runtime}.zip" ]
  then
    rm -rf "../$package_dir/${runtime}.zip"
  fi

  zip -r "../$package_dir/${runtime}.zip" "$runtime"

  echo "zip ${runtime}.zip success"
done