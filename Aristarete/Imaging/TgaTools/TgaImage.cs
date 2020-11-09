using System.IO;

namespace Aristarete.Imaging.TgaTools
{
    public class TgaImage
    {
        private readonly byte[] _data;

        private readonly int _width;
        private readonly int _height;
        private readonly TgaFormat _format;

        private int BytesPerRow => _width * (int) _format;

        private TgaImage(int width, int height, TgaFormat format)
        {
            _width = width;
            _height = height;
            _format = format;

            _data = new byte[height * BytesPerRow];
        }

        public static TgaImage FromBuffer(Buffer buffer)
        {
            var tga = new TgaImage(buffer.Width, buffer.Height, TgaFormat.Bgra);
            System.Buffer.BlockCopy(buffer.Pixels, 0, tga._data, 0, tga._data.Length);
            return tga;
        }

        public void WriteToFile(string path, bool compress = false)
        {
            var bpp = (int) _format;
            using var writer = new BinaryWriter(File.Create(path));
            var header = new TgaHeader
            {
                IdLength = 0, // The IDLength set to 0 indicates that there is no image identification field in the TGA file
                ColorMapType = 0, // a value of 0 indicates that no palette is included
                BitsPerPixel = (byte) (bpp * 8),
                Width = (short) _width,
                Height = (short) _height,
                DataTypeCode = GetCorrectDataType(bpp, compress),
                ImageDescriptor = (byte) (32 | (_format == TgaFormat.Bgra ? 8 : 0)) // top-left origin
            };

            WriteHeader(writer, header);
            if (!compress)
                writer.Write(_data);
            else
                UnloadRleData(writer);
        }


        private static void WriteHeader(BinaryWriter writer, TgaHeader header)
        {
            writer.Write(header.IdLength);
            writer.Write(header.ColorMapType);
            writer.Write((byte) header.DataTypeCode);
            writer.Write(header.ColorMapOrigin);
            writer.Write(header.ColorMapLength);
            writer.Write(header.ColorMapDepth);
            writer.Write(header.OriginX);
            writer.Write(header.OriginY);
            writer.Write(header.Width);
            writer.Write(header.Height);
            writer.Write(header.BitsPerPixel);
            writer.Write(header.ImageDescriptor);
        }

        private void UnloadRleData(BinaryWriter writer)
        {
            const int maxChunkLength = 128;
            var nPixels = _width * _height;
            var currentPixel = 0;
            var bpp = (int) _format;

            while (currentPixel < nPixels)
            {
                var chunkStart = currentPixel * bpp;
                var currentByte = currentPixel * bpp;
                var runLength = 1;
                var literal = true;
                while (currentPixel + runLength < nPixels && runLength < maxChunkLength &&
                       currentPixel + runLength < currentPixel + _width)
                {
                    var equationTrue = true;
                    for (var t = 0; equationTrue && t < bpp; t++)
                    {
                        equationTrue = _data[currentByte + t] == _data[currentByte + t + bpp];
                    }

                    currentByte += bpp;
                    if (1 == runLength)
                    {
                        literal = !equationTrue;
                    }

                    if (literal && equationTrue)
                    {
                        runLength--;
                        break;
                    }

                    if (!literal && !equationTrue)
                    {
                        break;
                    }

                    runLength++;
                }

                currentPixel += runLength;

                writer.Write((byte) (literal ? runLength - 1 : 128 + (runLength - 1)));
                writer.Write(_data, chunkStart, literal ? runLength * bpp : bpp);
            }
        }

        private static DataType GetCorrectDataType(int bpp, bool compress)
        {
            var format = (TgaFormat) bpp;
            if (format == TgaFormat.Grayscale)
            {
                return compress ? DataType.RleBlackAndWhiteImage : DataType.UncompressedBlackAndWhiteImage;
            }

            return compress ? DataType.RleTrueColorImage : DataType.UncompressedTrueColorImage;
        }
    }
}