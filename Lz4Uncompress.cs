using K4os.Compression.LZ4;

public class Lz4Uncompress
{
  private int uncompressedSize;

  public Lz4Uncompress(uint uncompressedSize)
  {
    this.uncompressedSize = (int)uncompressedSize;
  }

  public byte[] Decode(byte[] data)
  {
    var uncompressed = new byte[this.uncompressedSize];
    var decodedSize = LZ4Codec.Decode(data, 0, data.Length, uncompressed, 0, this.uncompressedSize);
    if (decodedSize != this.uncompressedSize) throw new Exception($"expected {this.uncompressedSize} bytes, got {decodedSize}");

    return uncompressed;
  }
}
