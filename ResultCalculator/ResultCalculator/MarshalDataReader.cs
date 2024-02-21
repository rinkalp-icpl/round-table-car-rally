﻿using Csv;
using Microsoft.Extensions.Logging;

internal sealed class MarshalDataReader(ILogger<MarshalDataReader> logger) : CsvReaderBase(logger)
{
    private const string configPath = "./data/marshal_data.csv";

    /// <summary>
    /// Read data file and validates the records
    /// CSV should have following columns
    ///     "Car Number" => car number
    ///     List of marshal points as per provided marshalPoints
    /// Records are validated for following
    ///     Car Number should be between 1 and 999
    ///     Time should be in HH:MM:SS format
    /// </summary>
    /// <param name="marshalPoints"></param>
    /// <param name="marshalRecords"></param>
    /// <returns></returns>
    public bool Read(List<MarshalPoint> marshalPoints, out List<MarshalDataRecord> marshalRecords)
    {
        marshalRecords = [];

        // Check if the file exists
        if (!File.Exists(configPath))
        {
            _logger.FileNotFound("MarshalData", configPath);
            return false;
        }

        var csv = File.ReadAllText(configPath);

        var lines = CsvReader.ReadFromText(csv, _csvOptions);

        if (!lines.Any())
        {
            _logger.FileIsEmpty(configPath);
            return false;
        }

        try
        {
            _logger.ValidatingCsvHeaders();

            // Validating headers.
            var headers = lines.First().Headers;

            if(headers.First() != "Car Number")
            {
                _logger.MissingCsvHeaderAtIndex(1, "Car Number", headers.First());
                return false;
            }

            for (int i = 1; i < marshalPoints.Count; i++)
            {
                if (headers[i] != marshalPoints[i - 1].PointName)
                {
                    _logger.MissingCsvHeaderAtIndex(i + 1, marshalPoints[i - 1].PointName, headers[i]);
                    return false;
                }
            }

            foreach (var item in lines)
            {
                _logger.ReadingDataLine(item.Raw);

                var carNumber = item["Car Number"];

                if (string.IsNullOrEmpty(carNumber))
                {
                    _logger.InvalidDataFormat("Car Number", "Car Number is required");
                    return false;
                }

                // validate values.
                if(!int.TryParse(carNumber, out int carNum) || carNum < 1 || carNum > 999)
                {
                    _logger.InvalidDataFormat("Car Number", "Car Number should be between 1 and 999");
                    return false;
                }

                var record = new MarshalDataRecord
                {
                    // Padding car number with 0
                    CarCode = $"ART40/24/{carNumber.PadLeft(3, '0')}"
                };

                foreach (var point in marshalPoints)
                {
                    var strTime = item[point.PointName];
                    
                    // marshal point time is optional
                    if (string.IsNullOrEmpty(strTime))
                    {
                        _logger.MissingTimeCaptured(carNumber, point.PointName);
                        record.MarshalScan.Add((point.PointName, []));
                        continue;
                    }

                    if (!TimeOnly.TryParse(strTime, out TimeOnly marshalTime))
                    {
                        _logger.InvalidDataFormat(point.PointName, "Time should be in HH:MM:SS format");
                        return false;
                    }

                    record.MarshalScan.Add((point.PointName, [marshalTime]));
                }

                marshalRecords.Add(record);
            }
        }
        catch (Exception ex)
        {
            _logger.UnhandledException(ex.Message);
            return false;
        }

        return true;
    }
}