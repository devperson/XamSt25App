using St25App.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace St25App.Services
{
    public interface ITagReadWriteMemory
    {
        Task<List<TagMemoryRow>> GetMemoryRowsAsync(int mStartAddress, int mNumberOfBytes);
        Task UpdateMemoryRowAsync(int mStartAddress, TagMemoryRow row);
        Task ClearMemoryAsync();
    }
}
