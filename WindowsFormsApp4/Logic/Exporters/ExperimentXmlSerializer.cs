using Entities.Dtos;
using MassTransit.Transports;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using System.Xml.XPath;
using WindowsFormsApp4.Properties;

namespace WindowsFormsApp4.Logic.Exporters
{
    internal class ExperimentXmlSerializer
    {
        public void Serialize(Stream stream, ExperimentDto experiment)
        {
            try
            {
                var document = new XmlDocument();
                var nav = document.CreateNavigator();
                using (var navWriter = nav.AppendChild())
                {
                    var serializer = new XmlSerializer(typeof(ExperimentDto));
                    serializer.Serialize(navWriter, experiment);
                }

                if (document.LastChild == null || document.LastChild.Name != "experiment")
                {
                    return;
                }

                var experimentNode = document.LastChild;

                var scenesNode = document.LastChild.ChildNodes.OfType<XmlNode>().FirstOrDefault(n => n.Name == "scenes");
                if (scenesNode == null)
                {
                    return;
                }

                foreach (var sceneNode in scenesNode.ChildNodes.OfType<XmlNode>())
                {
                    var idAttribute = sceneNode.Attributes.GetNamedItem("id");
                    if (idAttribute == null)
                    {
                        continue;
                    }

                    var id = idAttribute.Value;
                    var scene = experiment.Scenes.FirstOrDefault(s => s.Id == int.Parse(id));
                    if (scene == null || scene.Bathymetry == null)
                    {
                        continue;
                    }

                    var sceneNav = sceneNode.CreateNavigator();
                    using (var writer = sceneNav.AppendChild())
                    {
                        directStreamWrite(writer, scene.Bathymetry);
                    }
                }

                using (var writer = XmlWriter.Create(stream))
                {
                    document.WriteContentTo(writer);
                }
            }
            catch (Exception ex)
            {
            }
        }

        /// <summary>
        /// Безпосередньо транслює дані з ASC потоку в XML без створення проміжного об'єкта
        /// </summary>
        private void directStreamWrite(XmlWriter writer, Stream stream)
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

                // Початок XML документа
                writer.WriteStartElement("Bathymetry");

                // Запис основних властивостей як елементів XML
                writer.WriteElementString("points", (nRows * nCols).ToString(CultureInfo.InvariantCulture));
                writer.WriteElementString("step", cellSize.ToString(CultureInfo.InvariantCulture));

                // Запис елемента центру
                writer.WriteStartElement("center");
                writer.WriteElementString("x", centerX.ToString(CultureInfo.InvariantCulture));
                writer.WriteElementString("y", centerY.ToString(CultureInfo.InvariantCulture));
                writer.WriteEndElement(); // Кінець center

                // Запис метаданих
                writer.WriteStartElement("metadata");
                writer.WriteElementString("nCols", nCols.ToString(CultureInfo.InvariantCulture));
                writer.WriteElementString("nRows", nRows.ToString(CultureInfo.InvariantCulture));
                writer.WriteElementString("xllCorner", xllCorner.ToString(CultureInfo.InvariantCulture));
                writer.WriteElementString("yllCorner", yllCorner.ToString(CultureInfo.InvariantCulture));
                writer.WriteElementString("cellSize", cellSize.ToString(CultureInfo.InvariantCulture));

                if (noDataValue.HasValue)
                {
                    writer.WriteElementString("noDataValue", noDataValue.Value.ToString(CultureInfo.InvariantCulture));
                }
                else
                {
                    // В XML немає прямого аналога null, тому використовуємо порожній елемент або атрибут
                    writer.WriteStartElement("noDataValue");
                    writer.WriteAttributeString("isNull", "true");
                    writer.WriteEndElement();
                }

                writer.WriteEndElement(); // Кінець metadata

                // Початок запису елемента глибин
                writer.WriteStartElement("depths");

                int currentRow = 0;
                while (reader.PeekChar() != -1)
                {
                    writer.WriteStartElement("row");
                    writer.WriteAttributeString("index", currentRow.ToString(CultureInfo.InvariantCulture));

                    // Обробка значень у рядку
                    for (int col = 0; col < nCols && reader.PeekChar() != -1; col++)
                    {
                        double depthValue = readDouble(reader);
                        writer.WriteElementString("depth", depthValue.ToString(CultureInfo.InvariantCulture));

                        // Пропускаємо роздільник, якщо він є
                        if (reader.PeekChar() != -1)
                        {
                            reader.ReadChar();
                        }
                    }

                    writer.WriteEndElement(); // Кінець row
                    currentRow++;
                }

                writer.WriteEndElement(); // Кінець depths
                writer.WriteEndElement(); // Кінець Bathymetry
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
