mergeInto(LibraryManager.library, {
    ModelLoadCallback = function(idPtr) {
        var id = UTF8ToString(idPtr);
        
        if (typeof window.onModelLoad === "function") {
            window.onModelLoaded(id);
        }
    },
    
    CameraUpdateCallback = function(jsonPtr) {
        var jsonString = UTF8ToString(jsonPtr);
        
        if (typeof window.onCameraUpdate === "function") {
            window.onCameraUpdate(jsonString);
        }
    },
});