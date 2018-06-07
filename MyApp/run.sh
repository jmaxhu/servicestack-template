#!/usr/bin/env bash

ASPNETCORE_ENVIRONMENT=Development ASPNETCORE_URLS="http://*:5000" dotnet watch run

# Unix:
# ASPNETCORE_URLS="https://*:5123" dotnet run

# Windows PowerShell:
# $env:ASPNETCORE_URLS="https://*:5123" ; dotnet run

# Windows CMD (note: no quotes):
# SET ASPNETCORE_URLS=https://*:5123 && dotnet run

