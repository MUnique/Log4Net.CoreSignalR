// ReSharper disable InconsistentNaming naming is caused by C# classes which are serialized like that.
/*
 * This file contains TypeScript definitions which can be helpful at client-side development.
 */

export interface LogEventData {
    readonly Domain: string;
    readonly ExceptionString: string;
    readonly Identity: string;
    readonly Level: LogLevel;
    readonly LogLocation: LogLocation;
    readonly LoggerName: string;
    readonly Message: string;
    readonly Properties: { [key: string]: string };
    readonly ThreadName: string;
    readonly TimeStamp: string;
    readonly UserName: string;
}

export interface LogLevel {
    readonly Name: string;
    readonly DisplayName: string;
    readonly Value: number;
}

export interface LogLocation {
    readonly MethodName: string;
    readonly ClassName: string;
    readonly FileName: string;
    readonly LineNumber: number;
    readonly FullInfo: string;
}

export interface LogEventArgs {
    FormattedEvent: string;
    LoggingEvent: LogEventData;
    Id: number;
}