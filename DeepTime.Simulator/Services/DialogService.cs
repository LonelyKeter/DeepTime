namespace DeepTime.Simulator.Services;

using Microsoft.Win32;

using System;
using System.Text;


public sealed class DialogService
{
    public record struct FormatDescription(string Description, string[] formats);

    public string? OpenFile(FormatDescription[] formats)
    {
        var dialog = new OpenFileDialog
        {
            Filter = Filter(formats),
            CheckFileExists = true,
            AddExtension = true,
            DereferenceLinks = true,
            CheckPathExists = true,
        };

        if (dialog.ShowDialog() is true)
        {
            return dialog.FileName;
        }
        else
        {
            return null;
        }
    }

    public System.IO.Stream? OpenFileStream(FormatDescription[] formats)
    {
        var dialog = new OpenFileDialog
        {
            Filter = Filter(formats),
            CheckFileExists = true,
            AddExtension = true,
            DereferenceLinks = true,
            CheckPathExists = true,
        };

        if (dialog.ShowDialog() is true)
        {
            try
            {
                return dialog.OpenFile();
            }
            catch (InvalidOperationException)
            {
                return null;
            }
        }
        else
            return null;
    }

    public string? SaveFile(FormatDescription[] formats)
    {
        var dialog = new SaveFileDialog
        {
            Filter = Filter(formats),
            CheckFileExists = true,
            AddExtension = true,
            DereferenceLinks = true,
            CheckPathExists = true,
        };

        if (dialog.ShowDialog() is true)
        {
            return dialog.FileName;
        }
        else
        {
            return null;
        }
    }

    public System.IO.Stream? SaveFileStream(FormatDescription[] formats)
    {
        var dialog = new SaveFileDialog
        {
            Filter = Filter(formats),
            AddExtension = true,
            DereferenceLinks = true,
            CheckPathExists = true,
        };

        if (dialog.ShowDialog() is true)
        {
            try
            {
                return dialog.OpenFile();
            }
            catch (InvalidOperationException)
            {
                return null;
            }
        }
        else
            return null;
    }

    public static string Filter(FormatDescription[] formats)
    {
        var builder = new StringBuilder();

        var formatStrings = new string[formats.Length];

        for (int i = 0; i < formats.Length; i++)
        {
            var (description, format) = formats[i];

            var formatString = string.Join(';', format);

            formatStrings[i] = $"{description} ({formatString})|{formatString}";
        }

        return string.Join("|", formatStrings);
    }
}
