// This is a generated file! Please edit source .ksy file and use kaitai-struct-compiler to rebuild

using System.Collections.Generic;

namespace Kaitai
{
    public partial class UnityBundle : KaitaiStruct
    {
        public static UnityBundle FromFile(string fileName)
        {
            return new UnityBundle(new KaitaiStream(fileName));
        }


        public enum CompressionType
        {
            None = 0,
            Lzma = 1,
            Lz4 = 2,
            Lz4hc = 3,
            Lzham = 4,
        }
        public UnityBundle(KaitaiStream p__io, KaitaiStruct p__parent = null, UnityBundle p__root = null) : base(p__io)
        {
            m_parent = p__parent;
            m_root = p__root ?? this;
            _read();
        }
        private void _read()
        {
            _signature = System.Text.Encoding.GetEncoding("utf-8").GetString(m_io.ReadBytesTerm(0, false, true, true));
            _version = m_io.ReadU4be();
            _unityVersion = System.Text.Encoding.GetEncoding("utf-8").GetString(m_io.ReadBytesTerm(0, false, true, true));
            _unityReversion = System.Text.Encoding.GetEncoding("utf-8").GetString(m_io.ReadBytesTerm(0, false, true, true));
            if (Signature == "UnityFS") {
                _header = new HeaderT(m_io, this, m_root);
            }
            if ( ((Header.IsValid) && (Version == 6)) ) {
                _blockInfoAndDirectory = new BlockInfoAndDirectoryT(m_io, this, m_root);
            }
            _blocks = new List<Block>();
            for (var i = 0; i < BlockInfoAndDirectory.Data.BlocksInfoCount; i++)
            {
                _blocks.Add(new Block(BlockInfoAndDirectory.Data.BlocksInfo[i].UncompressedSize, BlockInfoAndDirectory.Data.BlocksInfo[i].CompressedSize, m_io, this, m_root));
            }
            _rest = m_io.ReadBytesFull();
        }
        public partial class HeaderT : KaitaiStruct
        {
            public static HeaderT FromFile(string fileName)
            {
                return new HeaderT(new KaitaiStream(fileName));
            }

            public HeaderT(KaitaiStream p__io, UnityBundle p__parent = null, UnityBundle p__root = null) : base(p__io)
            {
                m_parent = p__parent;
                m_root = p__root;
                f_blockInfoNeedPaddingAtStart = false;
                f_compressionType = false;
                f_blocksAndDirectoryInfoCombined = false;
                f_blockInfoAtTheEnd = false;
                f_oldWebPluginCompatibility = false;
                f_isValid = false;
                _read();
            }
            private void _read()
            {
                _size = m_io.ReadS8be();
                _compressedBlocksInfoSize = m_io.ReadU4be();
                _uncompressedBlocksInfoSize = m_io.ReadU4be();
                _flags = m_io.ReadU4be();
            }
            private bool f_blockInfoNeedPaddingAtStart;
            private bool _blockInfoNeedPaddingAtStart;
            public bool BlockInfoNeedPaddingAtStart
            {
                get
                {
                    if (f_blockInfoNeedPaddingAtStart)
                        return _blockInfoNeedPaddingAtStart;
                    _blockInfoNeedPaddingAtStart = (bool) ((Flags & 512) != 0);
                    f_blockInfoNeedPaddingAtStart = true;
                    return _blockInfoNeedPaddingAtStart;
                }
            }
            private bool f_compressionType;
            private CompressionType _compressionType;
            public CompressionType CompressionType
            {
                get
                {
                    if (f_compressionType)
                        return _compressionType;
                    _compressionType = (CompressionType) (((UnityBundle.CompressionType) (Flags & 63)));
                    f_compressionType = true;
                    return _compressionType;
                }
            }
            private bool f_blocksAndDirectoryInfoCombined;
            private bool _blocksAndDirectoryInfoCombined;
            public bool BlocksAndDirectoryInfoCombined
            {
                get
                {
                    if (f_blocksAndDirectoryInfoCombined)
                        return _blocksAndDirectoryInfoCombined;
                    _blocksAndDirectoryInfoCombined = (bool) ((Flags & 64) != 0);
                    f_blocksAndDirectoryInfoCombined = true;
                    return _blocksAndDirectoryInfoCombined;
                }
            }
            private bool f_blockInfoAtTheEnd;
            private bool _blockInfoAtTheEnd;
            public bool BlockInfoAtTheEnd
            {
                get
                {
                    if (f_blockInfoAtTheEnd)
                        return _blockInfoAtTheEnd;
                    _blockInfoAtTheEnd = (bool) ((Flags & 128) != 0);
                    f_blockInfoAtTheEnd = true;
                    return _blockInfoAtTheEnd;
                }
            }
            private bool f_oldWebPluginCompatibility;
            private bool _oldWebPluginCompatibility;
            public bool OldWebPluginCompatibility
            {
                get
                {
                    if (f_oldWebPluginCompatibility)
                        return _oldWebPluginCompatibility;
                    _oldWebPluginCompatibility = (bool) ((Flags & 256) != 0);
                    f_oldWebPluginCompatibility = true;
                    return _oldWebPluginCompatibility;
                }
            }
            private bool f_isValid;
            private bool _isValid;
            public bool IsValid
            {
                get
                {
                    if (f_isValid)
                        return _isValid;
                    _isValid = (bool) (Flags == 67);
                    f_isValid = true;
                    return _isValid;
                }
            }
            private long _size;
            private uint _compressedBlocksInfoSize;
            private uint _uncompressedBlocksInfoSize;
            private uint _flags;
            private UnityBundle m_root;
            private UnityBundle m_parent;
            public long Size { get { return _size; } }
            public uint CompressedBlocksInfoSize { get { return _compressedBlocksInfoSize; } }
            public uint UncompressedBlocksInfoSize { get { return _uncompressedBlocksInfoSize; } }
            public uint Flags { get { return _flags; } }
            public UnityBundle M_Root { get { return m_root; } }
            public UnityBundle M_Parent { get { return m_parent; } }
        }
        public partial class BlockInfoAndDirectoryT : KaitaiStruct
        {
            public static BlockInfoAndDirectoryT FromFile(string fileName)
            {
                return new BlockInfoAndDirectoryT(new KaitaiStream(fileName));
            }

