mergeInto(LibraryManager.library, {
    ModelLoadCallback: function(idPtr) {
        var id = UTF8ToString(idPtr);
        
        if (typeof window.dispatchUnityEvent === "function") {
            window.dispatchUnityEvent("ModelLoad", id);
        }
    },
    
    CameraUpdateCallback: function(jsonPtr) {
        var jsonString = UTF8ToString(jsonPtr);
        
        if (typeof window.dispatchUnityEvent === "function") {
            window.dispatchUnityEvent("CameraUpdate", jsonString);
        }
    },
});