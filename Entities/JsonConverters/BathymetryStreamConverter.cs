using System;
using System.Globalization;
using System.IO;
using System.Text;
using System.Text.Json.Serialization;
using System.Text.Json;

namespace WindowsFormsApp4.JsonConverters
{
    /// <summary>
    /// JsonConverter для прямої трансляції ASC-потоку в JSON без створення проміжних об'єктів
    /// </summary>
    public class BathymetryStreamConverter : JsonConverter<Stream>
    {
        /// <summary>
        /// Читає та десеріалізує JSON у Stream (не реалізовано, оскільки фокус на серіалізації)
        /// </summary>
        public override Stream Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            // Десеріалізація не підтримується в цьому контексті
            throw new NotImplementedException("Десеріалізація Stream з JSON не підтримується");
        }

        /// <summary>
        /// Серіалізує Stream з ASC даними у JSON формат без створення проміжних об'єктів
        /// </summary>
        public override void Write(Utf8JsonWriter writer, Stream value, JsonSerializerOptions options)
        {
            if (value == null || !value.CanRead)
            {
                writer.WriteNullValue();
                return;
            }

            try
            {
                value.Position = 0;
                DirectStreamWrite(writer, value);
            }
            catch (Exception ex)
            {
                throw new JsonException($"Помилка серіалізації ASC даних: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Безпосередньо транслює дані з ASC потоку в JSON без створення проміжного об'єкта
        /// </summary>
        private void DirectStreamWrite(Utf8JsonWriter writer, Stream stream)
        {
            // Повертаємо потік на початок для читання
            stream.Position = 0;

            // Буфер для зберігання заголовка ASC
            int nCols = 0;
            int nRows = 0;
            double xllCorner = 0;
            double yllCorner = 0;
            double cellSize = 0;
            double? noDataValue = null;

            using (var reader = new BinaryReader(stream, Encoding.UTF8))
            {
                while (reader.PeekChar() != -1)
                {
                    var startPosition = reader.BaseStream.Position;

                    var header = string.Empty;
                    var symbol = (char)reader.PeekChar();
                    if (char.IsWhiteSpace(symbol))
                    {
                        reader.ReadChar();
                        continue;
                    }


                    if (char.IsLetter(symbol))
                    {
                        var headerBuilder = new StringBuilder();
                        headerBuilder.Append(symbol);
                        reader.ReadChar();

                        while (!char.IsWhiteSpace(symbol = (char)reader.PeekChar()))
                        {
                            headerBuilder.Append(symbol);
                            reader.ReadChar();
                        }

                        header = headerBuilder.ToString();

                        while (char.IsWhiteSpace(symbol = (char)reader.PeekChar()))
                        {
                            reader.ReadChar();
                            continue;
                        }
                    }
                    else
                    {
                        break;
                    }

                    if (header.StartsWith("NCOLS", StringComparison.OrdinalIgnoreCase))
                    {
                        nCols = readInt(reader);
                    }
                    else if (header.StartsWith("NROWS", StringComparison.OrdinalIgnoreCase))
                    {
                        nRows = readInt(reader);
                    }
                    else if (header.StartsWith("XLLCORNER", StringComparison.OrdinalIgnoreCase))
                    {
                        xllCorner = readDouble(reader);
                    }
                    else if (header.StartsWith("YLLCORNER", StringComparison.OrdinalIgnoreCase))
                    {
                        yllCorner = readDouble(reader);
                    }
                    else if (header.StartsWith("CELLSIZE", StringComparison.OrdinalIgnoreCase))
                    {
                        cellSize = readDouble(reader);
                    }
                    else if (header.StartsWith("NODATA_VALUE", StringComparison.OrdinalIgnoreCase))
                    {
                        noDataValue = readDouble(reader);
                    }
                }

                // Обчислюємо центр
                double centerX = xllCorner + (nCols * cellSize) / 2;
                double centerY = yllCorner + (nRows * cellSize) / 2;

                // Початок запису JSON
                writer.WriteStartObject();

                // Запис основних властивостей
                writer.WriteNumber("points", nRows * nCols);
                writer.WriteNumber("step", cellSize);

                // Запис об'єкта центру
                writer.WritePropertyName("center");
                writer.WriteStartObject();
                writer.WriteNumber("x", centerX);
                writer.WriteNumber("y", centerY);
                writer.WriteEndObject();

                // Запис метаданих
                writer.WritePropertyName("metadata");
                writer.WriteStartObject();
                writer.WriteNumber("nCols", nCols);
                writer.WriteNumber("nRows", nRows);
                writer.WriteNumber("xllCorner", xllCorner);
                writer.WriteNumber("yllCorner", yllCorner);
                writer.WriteNumber("cellSize", cellSize);

                if (noDataValue.HasValue)
                {
                    writer.WriteNumber("noDataValue", noDataValue.Value);
                }
                else
                {
                    writer.WriteNull("noDataValue");
                }

                writer.WriteEndObject(); // Кінець метаданих

                // Початок запису масиву глибин
                writer.WritePropertyName("depths");
                writer.WriteStartArray();

                int currentRow = 0;
                while (reader.PeekChar() != -1)
                {
                    writer.WriteStartArray(); // Початок рядка глибин

                    // Обробка значень у рядку
                    for (int col = 0; col < nCols && reader.PeekChar() != -1; col++)
                    {
                        writer.WriteNumberValue(readDouble(reader));
                        if (reader.PeekChar() != -1)
                        {
                            reader.ReadChar();
                        }
                    }

                    writer.WriteEndArray(); // Кінець рядка глибин
                    currentRow++;
                }

                writer.WriteEndArray(); // Кінець масиву глибин
                writer.WriteEndObject(); // Кінець об'єкта батиметрії
            }
        }

        private static int readInt(BinaryReader reader)
        {
            var number = new StringBuilder();

            var symbol = ' ';
            var peekedChar = 0;
            while ((peekedChar = reader.PeekChar()) != -1)
            {
                symbol = (char)peekedChar;
                if (symbol == '-' || symbol == '+')
                {
                    if (number.Length > 0)
                    {
                        break;
                    }
                }
                else if (!char.IsDigit(symbol))
                {
                    break;
                }

                number.Append(symbol);
                reader.ReadChar();
            }

            return int.Parse(number.ToString());
        }

        private static double readDouble(BinaryReader reader)
        {
            var number = new StringBuilder();

            var symbol = ' ';
            var hasDelimiter = false;
            var peekedChar = 0;
            while ((peekedChar = reader.PeekChar()) != -1)
            {
                symbol = (char)peekedChar;
                if (symbol == '.' || symbol == ',')
                {
                    if (hasDelimiter)
                    {
                        break;
                    }

                    hasDelimiter = true;
                    number.Append(CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator);
                    reader.ReadChar();
                    continue;
                }

                if (symbol == '-' || symbol == '+')
                {
                    if (number.Length > 0)
                    {
                        break;
                    }
                }
                else if (!char.IsDigit(symbol))
                {
                    break;
                }

                number.Append(symbol);
                reader.ReadChar();
            }

            return double.Parse(number.ToString());
        }
    }
}