            public BlockInfoAndDirectoryT(KaitaiStream p__io, UnityBundle p__parent = null, UnityBundle p__root = null) : base(p__io)
            {
                m_parent = p__parent;
                m_root = p__root;
                _read();
            }
            private void _read()
            {
                __raw__raw_data = m_io.ReadBytes(M_Parent.Header.CompressedBlocksInfoSize);
                Lz4Uncompress _process__raw__raw_data = new Lz4Uncompress(M_Root.Header.UncompressedBlocksInfoSize);
                __raw_data = _process__raw__raw_data.Decode(__raw__raw_data);
                var io___raw_data = new KaitaiStream(__raw_data);
                _data = new DataT(io___raw_data, this, m_root);
            }
            public partial class DataT : KaitaiStruct
            {
                public static DataT FromFile(string fileName)
                {
                    return new DataT(new KaitaiStream(fileName));
                }

                public DataT(KaitaiStream p__io, UnityBundle.BlockInfoAndDirectoryT p__parent = null, UnityBundle p__root = null) : base(p__io)
                {
                    m_parent = p__parent;
                    m_root = p__root;
                    _read();
                }
                private void _read()
                {
                    _uncompressedDataHash = m_io.ReadBytes(16);
                    _blocksInfoCount = m_io.ReadS4be();
                    _blocksInfo = new List<Block>();
                    for (var i = 0; i < BlocksInfoCount; i++)
                    {
                        _blocksInfo.Add(new Block(m_io, this, m_root));
                    }
                    _nodesCount = m_io.ReadS4be();
                    _directoryInfo = new List<Node>();
                    for (var i = 0; i < NodesCount; i++)
                    {
                        _directoryInfo.Add(new Node(m_io, this, m_root));
                    }
                }
                public partial class Block : KaitaiStruct
                {
                    public static Block FromFile(string fileName)
                    {
                        return new Block(new KaitaiStream(fileName));
                    }

                    public Block(KaitaiStream p__io, UnityBundle.BlockInfoAndDirectoryT.DataT p__parent = null, UnityBundle p__root = null) : base(p__io)
                    {
                        m_parent = p__parent;
                        m_root = p__root;
                        f_compressionType = false;
                        _read();
                    }
                    private void _read()
                    {
                        _uncompressedSize = m_io.ReadU4be();
                        _compressedSize = m_io.ReadU4be();
                        _flags = m_io.ReadU2be();
                    }
                    private bool f_compressionType;
                    private CompressionType _compressionType;
                    public CompressionType CompressionType
                    {
                        get
                        {
                            if (f_compressionType)
                                return _compressionType;
                            _compressionType = (CompressionType) (((UnityBundle.CompressionType) (Flags & 63)));
                            f_compressionType = true;
                            return _compressionType;
                        }
                    }
                    private uint _uncompressedSize;
                    private uint _compressedSize;
                    private ushort _flags;
                    private UnityBundle m_root;
                    private UnityBundle.BlockInfoAndDirectoryT.DataT m_parent;
                    public uint UncompressedSize { get { return _uncompressedSize; } }
                    public uint CompressedSize { get { return _compressedSize; } }
                    public ushort Flags { get { return _flags; } }
                    public UnityBundle M_Root { get { return m_root; } }
                    public UnityBundle.BlockInfoAndDirectoryT.DataT M_Parent { get { return m_parent; } }
                }
                public partial class Node : KaitaiStruct
                {
                    public static Node FromFile(string fileName)
                    {
                        return new Node(new KaitaiStream(fileName));
                    }

