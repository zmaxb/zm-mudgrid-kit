using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MoonMud.DemoApp.Models;
using MudBlazor;

namespace MoonMud.DemoApp.Service;

public class AssetGridDataService : IGridDataService<AssetReadDto>
{
    private readonly List<AssetReadDto> _assets = new()
    {
        new AssetReadDto
        {
            GlobalId = Guid.NewGuid(), Name = "Asset 1", Type = "Type 1", Symbol = "SYM1", Precision = 2,
            CreatedAt = DateTime.Now
        },
        new AssetReadDto
        {
            GlobalId = Guid.NewGuid(), Name = "Asset 2", Type = "Type 2", Symbol = "SYM2", Precision = 3,
            CreatedAt = DateTime.Now.AddMinutes(-5)
        },
    };

    public Task<PagedResponse<AssetReadDto>> GetPagedAsync(GridState<AssetReadDto> state)
    {
        var filteredAssets = _assets
            .Skip((state.Page - 1) * state.PageSize)
            .Take(state.PageSize)
            .ToList();

        return Task.FromResult(new PagedResponse<AssetReadDto>
        {
            Items = filteredAssets,
            TotalCount = _assets.Count
        });
    }

    public async Task<GridData<AssetReadDto>> LoadData(GridState<AssetReadDto> state)
    {
        var response = await GetPagedAsync(state);
        var content = new GridData<AssetReadDto>
        {
            Items = response.Items,
            TotalItems = response.TotalCount
        };
        return content;
    }
}