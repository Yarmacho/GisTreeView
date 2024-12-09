using AxMapWinGIS;
using MapWinGIS;
using System;
using System.IO;
using System.Linq;

namespace Tools
{
    public class BattimetryInterpolator
    {
        private const int cellSize = 10;
        internal static string ShapesPath;

        private static string BattimetriesPath => Path.Combine(ShapesPath, "Battimetries");

        public static string CreateBatimetryIDW(Image battimetry, Shape sceneShape, double kvadrWidth, int sceneId)
        {
            if (!Directory.Exists(BattimetriesPath))
            {
                Directory.CreateDirectory(BattimetriesPath);
            }

            var step = cellSize;

            var chunksDir = GetBatimetryValueIDWExtent(battimetry, sceneShape, kvadrWidth, step, out int dimX, out int dimY, out double xCorner, out double yCorner);
            return CombineChunks(chunksDir, sceneId, dimX, dimY, xCorner, yCorner);
        }

        private static string GetBatimetryValueIDWExtent(Image battimetry, Shape sceneShape,
            double kvadrWidth, double step, out int dimX, out int dimY, out double xCorner, out double yCorner)
        {
            IGdalRasterBand _band = battimetry.Band[1];
            double minX = sceneShape.Extents.xMin;
            double maxX = sceneShape.Extents.xMax;
            double _minX = minX;
            double minY = sceneShape.Extents.yMin;
            double maxY = sceneShape.Extents.yMax;
            xCorner = minX;
            yCorner = minY;

            // Calculate dimensions
            dimX = Convert.ToInt32((maxX - minX) / step) + 1;
            dimY = Convert.ToInt32((maxY - minY) / step) + 1;

            // Define chunk size (rows per chunk)
            const int CHUNK_SIZE = 100;
            int numChunks = (int)Math.Ceiling((double)dimY / CHUNK_SIZE);

            // Create temp directory for chunks
            string tempDir = Path.Combine(Path.GetTempPath(), "batimetry_chunks_" + Guid.NewGuid().ToString());
            Directory.CreateDirectory(tempDir);

            // Process data in chunks
            for (int chunkIndex = 0; chunkIndex < numChunks; chunkIndex++)
            {
                int startRow = chunkIndex * CHUNK_SIZE;
                int rowsInChunk = Math.Min(CHUNK_SIZE, dimY - startRow);

                // Process chunk
                double[,] chunkValues = ProcessBatimetryChunk(battimetry, _band, minX, maxX, minY, maxY,
                    step, dimX, startRow, rowsInChunk);

                // Save chunk to temporary file
                string chunkFile = Path.Combine(tempDir, $"chunk_{chunkIndex}.chunk");
                SaveChunkToFile(chunkValues, chunkFile);
            }

            // Combine all chunks into final result
            return tempDir;
        }

        private static double[,] ProcessBatimetryChunk(Image img, IGdalRasterBand _band,
            double minX, double maxX, double minY, double maxY, double step,
            int dimX, int startRow, int rowsInChunk)
        {
            double[,] chunkValues = new double[rowsInChunk, dimX];
            double xLeftBott = img.OriginalXllCenter;
            double yLeftBott = img.OriginalYllCenter;
            int _maxPixY = 0;
            int _maxPixX = 0;
            img.ProjectionToImage(xLeftBott, yLeftBott, out _maxPixX, out _maxPixY);
            double _dx = img.OriginalDX;
            double _dy = img.OriginalDY;
            pixInt[,] p = new pixInt[3, 3];

            double currentY = maxY - (startRow * step);

            for (int l = 0; l < rowsInChunk; l++)
            {
                double currentX = minX;
                for (int k = 0; k < dimX; k++)
                {
                    int _columnMin = 0;
                    int _rowMin = 0;
                    img.ProjectionToImage(currentX, currentY, out _columnMin, out _rowMin);

                    // Calculate values for 3x3 grid around current point
                    CalculateGridValues(img, _band, _columnMin, _rowMin, xLeftBott,
                        _maxPixY, _dx, p);

                    // Calculate interpolated value
                    double chis = 0.0;
                    double znam = 0.0;
                    for (int i = 0; i < 3; i++)
                    {
                        for (int j = 0; j < 3; j++)
                        {
                            p[i, j].w = Math.Pow(1 / (Math.Sqrt((p[i, j].X - currentX) * (p[i, j].X - currentX) +
                                (p[i, j].Y - currentY) * (p[i, j].Y - currentY))), 4);
                            chis += p[i, j].w * p[i, j].Z;
                            znam += p[i, j].w;
                        }
                    }
                    chunkValues[l, k] = chis / znam;
                    currentX += step;
                }
                currentY -= step;
            }
            return chunkValues;
        }