                    public Node(KaitaiStream p__io, UnityBundle.BlockInfoAndDirectoryT.DataT p__parent = null, UnityBundle p__root = null) : base(p__io)
                    {
                        m_parent = p__parent;
                        m_root = p__root;
                        _read();
                    }
                    private void _read()
                    {
                        _offset = m_io.ReadS8be();
                        _size = m_io.ReadS8be();
                        _flags = m_io.ReadU4be();
                        _path = System.Text.Encoding.GetEncoding("utf-8").GetString(m_io.ReadBytesTerm(0, false, true, true));
                    }
                    private long _offset;
                    private long _size;
                    private uint _flags;
                    private string _path;
                    private UnityBundle m_root;
                    private UnityBundle.BlockInfoAndDirectoryT.DataT m_parent;
                    public long Offset { get { return _offset; } }
                    public long Size { get { return _size; } }
                    public uint Flags { get { return _flags; } }
                    public string Path { get { return _path; } }
                    public UnityBundle M_Root { get { return m_root; } }
                    public UnityBundle.BlockInfoAndDirectoryT.DataT M_Parent { get { return m_parent; } }
                }
                private byte[] _uncompressedDataHash;
                private int _blocksInfoCount;
                private List<Block> _blocksInfo;
                private int _nodesCount;
                private List<Node> _directoryInfo;
                private UnityBundle m_root;
                private UnityBundle.BlockInfoAndDirectoryT m_parent;
                public byte[] UncompressedDataHash { get { return _uncompressedDataHash; } }
                public int BlocksInfoCount { get { return _blocksInfoCount; } }
                public List<Block> BlocksInfo { get { return _blocksInfo; } }
                public int NodesCount { get { return _nodesCount; } }
                public List<Node> DirectoryInfo { get { return _directoryInfo; } }
                public UnityBundle M_Root { get { return m_root; } }
                public UnityBundle.BlockInfoAndDirectoryT M_Parent { get { return m_parent; } }
            }
            private DataT _data;
            private UnityBundle m_root;
            private UnityBundle m_parent;
            private byte[] __raw_data;
            private byte[] __raw__raw_data;
            public DataT Data { get { return _data; } }
            public UnityBundle M_Root { get { return m_root; } }
            public UnityBundle M_Parent { get { return m_parent; } }
            public byte[] M_RawData { get { return __raw_data; } }
            public byte[] M_RawM_RawData { get { return __raw__raw_data; } }
        }
        public partial class Block : KaitaiStruct
        {
            public Block(uint p_uncompressedSize, uint p_compressedSize, KaitaiStream p__io, UnityBundle p__parent = null, UnityBundle p__root = null) : base(p__io)
            {
                m_parent = p__parent;
                m_root = p__root;
                _uncompressedSize = p_uncompressedSize;
                _compressedSize = p_compressedSize;
                _read();
            }
            private void _read()
            {
                __raw_data = m_io.ReadBytes(CompressedSize);
                Lz4Uncompress _process__raw_data = new Lz4Uncompress(UncompressedSize);
                _data = _process__raw_data.Decode(__raw_data);
            }
            private byte[] _data;
            private uint _uncompressedSize;
            private uint _compressedSize;
            private UnityBundle m_root;
            private UnityBundle m_parent;
            private byte[] __raw_data;
            public byte[] Data { get { return _data; } }
            public uint UncompressedSize { get { return _uncompressedSize; } }
            public uint CompressedSize { get { return _compressedSize; } }
            public UnityBundle M_Root { get { return m_root; } }
            public UnityBundle M_Parent { get { return m_parent; } }
            public byte[] M_RawData { get { return __raw_data; } }
        }
        private string _signature;
        private uint _version;
        private string _unityVersion;
        private string _unityReversion;
        private HeaderT _header;
        private BlockInfoAndDirectoryT _blockInfoAndDirectory;
        private List<Block> _blocks;
        private byte[] _rest;
        private UnityBundle m_root;
        private KaitaiStruct m_parent;
        public string Signature { get { return _signature; } }
        public uint Version { get { return _version; } }
        public string UnityVersion { get { return _unityVersion; } }
        public string UnityReversion { get { return _unityReversion; } }
        public HeaderT Header { get { return _header; } }
        public BlockInfoAndDirectoryT BlockInfoAndDirectory { get { return _blockInfoAndDirectory; } }
        public List<Block> Blocks { get { return _blocks; } }
        public byte[] Rest { get { return _rest; } }
        public UnityBundle M_Root { get { return m_root; } }
        public KaitaiStruct M_Parent { get { return m_parent; } }
    }
}
