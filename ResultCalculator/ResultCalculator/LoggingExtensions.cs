﻿using Microsoft.Extensions.Logging;

internal static partial class LoggingExtensions
{
    // Starting: {AppName}
    [LoggerMessage(1001, LogLevel.Information, "Starting: {AppName}")]
    public static partial void StartingApp(this ILogger logger, string? appName);

    // Application is shutting down
    [LoggerMessage(1002, LogLevel.Information, "Application is shutting down")]
    public static partial void ShuttingDown(this ILogger logger);

    // {FileType} file not found at: {FilePath}
    [LoggerMessage(LogLevel.Error, "{FileType} file not found at: {FilePath}")]
    public static partial void FileNotFound(this ILogger logger, string fileType, string filePath);

    // File is empty
    [LoggerMessage(LogLevel.Error, "File is empty: {FilePath}")]
    public static partial void FileIsEmpty(this ILogger logger, string filePath);

    // File is expecting {Number} of lines. Found {Number} lines.
    [LoggerMessage(LogLevel.Error, "File is expecting {Expected} of lines. Found {Actual} lines.")]
    public static partial void FileLineCountMismatch(this ILogger logger, int expected, int actual);

    // Rally Configuration File read successfully
    [LoggerMessage(LogLevel.Information, "Rally Configuration File {ConfigurationName} read successfully")]
    public static partial void RallyConfigReadSuccessful(this ILogger logger, string configurationName);

    // Log invalid data format exception
    [LoggerMessage(LogLevel.Error, "Invalid data format: {MemberNames} : {Message}")]
    public static partial void InvalidDataFormat(this ILogger logger, string memberNames, string? message);

    // Log unhandled exception
    [LoggerMessage(LogLevel.Error, "Unhandled exception: {Message}")]
    public static partial void UnhandledException(this ILogger logger, string message);
}