        private static void CalculateGridValues(Image img, IGdalRasterBand _band,
            int _columnMin, int _rowMin, double xLeftBott, int _maxPixY, double _dx, pixInt[,] p)
        {
            double _val;
            // Center point
            p[1, 1] = new pixInt();
            p[1, 1].X = xLeftBott + _dx * _columnMin;
            p[1, 1].Y = xLeftBott + _dx * (_maxPixY - _rowMin);
            _ = _band.Value[_columnMin, _rowMin, out _val];
            p[1, 1].Z = _val;

            // Calculate surrounding points (8 points around center)
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    if (i == 1 && j == 1) continue; // Skip center point

                    int columnMod = _columnMin + (j - 1);
                    int rowMod = _rowMin + (i - 1);
                    p[i, j] = new pixInt();
                    p[i, j].X = xLeftBott + _dx * columnMod;
                    p[i, j].Y = xLeftBott + _dx * (_maxPixY - rowMod);

                    if (_band.Value[columnMod, rowMod, out _val])
                    {
                        p[i, j].Z = _val;
                    }
                    else
                    {
                        p[i, j].Z = p[1, 1].Z; // Use center value if no data
                    }
                }
            }
        }

        private static void SaveChunkToFile(double[,] chunk, string filePath)
        {
            using (var writer = new BinaryWriter(File.Open(filePath, FileMode.Create)))
            {
                int rows = chunk.GetLength(0);
                int cols = chunk.GetLength(1);
                writer.Write(rows);
                writer.Write(cols);

                for (int i = 0; i < rows; i++)
                {
                    for (int j = 0; j < cols; j++)
                    {
                        writer.Write(chunk[i, j]);
                    }
                }
            }
        }

        private static string CombineChunks(string tempDir, int sceneId, int totalRows, int totalCols, double xCorner, double yCorner)
        {
            // Get all chunk files sorted by index
            var chunkFiles = Directory.GetFiles(tempDir, "chunk_*.chunk")
                .OrderBy(f => int.Parse(Path.GetFileNameWithoutExtension(f).Split('_')[1]))
                .ToList();

            var battimetryPath = Path.Combine(BattimetriesPath, $"Scene_{sceneId}.asc");
            using (var sw = new StreamWriter(battimetryPath))
            {
                sw.WriteLine("ncols       " + totalRows.ToString());
                sw.WriteLine("nrows       " + totalCols.ToString());
                sw.WriteLine("xllcorner   " + xCorner.ToString());
                sw.WriteLine("yllcorner   " + yCorner.ToString());
                sw.WriteLine("cellsize    " + cellSize.ToString());
                sw.WriteLine("NODATA_value -32768");

                foreach (string chunkFile in chunkFiles)
                {
                    using (var reader = new BinaryReader(File.OpenRead(chunkFile)))
                    {
                        int rows = reader.ReadInt32();
                        int cols = reader.ReadInt32();

                        for (int i = 0; i < rows; i++)
                        {
                            for (int j = 0; j < cols; j++)
                            {
                                sw.Write($"{reader.ReadDouble()} ");
                            }
                        }
                    }
                }
            }

            return battimetryPath;
        }

        private class pixInt
        {
            public double X { get; set; }
            public double Y { get; set; }
            public double Z { get; set; }
            public double w { get; set; }
        }
    }
}
