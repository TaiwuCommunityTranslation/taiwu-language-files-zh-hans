# This is a generated file! Please edit source .ksy file and use kaitai-struct-compiler to rebuild

require 'kaitai/struct/struct'

unless Gem::Version.new(Kaitai::Struct::VERSION) >= Gem::Version.new('0.9')
  raise "Incompatible Kaitai Struct Ruby API: 0.9 or later is required, but you have #{Kaitai::Struct::VERSION}"
end

class UnityBundle < Kaitai::Struct::Struct

  COMPRESSION_TYPE = {
    0 => :compression_type_none,
    1 => :compression_type_lzma,
    2 => :compression_type_lz4,
    3 => :compression_type_lz4hc,
    4 => :compression_type_lzham,
  }
  I__COMPRESSION_TYPE = COMPRESSION_TYPE.invert
  def initialize(_io, _parent = nil, _root = self)
    super(_io, _parent, _root)
    _read
  end

  def _read
    @signature = (@_io.read_bytes_term(0, false, true, true)).force_encoding("utf-8")
    @version = @_io.read_u4be
    @unity_version = (@_io.read_bytes_term(0, false, true, true)).force_encoding("utf-8")
    @unity_reversion = (@_io.read_bytes_term(0, false, true, true)).force_encoding("utf-8")
    if signature == "UnityFS"
      @header = Header.new(@_io, self, @_root)
    end
    if  ((header.is_valid) && (version == 6)) 
      @block_info_and_directory = BlockInfoAndDirectory.new(@_io, self, @_root)
    end
    @blocks = []
    (block_info_and_directory.data.blocks_info_count).times { |i|
      @blocks << Block.new(@_io, self, @_root, block_info_and_directory.data.blocks_info[i].uncompressed_size, block_info_and_directory.data.blocks_info[i].compressed_size)
    }
    @rest__ = @_io.read_bytes_full
    self
  end
  class Header < Kaitai::Struct::Struct
    def initialize(_io, _parent = nil, _root = self)
      super(_io, _parent, _root)
      _read
    end

    def _read
      @size = @_io.read_s8be
      @compressed_blocks_info_size = @_io.read_u4be
      @uncompressed_blocks_info_size = @_io.read_u4be
      @flags = @_io.read_u4be
      self
    end
    def block_info_need_padding_at_start
      return @block_info_need_padding_at_start unless @block_info_need_padding_at_start.nil?
      @block_info_need_padding_at_start = (flags & 512) != 0
      @block_info_need_padding_at_start
    end
    def compression_type
      return @compression_type unless @compression_type.nil?
      @compression_type = Kaitai::Struct::Stream::resolve_enum(UnityBundle::COMPRESSION_TYPE, (flags & 63))
      @compression_type
    end
    def blocks_and_directory_info_combined
      return @blocks_and_directory_info_combined unless @blocks_and_directory_info_combined.nil?
      @blocks_and_directory_info_combined = (flags & 64) != 0
      @blocks_and_directory_info_combined
    end
    def block_info_at_the_end
      return @block_info_at_the_end unless @block_info_at_the_end.nil?
      @block_info_at_the_end = (flags & 128) != 0
      @block_info_at_the_end
    end
    def old_web_plugin_compatibility
      return @old_web_plugin_compatibility unless @old_web_plugin_compatibility.nil?
      @old_web_plugin_compatibility = (flags & 256) != 0
      @old_web_plugin_compatibility
    end
    def is_valid
      return @is_valid unless @is_valid.nil?
      @is_valid = flags == 67
      @is_valid
    end
    attr_reader :size
    attr_reader :compressed_blocks_info_size
    attr_reader :uncompressed_blocks_info_size
    attr_reader :flags
  end
  class BlockInfoAndDirectory < Kaitai::Struct::Struct
    def initialize(_io, _parent = nil, _root = self)
      super(_io, _parent, _root)
      _read
    end

    def _read
      @_raw__raw_data = @_io.read_bytes(_parent.header.compressed_blocks_info_size)
      _process = Lz4Uncompress.new(_root.header.uncompressed_blocks_info_size)
      @_raw_data = _process.decode(@_raw__raw_data)
      _io__raw_data = Kaitai::Struct::Stream.new(@_raw_data)
      @data = Data.new(_io__raw_data, self, @_root)
      self
    end
    class Data < Kaitai::Struct::Struct
      def initialize(_io, _parent = nil, _root = self)
        super(_io, _parent, _root)
        _read
      end

      def _read
        @uncompressed_data_hash = @_io.read_bytes(16)
        @blocks_info_count = @_io.read_s4be
        @blocks_info = []
        (blocks_info_count).times { |i|
          @blocks_info << Block.new(@_io, self, @_root)
        }
        @nodes_count = @_io.read_s4be
        @directory_info = []
        (nodes_count).times { |i|
          @directory_info << Node.new(@_io, self, @_root)
        }
        self
      end
      class Block < Kaitai::Struct::Struct
        def initialize(_io, _parent = nil, _root = self)
          super(_io, _parent, _root)
          _read
        end

        def _read
          @uncompressed_size = @_io.read_u4be
          @compressed_size = @_io.read_u4be
          @flags = @_io.read_u2be
          self
        end
        def compression_type
          return @compression_type unless @compression_type.nil?
          @compression_type = Kaitai::Struct::Stream::resolve_enum(UnityBundle::COMPRESSION_TYPE, (flags & 63))
          @compression_type
        end
        attr_reader :uncompressed_size
        attr_reader :compressed_size
        attr_reader :flags
      end
      class Node < Kaitai::Struct::Struct
        def initialize(_io, _parent = nil, _root = self)
          super(_io, _parent, _root)
          _read
        end

        def _read
          @offset = @_io.read_s8be
          @size = @_io.read_s8be
          @flags = @_io.read_u4be
          @path = (@_io.read_bytes_term(0, false, true, true)).force_encoding("utf-8")
          self
        end
        attr_reader :offset
        attr_reader :size
        attr_reader :flags
        attr_reader :path
      end
      attr_reader :uncompressed_data_hash
      attr_reader :blocks_info_count
      attr_reader :blocks_info
      attr_reader :nodes_count
      attr_reader :directory_info
    end
    attr_reader :data
    attr_reader :_raw_data
    attr_reader :_raw__raw_data
  end
  class Block < Kaitai::Struct::Struct
    def initialize(_io, _parent = nil, _root = self, uncompressed_size, compressed_size)
      super(_io, _parent, _root)
      @uncompressed_size = uncompressed_size
      @compressed_size = compressed_size
      _read
    end

    def _read
      @_raw_data = @_io.read_bytes(compressed_size)
      _process = Lz4Uncompress.new(uncompressed_size)
      @data = _process.decode(@_raw_data)
      self
    end
    attr_reader :data
    attr_reader :uncompressed_size
    attr_reader :compressed_size
    attr_reader :_raw_data
  end
  attr_reader :signature
  attr_reader :version
  attr_reader :unity_version
  attr_reader :unity_reversion
  attr_reader :header
  attr_reader :block_info_and_directory
  attr_reader :blocks
  attr_reader :rest__
end
