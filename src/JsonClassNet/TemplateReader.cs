namespace JsonClassNet;

public ref struct TemplateReader(ReadOnlySpan<byte> template)
{
    private ReadOnlySpan<byte> _buffer = template;
    private int _index = 0;
    private int _offset = 0;
    private TemplatePartKind _kind =  TemplatePartKind.None;
    private bool _readDefineValue = false;
    public bool Read(out TemplatePart part)
    {
        part = ReadNext();
        return part.Kind != TemplatePartKind.None;
    }

    private TemplatePart ReadNext()
    {
        if (!ReadNextKeyChar()) return default;
        
        if (_kind == TemplatePartKind.None && _buffer[_index] != GenerateContext.DefineStartingPrefix)
        {
            ThrowException();
        }

        if (_buffer[_index] == GenerateContext.DefineStartingPrefix)
        {
            var start = _index + _offset;
            var end = _index + _offset;
            if (_readDefineValue)
            {
                _kind = TemplatePartKind.Text;
                start = _offset;
                end++;
            }
            else
            {
                _kind = TemplatePartKind.Define;
                Slice();
                ReadDefineEnding();
                end = _index + _offset;
            }
            
            SliceNext();
            return new TemplatePart(_kind, start, end);
        }
        else if (_buffer[_index] == GenerateContext.DefineValueStartingPrefix)
        {
            if (_index > 0)//abc"
            {
                _kind = TemplatePartKind.Text;
                var start = _offset;
                var end = _index + _offset;
                SliceNext();
                _readDefineValue = false;
                return new TemplatePart(_kind, start, end);
            }
            SliceNext();
            _readDefineValue = true;
            return ReadNext();
        }
        else if (_buffer[_index] == GenerateContext.ParameterStartingPrefixChar)
        {
            var start = _offset;
            var end = _index + _offset;
            
            if (_index > 0)//"abc$(abc)"
            {
                _kind = TemplatePartKind.Text;
                Slice();
                return new TemplatePart(_kind, start, end);
            }

            if (!_buffer.Slice(_index, 2).SequenceEqual(GenerateContext.ParameterStartingPrefix))
            {
                _kind = TemplatePartKind.Text;
                ReadDefineValueEnding();
                end = _index + _offset;
                SliceNext();
                return new TemplatePart(_kind, start, end);
            }

            _kind = TemplatePartKind.Parameter;
            ReadParameterEnding();
            start += 2;
            end = _index + _offset;
            SliceNext();
            return new TemplatePart(_kind, start, end);
        }
        
        return default;
    }

    private void Slice()
    {
        _buffer = _buffer.Slice(_index);
        _offset += _index;
        _index = 0;
    }
    private void SliceNext()
    {
        var index = _index + 1;
        if (index < _buffer.Length)
        {
            _buffer = _buffer.Slice(index);
            _offset += index;
        }
        else
        {
            _buffer = ReadOnlySpan<byte>.Empty;
        }

        _index = 0;
    }

    private void ReadParameterStarting()
    {
      _index = _buffer.IndexOf(GenerateContext.ParameterStartingPrefix); 
      if (_index < 0) ThrowException();
    }

    private void ReadParameterEnding()
    {
      _index = _buffer.IndexOfAny(GenerateContext.ParameterEndingSuffix);
      if (_index < 0) ThrowException();
    }
    private void ReadDefineStarting()
    { 
        _index = _buffer.IndexOf(GenerateContext.DefineStartingPrefix);
        if (_index < 0) ThrowException();
    }
    private void ReadDefineEnding()
    {
        _index = _buffer.IndexOf(GenerateContext.DefineEndingSuffix);
        if (_index < 0) ThrowException();
    }

    private void ReadDefineValueStarting()
    {
        _index = _buffer.IndexOf(GenerateContext.DefineValueStartingPrefix);
        if (_index < 0) ThrowException();
    }

    private void ReadDefineValueEnding()
    {
       _index = _buffer.IndexOf(GenerateContext.DefineValueEndingSuffix);
       if (_index < 0) ThrowException();
    }

    private bool ReadNextKeyChar()
    {
       _index = _buffer.IndexOfAny(GenerateContext.KeyChars);
       return _index >= 0;
    } 

    private void ThrowException() => throw new InvalidDataException("Template invalid");
}