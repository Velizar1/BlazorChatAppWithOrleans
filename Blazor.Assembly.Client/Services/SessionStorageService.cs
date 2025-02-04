﻿using Microsoft.JSInterop;

namespace Blazor.Assembly.Client.Services
{
    public class SessionStorageService
    {
        private readonly IJSRuntime _jsRuntime;

        public SessionStorageService(IJSRuntime jsRuntime)
        {
            _jsRuntime = jsRuntime;
        }

        public async Task SetItemAsync(string key, string value)
        {
            // Store the data in memory, set an expiration time if needed
            await _jsRuntime.InvokeVoidAsync("sessionStorage.setItem", key, value);
        }

        public async Task<string> GetItemAsync(string key)
        {
            var data = await _jsRuntime.InvokeAsync<string>("sessionStorage.getItem", key);
            // Get the data from memory
            return data;
        }

        public async Task RemoveItemAsync(string key)
        {
            await _jsRuntime.InvokeVoidAsync("sessionStorage.removeItem", key);
        }
    }
}
